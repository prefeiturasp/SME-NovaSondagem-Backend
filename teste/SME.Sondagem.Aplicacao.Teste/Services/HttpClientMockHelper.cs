using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    public static class HttpClientMockHelper
    {
        public static HttpClient Create(
            HttpStatusCode statusCode,
            string? content = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = statusCode,
                   Content = content != null
                       ? new StringContent(content, Encoding.UTF8, "application/json")
                       : null
               });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
        }
    }
}
