using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using WebScrape.Common;

namespace WebScrape
{
    public class HttpRetryMessageHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        public HttpRetryMessageHandler(HttpClientHandler handler, ILogger logger) : base(handler)
        {
            _logger = logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, (retryAttempt) =>
                {
                    _logger.LogError($"Retrying {request.RequestUri}, attempt {retryAttempt}");
                    return TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                })
                
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}