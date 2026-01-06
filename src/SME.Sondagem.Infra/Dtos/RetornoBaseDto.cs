using FluentValidation.Results;
using System.Diagnostics.CodeAnalysis;

namespace SME.Sondagem.Infra.Dtos;

[ExcludeFromCodeCoverage]
public class RetornoBaseDto
{
    public RetornoBaseDto(IEnumerable<ValidationFailure> validationFailures)
    {
        if (validationFailures != null && validationFailures.Any())
            Mensagens = validationFailures.Select(c => c.ErrorMessage).ToList();
    }

    public RetornoBaseDto()
    {
        Mensagens = [];
    }

    public RetornoBaseDto(string mensagem)
    {
        Mensagens = [mensagem];
    }

    public List<string>? Mensagens { get; set; }
    public bool ExistemErros => Mensagens?.Count > 0;
}
