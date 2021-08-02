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
        private readonly HttpClient _httpClient;
        private readonly YoutubeClient _youtubeClient;

        public YashUtil(HttpClient httpClient, YoutubeClient youtubeClient = null)
        {
            this._httpClient = httpClient;
            this._youtubeClient = youtubeClient ?? new YoutubeClient();
        }
        public async Task<byte[]> GenerateYashFileAsync(string url)
        {
            var yt = new YoutubeVideo(_httpClient, _youtubeClient);
            var audioData = await yt.GetAudioBytesAsync(url);

            //fix later
            var id = VideoId.TryParse(url);

            var tempPath = $@"temp\\{id.Value.Value}.weba";
            #region Dumb temp file save stuff
            if (File.Exists(tempPath))
                File.Delete(tempPath);

            await File.WriteAllBytesAsync(tempPath, audioData);

            //So by mistakeIi found out UMP/libASM actually can read weba (Youtubes regular audio format)
            //though maybe not vp9/some specific opus version, but who knows
            //To keep in line with the OG yashgen I'm still converting to m4a
            var proc = Process.Start(new ProcessStartInfo
            {
#if !LINUX
                FileName = "ffmpeg.exe",
#elif LINUX
                FileName = "ffmpeg",
#endif
                Arguments = $"-i {tempPath} {tempPath.Replace("weba", "m4a")}"
            });
            await proc.WaitForExitAsync();
            File.Delete(tempPath);
            #endregion
            tempPath = $@"temp\\{id.Value.Value}.m4a";

            float duration = default;
            using (TagLib.File file = TagLib.File.Create(tempPath))
            {
                duration = (float)file.Properties.Duration.TotalSeconds;
            }

            try
            {
                byte[] yashData = default;
                using (var mp = new MediaPlayer())
                {
                    //Its technically possible to just get the duration from UMP/libASM
                    //duration = mp.GetDuration(tempPath.Replace("weba", "m4a"));
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
