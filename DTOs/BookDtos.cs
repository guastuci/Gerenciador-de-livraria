using System;
using System.ComponentModel.DataAnnotations;

namespace GerenciadorDeLivraria.DTOs
{
    public class BookCreateDto
    {
        [Required, StringLength(120, MinimumLength = 2)]
        public string Title { get; set; } = null!;

        [Required, StringLength(120, MinimumLength = 2)]
        public string Author { get; set; } = null!;

        [Required]
        public string Genre { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }

    public class BookUpdateDto : BookCreateDto { }

    public class BookResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
