namespace SME.Sondagem.Dominio
{
    public static class ListExtension
    {
        public static bool PossuiRegistros<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        public static bool NaoPossuiRegistros<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.PossuiRegistros();
        }

        public static bool PossuiRegistros<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable != null && enumerable.Any(predicate);
        }

        public static bool NaoPossuiRegistros<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return !enumerable.PossuiRegistros(predicate);
        }

        public static void LancarExcecaoNegocioSeNaoPossuiRegistros<T>(this IEnumerable<T> enumerable, string msgErro)
        {
            if (enumerable.NaoPossuiRegistros())
                throw new RegraNegocioException(msgErro);
        }
    }
}
