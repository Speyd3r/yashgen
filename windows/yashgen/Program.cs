using System;
using System.Threading.Tasks;
using System.Net.Http;
using YashLib;
using System.IO;

namespace yashgen
{
    class Program
    {
        private const int ExitNoArgs = 3;

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("yashgen video_id");
                Environment.Exit(ExitNoArgs);
            }

            var id = args[0];

            var yl = new YashUtil(new HttpClient());
            var yash = await yl.GenerateYashFileAsync(id);
            await File.WriteAllBytesAsync($@"youtube_{id}.yash", yash);
        }

    }
}
