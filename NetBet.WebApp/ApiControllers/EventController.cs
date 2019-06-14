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
    public class EventController : ControllerBase
    {   
        [HttpGet("{eventid}")]
        public Event GetById(int eventid)
        {
            return EventService.getEventByID(eventid);
        }

        [HttpGet("{seasonid}")]
        public Event[] GetEventsForSeason(int seasonid)
        {
            return EventService.getEventsForSeason(seasonid);
        }

        [HttpGet("{eventid}")]
        public EventWithPrettyMatches GetEventWithMatches(int eventid)
        {
            return EventService.getEventAndMatches(eventid);
        }

        [HttpGet("{eventid}")]
        public FullEvent GetFullEvent(int eventid)
        {
            return EventService.getFullEvent(eventid);
        }

        [HttpGet]
        public EventWithPrettyMatches[] GetUpcomingEventsFromWeb()
        {
            return EventService.getUpcomingEventsFromWeb();
        }

        [HttpGet]
        public EventWithPrettyMatches GetEmpty()
        {
            var emptyevt = new Event();
            var emptyMatches = new PrettyMatch[] { };
            return new EventWithPrettyMatches(emptyevt, emptyMatches);
        }

        [HttpPost]
        public void CreateOrUpdate([FromBody] EventWithPrettyMatches evt)
        {
            EventService.createEventWithMatches(evt);
        }
        
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            EventService.deleteEvent(id);
        }
    }
}
