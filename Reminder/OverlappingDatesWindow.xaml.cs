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
    /// Interaction logic for OverlappingDatesWindow.xaml
    /// </summary>
    public partial class OverlappingDatesWindow : Window
    {
        public OverlappingDatesWindow(List<Events> events)
        {
            InitializeComponent();
            foreach (var element in events)
                lb_overlappingDates.Items.Add(element);

            lb_overlappingDates.SelectionMode = SelectionMode.Single;
        }

        private void ButtonOverwrite_Click(object sender, RoutedEventArgs e)
        {
            Result = (Events)lb_overlappingDates.SelectedItem;
            Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            Result = null;
            Close();
        }

        public Events Result { get; set; }
    }
}
