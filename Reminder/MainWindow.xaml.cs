using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EventsEntities eventsEntities;
        List<int> hoursWhenToRemind;
        int lastHourWhenReminded;
        DateTime lastDateConfirmed;

        private delegate void DateChange();
        event DateChange DateChanged;

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;

        RegistryKey _rk;
        string _appName;

        public MainWindow()
        {
            InitializeComponent();
            DateCalendar.DisplayDate = DateTime.Now;
            DateCalendar.SelectionMode = CalendarSelectionMode.SingleDate;
            rb_single.IsChecked = true;

            Initialization();

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = Properties.Resources.MyIcon;
            _notifyIcon.Visible = true;

            CreateContextMenu();

            _rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            _appName = "Reminder";

            String val = _rk.GetValue(_appName) as string;
            if (val != null)
            {
                mi_startAppAuto.IsChecked = true;
            }

        }

        private void Initialization()
        {
            eventsEntities = new EventsEntities();

            DateChanged += () => { DateCalendar.DisplayDate = DateTime.Now; };

            InitializeRemindingHours();

            DeletePastEvents();

            PopReminderWindow(DateTime.Now);

            Thread t = new Thread(WatchOutForEvents);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        private void WatchOutForEvents()
        {
            while (true)
            {
                Thread.Sleep(60000);
                DateTime now = DateTime.Now;
                if (lastDateConfirmed.ToShortDateString() != now.ToShortDateString())
                {
                    Dispatcher.Invoke(DateChanged);
                    if (lastHourWhenReminded != now.Hour && hoursWhenToRemind.Contains(now.Hour))
                    {
                        PopReminderWindow(now);
                    }
                }
            }
        }

        private void PopReminderWindow(DateTime now)
        {
            var todayEvents = from Events in eventsEntities.Events
                              where now.Day == Events.Date.Day &&
                                   now.Month == Events.Date.Month
                              select Events;

            ReminderWindow reminder = new ReminderWindow(todayEvents.ToList(), (hoursWhenToRemind.Last() > now.Hour) && (todayEvents.ToList().Count > 0));
            
            switch (reminder.ShowDialog())
            {
                case true:
                    lastDateConfirmed = now;
                    lastHourWhenReminded = 25;
                    foreach (Events e in todayEvents)
                        if (!e.Annually)
                            eventsEntities.Events.Remove(e);
                    eventsEntities.SaveChanges();
                    break;
                case false:
                default:
                    lastHourWhenReminded = now.Hour;
                    break;
            }
        }

        private void DeletePastEvents()
        {
            DateTime now = DateTime.Now;
            var pastEvents = from Events in eventsEntities.Events
                             where Events.Annually == false &&
                                   (Events.Date.Year < now.Year ||
                                   (Events.Date.Year == now.Year && Events.Date.Month < now.Month) ||
                                   (Events.Date.Year == now.Year && Events.Date.Month == now.Month && Events.Date.Day < now.Day))
                             select Events;

            //foreach (var element in pastEvents)
            //{
            //    eventsEntities.Events.Remove(element);
            //}
            eventsEntities.SaveChanges();
        }

        private void InitializeRemindingHours()
        {
            hoursWhenToRemind = new List<int>();
            StreamReader sr;
            try
            {
                sr = new StreamReader("./hours.ini");
                String hour = null;

                while ((hour = sr.ReadLine()) != null)
                    hoursWhenToRemind.Add(Int32.Parse(hour));


                sr.Close();
                sr.Dispose();
            }
            catch(FileNotFoundException e)
            {
                hoursWhenToRemind.Add(0);
                hoursWhenToRemind.Add(14); // In case the file is not initialized
                hoursWhenToRemind.Add(20);
            }
            catch(Exception e)
            {
                MessageBox.Show("Unable to open file: " + e.Message);
            }

            hoursWhenToRemind.Sort();
        }

        private void DateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateCalendar.SelectedDate != null)
                tb_date.Text = DateCalendar.SelectedDate.ToString()
                    .Remove(DateCalendar.SelectedDate.ToString().IndexOf(" "));
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = ExtractDate();

            if (date == DateTime.MinValue)
                return;

            if (IsDateInThePast(date, "Date can not be in the past"))
                return;

            List<Events> overlappingDates = GetOverlappingDates(date);

            if (overlappingDates.Count() > 0)
            {
                OverlappingDatesWindow overlappingDatesWindow = new OverlappingDatesWindow(overlappingDates);
                overlappingDatesWindow.ShowDialog();

                if (overlappingDatesWindow.Result != null)
                {
                    Events eventToDelete = overlappingDatesWindow.Result;
                    eventsEntities.Events.Remove(eventToDelete);
                }
            }

            Events newEvent = new Events();
            newEvent.Date = date;
            newEvent.Name = new TextRange(rtb_description.Document.ContentStart,
                rtb_description.Document.ContentEnd).Text;
            newEvent.Annually = ((rb_every.IsChecked ?? false));

            eventsEntities.Events.Add(newEvent);
            eventsEntities.SaveChanges();

            Reset();
        }

        private DateTime ExtractDate()
        {
            if (tb_date.Text != "" && tb_date.Text.IndexOf("/") == tb_date.Text.LastIndexOf("/"))
            {
                MessageBox.Show("Date must be in dd/mm/yyyy format!" +
                    "\nIf you want the reminder for every year, enter current year.");
                return DateTime.MinValue;
            }

            DateTime date;
            if (!DateTime.TryParse(tb_date.Text, out date))
            {
                MessageBox.Show("Date must be in dd/mm/yyyy format!");
                return DateTime.MinValue;
            }
            return date;
        }

        private List<Events> GetOverlappingDates(DateTime date)
        {
            var overlappingDates = (rb_single.IsChecked ?? false) ?
                                    from Events in eventsEntities.Events
                                    where (date.Day == Events.Date.Day) &&
                                         (date.Month == Events.Date.Month) &&
                                         (date.Year == Events.Date.Year) &&
                                         ((rb_single.IsChecked ?? false) == !Events.Annually)
                                    select Events
                                   : from Events in eventsEntities.Events
                                     where (date.Day == Events.Date.Day) &&
                                          (date.Month == Events.Date.Month) &&
                                          ((rb_every.IsChecked ?? false) == Events.Annually)
                                     select Events;

            return overlappingDates.ToList();
        }

        private bool IsDateInThePast(DateTime date, string messageInCaseOfPastDate)
        {
            if ((rb_single.IsChecked ?? false) && date < DateTime.Now.AddDays(-1))
            {
                MessageBox.Show(messageInCaseOfPastDate);
                return true;
            }
            return false;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = ExtractDate();

            if (date == DateTime.MinValue)
                return;

            if (IsDateInThePast(date, "Past dates are deleted automaticly!"))
                return;

            List<Events> overlappingDates = GetOverlappingDates(date);

            if (overlappingDates.Count() > 0)
            {
                RemovingDatesWindow removingDatesWindow = new RemovingDatesWindow(overlappingDates);
                removingDatesWindow.ShowDialog();

                if (removingDatesWindow.Result.Count > 0)
                {
                    List<Events> eventsToDelete = removingDatesWindow.Result;
                    foreach (Events theEvent in eventsToDelete)
                        eventsEntities.Events.Remove(theEvent);
                    eventsEntities.SaveChanges();
                    Reset();
                }
            }
            else
            {
                MessageBox.Show("No dates found");
            }
        }

        private void Reset()
        {
            rtb_description.Document.Blocks.Clear();
            tb_date.Text = "";
            rb_single.IsChecked = true;
            DateCalendar.SelectedDate = null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void MenuItemReminderWindow_Click(object sender, RoutedEventArgs e)
        {
            PopReminderWindow(DateTime.Now);
        }

        private void MenuItemHours_Click(object sender, RoutedEventArgs e)
        {
            HoursWindow hoursWindow = new HoursWindow(hoursWhenToRemind);
            hoursWindow.ShowDialog();
            hoursWhenToRemind = hoursWindow.Result;
        }

        private void AllEventsMenu_Click(object sender, RoutedEventArgs e)
        {
            EventsPresenterWindow presenterWindow = new EventsPresenterWindow(eventsEntities);
            presenterWindow.ShowDialog();
            eventsEntities.SaveChanges();
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Show Reminder").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (IsVisible)
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }
                Activate();
            }
            else
            {
                Show();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                Hide(); // A hidden window can be shown again, a closed one not
            }
            else
            {
                SaveRemindingHoursInAFile();
            }
        }

        private void SaveRemindingHoursInAFile()
        {
            StreamWriter sw = new StreamWriter("./hours.ini");
            foreach (int hour in hoursWhenToRemind)
                sw.WriteLine(hour.ToString());

            sw.Close();
            sw.Dispose();
        }

        private void MenuItemStartAppAuto_Click(object sender, RoutedEventArgs e)
        {
            mi_startAppAuto.IsChecked = !mi_startAppAuto.IsChecked;
            if (mi_startAppAuto.IsChecked)
            {
                _rk.SetValue(_appName, Process.GetCurrentProcess().MainModule.FileName /*System.AppDomain.CurrentDomain.BaseDirectory*/);
            }
            else
            {
                _rk.DeleteValue(_appName, false);
            }
        }
    }
}
