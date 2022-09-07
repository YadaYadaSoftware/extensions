using System.Reflection;
using FedEx.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FedEx.Tracking;

public class TrackingHelper
{
    private readonly FedExOptions _options;
    private readonly ILogger _logger;

    public TrackingHelper(IOptions<FedExOptions> options, ILoggerProvider loggerProvider)
    {
        _options = options.Value;
        _logger = loggerProvider.CreateLogger(typeof(TrackingHelper).FullName!);
    }

    // ReSharper disable once UnusedMember.Global
    public async Task<TrackingStatus> GetStatus(string trackingNumber)
    {
        using (_logger.AddMember())
        using(var httpClient = new HttpClient{BaseAddress = new Uri("https://apis.fedex.com") })
        {
            _logger.LogTrace("{0}={1},{2}={3},{4}={5}", nameof(_options.AccountId), _options.AccountId, nameof(_options.SecretKey), _options.SecretKey, nameof(trackingNumber), trackingNumber);

            var authClient = new FedEx.Authorization.Client(httpClient)
            {
                BaseUrl = "https://apis.fedex.com"
            };
            var fullSchema = new FullSchema
            {
                Grant_type = "client_credentials",
                Client_id = _options.AccountId,
                Client_secret = _options.SecretKey
            };
            var oauthTokenAsync = await authClient.OauthTokenAsync("application/json", fullSchema);
            var client = new FedEx.Tracking.Client(httpClient)
            {
                JsonSerializerSettings =
                {
                    ContractResolver = new SafeContractResolver()
                }
            };

            var fullSchemaTrackingNumbers = new Full_Schema_Tracking_Numbers();
            fullSchemaTrackingNumbers.TrackingInfo.Add(new MasterTrackingInfo() { TrackingNumberInfo = new TrackingNumberInfo { TrackingNumber = trackingNumber } });

            var trackingnumbersAsync = await client.TrackV1TrackingnumbersAsync(fullSchemaTrackingNumbers, Guid.NewGuid().ToString(), "application/json", "en_US", $"Bearer {oauthTokenAsync.Access_token}");
            foreach (var outputCompleteTrackResult in trackingnumbersAsync.Output.CompleteTrackResults)
            {
                foreach (var trackResult in outputCompleteTrackResult.TrackResults)
                {
                    
                    _logger.LogTrace($"{trackResult.TrackingNumberInfo.TrackingNumber}:{System.Text.Json.JsonSerializer.Serialize(trackResult)}");
                }
            }
            return TrackingStatus.LabelCreated;

        }
    }
}
public class SafeContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var jsonProp = base.CreateProperty(member, memberSerialization);
        jsonProp.Required = Required.Default;
        return jsonProp;
    }
}