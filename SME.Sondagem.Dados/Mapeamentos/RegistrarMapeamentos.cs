using Microsoft.EntityFrameworkCore;
using SME.Sondagem.Dominio.Entidades.Questionario;

namespace SME.Sondagem.Dados.Mapeamentos;

public static class RegistrarMapeamentos
{
    public static void Registrar(ModelBuilder modelBuilder)
    {
        // Aplicar todas as configurações do assembly automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegistrarMapeamentos).Assembly);
    }
}