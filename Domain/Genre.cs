using System;
using System.Collections.Generic;
using System.Linq;

namespace GerenciadorDeLivraria.Domain
{
    public enum Genre
    {
        Ficcao,
        Romance,
        Misterio,
        Fantasia,
        Ciencia,
        Biografia,
        Historia,
        Tecnologia,
        Autoajuda,
        Poesia
    }

    public static class GenreHelper
    {
        private static readonly Dictionary<string, Genre> Map = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ["ficção"] = Genre.Ficcao,
            ["ficcao"] = Genre.Ficcao,
            ["romance"] = Genre.Romance,
            ["mistério"] = Genre.Misterio,
            ["misterio"] = Genre.Misterio,
            ["fantasia"] = Genre.Fantasia,
            ["ciência"] = Genre.Ciencia,
            ["ciencia"] = Genre.Ciencia,
            ["biografia"] = Genre.Biografia,
            ["história"] = Genre.Historia,
            ["historia"] = Genre.Historia,
            ["tecnologia"] = Genre.Tecnologia,
            ["autoajuda"] = Genre.Autoajuda,
            ["poesia"] = Genre.Poesia
        };

        public static bool TryParse(string value, out Genre genre)
            => Map.TryGetValue(value.Trim(), out genre);

        public static IEnumerable<string> ValidValues()
            => Map.Keys.Distinct(StringComparer.InvariantCultureIgnoreCase).OrderBy(x => x);
    }
}
