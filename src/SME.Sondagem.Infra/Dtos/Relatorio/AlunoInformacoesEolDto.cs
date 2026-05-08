using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infrastructure.Dtos.Relatorio;

[ExcludeFromCodeCoverage]
public class AlunoInformacoesEolDto
{
    public long CodigoAluno { get; set; }
    public string? NomeAluno { get; set; }
    public string? NomeMae { get; set; }
    public string? Sexo { get; set; }

    /// <summary>Grupo étnico (ex.: PARDA); equivalente ao campo de raça em informações por turma.</summary>
    public string? GrupoEtnico { get; set; }

    public string? Nacionalidade { get; set; }
    public EnderecoAlunoInformacoesEolDto? Endereco { get; set; }
    public bool EhImigrante { get; set; }
    public string? Nis { get; set; }
    public string? Cns { get; set; }
}

[ExcludeFromCodeCoverage]
public class EnderecoAlunoInformacoesEolDto
{
    public long Id { get; set; }
    public string? Nro { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public long? Cep { get; set; }
    public string? NomeMunicipio { get; set; }
    public string? SiglaUF { get; set; }

    /// <summary>Equivalente ao JSON <c>tipologradouro</c>.</summary>
    public string? Tipologradouro { get; set; }

    public string? Logradouro { get; set; }
}
