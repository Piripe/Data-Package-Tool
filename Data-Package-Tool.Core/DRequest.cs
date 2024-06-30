using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataPackageTool.Core
{
    public enum DRequestContext
    {
        None,
        Invite,
        User,
        Bot
    }
    class DSuperProperties
    {
        public string? os;
        public string? browser;
        public string? device;
        public string? system_locale;
        public string? browser_user_agent;
        public string? browser_version;
        public string? os_version;
        public string? referrer;
        public string? referring_domain;
        public string? referrer_current;
        public string? referring_domain_current;
        public string? release_channel;
        public int? client_build_number;
        public string? client_event_source;

    }
    class DHeaders
    {
        public static string? BROWSER_VERSION;
        public static string? BROWSER_VERSION_FULL;

        public static string? USER_AGENT;

        public static int ClientBuildNumber;

        private static bool _initialized = false;

        public static async Task Init()
        {
            if (_initialized) return;

            ClientBuildNumber = await GetLatestBuildNumber();

            BROWSER_VERSION = await GetLatestChromeVersion();
            BROWSER_VERSION_FULL = $"{BROWSER_VERSION}.0.0.0";
            USER_AGENT = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{BROWSER_VERSION_FULL} Safari/537.36";

            _initialized = true;
        }

        public static string SuperProperties()
        {
            //throw new NotImplementedException();
            if (!_initialized) throw new Exception("Headers not initialized");

            var propsJson = JsonSerializer.Serialize(new DSuperProperties
            {
                os = "Windows",
                browser = "Chrome",
                device = "",
                system_locale = "en-US",
                browser_user_agent = USER_AGENT,
                browser_version = BROWSER_VERSION_FULL,
                os_version = "10",
                referrer = "",
                referring_domain = "",
                referrer_current = "",
                referring_domain_current = "",
                release_channel = "canary",
                client_build_number = ClientBuildNumber,
                client_event_source = null
            });

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(propsJson));
        }

        public static Dictionary<string, string> DefaultBrowserHeaders(Dictionary<string, string>? extraHeaders = null)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>{
                {"Accept", "*/*"},
                {"Accept-Language", "en-US,en;q=0.5"},
                {"sec-ch-ua", $"\"Not.A/Brand\";v=\"8\", \"Chromium\";v=\"{BROWSER_VERSION}\", \"Google Chrome\";v=\"{BROWSER_VERSION}\""},
                {"sec-ch-ua-mobile", "?0"},
                {"sec-ch-ua-platform", "\"Windows\""},
                {"sec-fetch-dest", "empty"},
                {"sec-fetch-mode", "cors"},
                {"sec-fetch-site", "same-origin"},
                {"User-Agent", USER_AGENT!},
                {"x-debug-options", "bugReporterEnabled"},
                {"x-discord-locale", "en-US"},
                {"x-discord-timezone", "America/New_York"},
                {"x-super-properties", SuperProperties()}
            };

            if (extraHeaders != null)
            {
                foreach (KeyValuePair<string, string> kvp in extraHeaders)
                {
                    headers.Add(kvp.Key, kvp.Value);
                }
            }

            return headers;
        }

        public static async Task<int> GetLatestBuildNumber()
        {
            var client = new HttpClient();

            var res = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://canary.discord.com/app"));
            var content = await res.Content.ReadAsStringAsync();
            foreach (Match match in Regex.Matches(content, "<script src=\"(\\/assets\\/.+?\\.js)", RegexOptions.None))
            {
                var scriptPath = match.Groups[1].Value;
                var scriptRes = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://canary.discord.com{scriptPath}"));
                var scriptContent = await scriptRes.Content.ReadAsStringAsync();
                if (scriptContent.Contains("build_number"))
                {
                    var buildNumber = Regex.Match(scriptContent, "build_number:\"(\\d+)\"").Groups[1].Value;
                    if (buildNumber != "") return Int32.Parse(buildNumber);
                }
            }

            throw new Exception("Failed to get client build number");
        }

        public static async Task<string> GetLatestChromeVersion()
        {
            return "126";
            var res = await new HttpClient().SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://versionhistory.googleapis.com/v1/chrome/platforms/win/channels/stable/versions"));
            var content = await res.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<dynamic>(content);
            string latest = data?["versions"][0]["version"] ?? "126.0.6478.61";
            var majorNum = latest.Split('.')[0];

            return majorNum;
        }
    }
    class DRequestQueue
    {
        public static readonly Dictionary<string, Action<DRequestQueue>> DRequestQueueOptions = new () {
            { "invite", (queue) => {queue.RetryAfter = DateTime.Now.AddSeconds(1);} }
        };

public Queue<DRequest> Queue { get; } = new();
        public DateTime RetryAfter { get; set; } = DateTime.Now;
        public Action<DRequestQueue>? AfterRes { get; set; }
    }
    public class DRequest
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public bool IsCDN { get; set; }
        public DRequestContext Context { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? BodyData { get; set; }
        public bool IncludeDefaultHeaders { get; set; }
        public TaskCompletionSource<HttpResponseMessage>? OnResponse { get; set; }
        public uint MaxRetries { get; set; }
        public string Queue { get; set; }

        private DRequest(HttpMethod method, string url, bool isCDN, DRequestContext context, Dictionary<string, string>? headers, string? bodyData, bool includeDefaultHeaders, TaskCompletionSource<HttpResponseMessage>? onResponse, uint maxRetries, string queue = "main")
        {
            Method = method;
            Url = url;
            IsCDN = isCDN;
            Context = context;
            Headers = headers;
            BodyData = bodyData;
            IncludeDefaultHeaders = includeDefaultHeaders;
            OnResponse = onResponse;
            MaxRetries = maxRetries;
            Queue = queue;
        }

        public static HttpClient client = new HttpClient();
        public static Dictionary<DRequestContext,string> Contexts = new();
        private static Dictionary<string, DRequestQueue> _requestsQueues = new();
        private static ConcurrentQueue<KeyValuePair<string, DRequest>> _asyncQueue = new();
        private static CancellationTokenSource _restartHandler = new CancellationTokenSource();

        private static Task _ = RequestHandler();

        private static async Task RequestHandler()
        {
            while (true)
            {
                bool error = false;
                try
                {
                    bool waiting = true;
                    while (_asyncQueue.Count > 0)
                    {
                        try
                        {
                            if (_asyncQueue.TryDequeue(out var pendingDreq))
                            {
                                if (!_requestsQueues.ContainsKey(pendingDreq.Key)) _requestsQueues.Add(pendingDreq.Key, new DRequestQueue() { AfterRes = DRequestQueue.DRequestQueueOptions.TryGetValue(pendingDreq.Key, out var options) ? options : null });
                                _requestsQueues[pendingDreq.Key].Queue.Enqueue(pendingDreq.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                            waiting = false;
                            error = true;
                            break;
                        }
                    }

                    foreach (var queue in _requestsQueues.Values)
                    {
                        if (queue.RetryAfter > DateTime.Now) continue;
                        if (queue.Queue.Count == 0) continue;

                        waiting = false;

                        var dreq = queue.Queue.Peek();
                        Debug.WriteLine($"Requesting ({dreq.Url}) in queue {dreq.Queue} with context {dreq.Context}");
                        try
                        {
                            var request = new HttpRequestMessage(dreq.Method, new Uri(new Uri(dreq.IsCDN ? Constants.CDNEndpoint : Constants.APIEndpoint), dreq.Url));

                            switch (dreq.Context)
                            {
                                case DRequestContext.User:
                                    request.Headers.Add("Authorization", Contexts[dreq.Context]);
                                    break;
                                case DRequestContext.Bot:
                                    request.Headers.Add("Authorization", "Bot "+Contexts[dreq.Context]);
                                    break;
                            }

                            if (dreq.IncludeDefaultHeaders)
                            {
                                if (DHeaders.USER_AGENT == null) await DHeaders.Init();
                                foreach (KeyValuePair<string, string> kvp in DHeaders.DefaultBrowserHeaders(dreq.Headers))
                                {
                                    request.Headers.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else if (dreq.Headers != null)
                            {
                                foreach (KeyValuePair<string, string> kvp in dreq.Headers)
                                {
                                    request.Headers.Add(kvp.Key, kvp.Value);
                                }
                            }

                            if (dreq.BodyData != null)
                            {
                                request.Content = new StringContent(dreq.BodyData, Encoding.UTF8, "application/json");
                            }

                            HttpResponseMessage res = await client.SendAsync(request);

                            if (res.StatusCode == HttpStatusCode.TooManyRequests)
                            {
                                if (dreq.MaxRetries > 0)
                                {
                                    dreq.MaxRetries--;

                                    if (res.Headers.RetryAfter != null && res.Headers.RetryAfter.Delta != null) queue.RetryAfter = DateTime.Now.AddSeconds(Math.Min(60 * 5, res.Headers.RetryAfter.Delta.Value.TotalSeconds));
                                    else queue.RetryAfter = DateTime.Now.AddSeconds(30);

                                    continue;
                                }
                            }
                            queue.Queue.Dequeue();
                            dreq.OnResponse?.TrySetResult(res);
                            queue.AfterRes?.Invoke(queue);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                            error = true;
                        }
                    }

                    if (waiting)
                    {
                        if (_requestsQueues.Values.Select(x => x.Queue.Count).Sum() == 0)
                        {
                            _restartHandler = new CancellationTokenSource();
                            await Task.Delay(30000, _restartHandler.Token).ContinueWith(_ => { });
                        }
                        else
                        {
                            int delay = _requestsQueues.Values.Select(x => (int)x.RetryAfter.Subtract(DateTime.Now).TotalMilliseconds).Where(x => x > 0).Min();
                            if (delay == 0) continue;
                            _restartHandler = new CancellationTokenSource();
                            await Task.Delay(delay, _restartHandler.Token).ContinueWith(_ => { });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    error = true;
                }
                if (error) await Task.Delay(500); // Wait a bit when error happen to avoid freezes
            }
        }
        public static async Task<HttpResponseMessage> RequestAsync(DRequest dreq)
        {
            var onResponse = new TaskCompletionSource<HttpResponseMessage>();

            dreq.OnResponse = onResponse;

            _asyncQueue.Enqueue(new KeyValuePair<string, DRequest>(dreq.Queue, dreq));

            if (!_restartHandler.IsCancellationRequested) _restartHandler.Cancel(); // Restart request handler is it's stopped

            HttpResponseMessage res = await onResponse.Task;

            return res;
        }
        public static async Task<Stream?> GetStreamAsync(DRequest dreq)
        {
            var res = await RequestAsync(dreq);
            if (res.IsSuccessStatusCode)
            {
                return res.Content.ReadAsStream();
            }
            return null;
        }
        public static async Task<string?> GetStringAsync(DRequest dreq)
        {
            var res = await RequestAsync(dreq);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadAsStringAsync();
            }
            return null;
        }

        public static DRequest Get(string url, bool isCDN = false, DRequestContext context = DRequestContext.None, Dictionary<string, string>? headers = null, bool defaultHeaders = true, uint maxRetries = 10, string queue = "main") => new DRequest(HttpMethod.Get, url, isCDN, context, headers, null, defaultHeaders, null, maxRetries, queue);
    }
}