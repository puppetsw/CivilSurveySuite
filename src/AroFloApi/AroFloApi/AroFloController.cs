using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AroFloApi.Helpers;

namespace AroFloApi
{
    internal class AroFloController
    {
        private const string AROFLO_API_URL = "https://api.aroflo.com/";
        private const string SECRET_KEY = "RHIzTUFiUlJhSUpPenNQaFA2WHBzcGMzYXJlM1RxMCtDVW5uNkRKdnhITzI1S0krNW4vM0NZdk45SnlnNFFTaG1wcnB0WXBlRGMzNlFYeDEwVE9Wbmc9PQ==";
        private const string U_ENCODE = "PjZPQjtBSEM7RihdOjI6JDJMKlwgJiohQ0AxTVw4Klg9Jzk6NDUpWiwK";
        private const string P_ENCODE = "cTdod3FkODFlNnI0TGVk";
        private const string ORG_ENCODE = "JSc6TyBQLFAgCg==";

        private int _currentPage = 1;

        // TODO: Improve zoning request strings to make them more safe and filters.
        internal AroFloController() { }

        internal async Task<List<TObject>> GetAroFloObjectsAsync<TZone, TObject>(CancellationToken cancellationToken = default)
            where TZone : ZoneResult<TObject>
            where TObject : AroFloObject
        {
            Zones zone = Zones.None;

            if (typeof(TZone) == typeof(ProjectZoneResult) &&
                typeof(TObject) == typeof(Project))
            {
                zone = Zones.Projects;
            }
            else if (typeof(TZone) == typeof(LocationZoneResult) &&
                     typeof(TObject) == typeof(Location))
            {
                zone = Zones.Locations;
            }

            if (zone == Zones.None)
            {
                throw new InvalidOperationException("Zone was not set.");
            }

            string zoneRequest = $"zone={zone.GetDescription()}&page=";
            return await GetAroFloObjectsAsync<TZone, TObject>(zoneRequest, cancellationToken);
        }

        /// <summary>
        /// Get an <see cref="AroFloObject"/> from the AroFlo API.
        /// </summary>
        /// <typeparam name="TZone">The <see cref="ZoneResult{T}"/> object.</typeparam>
        /// <typeparam name="TObject">The <see cref="AroFloObject"/> associated with the TZone.</typeparam>
        /// <param name="requestString">The request string. eg. zone=projects&page=</param>
        /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to
        /// receive notice of cancellation.</param>
        /// <remarks>
        /// Leave the page number off the request as the method will automatically get the next page of objects
        /// if required.
        /// </remarks>
        /// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
        private async Task<List<TObject>> GetAroFloObjectsAsync<TZone, TObject>(string requestString,
            CancellationToken cancellationToken = default)
            where TZone : ZoneResult<TObject>
            where TObject : AroFloObject
        {
            var list = new List<TObject>();

            try
            {
                // CONNECTION IS REFUSED IF SECURITY IS NOT SET TO TLS12 for .NET 4.5...
                // https://stackoverflow.com/questions/28286086/default-securityprotocol-in-net-4-5
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                // BASE URL
                var url = new Uri($"{AROFLO_API_URL}?{requestString}{_currentPage}");

                var responseData = await SendAroFloRequestAsync(url, requestString, cancellationToken);
                var aroFloObject = await Deserialize<TZone, TObject>(responseData);

                list.AddRange(aroFloObject.ZoneResponse.GetResults());

                do
                {
                    _currentPage++;

                    url = new Uri($"{AROFLO_API_URL}?{requestString}{_currentPage}");

                    var nextPage = await SendAroFloRequestAsync(url, requestString, cancellationToken);
                    aroFloObject = await Deserialize<TZone, TObject>(nextPage);

                    list.AddRange(aroFloObject.ZoneResponse.GetResults());

                } while (aroFloObject.ZoneResponse.IsMorePages);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"GET task was cancelled.");
            }

            return list;
        }

        private async Task<string> SendAroFloRequestAsync(Uri uri, string requestString, CancellationToken cancellationToken = default)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    AddHeaders(request, requestString, _currentPage);

                    Console.WriteLine($"Creating connection: {uri}");

                    Stopwatch timer = Stopwatch.StartNew();
                    var responseContent = await client.SendAsync(request, cancellationToken);
                    timer.Stop();
                    var timespan = timer.Elapsed;
                    Console.WriteLine($"GET time elapsed: {timespan.Hours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}");

                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Fetch Cancelled
                        Console.WriteLine("GET was cancelled.");
                    }

                    var responseData = await responseContent.Content.ReadAsStringAsync();

                    return responseData;
                }
            }
        }

        private static Task<AroFloResponse<TZoneResult, TAroFloObject>> Deserialize<TZoneResult, TAroFloObject>(string aroFloObject)
            where TZoneResult : ZoneResult<TAroFloObject>
            where TAroFloObject : AroFloObject
        {
            var xml = new XmlSerializer(typeof(AroFloResponse<TZoneResult, TAroFloObject>));

            using (var reader = new StringReader(aroFloObject))
            {
                var response = (AroFloResponse<TZoneResult, TAroFloObject>)xml.Deserialize(reader);
                return Task.FromResult(response);
            }
        }

        private static void AddHeaders(HttpRequestMessage requestMessage, string zoneString, int page)
        {
            // HEADER SECTION
            // DATE
            var isoTimeStamp = DateTime.UtcNow.ToString("o");
            const string urlPath = "";
            const string accept = "text/xml";

            // AUTHORIZATION
            var authString = new StringBuilder();
            authString.Append($"uencoded={Uri.EscapeDataString(U_ENCODE)}");
            authString.Append($"&pencoded={Uri.EscapeDataString(P_ENCODE)}");
            authString.Append($"&orgEncoded={Uri.EscapeDataString(ORG_ENCODE)}");
            var authorization = authString.ToString();

            // Create the payload for hashing
            var payloadArray = new List<string>
            {
                "GET",
                urlPath,
                accept,
                authorization,
                isoTimeStamp,
                $"{zoneString}{page}"
            };

            // PAYLOAD
            var payload = string.Join("+", payloadArray.ToArray());
            Console.WriteLine($"Header payload: {payload}");

            // HASH
            var hash = GetHash(payload, SECRET_KEY);

            requestMessage.Headers.Add("Authentication", $"HMAC {hash}");
            // ADD WITHOUT VALIDATION
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authorization);
            requestMessage.Headers.Add("Accept", accept);
            requestMessage.Headers.Add("afdatetimeutc", isoTimeStamp);
        }

        private static string GetHash(string text, string key)
        {
            using (var hmacsha = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmacsha.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
