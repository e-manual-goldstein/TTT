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
            _logger.Debug("Finding online game...");
            using (var client = new HttpClient())
            {
                var httpTask = client.GetStringAsync("http://www.goldstein.somee.com/client/connect");
                return await httpTask.ContinueWith(task => 
                    {
                        _logger.Debug("Parsing results");
                        var endPoint = ParseIPEndPoint(task.Result);
                        _logger.Debug("Found game");
                        return endPoint;
                    });
            }
        }

        private IPEndPoint ParseIPEndPoint(string result)
        {
            var match = Regex.Match(result, @"ADDRESS:\[(?'IpAddress'.*?)\]##");
            _logger.Debug($"Host Address: {match.Groups["IpAddress"].Value}");
            return IPAddress.TryParse(match.Groups["IpAddress"].Value, out var iPAddress)
                ? new IPEndPoint(iPAddress, 58008) 
                : null;
        }

        public IPEndPoint ForceIPEndPoint()
        {
            _logger.Debug("Forcing IP Address");
            return IPAddress.TryParse("99.238.159.30", out var iPAddress)
                ? new IPEndPoint(iPAddress, 58008)
                : null;
        }
    }
}