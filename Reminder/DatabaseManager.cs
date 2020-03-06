using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder
{
    static class DatabaseManager
    {
        private static EventsEntities eventsEntities = new EventsEntities();

        public static void AddNewEvent(Events e)
        {
            eventsEntities.Events.Add(e);
            eventsEntities.SaveChanges();
        }

        public static IQueryable<Events> GetEventsForTheDay(DateTime date)
        {
            return from Events in eventsEntities.Events
                   where date.Day == Events.Date.Day &&
                        date.Month == Events.Date.Month
                   select Events;
        }

        public static IQueryable<Events> GetEventsBeforeDate(DateTime date)
        {
            return from Events in eventsEntities.Events
                   where Events.Annually == false &&
                         (Events.Date.Year < date.Year ||
                         (Events.Date.Year == date.Year && Events.Date.Month < date.Month) ||
                         (Events.Date.Year == date.Year && Events.Date.Month == date.Month && Events.Date.Day < date.Day))
                   select Events;

        }

        public static IQueryable<Events> GetOverlappingDates(DateTime date, bool annual)
        {
            return from Events in eventsEntities.Events
                   where (date.Day == Events.Date.Day) &&
                        (date.Month == Events.Date.Month) &&
                        (date.Year == Events.Date.Year) &&
                        (annual == Events.Annually)
                   select Events;
        }

        public static List<Events> GetAllEvents()
        {
            return eventsEntities.Events.ToList();
        }

        public static void RemoveEvent(List<Events> events)
        {
            foreach(Events e in events)
                RemoveEvent(e);

            eventsEntities.SaveChanges();
        }

        public static void RemoveEvent(Events e)
        {
            eventsEntities.Events.Remove(e);
            //eventsEntities.SaveChangesAsync();
            eventsEntities.SaveChanges();
        }
    }
}
