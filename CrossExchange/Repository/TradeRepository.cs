using CrossExchange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrossExchange
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        public TradeRepository(ExchangeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Trade> Buy(TradeModel model, Func<HourlyShareRate> hourlyShareRateFunc)
        {
            Trade trade = new Trade
            {
                NoOfShares = model.NoOfShares,
                PortfolioId = model.PortfolioId,
                Symbol = model.Symbol,
                Action = model.Action,
                Price = hourlyShareRateFunc().Rate
            };

            await InsertAsync(trade);

            return trade;
        }

        public IQueryable<TradeSummary> GetSummaryTradeByProfileIdAndSymbol(int profileId, string symbol)
        {
            //In this case I recomend implementing the blockchain

            IQueryable<TradeSummary> query =  _dbContext.Trades.Where(p => p.PortfolioId == profileId && p.Symbol.Equals(symbol))
                .GroupBy(p => p.Action)
                .Select(p => new TradeSummary
                {
                    Action = p.Key,
                    NoOfShares = p.Sum(z => z.NoOfShares),
                    Symbol = symbol
                });

            return query;
        }

        public async Task<Trade> Sell(TradeModel model, Func<HourlyShareRate> hourlyShareRateFunc)
        {
            IEnumerable<TradeSummary> trades = GetSummaryTradeByProfileIdAndSymbol(model.PortfolioId, model.Symbol);

            int noOfShare = - model.NoOfShares;

            foreach (TradeSummary item in trades)
            {
                switch (item.Action)
                {
                    case "BUY":
                        noOfShare += item.NoOfShares;
                        break;
                    case "SELL":
                        noOfShare -= item.NoOfShares;
                        break;
                    default:
                        throw new HttpStatusCodeException(400, "Action not exists");
                }
            }

            if (noOfShare < 0)
            {
                throw new HttpStatusCodeException(400, "Insufficient quantities available");
            }

            Trade trade = new Trade
            {
                NoOfShares = model.NoOfShares,
                PortfolioId = model.PortfolioId,
                Symbol = model.Symbol,
                Action = model.Action,
                Price = hourlyShareRateFunc().Rate
            };

            await InsertAsync(trade);

            return trade;
        }
    }
}