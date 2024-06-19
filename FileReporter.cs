using System;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace FileReporter
{
    public partial class FileReporter : Form
    {
        private string targetFilePath = "";
        private FileSystemWatcher watcher;
        private DateTime lastModifiedTime;
        private Timer timer1;
        private Timer logTimer;
        private OpenFileDialog openFileDialog;

        public FileReporter()
        {
            InitializeComponent();
            InitializeFileSystemWatcher();
            ApplyModernStyles();
            InitializeOpenFileDialog();

          
            timer1 = new Timer();
            timer1.Interval = 15000;
            timer1.Tick += Timer1_Tick;

            logTimer = new Timer();
            logTimer.Interval = 1000;
            logTimer.Tick += LogTimer_Tick;

            lastModifiedTime = DateTime.Now;
        }

        private void InitializeOpenFileDialog()
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Select a .txt file";
        }

        private void ApplyModernStyles()
        {
            // Form appearance
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Button appearance
            button1.FlatStyle = FlatStyle.Flat;
            button1.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            button1.ForeColor = System.Drawing.Color.White;
            button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            button1.Text = "Select .txt File";

            // ListView appearance
            listViewLog.View = View.Details;
            listViewLog.Columns.Add("Logs", -2, HorizontalAlignment.Left);
            listViewLog.HeaderStyle = ColumnHeaderStyle.None;
            listViewLog.FullRowSelect = true;
            listViewLog.GridLines = true;
            listViewLog.BackColor = System.Drawing.Color.White;
            listViewLog.ForeColor = System.Drawing.Color.Black;
            listViewLog.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

            // Label appearance
            label1.Text = "File Change Log:";
            label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.ForeColor = System.Drawing.Color.FromArgb(52, 152, 219);
        }
        private void InitializeFileSystemWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += OnChanged;

            watcher.Path = @"C:\";
            watcher.Filter = "*.txt";
            watcher.EnableRaisingEvents = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime currentModifiedTime = File.GetLastWriteTime(targetFilePath);

            if (currentModifiedTime != lastModifiedTime)
            {
                string message = $"File '{Path.GetFileName(targetFilePath)}' has been modified.";
                NotifyFileChange(message);

                lastModifiedTime = currentModifiedTime;
            }
        }

        private void LogTimer_Tick(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(targetFilePath))
            {
                LogMessage($"No Changes at {DateTime.Now:HH:mm:ss}");
            }
        }

        private void LogMessage(string message)
        {
          
            listViewLog.Items.Add(new ListViewItem(message));
            if (listViewLog.Items.Count > 0)
            {
                listViewLog.Items[listViewLog.Items.Count - 1].EnsureVisible();
            }
        }

        private void NotifyFileChange(string message)
        {
            MessageBox.Show(message, "File Change Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                if (Path.GetExtension(selectedFile).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Selected file: {selectedFile}");
                    targetFilePath = selectedFile;
                    listViewLog.Items.Clear();
                    watcher.Path = Path.GetDirectoryName(targetFilePath);
                    watcher.Filter = Path.GetFileName(targetFilePath);
                    watcher.EnableRaisingEvents = true;
                    LogMessage($"Monitoring file: {targetFilePath}");
                    timer1.Start();
                    logTimer.Start();
                }
                else
                {
                    MessageBox.Show("Please select a .txt file.");
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string message = $"File '{e.FullPath}' has been modified.";

            // If you want to get notified immediatly when there is a file change , uncomment the function call below. 
           // NotifyFileChange(message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
