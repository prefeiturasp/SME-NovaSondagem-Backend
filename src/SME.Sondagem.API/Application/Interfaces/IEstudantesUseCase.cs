using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.Sondagem.Application.Interfaces
{
    public interface IEstudantesUseCase
    {
        Task<IEnumerable<object>> ObterEstudantesAsync();
    }
}
