using NAudio.Wave;              
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Soundbox
{
    public partial class DelScreen : Form
    {
        private SoundFile[] sounds;
        private int curPoz = 3;
        private List<Button> buttons = new List<Button>();
        public DelScreen(SoundFile[] _sounds)
        {
            sounds = _sounds;
            InitializeComponent();
            reload();            
        }
        private void reload() {
            while (buttons.Count > 0)
            {
                buttons[0].Dispose();
                buttons.RemoveAt(0);                
            }
            curPoz = 3;
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].path != "\0") 
                    createButton(sounds[i], i);
            }
        }

    private void createButton(SoundFile s, int index)
        {
            Button btn = new Button();
            btn.Name = "bd" + index.ToString();
            btn.Text = s.name;
            btn.AutoSize = true;
            btn.TabIndex = 0;
            btn.UseVisualStyleBackColor = true;
            btn.Font = new Font("Segoe UI", 20);
            btn.Location = new Point(3, curPoz);
            btn.Click += clickEvent;
            buttons.Add(btn);
            panelMain.Controls.Add(btn);
            curPoz += 50;  
        }

        private void clickEvent(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                string indexStr = btn.Name.Substring(2);
                if (Int32.TryParse(indexStr, out int indexInt))
                {
                    if (sounds[indexInt].delete() == 0)
                        sounds[indexInt].path = "\0";
                    reload();
                }
                else
                {
                    MessageBox.Show(
                        "Error: private void clickEvent(object sender, EventArgs e) has been called with a wrong object.",
                        "Invalid Button",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void DelScreen_Load(object sender, EventArgs e)
        {
            
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
