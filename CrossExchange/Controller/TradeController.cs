using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace CrossExchange.Controller
{
    [Route("api/Trade")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IShareRepository _shareRepository;

        public TradeController(ITradeRepository tradeRepository, IPortfolioRepository portfolioRepository, IShareRepository shareRepository)
        {
            _tradeRepository = tradeRepository;
            _portfolioRepository = portfolioRepository;
            _shareRepository = shareRepository;
    }


        [HttpGet("{portfolioid}")]
        public async Task<IActionResult> GetAllTradings([FromRoute]int portFolioid)
        {
            var trade = _tradeRepository.Query().Where(x => x.PortfolioId.Equals(portFolioid));
            return Ok(trade);
        }


        /*************************************************************************************************************************************
        For a given portfolio, with all the registered shares you need to do a trade which could be either a BUY or SELL trade. For a particular trade keep following conditions in mind:
		BUY:
        a) The rate at which the shares will be bought will be the latest price in the database.
		b) The share specified should be a registered one otherwise it should be considered a bad request. 
		c) The Portfolio of the user should also be registered otherwise it should be considered a bad request. 
                
        SELL:
        a) The share should be there in the portfolio of the customer.
		b) The Portfolio of the user should be registered otherwise it should be considered a bad request. 
		c) The rate at which the shares will be sold will be the latest price in the database.
        d) The number of shares should be sufficient so that it can be sold. 
        Hint: You need to group the total shares bought and sold of a particular share and see the difference to figure out if there are sufficient quantities available for SELL. 

        *************************************************************************************************************************************/

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TradeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_portfolioRepository.Query().Any(p => p.Id == model.PortfolioId))
            {
                throw new HttpStatusCodeException(400, "Share not exists");
            }

            if (!_shareRepository.Query().Any(x => x.Symbol.Equals(model.Symbol)))
            {
                throw new HttpStatusCodeException(400, "Share not exists");
            }

            Trade trade = null;
            switch (model.Action)
            {
                case "BUY":
                    trade = await _tradeRepository.Buy(model, () => {
                        return _shareRepository.GetLatestPrice(model.Symbol);
                    });
                    break;
                case "SELL":
                    trade = await _tradeRepository.Sell(model, () => {
                        return _shareRepository.GetLatestPrice(model.Symbol);
                    });
                    break;
                default:
                    throw new HttpStatusCodeException(400, "Action Incorrect");
            }

            return Created("Trade", trade);
        }
    }
}
