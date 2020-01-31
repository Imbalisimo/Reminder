using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for RemovingDatesWindow.xaml
    /// </summary>
    public partial class RemovingDatesWindow : Window
    {
        public RemovingDatesWindow(List<Events> events)
        {
            InitializeComponent();

            foreach (var element in events)
                lb_overlappingDates.Items.Add(element);

            lb_overlappingDates.SelectionMode = SelectionMode.Multiple;
            Result = new List<Events>();
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (var theEvent in lb_overlappingDates.SelectedItems)
                Result.Add((Events)theEvent);
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public List<Events> Result { get; set; }
    }
}
