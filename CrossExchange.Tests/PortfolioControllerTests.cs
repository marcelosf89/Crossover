using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace CrossExchange.Tests
{
    public class PortfolioControllerTests
    {
        private Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private PortfolioController _portfolioController;

        public PortfolioControllerTests()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _portfolioController = new PortfolioController(_portfolioRepositoryMock.Object);
        }

        [Test]
        public async Task GetPortfolioInfo_ReturnPortifolioInfo()
        {
            // Arrange

            _portfolioRepositoryMock.Setup(p => p.GetPortfolioById(It.IsIn<int>(1))).Returns(new Portfolio
            {
                Id = 1,
                Name = "Test 01",
                Trade = new List<Trade>()
            });

            // Act
            var result = await _portfolioController.GetPortfolioInfo(1);

            // Assert
            Assert.NotNull(result);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            Assert.IsTrue(okResult.Value is Portfolio);
            Assert.AreEqual((okResult.Value as Portfolio).Name, "Test 01");
        }

        [Test]
        public async Task Post_ModelStateInvalid()
        {

            var portfolio = new Portfolio();
            _portfolioController.ModelState.AddModelError("", "Error");
            // Arrange

            // Act
            var result = await _portfolioController.Post(portfolio);

            // Assert
            Assert.NotNull(result);

            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Post_ErrorExistsPortfolio()
        {
            var portfolio = new Portfolio
            {
                Name = "Test 01"
            };

            var portfolios = new List<Portfolio> {
                new Portfolio
                {
                    Id = 1,
                    Name = "Test 01",
                    Trade = new List<Trade>()
                },
                new Portfolio
                {
                    Id = 2,
                    Name = "Test 02",
                    Trade = new List<Trade>()
                }
            };

            // Arrange
            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(portfolios.AsQueryable());

            // Act
            var result = await _portfolioController.Post(portfolio);

            // Assert
            Assert.NotNull(result);

            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }


        [Test]
        public async Task Post_ShouldCreatePortfolio()
        {
            var portfolio = new Portfolio
            {
                Name = "Test 011"
            };

            var portfolios = new List<Portfolio> {
                new Portfolio
                {
                    Id = 1,
                    Name = "Test 01",
                    Trade = new List<Trade>()
                },
                new Portfolio
                {
                    Id = 2,
                    Name = "Test 02",
                    Trade = new List<Trade>()
                }
            };

            // Arrange
            _portfolioRepositoryMock.Setup(p => p.Query()).Returns(portfolios.AsQueryable());

            // Act
            var result = await _portfolioController.Post(portfolio);

            // Assert
            Assert.NotNull(result);

            CreatedResult createdResult  = result as CreatedResult;
            Assert.NotNull(createdResult );
            Assert.AreEqual(201, createdResult.StatusCode);
        }

       
    }
}
