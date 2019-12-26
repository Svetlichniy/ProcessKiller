using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ProcessKiller.Helpers;

namespace ProcessKiller
{
    public partial class Form1 : Form
    {
        string[] IgnorProcceses = new string[] { "svchost.exe", "conhost.exe" };
        bool isPause = false;
        string FilePath = string.Empty;

        public Form1()
        {  
            InitializeComponent();
            this.Width = 800;
            this.Height = 450;
            FilePath = Properties.Settings.Default.FilePath;
            RunSettings.SelectedIndex = Properties.Settings.Default.RunSettings;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            InitTimer();

            var BlackList = FileHelper.ImportBlackList(FilePath);
            if (BlackList.Count > 0)
            {
                listBox2.Items.Clear();
                BlackList.ForEach(o => listBox2.Items.Add(o));
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {            
            var SelectedItem = listBox1.SelectedItem.ToString();
            if (IgnorProcceses.Contains(SelectedItem.Split('\t')[0]))
            {
                MessageBox.Show("Добавление недопустимо", "Недопустимый процесс", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            listBox2.Items.Add(SelectedItem);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var SelectedIndex = listBox2.SelectedIndex;
            if(SelectedIndex != -1)
                listBox2.Items.RemoveAt(SelectedIndex);
        }

        private void импортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var BlackList = FileHelper.ImportBlackList();
            if (BlackList.Count > 0)
            {
                listBox2.Items.Clear();
                BlackList.ForEach(o => listBox2.Items.Add(o));                
            }
            Properties.Settings.Default.FilePath = FileHelper.filePath;
            Properties.Settings.Default.Save();
        }

        private void экспортToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileHelper.ExportBlackList(listBox2.Items.Cast<string>().ToList());
        }

        #region ServiceMethods
        private void LoadData()
        {
            Task.Run(() =>
            {
                var ProcessesList = new List<string>();
                ProcessesList = ProcessHelper.GetProcesses();

                ProcessesList.AddRange(ProcessHelper.GetTasks());

                Invoke(new Action(() => CountLabel.Text = string.Format("Загружено {0}.", ProcessesList.Count().ToString())));
                Invoke(new Action(() => listBox1.DataSource = ProcessesList));
            });
        }

        private void InitTimer()
        {
            var tmrShow = new Timer();
            tmrShow.Interval = 20000;
            tmrShow.Tick += tmrShow_Tick;
            tmrShow.Start();
        }

        private void tmrShow_Tick(object sender, EventArgs e)
        {
            var CommonList = listBox1.Items.Cast<string>().ToArray();
            var BlackList = listBox2.Items.Cast<string>().ToArray();
            if (BlackList.Length == 0 || isPause)
                return;
            
            var ResultBlackList = CommonList.Intersect(BlackList).ToArray();
            if(ResultBlackList.Count() != 0)
                Task.Run(() => ProcessHelper.KillAll(ResultBlackList));
            LoadData();
        }

        private void SetPause_Click(object sender, EventArgs e)
        {
            if (!isPause)
                SetPause.Checked = isPause = true;
            else
                SetPause.Checked = isPause = false;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
        }

        private void Tray_Click(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if(RunSettings.SelectedIndex == 0)
                Hide();
        }

        private void RunSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RunSettings = RunSettings.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
