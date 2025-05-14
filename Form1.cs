using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Soundbox
{
    public partial class Form1 : Form
    {
        private bool fadeInIf = false, rS = false;
        private int fade_out_time, fade_in_time;
        private string[] sounds;
        private string path = AppContext.BaseDirectory;
        private List<WaveOutEvent> activePlayers = new List<WaveOutEvent>();
        private List<VolumeSampleProvider> activeVolumeProviders = new List<VolumeSampleProvider>();

        public Form1()
        {
            InitializeComponent();
            f1C();
        }

        private void f1C()
        {
            reloadS();

            if (sounds.Length == 0)
            {
                DialogResult rez = MessageBox.Show("Warning: There are no .wav or .mp3 files in the Sounds folder.", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                if (rez == DialogResult.Retry)
                {
                    f1C();
                }
                else if(rez == DialogResult.Abort)
                {
                    Close();
                }
            }
        }
        public void reloadS()
        {
            using (StreamReader fin = new StreamReader(Path.Combine(path, "fade_time.txt")))
            {
                string read1 = fin.ReadLine();
                string read2 = fin.ReadLine();
                try
                {
                    fade_out_time = Int32.Parse(read1);
                    fade_in_time = Int32.Parse(read2);
                }
                catch
                {
                    MessageBox.Show("Error: The fade_time.txt file doesn't contain a number!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }           
            try
            {
                sounds = Directory.GetFiles(Path.Combine(path, "Sounds"), "*.wav")
                        .Concat(Directory.GetFiles(Path.Combine(path, "Sounds"), "*.mp3"))
                        .ToArray();
            }
            catch
            {
                MessageBox.Show("Error: The Sounds folder has been deleted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            buttonUp.Enabled = sounds.Length < 20;

            Button[] buttons = { b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20 };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < sounds.Length)
                {
                    string fileName = Path.GetFileNameWithoutExtension(sounds[i]);
                    fileName = fileName.IndexOf('`') != -1 ? fileName.Substring(fileName.IndexOf('`') + 1) : fileName;
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

        private VolumeSampleProvider playSound(string path)
        {
            try
            {
                VolumeSampleProvider volumeProvider = new VolumeSampleProvider(new AudioFileReader(path));
                WaveOutEvent player = new WaveOutEvent();
                volumeProvider.Volume = 1.0f;
                player.Init(volumeProvider);
                player.Play();

                activePlayers.Add(player);
                activeVolumeProviders.Add(volumeProvider);

                return volumeProvider;
            }
            catch
            {
                MessageBox.Show("Warning: Tried playing a non-existing sound.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }


        private void playButtonSound(Button button)
        {
            string buttonText = button.Text;            
            foreach (string sound in sounds)
            {
                string fileName = Path.GetFileNameWithoutExtension(sound);
                if (fileName == buttonText)
                {
                    if (fadeInIf == false)
                        playSound(sound);
                    else
                    {
                        fadeIn(fade_in_time, sound);
                    }
                    return;
                }
            }
            DialogResult rez = MessageBox.Show("Warning: No sound file found.", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
            if (rez == DialogResult.Retry)
            {
                playButtonSound(button);
            }
        }

        private async void fadeIn(float fadeTime, string file)
        {
            const int steps = 30;
            int interval = (int)(fadeTime / steps);

            if (rS)  
                return;  

            VolumeSampleProvider volumeProvider = playSound(file);
            if (volumeProvider == null) 
                return;

            volumeProvider.Volume = 0.0f;

            for (int i = 0; i <= steps; i++)
            {
                if (rS) 
                    return;  
                volumeProvider.Volume = (float)i / steps;
                await Task.Delay(interval);
            }

            fadeInIf = false;
        }


        private async void stopAllSounds(float fadeTime = 0f)
        {
            const int steps = 30;
            int interval = (int)(fadeTime / steps);
            rS = true;
            foreach (VolumeSampleProvider volumeProvider in activeVolumeProviders)
            {
                try
                {
                    if (fadeTime > 0)
                    {
                        for (int i = 0; i <= steps; i++)
                        {
                            float volume = 1.0f - (float)i / steps;
                            volumeProvider.Volume = Math.Max(0, volume);
                            await Task.Delay(interval);
                        }
                    }

                    volumeProvider.Volume = 0;
                }
                catch (Exception)
                {
                    MessageBox.Show("Warning: Error while stopping sound.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                rS = false;
            }

            foreach (WaveOutEvent player in activePlayers)
            {
                player.Stop();
                player.Dispose();
            }

            activePlayers.Clear();
            activeVolumeProviders.Clear();
        }

        private void buttonS_Click(object sender, EventArgs e) => stopAllSounds();

        private void buttonFo_Click(object sender, EventArgs e) => stopAllSounds(fade_out_time);

        private void buttonrRe_Click(object sender, EventArgs e) => reloadS();

        private void buttonUp_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select WAV or MP3 files",
                Filter = "Audio Files (*.wav;*.mp3)|*.wav;*.mp3",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;

                foreach (string filePath in selectedFiles)
                {
                    string destPath = Path.Combine(path, "Sounds", Path.GetFileName(filePath));
                    File.Copy(filePath, destPath, overwrite: true);
                }
            }

            reloadS();
        }

        private void buttonOp_Click(object sender, EventArgs e)
        {
            string folderPath = Path.Combine(path, "Sounds");

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Error: The Sounds folder has been deleted.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            try
            {
                Process.Start("explorer.exe", folderPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Unable to open the Sounds folder.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void b17_Click(object sender, EventArgs e) => playButtonSound(b17);
        private void b18_Click(object sender, EventArgs e) => playButtonSound(b18);
        private void b19_Click(object sender, EventArgs e) => playButtonSound(b19);
        private void b20_Click(object sender, EventArgs e) => playButtonSound(b20);

        private void buttonFi_Click(object sender, EventArgs e) => fadeInIf = true;

        private void buttonFic_Click(object sender, EventArgs e) => fadeInIf = true;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
