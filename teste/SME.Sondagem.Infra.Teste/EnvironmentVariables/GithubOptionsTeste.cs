using SME.Sondagem.Infra.EnvironmentVariables;
using Xunit;

namespace SME.Sondagem.Infra.Teste.EnvironmentVariables
{
    public class GithubOptionsTeste
    {
        [Fact]
        public void Deve_Permitir_Setar_Propriedades()
        {
            var options = new GithubOptions
            {
                Url = "https://github.com/org/repo",
                RepositorioApi = "ApiRepo",
                RepositorioFront = "FrontRepo"
            };

            Assert.Equal("https://github.com/org/repo", options.Url);
            Assert.Equal("ApiRepo", options.RepositorioApi);
            Assert.Equal("FrontRepo", options.RepositorioFront);
        }

        [Fact]
        public void Propriedades_Devem_Ser_Nulas_Por_Padrao()
        {
            var options = new GithubOptions();

            Assert.Null(options.Url);
            Assert.Null(options.RepositorioApi);
            Assert.Null(options.RepositorioFront);
        }
    }
}
