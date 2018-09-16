namespace CrossExchange
{
    public interface IShareRepository : IGenericRepository<HourlyShareRate>
    {
        HourlyShareRate GetLatestPrice(string symbol);
    }
}