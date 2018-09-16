using System.Linq;

namespace CrossExchange
{
    public class ShareRepository : GenericRepository<HourlyShareRate>, IShareRepository
    {
        public ShareRepository(ExchangeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public HourlyShareRate GetLatestPrice(string symbol)
        {
            return _dbContext.Shares.Where(x => x.Symbol.Equals(symbol))
                                        .OrderByDescending(p => p.TimeStamp)
                                        .FirstOrDefault();
        }
    }
}