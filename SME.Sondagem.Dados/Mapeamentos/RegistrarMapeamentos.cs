using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;

namespace SME.Sondagem.Dados.Mapeamentos;

public static class RegistrarMapeamentos
{
    public static void Registrar()
    {
        FluentMapper.Initialize(config =>
        {

            config.ForDommel();
        });
    }
}
