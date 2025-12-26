using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Dados.Mapeamentos;

public static class RegistrarMapeamentos
{
    [ExcludeFromCodeCoverage]
    public static void Registrar(ModelBuilder modelBuilder)
    {
        // Aplicar todas as configurações do assembly automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegistrarMapeamentos).Assembly);
    }
}