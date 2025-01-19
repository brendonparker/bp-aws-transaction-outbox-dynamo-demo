using Amazon.Runtime;

namespace BP.TransactionOutboxAspire.Web;

public class CustomHttpClientFactory(HttpClient httpClient) : HttpClientFactory
{
    public override HttpClient CreateHttpClient(IClientConfig clientConfig) =>
        httpClient;
}