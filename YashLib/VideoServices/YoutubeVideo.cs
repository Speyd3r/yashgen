using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YashLib.VideoServices
{
    public class YoutubeVideo : IVideo
    {
        private readonly YoutubeClient _youtubeClient;
        private readonly DownloadUtility _downloadUtility;

        public YoutubeVideo(YoutubeClient youtubeClient, DownloadUtility downloadUtility)
        {
            this._youtubeClient = youtubeClient;
            this._downloadUtility = downloadUtility;
        }

        public async Task<string> GetTempAudioLocationAsync(string url)
        {
            var vId = VideoId.TryParse(url);
            if (vId == null) 
                return default;

            var streams = await _youtubeClient.Videos.Streams.GetManifestAsync(vId.Value);
            var audioStreams = streams.GetAudioOnlyStreams();
            var audioStream = audioStreams.GetWithHighestBitrate();

            var result = await _downloadUtility.DownloadUrlAsync(audioStream.Url);
            if (result == default)
                return default;
            var tempPath = $@"temp\\{vId.Value.Value}.weba";
            await File.WriteAllBytesAsync(tempPath, result);

            Console.WriteLine("Stream downloaded");
            return tempPath;
        }
    }
}
