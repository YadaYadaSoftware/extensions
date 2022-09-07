using FedEx.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FedEx.Tracking;

public class TrackingHelper
{
    private readonly Token _token;
    private readonly ILogger _logger;

    public TrackingHelper(ILoggerProvider loggerProvider, Token token)
    {
        _token = token;
        _logger = loggerProvider.CreateLogger(typeof(TrackingHelper).FullName!);
    }

    // ReSharper disable once UnusedMember.Global
    public async Task<TrackingStatus> GetStatusAsync(string trackingNumber)
    {
        using (_logger.AddMember())
        using(var httpClient = new HttpClient{BaseAddress = new Uri("https://apis.fedex.com") })
        {
            _logger.LogTrace("{0}={1},{2}={3}", nameof(_token.GetValueAsync), await _token.GetValueAsync(), nameof(trackingNumber), trackingNumber);

            var client = new FedEx.Tracking.Client(httpClient);

            var fullSchemaTrackingNumbers = new Full_Schema_Tracking_Numbers();
            fullSchemaTrackingNumbers.TrackingInfo.Add(new MasterTrackingInfo() { TrackingNumberInfo = new TrackingNumberInfo { TrackingNumber = trackingNumber } });

            TrkcResponseVO_TrackingNumber? trackingnumbersAsync = await client.TrackV1TrackingnumbersAsync(fullSchemaTrackingNumbers, Guid.NewGuid().ToString(), "application/json", "en_US", $"Bearer {await _token.GetValueAsync()}");
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