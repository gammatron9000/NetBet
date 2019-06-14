using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DbTypes;

namespace NetBet.WebApp.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        [HttpGet]
        public Player[] Get()
        {
            return SeasonService.getAllPlayers();
        }
        
        [HttpGet("{seasonid}")]
        public SeasonPlayer[] GetPlayersForSeason(int seasonid)
        {
            return SeasonService.getPlayersForSeason(seasonid);
        }

        [HttpGet]
        public Player GetEmpty()
        {
            var player = new Player();
            player.ID = 0;
            player.Name = "";
            return player;
        }

        [HttpPost]
        public void CreateOrUpdate([FromBody] Player p)
        {
            SeasonService.createOrUpdatePlayer(p);
        }
        
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            SeasonService.deletePlayer(id);
        }
    }
}
