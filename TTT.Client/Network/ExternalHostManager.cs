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

namespace TTT.Client
{
    public class ExternalHostManager
    {
        public ExternalHostManager()
        {

        }

        public async Task<IPEndPoint> FindOnlineGame()
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync("http://www.goldstein.somee.com/client/connect")
                    .ContinueWith(task => ParseIPEndPoint(task.Result));
            }
        }

        private IPEndPoint ParseIPEndPoint(string result)
        {
            var match = Regex.Match(result, @"ADDRESS:\[(?'IpAddress'.*?)\]##");
            return new IPEndPoint(IPAddress.Parse(match.Groups["IpAddress"].Value), 58008);
        }
    }
}