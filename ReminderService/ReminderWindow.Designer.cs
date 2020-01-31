namespace ReminderService
{
    partial class ReminderWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOK = new System.Windows.Forms.Button();
            this.listViewEvents = new System.Windows.Forms.ListView();
            this.buttonRemind = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(176, 295);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // listViewEvents
            // 
            this.listViewEvents.Location = new System.Drawing.Point(12, 12);
            this.listViewEvents.Name = "listViewEvents";
            this.listViewEvents.Size = new System.Drawing.Size(710, 277);
            this.listViewEvents.TabIndex = 1;
            this.listViewEvents.UseCompatibleStateImageBehavior = false;
            // 
            // buttonRemind
            // 
            this.buttonRemind.Location = new System.Drawing.Point(467, 295);
            this.buttonRemind.Name = "buttonRemind";
            this.buttonRemind.Size = new System.Drawing.Size(92, 23);
            this.buttonRemind.TabIndex = 2;
            this.buttonRemind.Text = "Remind me later";
            this.buttonRemind.UseVisualStyleBackColor = true;
            this.buttonRemind.Click += new System.EventHandler(this.buttonRemind_Click);
            // 
            // ReminderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 330);
            this.Controls.Add(this.buttonRemind);
            this.Controls.Add(this.listViewEvents);
            this.Controls.Add(this.buttonOK);
            this.Name = "ReminderWindow";
            this.Text = "Reminder";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ListView listViewEvents;
        private System.Windows.Forms.Button buttonRemind;
    }
}