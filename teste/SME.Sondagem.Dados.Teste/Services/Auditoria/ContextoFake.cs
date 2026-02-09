using SME.Sondagem.Infra.Contexto;
using SME.Sondagem.Infra.Interfaces;

namespace SME.Sondagem.Dados.Teste.Services.Auditoria
{
    public class ContextoFake : ContextoBase
    {
        public override void AdicionarVariaveis(IDictionary<string, object> variaveis)
        {
            Variaveis = variaveis;
        }

        public override IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto)
        {
            return this;
        }
    }
}
