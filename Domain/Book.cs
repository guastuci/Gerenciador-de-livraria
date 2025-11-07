using System.ComponentModel.DataAnnotations;

namespace GerenciadorDeLivraria.Domain
{
    public class Book : BaseEntity
    {
        [Required, StringLength(120, MinimumLength = 2)]
        public string Title { get; private set; } = null!;

        [Required, StringLength(120, MinimumLength = 2)]
        public string Author { get; private set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; private set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; private set; }

        public Genre Genre { get; private set; }

        private Book() { } // EF

        private Book(string title, string author, Genre genre, decimal price, int stock)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Price = price;
            Stock = stock;
        }

        public static Book Create(string title, string author, Genre genre, decimal price, int stock)
            => new(title, author, genre, price, stock);

        public void Update(string title, string author, Genre genre, decimal price, int stock)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Price = price;
            Stock = stock;
            TouchUpdated();
        }
    }
}
