using System;
using System.Collections.Generic;
using System.Text;

namespace SME.Sondagem.Infrastructure.Interfaces
{
    public interface IServicoUsuario
    {
        string ObterUsuarioLogado();
        string ObterRFUsuarioLogado();
    }
}