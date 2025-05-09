using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace Soundbox
{
    public partial class Form1 : Form
    {
        private string[] sounds;
        private string path = AppContext.BaseDirectory;
        private List<SoundPlayer> activePlayers = new List<SoundPlayer>();

        public Form1()
        {
            InitializeComponent();
            reloadS();
        }

        public void reloadS()
        {
            sounds = Directory.GetFiles(Path.Combine(path, "Sounds"), "*.wav");

            if (sounds.Length == 0)
            {
                MessageBox.Show("Error: There are no .wav files in the Sounds folder.");
                Close();
                return;
            }

            Button[] buttons = { b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16 };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < sounds.Length)
                {
                    string fileName = Path.GetFileNameWithoutExtension(sounds[i]);
                    buttons[i].Enabled = true;
                    buttons[i].Text = fileName;
                }
                else
                {
                    buttons[i].Enabled = false;
                    buttons[i].Text = "No Sound";
                }
            }
        }
        private void playSound(string path)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(path);
                player.Play();
                activePlayers.Add(player);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}");
            }
        }

        private void stopAllSounds()
        {
            foreach (SoundPlayer player in activePlayers)
            {
                try
                {
                    player.Stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping sound: {ex.Message}");
                }
            }

            activePlayers.Clear();
        }
        private void playButtonSound(Button button)
        {
            string buttonText = button.Text;

            foreach (string sound in sounds)
            {
                string fileName = Path.GetFileNameWithoutExtension(sound);
                if (fileName == buttonText)
                {
                    playSound(sound);
                    return;
                }
            }

            MessageBox.Show($"No sound file found for {buttonText}");
        }

        private void b1_Click(object sender, EventArgs e) => playButtonSound(b1);
        private void b2_Click(object sender, EventArgs e) => playButtonSound(b2);
        private void b3_Click(object sender, EventArgs e) => playButtonSound(b3);
        private void b4_Click(object sender, EventArgs e) => playButtonSound(b4);
        private void b5_Click(object sender, EventArgs e) => playButtonSound(b5);
        private void b6_Click(object sender, EventArgs e) => playButtonSound(b6);
        private void b7_Click(object sender, EventArgs e) => playButtonSound(b7);
        private void b8_Click(object sender, EventArgs e) => playButtonSound(b8);
        private void b9_Click(object sender, EventArgs e) => playButtonSound(b9);
        private void b10_Click(object sender, EventArgs e) => playButtonSound(b10);
        private void b11_Click(object sender, EventArgs e) => playButtonSound(b11);
        private void b12_Click(object sender, EventArgs e) => playButtonSound(b12);
        private void b13_Click(object sender, EventArgs e) => playButtonSound(b13);
        private void b14_Click(object sender, EventArgs e) => playButtonSound(b14);
        private void b15_Click(object sender, EventArgs e) => playButtonSound(b15);
        private void b16_Click(object sender, EventArgs e) => playButtonSound(b16);


        private void buttonS_Click(object sender, EventArgs e)
        {
            stopAllSounds();
        }

        private void buttonrRe_Click(object sender, EventArgs e)
        {
            reloadS();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select WAV files",
                Filter = "WAV Files (*.wav)|*.wav",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;

                Console.WriteLine("Selected WAV files:");
                foreach (string filePath in selectedFiles)
                {
                    File.Copy(filePath, Path.Combine(path, "Sounds", Path.GetFileName(filePath)), overwrite: true);
                }
            }
            reloadS();
        }

        private void buttonOp_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(path, "Sounds");

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("The Sounds has been deleted.");
                return;
            }

            try
            {
                Process.Start("explorer.exe", folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder: {ex.Message}");
            }
        }
    }
}
