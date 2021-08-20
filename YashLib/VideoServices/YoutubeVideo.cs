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
        private readonly HttpClient _httpClient;
        private readonly YoutubeClient _youtubeClient;

        public YoutubeVideo(HttpClient httpClient, YoutubeClient youtubeClient)
        {
            this._httpClient = httpClient;
            this._youtubeClient = youtubeClient;
        }

        public async Task<string> GetTempAudioLocationAsync(string url)
        {
            var vId = VideoId.TryParse(url);
            if (vId == null) 
                return default;

            var streams = await _youtubeClient.Videos.Streams.GetManifestAsync(vId.Value);
            var audioStreams = streams.GetAudioOnlyStreams();
            var audioStream = audioStreams.GetWithHighestBitrate();

            var result = await _httpClient.GetAsync(audioStream.Url, HttpCompletionOption.ResponseHeadersRead);
            var tempPath = $@"temp\\{vId.Value.Value}.weba";

            Console.WriteLine("Stream downloaded");
            return tempPath;
        }
    }
}
