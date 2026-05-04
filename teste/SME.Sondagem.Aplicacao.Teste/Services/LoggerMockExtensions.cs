using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Aplicacao.Teste.Services
{
    [ExcludeFromCodeCoverage]
    public static class LoggerMockExtensions
    {
        [SuppressMessage("Major Code Smell", "S2629:Logging templates should be constant",
            Justification = "Expressão é apenas uma verificação de mock do Moq, não há logging real.")]
        [SuppressMessage("Performance", "CA1873:Evaluation of this argument may be expensive and unnecessary if logging is disabled",
            Justification = "This is a mock verification for tests, not actual logging.")]
        public static void VerifyLog<T, TException>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
            where TException : Exception
        {
            logger.Verify(l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<TException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
        }
    }
}
