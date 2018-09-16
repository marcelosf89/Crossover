using CrossExchange.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrossExchange
{
    public interface ITradeRepository : IGenericRepository<Trade>
    {
        IQueryable<TradeSummary> GetSummaryTradeByProfileIdAndSymbol(int profileId, string symbol);

        Task<Trade> Buy(TradeModel model, Func<HourlyShareRate> hourlyShareRateFunc);

        Task<Trade> Sell(TradeModel model, Func<HourlyShareRate> hourlyShareRateFunc);
    }
}