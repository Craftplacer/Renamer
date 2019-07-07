using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Renamer
{
    public partial class MainForm : Form
    {
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

        public List<Item> Items = new List<Item>();

        public MainForm() => this.InitializeComponent();

        private void MainForm_Load(object sender, EventArgs e) => DarkMode = Properties.Settings.Default.DarkMode;

        private void MainForm_DragEnter(object sender, DragEventArgs e) => e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var item = new Item(files);
            Items.Add(item);
            Process.Start(new ProcessStartInfo(item.tempPath)
            {
                UseShellExecute = true
            });
        }
    }
}