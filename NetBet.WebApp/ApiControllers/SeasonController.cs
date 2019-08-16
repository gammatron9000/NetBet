﻿using System;
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
        { return SeasonService.getAllSeasons(); }
        
        [HttpGet("{id}")]
        public Season GetById(int id)
        { return SeasonService.getSeasonByID(id);}

        [HttpGet("{id}")]
        public SeasonWithPlayers GetSeasonWithPlayers(int id)
        { return SeasonService.getSeasonWithPlayers(id); }

        [HttpGet("{id}")]
        public FullSeason GetFullSeason(int id)
        { return SeasonService.getFullSeason(id); }

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
        public void Edit([FromBody] EditSeasonDto s)
        { SeasonService.createSeasonWithPlayers(s); }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        { SeasonService.deleteSeason(id); }
    }
}
