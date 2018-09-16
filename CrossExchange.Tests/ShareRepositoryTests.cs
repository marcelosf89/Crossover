using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace CrossExchange.Tests
{
    public class ShareRepositoryTests
    {
        private Mock<ExchangeContext> _exchangeContext;
        private Mock<DbSet<HourlyShareRate>> _dbSetHourlyShareRate;
        private ShareRepository _shareRepository;

        public ShareRepositoryTests()
        {            
        }

        [SetUp]
        public void Initialize()
        {
            _exchangeContext = new Mock<ExchangeContext>();
            _dbSetHourlyShareRate = new Mock<DbSet<HourlyShareRate>>();
            _shareRepository = new ShareRepository(_exchangeContext.Object);
        }

        [Test]
        public async Task GetLatestPrice_ReturnLatestPriceBySymbol()
        {
            IQueryable<HourlyShareRate> hourRates = new List<HourlyShareRate> {
                new HourlyShareRate
                {
                    Symbol = "CBI",
                    Rate = 330.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 0, 0)
                },
                new HourlyShareRate
                {
                    Symbol = "CBI",
                    Rate = 333.0M,
                    TimeStamp = new DateTime(2018, 08, 17, 5, 1, 0)
                }
            }.AsQueryable();
            // Arrange

            _dbSetHourlyShareRate.As<IQueryable<HourlyShareRate>>().Setup(m => m.Provider).Returns(hourRates.Provider);
            _dbSetHourlyShareRate.As<IQueryable<HourlyShareRate>>().Setup(m => m.Expression).Returns(hourRates.Expression);
            _dbSetHourlyShareRate.As<IQueryable<HourlyShareRate>>().Setup(m => m.ElementType).Returns(hourRates.ElementType);
            _dbSetHourlyShareRate.As<IQueryable<HourlyShareRate>>().Setup(m => m.GetEnumerator()).Returns(hourRates.GetEnumerator());

            _exchangeContext.Setup(p => p.Shares).Returns(_dbSetHourlyShareRate.Object);

            // Act
            HourlyShareRate hourlyShare = _shareRepository.GetLatestPrice("CBI");

            // Assert
            Assert.NotNull(hourlyShare);
            Assert.AreEqual(hourlyShare.Symbol, "CBI");
            Assert.AreEqual(hourlyShare.TimeStamp, new DateTime(2018, 08, 17, 5, 1, 0));
            Assert.AreEqual(hourlyShare.Rate, 333.0M);
        }
    }
}
