using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CrossExchange
{
    public class PortfolioRepository : GenericRepository<Portfolio>, IPortfolioRepository
    {
        public PortfolioRepository(ExchangeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Portfolio> GetAll()
        {
            return _dbContext.Portfolios.Include(x => x.Trade).AsQueryable();
        }

        public Portfolio GetPortfolioById(int portfolioId)
        {
            return _dbContext.Portfolios.Where(x => x.Id == portfolioId).Include(x => x.Trade).SingleOrDefault();
        }
    }
}