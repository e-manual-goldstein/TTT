using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class ExternalHostManager
    {
        Logger _logger;

        public ExternalHostManager(Logger logger)
        {
            _logger = logger;
        }

        public async Task<IPEndPoint> FindOnlineGame()
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync("http://www.goldstein.somee.com/client/connect")
                .ContinueWith(task => 
                {
                    _logger.Debug("Found game");
                    return ParseIPEndPoint(task.Result);
                });
            }
        }

        private IPEndPoint ParseIPEndPoint(string result)
        {
            var match = Regex.Match(result, @"ADDRESS:\[(?'IpAddress'.*?)\]##");
            _logger.Debug($"Host Address: {match.Groups["IpAddress"].Value}");
            return new IPEndPoint(IPAddress.Parse(match.Groups["IpAddress"].Value), 58008);
        }
    }
}