using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DbTypes;
using static DtoTypes;

namespace NetBet.WebApp.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        [HttpGet("{matchid}")]
        public BetWithOdds[] GetBetsForMatch(int matchid)
        {
            return BetService.getBetsForMatch(matchid);
        }
        
        [HttpGet("{eventid}")]
        public BetWithOdds[] GetBetsForEvent(int eventid)
        {
            return BetService.getBetsForEvent(eventid);
        }

        [HttpGet("{eventid}")]
        public PrettyBet[] GetPrettyBetsForEvent(int eventid)
        {
            return BetService.getPrettyBetsForEvent(eventid);
        }
        
        [HttpPost]
        public void PlaceBet([FromBody] PlaceBetDto dto)
        {
            BetService.placeBet(dto);
        }
        
        [HttpDelete()]
        public void Delete([FromBody] Bet b)
        {
            BetService.deleteBet(b);
        }

        [HttpGet("{seasonID}")]
        public SeasonWinPercent[] GetBetStatsForSeason(int seasonID)
        {
            return BetService.getBetStatsForSeason(seasonID);
        }
    }
}
