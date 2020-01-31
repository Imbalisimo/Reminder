using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReminderService
{
    public partial class Reminder : ServiceBase
    {
        bool active;
        EventLog Log;
        EventsEntities EventsEntities;
        DateTime lastDateConfirmed;
        int lastHourWhenReminded;
        List<int> hoursWhenToRemind;

        public Reminder()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("ReminderService"))
            {
                EventSourceCreationData sourceData = new EventSourceCreationData("ReminderService", "Application");
                EventLog.CreateEventSource(sourceData);
            }
            
            Log = new EventLog();
            Log.Log = "Application";
            Log.Source = "ReminderService";
        }

        protected override void OnStart(string[] args)
        {
            active = true;

            Initialization();

            PopReminderWindow("start");

            WaitCallback waitCallback = new WaitCallback(checkingDates);
            ThreadPool.QueueUserWorkItem(waitCallback);
        }

        private void checkingDates(object state)
        {
            while (active)
            {
                Thread.Sleep(60000);
                DateTime now = DateTime.Now;
                if(lastDateConfirmed.ToShortDateString() != now.ToShortDateString())
                {
                    if (lastHourWhenReminded != now.Hour && hoursWhenToRemind.Contains(now.Hour))
                    {
                        PopReminderWindow("working");
                    }
                }
            }
        }

        private void Initialization()
        {
            EventsEntities = new EventsEntities();

            hoursWhenToRemind = new List<int>();
            hoursWhenToRemind.Add(0); // Always
            hoursWhenToRemind.Add(14);
            hoursWhenToRemind.Add(20);
            hoursWhenToRemind.Sort();
            DateTime now = DateTime.Now;

            var pastEvents = from Events in EventsEntities.Events
                             where Events.Every_year == false &&
                                   (Events.Event_date.Year < now.Year ||
                                   (Events.Event_date.Year == now.Year && Events.Event_date.Month < now.Month) ||
                                   (Events.Event_date.Year == now.Year && Events.Event_date.Month == now.Month && Events.Event_date.Day < now.Day))
                             select Events;
            
                DeleteEvents(pastEvents.ToList());
        }

        private void DeleteEvents(List<Events> events)
        {
            foreach (var element in events)
            {
                EventsEntities.Events.Remove(element);
            }
        }

        protected override void OnStop()
        {
            active = false;
        }

        private void PopReminderWindow(string whenIsThisCalled)
        {
            DateTime date = DateTime.Now;

            var todayEvents = from Events in EventsEntities.Events
                              where (date.Day == Events.Event_date.Day) &&
                                   (date.Month == Events.Event_date.Month)
                              select Events;

            Log.WriteEntry("On the " + whenIsThisCalled + " of the Reminder service, date " + date.ToShortDateString()
                + ", found " + todayEvents.Count() + " events.");

            ReminderWindow reminder = new ReminderWindow(todayEvents.ToList(), hoursWhenToRemind.Last() <= date.Hour);

            switch (reminder.ShowDialog())
            {
                case System.Windows.Forms.DialogResult.OK:
                    lastDateConfirmed = date;
                    lastHourWhenReminded = 25;
                    foreach (Events e in todayEvents)
                        if (!e.Every_year)
                            EventsEntities.Events.Remove(e);
                    break;
                default:
                    lastHourWhenReminded = date.Hour;
                    break;
            }
        }
    }
}
