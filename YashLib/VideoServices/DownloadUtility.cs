using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YashLib.VideoServices
{
    public class DownloadUtility
    {
        private readonly HttpClientManager _httpClientManager;
        private readonly int _connections;

        public DownloadUtility(HttpClientManager httpClientManager, int connections = 10)
        {
            this._httpClientManager = httpClientManager;
            this._connections = connections;
        }

        public async Task<byte[]> DownloadUrlAsync(string url)
        {
            using (var mc = this._httpClientManager.GetClient())
            {

                var raw = await mc.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                if (!raw.IsSuccessStatusCode)
                    return default;
                var size = raw.Content.Headers.ContentLength.Value;
                var partSize = (size) / _connections;
                var tasks = new Task[_connections];
                var result = new Dictionary<int, byte[]>();

                //I dislike this implementation, but I couldnt make a shorter inline version work properly, so it be how it be
                var curStart = 0L;
                var curEnd = partSize;

                for (int i = 0; i < tasks.Length; i++)
                {
                    var j = i;
                    var startCpy = curStart;
                    var endCpy = curEnd;
                    tasks[j] = Task.Run(async () => await DownloadBytesAsync(j, 
                        url, 
                        startCpy, 
                        endCpy, 
                        result));
                    curStart = curEnd + 1;
                    if (j == tasks.Length - 1) curEnd = size;
                    else curEnd = curEnd + partSize + 1; 
                }
                await Task.WhenAll(tasks);

                var full = new byte[Convert.ToInt32(size)];
                var offset = 0;
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < result[i].Length; j++)
                        full[offset + j] = result[i % (_connections - 1)][j];

                    offset += result[i].Length;
                    result.Remove(i);
                }
                Console.WriteLine("Expected size: " + raw.Content.Headers.ContentLength);
                Console.WriteLine("Size got: " + full.Length);
                return full;
            }
        }

        private async Task DownloadBytesAsync(int id, string url, long? start, long? end, Dictionary<int, byte[]> result)
        {
            using (var mc = this._httpClientManager.GetClient())
            {
                using (var msg = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    msg.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(start, end);
                    using (var response = await mc.Client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead))
                    {
                        result.Add(id, await response.Content.ReadAsByteArrayAsync());
                    }
                }
            }
        }
    }

    public class HttpClientManager
    {
        private List<ManagedClient> _managedClients { get; set; } = new();
        static object _lock = new();
        public ManagedClient GetClient()
        {
            lock (_lock)
            {
                if (_managedClients.All(x => x.InUse)
                    || _managedClients.Count == 0)
                {
                    var mc = new ManagedClient
                    {
                        Client = new(),
                        InUse = true
                    };
                    _managedClients.Add(mc);
                    return mc;
                }
                return _managedClients.First(x => !x.InUse);
            }
        }
    }

    public struct ManagedClient : IDisposable
    {
        public bool InUse { get; set; }
        public HttpClient Client { get; set; }

        public void Dispose()
        {
            this.InUse = false;
        }
    }
}
