using System.Threading.Tasks;
using CrossExchange.Controller;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using CrossExchange.Model;
using Microsoft.EntityFrameworkCore;

namespace CrossExchange.Tests
{
    public class TradeRepositoryTests
    {
        private Mock<ExchangeContext> _exchangeContext = new Mock<ExchangeContext>();
        private Mock<DbSet<Trade>> _dbSetTrade = new Mock<DbSet<Trade>>();
        private TradeRepository _tradeRepository;

        public TradeRepositoryTests()
        {
        }

        [SetUp]
        public void Initialize()
        {
            _exchangeContext = new Mock<ExchangeContext>();
            _dbSetTrade = new Mock<DbSet<Trade>>();
            _tradeRepository = new TradeRepository(_exchangeContext.Object);
        }

        [Test]
        public async Task Buy_PortfolioNotExists()
        {
            Func<HourlyShareRate> func = () =>
            {
                return new HourlyShareRate
                {
                    Rate = 20.00M
                };
            };

            TradeModel model = new TradeModel
            {
                Action = "BUY",
                NoOfShares = 10,
                PortfolioId = 1,
                Symbol = "SIM"
            };
            // Arrange

            _exchangeContext.Setup(p => p.Set<Trade>().Add(It.IsAny<Trade>()));
            _exchangeContext.Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            Trade trade = await _tradeRepository.Buy(model, func);

            // Assert
            Assert.NotNull(trade);
            Assert.AreEqual(trade.Symbol, model.Symbol);
            Assert.AreEqual(trade.PortfolioId, model.PortfolioId);
            Assert.AreEqual(trade.NoOfShares, model.NoOfShares);
            Assert.AreEqual(trade.Price, 20.00M);
        }


        [Test]
        public async Task GetSummaryTradeByProfileIdAndSymbol_ReturnTradeSummaryByPortfolio()
        {
            IQueryable<Trade> trades = new List<Trade>
            {
                new Trade
                {
                    Action = "BUY",
                    NoOfShares = 10,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 10.00M
                },
                new Trade
                {
                    Action = "SELL",
                    NoOfShares = 3,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 12.00M
                },
                new Trade
                {
                    Action = "BUY",
                    NoOfShares = 30,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 8.00M
                },
                new Trade
                {
                    Action = "SELL",
                    NoOfShares = 30,
                    PortfolioId = 2,
                    Symbol = "SIM",
                    Price = 8.00M
                }
            }.AsQueryable();

            // Arrange
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Provider).Returns(trades.Provider);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Expression).Returns(trades.Expression);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.ElementType).Returns(trades.ElementType);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.GetEnumerator()).Returns(trades.GetEnumerator());

            _exchangeContext.Setup(p => p.Trades).Returns(_dbSetTrade.Object);
            // Act
            IQueryable<TradeSummary> tradeSummary = _tradeRepository.GetSummaryTradeByProfileIdAndSymbol(1, "SIM");

            // Assert
            Assert.NotNull(tradeSummary);
            Assert.AreEqual(tradeSummary.Count(), 2);
            Assert.IsTrue(tradeSummary.Any(p => p.Action.Equals("BUY")));
            Assert.IsTrue(tradeSummary.Any(p => p.Action.Equals("SELL")));
        }


        [Test]
        public async Task Sell_ReturnTradeCreated()
        {
            IQueryable<Trade> trades = new List<Trade>
            {
                new Trade
                {
                    Action = "BUY",
                    NoOfShares = 10,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 10.00M
                },
                new Trade
                {
                    Action = "SELL",
                    NoOfShares = 3,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 12.00M
                }
            }.AsQueryable();

            Func<HourlyShareRate> func = () =>
            {
                return new HourlyShareRate
                {
                    Rate = 20.00M
                };
            };

            TradeModel model = new TradeModel
            {
                Action = "SELL",
                NoOfShares = 4,
                PortfolioId = 1,
                Symbol = "SIM"
            };
            // Arrange

            _exchangeContext.Setup(p => p.Set<Trade>().Add(It.IsAny<Trade>()));
            _exchangeContext.Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Provider).Returns(trades.Provider);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Expression).Returns(trades.Expression);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.ElementType).Returns(trades.ElementType);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.GetEnumerator()).Returns(trades.GetEnumerator());

            _exchangeContext.Setup(p => p.Trades).Returns(_dbSetTrade.Object);
            // Act
            Trade trade = await _tradeRepository.Sell(model, func);

            // Assert
            Assert.NotNull(trade);
            Assert.AreEqual(trade.Symbol, model.Symbol);
            Assert.AreEqual(trade.PortfolioId, model.PortfolioId);
            Assert.AreEqual(trade.NoOfShares, model.NoOfShares);
            Assert.AreEqual(trade.Price, 20.00M);
        }

        [Test]
        public async Task Sell_ThrowInsuficient()
        {
            IQueryable<Trade> trades = new List<Trade>
            {
                new Trade
                {
                    Action = "BUY",
                    NoOfShares = 10,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 10.00M
                },
                new Trade
                {
                    Action = "SELL",
                    NoOfShares = 3,
                    PortfolioId = 1,
                    Symbol = "SIM",
                    Price = 12.00M
                }
            }.AsQueryable();

            Func<HourlyShareRate> func = () =>
            {
                return new HourlyShareRate
                {
                    Rate = 20.00M
                };
            };

            TradeModel model = new TradeModel
            {
                Action = "SELL",
                NoOfShares = 10,
                PortfolioId = 1,
                Symbol = "SIM"
            };
            // Arrange

            _exchangeContext.Setup(p => p.Set<Trade>().Add(It.IsAny<Trade>()));
            _exchangeContext.Setup(p => p.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Provider).Returns(trades.Provider);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.Expression).Returns(trades.Expression);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.ElementType).Returns(trades.ElementType);
            _dbSetTrade.As<IQueryable<Trade>>().Setup(m => m.GetEnumerator()).Returns(trades.GetEnumerator());

            _exchangeContext.Setup(p => p.Trades).Returns(_dbSetTrade.Object);
            // Act
            try
            {
                await _tradeRepository.Sell(model, func);
                Assert.Fail();
            }
            catch (HttpStatusCodeException ex)
            {
                // Assert
                Assert.AreEqual(ex.StatusCode, 400);
                Assert.AreEqual(ex.Message, "Insufficient quantities available");
            }
            catch (Exception)
            {
                Assert.Fail();
                throw;
            }
        }
    }
}
