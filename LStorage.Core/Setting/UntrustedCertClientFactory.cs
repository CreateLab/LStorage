using Flurl.Http.Configuration;

namespace LStorage.Core.Setting;

public class UntrustedCertClientFactory: DefaultHttpClientFactory
{
    public override HttpMessageHandler CreateMessageHandler() {
        return new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
    }
}