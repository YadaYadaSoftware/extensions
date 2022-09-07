using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    public TrackingStatus GetStatus(string trackingNumber)
    {
        using (_logger.AddMember())
        {
            _logger.LogTrace("{0}={1},{2}={3}", nameof(_options.AccountId), _options.AccountId, nameof(_options.SecretKey),_options.SecretKey);
            return TrackingStatus.LabelCreated;

        }
    }
}