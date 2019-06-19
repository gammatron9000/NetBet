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
    public class SeasonController : ControllerBase
    {
        [HttpGet]
        public Season[] GetAll()
        {
            return SeasonService.getAllSeasons();
        }
        
        [HttpGet("{id}")]
        public Season GetById(int id)
        {
            return SeasonService.getSeasonByID(id);
        }

        [HttpGet("{id}")]
        public SeasonWithPlayers GetSeasonWithPlayers(int id)
        {
            return SeasonService.getSeasonWithPlayers(id);
        }

        [HttpGet]
        public SeasonWithPlayers GetEmpty()
        {
            var emptySeason = new Season()
            {
                ID = 0,
                Name = "",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                StartingCash = 0,
                MinimumCash = 0,
                MaxParlaySize = 0
            };
            var emptyPlayers = new SeasonPlayer[] { };
            var empty = new SeasonWithPlayers(emptySeason, emptyPlayers);
            return empty;
        }
        
        [HttpPost]
        public void Create([FromBody] SeasonWithPlayers sp)
        {
            SeasonService.createSeasonWithPlayers(sp);
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            SeasonService.deleteSeason(id);
        }
    }
}
