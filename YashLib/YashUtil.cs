using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YashLib.VideoServices;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace YashLib
{
    public class YashUtil
    {
        private readonly YoutubeClient _youtubeClient;
        private readonly DownloadUtility _downloadUtility;

        public YashUtil(YoutubeClient youtubeClient = null, DownloadUtility downloadUtility = null)
        {
            this._youtubeClient = youtubeClient ?? new YoutubeClient();
            this._downloadUtility = downloadUtility ?? new DownloadUtility(new HttpClientManager());
        }
        public async Task<byte[]> GenerateYashFileAsync(string url)
        {
            var yt = new YoutubeVideo(_youtubeClient, _downloadUtility);

            var tempPath = await yt.GetTempAudioLocationAsync(url);
            if (tempPath == default)
                return default;

            float duration = default;

            try
            {
                byte[] yashData = default;
                using (var mp = new MediaPlayer())
                {
                    //Its technically possible to just get the duration from UMP/libASM
                    duration = mp.GetDuration(tempPath);

                    var sl = new SongLoader(mp);
                    var sums = sl.DecodeSongSums(tempPath);
                    yashData = GenerateYashFile(sums, duration);
                }
                File.Delete(tempPath);
                return yashData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default;
            }
        }

        private byte[] GenerateYashFile(List<float> sums, float duration)
        {
            const int HEADER_SIZE = 3 * 4;
            var sumBytes = new byte[HEADER_SIZE + (sums.Count * 4)];
            Buffer.BlockCopy(BitConverter.GetBytes(1), 0, sumBytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(duration), 0, sumBytes, 1 * 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(sums.Count), 0, sumBytes, 2 * 4, 4);
            Buffer.BlockCopy(sums.ToArray(), 0, sumBytes, 3 * 4, sums.Count * 4);
            return sumBytes;
        }
    }
}
