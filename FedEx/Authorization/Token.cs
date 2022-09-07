using Microsoft.Extensions.Options;

namespace FedEx.Authorization;

public class Token
{
    private readonly FedExOptions _options;
    private string? _value = null;

    public Token(IOptions<FedExOptions> options)
    {
        _options = options.Value;
    }


    public async Task<string> GetValueAsync()
    {
        if (_value != null) return _value;

        using var httpClient = new HttpClient { BaseAddress = new Uri("https://apis.fedex.com") };
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
        _value = (await authClient.OauthTokenAsync("application/json", fullSchema)).Access_token;

        return _value;

    }
}