using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.Sondagem.Application.Interfaces
{
    public interface IQuestionarioUseCase
    {
        Task<IEnumerable<object>> ObterQuestionariosAsync();
    }
}
