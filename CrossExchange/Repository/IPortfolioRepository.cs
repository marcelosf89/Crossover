using System.Linq;

namespace CrossExchange
{
    public interface IPortfolioRepository : IGenericRepository<Portfolio>
    {
        IQueryable<Portfolio> GetAll();
        Portfolio GetPortfolioById(int portfolioid);
    }
}