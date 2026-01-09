using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SME.Sondagem.Dados.Contexto;

public class SondagemDbContextFactory : IDesignTimeDbContextFactory<SondagemDbContext>
{
    private const string ConfigKeyConnectionStrings = "ConnectionStrings";
    private const string ConfigKeyDatabase = "SondagemConnection";
    private const string ConfigKeySeparator = ":";
    private const string NameSpaceApi = "SME.Sondagem.API.csproj";

    public SondagemDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Console.WriteLine($"📂 Diretório atual: {currentDirectory}");

        var apiProjectPath = FindApiProjectPath(currentDirectory);
        Console.WriteLine($"📂 Diretório do projeto API: {apiProjectPath}");

        if (string.IsNullOrEmpty(apiProjectPath) || !Directory.Exists(apiProjectPath))
        {
            throw new InvalidOperationException(
                $"❌ Não foi possível encontrar o projeto SME.Sondagem.API.\n" +
                $"Diretório atual: {currentDirectory}");
        }

        var apiCsprojPath = Path.Combine(apiProjectPath, NameSpaceApi);
        if (!File.Exists(apiCsprojPath))
        {
            throw new InvalidOperationException(
                $"❌ Arquivo NameSpaceApi não encontrado em: {apiProjectPath}");
        }

        var userSecretsId = GetUserSecretsIdFromCsproj(apiCsprojPath);

        if (string.IsNullOrEmpty(userSecretsId))
        {
            throw new InvalidOperationException(
                $"❌ UserSecretsId não encontrado no arquivo: {apiCsprojPath}\n\n" +
                "Execute no diretório do projeto API:\n" +
                "dotnet user-secrets init");
        }

        Console.WriteLine($"🔑 UserSecretsId encontrado: {userSecretsId}");

        var secretsPath = GetUserSecretsPath(userSecretsId);
        Console.WriteLine($"📂 Caminho das secrets: {secretsPath}");

        if (!File.Exists(secretsPath))
        {
            var connectionStringKey = $"{ConfigKeyConnectionStrings}{ConfigKeySeparator}{ConfigKeyDatabase}";
            throw new InvalidOperationException(
                $"❌ Arquivo secrets.json não encontrado em: {secretsPath}\n\n" +
                "Execute no diretório do projeto API (SME.Sondagem.API):\n" +
                $"dotnet user-secrets set \"{connectionStringKey}\" \"sua-connection-string\"");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(secretsPath, optional: false, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString(ConfigKeyDatabase);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{ConfigKeyDatabase}' não encontrada nas user secrets.\n\n");
        }

        Console.WriteLine("✅ Connection string encontrada com sucesso!");

        var optionsBuilder = new DbContextOptionsBuilder<SondagemDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

        Console.WriteLine("✅ DbContext criado com sucesso!\n");

        return new SondagemDbContext(optionsBuilder.Options);
    }

    private static string? FindApiProjectPath(string startDirectory)
    {
        var currentDir = new DirectoryInfo(startDirectory);

        if (File.Exists(Path.Combine(currentDir.FullName, NameSpaceApi)))
        {
            return currentDir.FullName;
        }

        while (currentDir != null)
        {
            var apiPath = Path.Combine(currentDir.FullName, "SME.Sondagem.API");
            if (Directory.Exists(apiPath) && File.Exists(Path.Combine(apiPath, NameSpaceApi)))
            {
                return apiPath;
            }

            var srcApiPath = Path.Combine(currentDir.FullName, "src", "SME.Sondagem.API");
            if (Directory.Exists(srcApiPath) && File.Exists(Path.Combine(srcApiPath, NameSpaceApi)))
            {
                return srcApiPath;
            }

            currentDir = currentDir.Parent;
        }

        return null;
    }

    private static string? GetUserSecretsIdFromCsproj(string csprojPath)
    {
        try
        {
            var csprojContent = File.ReadAllText(csprojPath);
            var startTag = "<UserSecretsId>";
            var endTag = "</UserSecretsId>";

            var startIndex = csprojContent.IndexOf(startTag);
            if (startIndex == -1) return null;

            startIndex += startTag.Length;
            var endIndex = csprojContent.IndexOf(endTag, startIndex);
            if (endIndex == -1) return null;

            return csprojContent.Substring(startIndex, endIndex - startIndex).Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Erro ao ler UserSecretsId: {ex.Message}");
            return null;
        }
    }

    private static string GetUserSecretsPath(string userSecretsId)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var secretsPath = Path.Combine(appData, "Microsoft", "UserSecrets", userSecretsId, "secrets.json");
        return secretsPath;
    }
}