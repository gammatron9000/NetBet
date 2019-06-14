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
    public class MatchController : ControllerBase
    {
        [HttpGet("{matchid}")]
        public Match GetById(int matchid)
        {
            return MatchService.getMatchByID(matchid);
        }
        
        [HttpGet("{eventid}")]
        public PrettyMatch[] GetMatchesForEvent(int eventid)
        {
            return MatchService.getMatchesForEvent(eventid);
        }

        [HttpGet]
        public Match GetEmpty()
        {
            var emptyMatch = new Match();
            emptyMatch.ID = 0;
            emptyMatch.EventID = 0;
            emptyMatch.Fighter1ID = 0;
            emptyMatch.Fighter2ID = 0;
            emptyMatch.Fighter1Odds = 0M;
            emptyMatch.Fighter2Odds = 0M;
            emptyMatch.WinnerFighterID = null;
            emptyMatch.LoserFighterID = null;
            emptyMatch.IsDraw = null;
            emptyMatch.DisplayOrder = 0;
            return emptyMatch;
        }

        [HttpPost]
        public void ResolveMatch([FromBody] ResolveMatchDto resolve)
        {
            MatchService.resolveMatch(resolve);
            EventService.getFullEvent(resolve.EventID); // send back the updated event
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            MatchService.deleteMatch(id);
        }
    }
}
