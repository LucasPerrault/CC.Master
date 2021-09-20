using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Testing.Infra
{
    public static class HttpHelper
    {
        public static ISetup<HttpClientHandler, Task<HttpResponseMessage>> SetupSendAsync(this Mock<HttpClientHandler> handler, ItIsRequestMessage requestMessage)
        {
            return handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", requestMessage.Expression, ItExpr.IsAny<CancellationToken>());
        }
    }

    public class ItIsRequestMessage
    {
        internal Expression Expression { get; }

        private ItIsRequestMessage(Expression expression)
        {
            Expression = expression;
        }

        public static ItIsRequestMessage Any() => new ItIsRequestMessage(ItExpr.IsAny<HttpRequestMessage>());
        public static ItIsRequestMessage Matching( Expression<Func<HttpRequestMessage, bool>> expr) => new ItIsRequestMessage(ItExpr.Is(expr));
    }
}
