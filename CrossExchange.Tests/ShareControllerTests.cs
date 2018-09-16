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
    public class ShareControllerTests
    {
        private Mock<IShareRepository> _shareRepositoryMock;
        private ShareController _shareController;

        public ShareControllerTests()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _shareRepositoryMock = new Mock<IShareRepository>();
            _shareController = new ShareController(_shareRepositoryMock.Object);
        }

        [Test]
        public async Task Post_ShouldInsertHourlySharePrice()
        {
            var hourRate = new HourlyShareRate
            {
                Symbol = "CBI",
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            // Arrange

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task Post_ModelInvalid()
        {

            var hourRate = new HourlyShareRate
            {
                Rate = 330.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
            };

            // Arrange
            _shareController.ModelState.AddModelError("", "Error");

            // Act
            var result = await _shareController.Post(hourRate);

            // Assert
            Assert.NotNull(result);

            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task Get_ShoudGetTwoHourlyShareRates()
        {
            var hourRates = new List<HourlyShareRate> {
                new HourlyShareRate
                {
                    Symbol = "CBI",
                    Rate = 330.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Symbol = "CBI",
                    Rate = 330.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                }
            };

            _shareRepositoryMock.Setup(p => p.Query()).Returns(hourRates.AsQueryable());

            // Arrange

            // Act
            var result = await _shareController.Get("CBI");

            // Assert
            Assert.NotNull(result);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual((okResult.Value as IEnumerable<HourlyShareRate>).Count(), 2);
        }

        [Test]
        public async Task GetLatestPrice_ShouldReturnLastRate()
        {
            // Arrange
            _shareRepositoryMock.Setup(p => p.GetLatestPrice(It.IsIn<String>("CBI"))).Returns(new HourlyShareRate
            {
                Symbol = "CBI",
                Rate = 333.0M,
                TimeStamp = new DateTime(2018, 08, 17, 5, 1, 0)
            });

            // Act
            var result = await _shareController.GetLatestPrice("CBI");

            // Assert
            Assert.NotNull(result);

            OkObjectResult okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(decimal.Parse(okResult.Value.ToString()), 333.0M);
        }
    }
}
