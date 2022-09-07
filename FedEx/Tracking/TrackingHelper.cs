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
        using (_logger.AddScope(nameof(trackingNumber),trackingNumber))
        using(var httpClient = new HttpClient{BaseAddress = new Uri("https://apis.fedex.com") })
        {
            var client = new FedEx.Tracking.Client(httpClient);
            client.JsonSerializerSettings.Converters.Add(new LocationDetail1Converter());

            var fullSchemaTrackingNumbers = new Full_Schema_Tracking_Numbers();
            fullSchemaTrackingNumbers.TrackingInfo.Add(new MasterTrackingInfo() { TrackingNumberInfo = new TrackingNumberInfo { TrackingNumber = trackingNumber } });
            fullSchemaTrackingNumbers.IncludeDetailedScans = true;

            TrkcResponseVO_TrackingNumber? trackingnumbersAsync = await client.TrackV1TrackingnumbersAsync(fullSchemaTrackingNumbers, Guid.NewGuid().ToString(), "application/json", "en_US", $"Bearer {await _token.GetValueAsync()}");
            foreach (var outputCompleteTrackResult in trackingnumbersAsync.Output.CompleteTrackResults)
            {
                _logger.LogTrace("{0}:{1}={2}", trackingNumber, nameof(outputCompleteTrackResult.TrackResults), outputCompleteTrackResult.TrackResults.Count);

                foreach (var trackResult in outputCompleteTrackResult.TrackResults)
                {
                    
                    _logger.LogTrace(System.Text.Json.JsonSerializer.Serialize(trackResult));
                }
            }
            return TrackingStatus.LabelCreated;

        }
    }
}