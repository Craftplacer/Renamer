using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Renamer
{
    public partial class MainForm : Form
    {
        public NotifyIcon notifyIcon { get; }

        #region Dark Mode

        private bool _darkMode = true;

        public bool DarkMode
        {
            get => _darkMode;
            set
            {
                this.BackColor = value ? Color.FromArgb(24, 26, 27) : Color.White;
                this.ForeColor = value ? Color.FromArgb(232, 230, 227) : Color.Black;
                this.darkButton.Image = value ? Properties.Resources.sun : Properties.Resources.moon;
                this.instructionLabel.ForeColor = value ? Color.FromArgb(54, 108, 216) : Color.FromArgb(0, 51, 153);
                Properties.Settings.Default.DarkMode = _darkMode = value;
                Properties.Settings.Default.Save();
            }
        }

        private void DarkButton_Click(object sender, EventArgs e) => DarkMode = !DarkMode;

        #endregion Dark Mode

        public MainForm()
        {
            this.InitializeComponent();
            notifyIcon = new NotifyIcon()
            {
                Text = "Renamer",
                Icon = GetNotifyIcon(),
                Visible = true
            };
        }

        private Icon GetNotifyIcon()
        {
            var icon = Properties.Resources.tray;

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");

                if (key != null)
                {
                    var light = (int)key.GetValue("SystemUsesLightTheme") == 1;
                    
                    if (light)
                        icon = Properties.Resources.tray_dark;
                    else
                        icon = Properties.Resources.tray_light;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't set notify icon to system theme");
                Debug.WriteLine(ex);
            }

            return icon;
        }

        private void MainForm_Load(object sender, EventArgs e) => DarkMode = Properties.Settings.Default.DarkMode;

        private void MainForm_DragEnter(object sender, DragEventArgs e) => e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var item = new Item(files);
            
            item.Renamed += (s,ie) =>
            {
                notifyIcon.ShowBalloonTip
                (
                    2500,
                    "Files renamed",
                    ie.TotalFilesChanged > 0
                        ? $"{ie.TotalFilesChanged} files have been renamed."
                        : "No files have been renamed",
                    ToolTipIcon.None
                );
            };

            Process.Start(new ProcessStartInfo(item.TempPath)
            {
                UseShellExecute = true
            });
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => notifyIcon.Visible = false;
    }
}