using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Magnanibot.Extensions
{
    public static class UriExtensions
    {
        /// <summary> 
        ///   Fetches an image from a URL as a base 64 string. 
        /// </summary>
        /// <param name="url">The url to fetch from.</param>
        /// <returns>A base 64 string.</returns>
        public static async Task<string> AsBase64Url(this Uri url)
        {
            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(url.AbsoluteUri);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
