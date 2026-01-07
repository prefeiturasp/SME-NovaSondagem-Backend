using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SME.Sondagem.Dominio;

public static class EnumExtension
{
    public static bool EhUmDosValores(this Enum valorEnum, params Enum[] valores)
    {
        return valores.Contains(valorEnum);
    }

    public static string ObterNomeCurto(this Enum enumValue)
    {
        return enumValue.ObterAtributo<DisplayAttribute>()?.ShortName ?? string.Empty;
    }

    public static string ObterNome(this Enum enumValue)
    {
        return enumValue.ObterAtributo<DisplayAttribute>()?.Name ?? string.Empty;
    }

    public static string ObterNomeGrupo(this Enum enumValue)
    {
        return enumValue.ObterAtributo<DisplayAttribute>()?.GroupName ?? string.Empty;
    }

    public static TAttribute ObterAtributo<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<TAttribute>()!;
    }
}