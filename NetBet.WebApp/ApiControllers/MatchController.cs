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
