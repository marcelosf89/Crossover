using System;
using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace CrossExchange.Tests
{
    public class TradeControllerTests
    {
        private Mock<ITradeRepository> _tradeRepositoryMock = new Mock<ITradeRepository>();
        private Mock<IPortfolioRepository> _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
        private Mock<IShareRepository> _shareRepositoryMock = new Mock<IShareRepository>();
        private TradeController _tradeController;

        public TradeControllerTests()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _tradeRepositoryMock = new Mock<ITradeRepository>();
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _shareRepositoryMock = new Mock<IShareRepository>();
            _tradeController = new TradeController(_tradeRepositoryMock.Object, _portfolioRepositoryMock.Object, _shareRepositoryMock.Object);
        }

        [Test]
        public async Task Get_GetAllTradings()
        {
            var trade = new Trade
            {
                Symbol = "CBI",
                Action = "BUY",
                PortfolioId = 1,
                NoOfShares = 5,
                Id = 1,
                Price = 300.00M
            };

            _tradeRepositoryMock.Setup(p => p.Query()).Returns(new List<Trade> { trade }.AsQueryable());

            // Arrange

            // Act
            var result = await _tradeController.GetAllTradings(1);

            // Assert
            Assert.NotNull(result);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task Post_ModelStateInvalid()
        {

            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                NoOfShares = 5
            };
            // Arrange
            _tradeController.ModelState.AddModelError("", "Error");

            // Act
            var result = await _tradeController.Post(tradeModel);

            // Assert
            Assert.NotNull(result);

            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Post_ShouldCreateBuyTrade()
        {
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "BUY",
                PortfolioId = 1,
                NoOfShares = 5
            };

            // Arrange

            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(new List<Portfolio> { new Portfolio { Id = 1 } }.AsQueryable());
            _shareRepositoryMock.Setup(p => p.Query()).Returns(new List<HourlyShareRate> { new HourlyShareRate { Symbol = "CBI" } }.AsQueryable());

            _tradeRepositoryMock.Setup(p => p.Buy(It.IsAny<TradeModel>(), It.IsAny<Func<HourlyShareRate>>())).ReturnsAsync(new Trade
            {
                Id = 1
            });

            // Act
            var result = await _tradeController.Post(tradeModel);

            // Assert
            Assert.NotNull(result);

            _tradeRepositoryMock.Verify(p => p.Buy(It.IsAny<TradeModel>(), It.IsAny<Func<HourlyShareRate>>()), Times.Once());

            CreatedResult createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsTrue(createdResult.Value is Trade);
        }

        [Test]
        public async Task Post_ShouldCreateSellTrade()
        {
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "SELL",
                PortfolioId = 1,
                NoOfShares = 5
            };

            // Arrange

            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(new List<Portfolio> { new Portfolio { Id = 1 } }.AsQueryable());
            _shareRepositoryMock.Setup(p => p.Query()).Returns(new List<HourlyShareRate> { new HourlyShareRate { Symbol = "CBI" } }.AsQueryable());

            _tradeRepositoryMock.Setup(p => p.Sell(It.IsAny<TradeModel>(), It.IsAny<Func<HourlyShareRate>>())).ReturnsAsync(new Trade
            {
                Id = 1
            });

            // Act
            var result = await _tradeController.Post(tradeModel);

            // Assert
            Assert.NotNull(result);

            _tradeRepositoryMock.Verify(p => p.Sell(It.IsAny<TradeModel>(), It.IsAny<Func<HourlyShareRate>>()), Times.Once());

            CreatedResult createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.IsTrue(createdResult.Value is Trade);
        }

        [Test]
        public async Task Post_ErrorActionTrade()
        {
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "ERR",
                PortfolioId = 1,
                NoOfShares = 5
            };

            // Arrange

            // Act
            try
            {
                var result = await _tradeController.Post(tradeModel);
                Assert.Fail();
            }
            catch (HttpStatusCodeException ex)
            {
                // Assert
                Assert.AreEqual(400, ex.StatusCode);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task Post_ErrorPortfolioNotExists()
        {
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "ERR",
                PortfolioId = 1,
                NoOfShares = 5
            };

            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(new List<Portfolio> { new Portfolio { Id = 1000 } }.AsQueryable());

            // Arrange

            // Act
            try
            {
                var result = await _tradeController.Post(tradeModel);
                Assert.Fail();
            }
            catch (HttpStatusCodeException ex)
            {
                // Assert
                Assert.AreEqual(400, ex.StatusCode);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task Post_ErrorShareNotExists()
        {
            var tradeModel = new TradeModel
            {
                Symbol = "CBI",
                Action = "ERR",
                PortfolioId = 1,
                NoOfShares = 5
            };

            // Arrange

            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(new List<Portfolio> { new Portfolio { Id = 1 } }.AsQueryable());
            _shareRepositoryMock.Setup(p => p.Query()).Returns(new List<HourlyShareRate> { new HourlyShareRate { Symbol = "OTH" } }.AsQueryable());

            // Act
            try
            {
                var result = await _tradeController.Post(tradeModel);
                Assert.Fail();
            }
            catch (HttpStatusCodeException ex)
            {
                // Assert
                Assert.AreEqual(400, ex.StatusCode);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}
