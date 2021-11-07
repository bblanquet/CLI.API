using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;
using CLICKA.TechnicalAssessment.Model;

namespace CLICKA.TechnicalAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommunicationController : ControllerBase
    {
        [HttpGet("list")]
        public IpContainer Get()
        {
            var result = new IpContainer
            {
                Ips = GetIps().ToArray()
            };
            return result;
        }

        [HttpGet("broadcast")]
        public async Task<List<PingData>> Broadcast()
        {
            var result = new List<PingData>();
            foreach (var ip in GetIps())
            {
                var res = await Send($"http://{ip}/communication/ping");
                result.Add(res);
            }
            return result;
        }

        private static IEnumerable<string> GetIps()
        {
            return Environment.GetEnvironmentVariable("LIST").Split(',').Except(new[] { Environment.GetEnvironmentVariable("IP") });
        }

        [HttpGet("Send")]
        public async Task<PingData> SendPing(string ip)
        {
            var ips = Environment.GetEnvironmentVariable("LIST").Split(',');
            if (ips.Contains(ip))
            {
                var result =  await Send($"http://{ip}/communication/ping");
                return result;
            }
            else {
                throw new ArgumentOutOfRangeException($"{ip} is not present in IP tables.");
            }
        }

        public async static Task<PingData> Send(string endpoint)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(3);
            var watch = Stopwatch.StartNew();
            Console.WriteLine($"sending ping {endpoint}");
            try
            {
                var result = await httpClient.GetAsync(endpoint);
                if (result.IsSuccessStatusCode)
                {
                    return new PingData()
                    {
                        Endpoint = endpoint,
                        IsSuccess = true,
                        Duration = $"{watch.ElapsedMilliseconds}ms"
                    };
                }
                else
                {
                    return new PingData()
                    {
                        Endpoint = endpoint,
                        IsSuccess = false,
                        Duration = $"{watch.ElapsedMilliseconds}ms",
                        ErrorMessage = result.Content.ToString()
                    };
                }
            }
            catch (Exception e) {
                return new PingData()
                {
                    Endpoint = endpoint,
                    IsSuccess = false,
                    Duration = $"{watch.ElapsedMilliseconds}ms",
                    ErrorMessage = e.Message
                };
            }
        }

        [HttpGet("ping")]
        public string Ping()
        {
            return "pong";
        }
    }
}
