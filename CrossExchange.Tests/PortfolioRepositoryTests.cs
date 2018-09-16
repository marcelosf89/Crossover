using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace CrossExchange.Tests
{
    public class PortfolioRepositoryTests
    {
        private Mock<ExchangeContext> _exchangeContext;
        private Mock<DbSet<Portfolio>> _dbSetPortfolio;
        private PortfolioRepository _portfolioRepository;

        public PortfolioRepositoryTests()
        {            
        }

        [SetUp]
        public void Initialize()
        {
            _exchangeContext = new Mock<ExchangeContext>();
            _dbSetPortfolio = new Mock<DbSet<Portfolio>>();
            _portfolioRepository = new PortfolioRepository(_exchangeContext.Object);
        }

        [Test]
        public async Task GetAll_ReturnAllPortfolio()
        {
            IQueryable<Portfolio> portfolios = new List<Portfolio> {
                new Portfolio
                {
                    Name = "Test 01",
                    Trade = new List<Trade>(),
                    Id = 1
                },
                new Portfolio
                {
                    Name = "Test 01",
                    Trade = new List<Trade>(),
                    Id = 2
                }
            }.AsQueryable();
            // Arrange

            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.Provider).Returns(portfolios.Provider);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.Expression).Returns(portfolios.Expression);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.ElementType).Returns(portfolios.ElementType);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.GetEnumerator()).Returns(portfolios.GetEnumerator());

            _exchangeContext.Setup(p => p.Portfolios).Returns(_dbSetPortfolio.Object);

            // Act
            IEnumerable<Portfolio> result = _portfolioRepository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count(), 2);

            //Assert.NotNull(hourlyShare);
            //Assert.AreEqual(hourlyShare.Symbol, "Test");
            //Assert.AreEqual(hourlyShare.TimeStamp, new DateTime(2018, 08, 17, 5, 1, 0));
            //Assert.AreEqual(hourlyShare.Rate, 333.0M);
        }


        [Test]
        public async Task GetPortfolioById_ReturnPortfolioById()
        {
            IQueryable<Portfolio> portfolios = new List<Portfolio> {
                new Portfolio
                {
                    Name = "Test 01",
                    Trade = new List<Trade>(),
                    Id = 1
                },
                new Portfolio
                {
                    Name = "Test 01",
                    Trade = new List<Trade>(),
                    Id = 2
                }
            }.AsQueryable();

            // Arrange

            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.Provider).Returns(portfolios.Provider);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.Expression).Returns(portfolios.Expression);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.ElementType).Returns(portfolios.ElementType);
            _dbSetPortfolio.As<IQueryable<Portfolio>>().Setup(m => m.GetEnumerator()).Returns(portfolios.GetEnumerator());

            _exchangeContext.Setup(p => p.Portfolios).Returns(_dbSetPortfolio.Object);

            // Act
            Portfolio result = _portfolioRepository.GetPortfolioById(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Name, "Test 01");
        }
    }
}
