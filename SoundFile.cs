using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundbox
{
    public class SoundFile
    {
        public string path;
        public string name;
        public WaveOut wave;        
        public SoundFile (string _path, string _name)
        {
            path = _path;
            name = _name;
        }
        public int delete() {             
            try
            {
                File.Delete(path);
                if (wave != null)
                {
                    wave.Dispose();
                }
            }
            catch {
                MessageBox.Show("Warning: Failed deleting the file.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return -1;
            }
            return 0;
        }
    }
}
