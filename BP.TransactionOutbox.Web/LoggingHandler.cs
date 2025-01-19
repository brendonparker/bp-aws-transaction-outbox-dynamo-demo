namespace BP.TransactionOutboxAspire.Web;

public class LoggingHandler(ILogger<LoggingHandler> log) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Log request
        log.LogInformation($"Sending request: {request.Method} {request.RequestUri}");

        if (request.Content != null)
        {
            string requestBody = await request.Content.ReadAsStringAsync();
            log.LogInformation($"Request body: {requestBody}");
        }

        // Send the request to the next handler in the chain
        var response = await base.SendAsync(request, cancellationToken);
        if (response != null)
        {
            // Log response
            log.LogInformation(
                $"Received response: {response.StatusCode} for {response.RequestMessage?.RequestUri}");

            if (response.Content != null)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                log.LogInformation($"Response body: {responseBody}");
            }
        }

        return response!;
    }
}