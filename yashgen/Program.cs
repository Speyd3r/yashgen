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
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No URL specified");
                Environment.Exit(1);
            }
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var util = new YashUtil();
            var data = await util.GenerateYashFileAsync(args[0]);
            if (data == default)
            {
                Console.WriteLine("Error downloading data");
                Environment.Exit(2);
            }
            await File.WriteAllBytesAsync(@"A.yash", data);
            sw.Stop();
            Console.WriteLine($"Finished in {sw.ElapsedMilliseconds}");
            Environment.Exit(0);
        }

    }
}
