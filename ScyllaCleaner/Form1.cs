using Spotify.Events;
using Spotify.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScyllaCleaner {
    public partial class Form1 : Form {

#region Properties
        int _percentage = 0;
        int Percentage {
            get {
                return _percentage;
            }
            set {
                _percentage = value;
                OnPercentValueChanged();
            }
        }
#endregion

        public Form1() {
            InitializeComponent();
        }

        private void Start(object sender, EventArgs e) {
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;

            Spotify.Controls.ProgBar progressBar = new Spotify.Controls.ProgBar();
            progressBar.Location = new Point(0, 0);

            timer1.Tick += new EventHandler(ProgressBar);

            Clean();
        }

        private void ProgressBar(object sender, EventArgs e) {
            if (Spotify.Cleaner.Tasks.LastPerformanceMaximum() <= 0)
                return;

            float perBlock = 100f / Spotify.Cleaner.Tasks.LastPerformanceMaximum();
            Percentage = (int)Math.Round((Spotify.Cleaner.Tasks.LastPerformanceMaximum() - Spotify.Cleaner.Tasks.TasksInQueue()) * perBlock, 0);

            if (Spotify.Cleaner.Tasks.ActiveTask() != null)
                label1.Text = $"Performing {Spotify.Cleaner.Tasks.ActiveTask().Item2}, {Percentage}%...";
        }

        private void OnPercentValueChanged() {
            progBar1.Value = Percentage;

            if (Percentage >= 100) {
                label1.Text = $"Finished.";
                System.Threading.Thread.Sleep(2000);
                Environment.FailFast("");
            }
        }

        private static void Clean() {
            Spotify.NativeImport.NativeImport.AllocConsole();
            Logger.Log("Welcome to Scylla 'Cleaner' by Conrado.", Spotify.Enums.LogLevel.Debug);

            Spotify.Cleaner.Tasks.AddAction(() => Spotify.Cleaner.Explorer.RmvAll(), "(1). Removing Scylla Files From Known Paths");
            Spotify.Cleaner.Tasks.AddAction(() => Spotify.Cleaner.RegClass.RmvAllRegistryKeys(), "(2). Removing Known Registry Keys");

            Spotify.Cleaner.Tasks.AddAction(() => MessageBox.Show("All Done Correctly :)"), "(3). Message Box");

            Task.Run(() => Spotify.Cleaner.Tasks.PerformActions());
        }
    }
}