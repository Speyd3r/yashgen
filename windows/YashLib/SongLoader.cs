using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YashLib
{
    internal class SongLoader
    {
        private readonly MediaPlayer _mediaPlayer;

        public SongLoader(MediaPlayer mediaPlayer)
        {
            this._mediaPlayer = mediaPlayer;
        }

        public List<float> DecodeSongSums(string tempFilename)
        {
            List<float> sums = new List<float>();
            _mediaPlayer.StartPrescan(tempFilename);
            float[] fft = _mediaPlayer.fft;

            float item;
            while (true)
            {
                item = FastDecodeStep(fft);
                if (item < 0f)
                    break;

                sums.Add(item);
            }
            return sums;
        }

        private float FastDecodeStep(float[] fft)
        {
            if (_mediaPlayer.GetFFT512KISS() < 1)
            {
                return -1f;
            }
            float b = 0f;
            for (int i = 1; i < 0x200; i++)
            {
                b += (float)(Math.Sqrt(Math.Max(0f, fft[i])));
            }
            return Math.Max(0f, b);
        }
    }
}
