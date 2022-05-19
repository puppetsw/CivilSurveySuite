using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AroFloApi.Enums;
using AroFloApi.Exceptions;
using AroFloApi.Helpers;
using AroFloApi.Models;
using AroFloApi.Responses;

namespace AroFloApi
{
    internal class AroFloController
    {
        private readonly Dictionary<Type, Zone> _zones;

        private int _currentPage = 1;

        public AroFloController()
        {
            _zones = new Dictionary<Type, Zone>
            {
                { typeof(ProjectZoneResponse), Zone.Projects },
                { typeof(LocationZoneResponse), Zone.Locations }
            };
        }

        internal async Task<TObjectType> GetAroFloObject<TZoneResponse, TObjectType>(Field field, string value, CancellationToken cancellationToken = default)
            where TZoneResponse : ZoneResponse<TObjectType>
            where TObjectType : AroFloObject
        {
            // convert field and value into where string
            // where=and|status|=|pending& zone=Invoices&page=1
            // build filter string.
            // where content has to be URI encoded
            var filterString = $"where={Uri.EscapeDataString($"and|{field.GetDescription()}|=|{value}")}";

            // add filter string to request.
            string zoneRequest = $"{filterString}&zone={GetZoneFromResponseType(typeof(TZoneResponse)).GetDescription()}&page=";
            var result = await GetAroFloObjectsAsync<TZoneResponse, TObjectType>(zoneRequest, cancellationToken);

            // return the first only?
            return result?.FirstOrDefault();
        }

        internal async Task<List<TObjectType>> GetAroFloObjectsAsync<TZoneResponse, TObjectType>(Field field, string value, CancellationToken cancellationToken = default)
            where TZoneResponse : ZoneResponse<TObjectType>
            where TObjectType : AroFloObject
        {
            var filterString = $"where={Uri.EscapeDataString($"and|{field.GetDescription()}|=|{value}")}";

            // add filter string to request.
            string zoneRequest = $"{filterString}&zone={GetZoneFromResponseType(typeof(TZoneResponse)).GetDescription()}&page=";
            var result = await GetAroFloObjectsAsync<TZoneResponse, TObjectType>(zoneRequest, cancellationToken);
            return result;
        }

        internal async Task<List<TObjectType>> GetAroFloObjectsAsync<TZoneResponse, TObjectType>(CancellationToken cancellationToken = default)
            where TZoneResponse : ZoneResponse<TObjectType>
            where TObjectType : AroFloObject
        {
            string zoneRequest = $"zone={GetZoneFromResponseType(typeof(TZoneResponse)).GetDescription()}&page=";
            return await GetAroFloObjectsAsync<TZoneResponse, TObjectType>(zoneRequest, cancellationToken);
        }

        private async Task<List<TObjectType>> GetAroFloObjectsAsync<TZoneResponse, TObjectType>(string requestString,
            CancellationToken cancellationToken = default, bool firstPageOnly = false)
            where TZoneResponse : ZoneResponse<TObjectType>
            where TObjectType : AroFloObject
        {
            VerifyConfiguration();

            var list = new List<TObjectType>();

            try
            {
                // CONNECTION IS REFUSED IF SECURITY IS NOT SET TO TLS12 for .NET 4.5...
                // https://stackoverflow.com/questions/28286086/default-securityprotocol-in-net-4-5
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                // BASE URL
                var url = new Uri($"{AroFloConfiguration.AROFLO_API_URL}?{requestString}{_currentPage}");

                var responseData = await SendAroFloRequestAsync(url, requestString, cancellationToken);
                var aroFloObject = Deserialize<TZoneResponse, TObjectType>(responseData);

                switch (aroFloObject.Status)
                {
                    case Status.AuthenticationFailed:
                        throw new AuthenticationException();
                    case Status.LoginOk:
                        break;
                    case Status.LoginFailedInvalidRequestString:
                        throw new LoginFailedException();
                    case Status.LoginFailedInvalidUsernamePassword:
                        throw new LoginFailedException();
                    case Status.LoginFailedPermissionDenied:
                        throw new LoginFailedException();
                    case Status.LoginFailedPermissionDeniedNoApiAccess:
                        throw new LoginFailedException();
                    case Status.InvalidRequestMethod:
                        throw new InvalidRequestException();
                    case Status.ExceededRateLimit:
                        throw new RateLimitException();
                    case Status.ExceededRateLimitDaily:
                        throw new RateLimitException();
                    case Status.ExceededSizeLimit:
                        throw new SizeLimitException();
                    case Status.LoginFailedPermissionDeniedLegacy:
                        throw new LoginFailedException();
                    case Status.LoginFailedPermissionDeniedApiDisabled:
                        throw new LoginFailedException();
                    case Status.TooManyRequests:
                        throw new TooManyRequestsException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                list.AddRange(aroFloObject.ZoneResponse.GetContent());

                do
                {
                    _currentPage++;

                    url = new Uri($"{AroFloConfiguration.AROFLO_API_URL}?{requestString}{_currentPage}");

                    var nextPage = await SendAroFloRequestAsync(url, requestString, cancellationToken);
                    aroFloObject = Deserialize<TZoneResponse, TObjectType>(nextPage);

                    list.AddRange(aroFloObject.ZoneResponse.GetContent());

                } while (aroFloObject.ZoneResponse.IsMorePages && !firstPageOnly);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"GET task was cancelled.");
            }

            return list;
        }

        private Zone GetZoneFromResponseType(Type responseType)
        {
            return _zones[responseType];
        }

        private static void VerifyConfiguration()
        {
            if (string.IsNullOrEmpty(AroFloConfiguration.SECRET_KEY))
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(AroFloConfiguration.U_ENCODE))
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(AroFloConfiguration.P_ENCODE))
                throw new InvalidOperationException();

            if (string.IsNullOrEmpty(AroFloConfiguration.ORG_ENCODE))
                throw new InvalidOperationException();
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

        private static AroFloResponse<TZoneResult, TObjectType> Deserialize<TZoneResult, TObjectType>(string aroFloObject)
            where TZoneResult : ZoneResponse<TObjectType>
            where TObjectType : AroFloObject
        {
            var xml = new XmlSerializer(typeof(AroFloResponse<TZoneResult, TObjectType>));

            using (var reader = new StringReader(aroFloObject))
            {
                return (AroFloResponse<TZoneResult, TObjectType>)xml.Deserialize(reader);
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
            authString.Append($"uencoded={Uri.EscapeDataString(AroFloConfiguration.U_ENCODE)}");
            authString.Append($"&pencoded={Uri.EscapeDataString(AroFloConfiguration.P_ENCODE)}");
            authString.Append($"&orgEncoded={Uri.EscapeDataString(AroFloConfiguration.ORG_ENCODE)}");
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
            var hash = GetHash(payload, AroFloConfiguration.SECRET_KEY);

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
