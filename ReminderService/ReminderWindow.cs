using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderService
{
    public partial class ReminderWindow : Form
    {
        public ReminderWindow(List<Events> events, bool disableSnooze)
        {
            InitializeComponent();

            foreach (Events theEvent in events)
                listViewEvents.Items.Add(theEvent.ToString());
            listViewEvents.View = View.List;
            buttonRemind.Enabled = !disableSnooze;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void buttonRemind_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
        }
    }
}
