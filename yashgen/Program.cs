using System;
using System.Threading.Tasks;
using System.Net.Http;
using YashLib;
using System.IO;
using YashLib.VideoServices;

namespace yashgen
{
    class Program
    {
        private const int ExitNoArgs = 3;

        static async Task Main(string[] args)
        {
            var test = new DownloadUtility(new HttpClientManager());
            //var data = await new HttpClient().GetByteArrayAsync("https://vita.meek.moe/01.%20O-Ku-Ri-Mo-No%20Sunday%21%20%28M%40STER%20VERSION%29.flac");
            var data = await test.DownloadUrlAsync("https://vita.meek.moe/01.%20O-Ku-Ri-Mo-No%20Sunday%21%20%28M%40STER%20VERSION%29.flac");
            await File.WriteAllBytesAsync(@"test.flac", data);
        }

    }
}
