using NAudio.Wave;
using System;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace Soundbox
{
    public partial class Soundbox : Form
    {
        private float fadeOutConst = 0.5f;
        private const int steps = 40;
        private string[] sounds;
        private Mp3FileReader[] readers;
        private string path = AppContext.BaseDirectory;
        private List<WaveOut> activePlayers = new List<WaveOut>();
        private SoundFile[] so;

        public Soundbox()
        {
            InitializeComponent();
            reloadS();
        }       

        public void reloadS()
        {
            stopAllSounds();
            try
            {
                sounds = Directory.GetFiles(Path.Combine(path, "Sounds"), "*.mp3");
            }
            catch
            {
                MessageBox.Show("Error: The Sounds folder has been deleted!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (sounds.Length == 20)
            {
                buttonUp.Enabled = false;
            }
            else
            {

                buttonUp.Enabled = true;
            }

            Button[] buttons = { b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13, b14, b15, b16, b17, b18, b19, b20 };

            readers = new Mp3FileReader[sounds.Length];
            so = new SoundFile[sounds.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < sounds.Length)
                {
                    try
                    {
                        readers[i] = new Mp3FileReader(sounds[i]);
                    }
                    catch {
                        MessageBox.Show("Error: mp3 file is corupted!.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(sounds[i]);                    
                    buttons[i].Enabled = true;
                    buttons[i].Text = fileName;
                    so[i] = new SoundFile(Path.Combine(path, "Sounds", sounds[i]), fileName);
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
            int index = 0;
            for (int i = 0; i < sounds.Length; i++)
            {
                if (path == sounds[i])
                    index = i;
            }
            try
            {
                WaveOut wave = new WaveOut();
                try
                {
                    wave.Init(readers[index]);
                }
                catch
                {
                    MessageBox.Show("Error: mp3 file is corupted!.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                wave.Volume = 1.0f;

                wave.PlaybackStopped += (sender, e) =>
                {
                    var player = sender as WaveOut;
                    if (player != null)
                    {                        
                        if (activePlayers.Contains(player))
                            activePlayers.Remove(player);
                        player.Dispose();
                    }
                };

                wave.Play();
                activePlayers.Add(wave);
                try
                {
                    readers[index] = new Mp3FileReader(sounds[index]);
                }
                catch {
                    MessageBox.Show("Error: mp3 file is corupted!.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Warning: Tried playing an unexisting sound.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private float max(float a, float b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        private async void fade(WaveOut player, float secs)
        {
            int interval = (int)((secs * 1000) / steps);
            if (interval < 0)
            {
                player.Stop();
                player.Dispose();
                MessageBox.Show("Error: The fade time has been set to a negative number or the interval variable got bigger than 2,147,483,647.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < steps; i++)
            {
                player.Volume = max(0.0f, 1.0f - (float)(i) / steps);
                await Task.Delay(interval);
            }
            player.Stop();
            player.Dispose();
        }

        private void stopAllSounds(float secsOfFade = 0.0f)
        {
            foreach (WaveOut player in activePlayers)
            {
                try
                {
                    if (secsOfFade == 0.0f)
                        player.Stop();
                    else
                        fade(player, secsOfFade);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Warning: tried to stop an unexisting sound", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            MessageBox.Show("Warning: No sound file found.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

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
                Title = "Select MP3 files",
                Filter = "MP3 Files (*.mp3)|*.mp3",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;
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
                MessageBox.Show("Error: The Sounds has been deleted.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Process.Start("explorer.exe", folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: The Sounds has been deleted.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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

        private void buttonFade_Click(object sender, EventArgs e)
        {

            stopAllSounds(fadeOutConst);
        }

        private void textBoxFadeOut_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBoxFadeOut.Text == "cacatoare")
                {
                    MessageBox.Show("This is NOT an easter egg!", "I'm lying", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                fadeOutConst = (float)(Int32.Parse(textBoxFadeOut.Text));
            }
            catch { }
        }

        private void buttonDelPan_Click(object sender, EventArgs e)
        {            
            DelScreen delScreen = new DelScreen(so);
            delScreen.Show();
        }
    }
}
