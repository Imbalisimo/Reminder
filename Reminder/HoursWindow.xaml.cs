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
    /// Interaction logic for HoursWindow.xaml
    /// </summary>
    public partial class HoursWindow : Window
    {
        public HoursWindow(List<int> hoursWhenToRemind)
        {
            InitializeComponent();

            for(int i=1; i<24; ++i)
                lb_hours.Items.Add(i);
            lb_hours.Items.Add(0);

            lb_hours.SelectionMode = SelectionMode.Multiple;
            foreach (int hour in hoursWhenToRemind)
                lb_hours.SelectedItems.Add(hour);

            lb_hours.SelectionChanged += (o, e) => { if (!lb_hours.SelectedItems.Contains(0))
                                                        { lb_hours.SelectedItems.Add(0);
                                                            MessageBox.Show("Can't remove 0");
                                                        } };

            lb_hours.Focus();

            lb_hours.FontSize = 1.27 * lb_hours.Width/24;

            Result = new List<int>();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var hour in lb_hours.SelectedItems)
                Result.Add((int)hour);

            Close();
        }

        public List<int> Result { get; set; }
    }
}
