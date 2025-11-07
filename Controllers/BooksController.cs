using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GerenciadorDeLivraria.Data;
using GerenciadorDeLivraria.Domain;
using GerenciadorDeLivraria.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeLivraria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>POST /api/books - Cria um novo livro</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BookResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create([FromBody] BookCreateDto input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!GenreHelper.TryParse(input.Genre, out var genre))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Gênero inválido",
                    Detail = $"Informe um gênero válido. Valores aceitos: {string.Join(", ", GenreHelper.ValidValues())}",
                    Status = 400
                });
            }

            bool exists = await _db.Books.AnyAsync(b =>
                b.Title.ToLower() == input.Title.ToLower().Trim() &&
                b.Author.ToLower() == input.Author.ToLower().Trim());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflito de dados",
                    Detail = "Já existe um livro com o mesmo título e autor.",
                    Status = 409
                });
            }

            var entity = Book.Create(input.Title.Trim(), input.Author.Trim(), genre, input.Price, input.Stock);
            _db.Books.Add(entity);
            await _db.SaveChangesAsync();

            var dto = _mapper.Map<BookResponseDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
        }

        /// <summary>GET /api/books - Lista com filtros opcionais</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookResponseDto>), 200)]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookResponseDto>), 200)]
        public async Task<IActionResult> GetAll(
    [FromQuery] string? title,
    [FromQuery] string? author,
    [FromQuery] string? genre,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] bool? inStock,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sortBy = "title",
    [FromQuery] string? sortDir = "asc")
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var q = _db.Books.AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(title))
                q = q.Where(b => EF.Functions.Like(b.Title, $"%{title}%"));
            if (!string.IsNullOrWhiteSpace(author))
                q = q.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));
            if (!string.IsNullOrWhiteSpace(genre) && GenreHelper.TryParse(genre, out var g))
                q = q.Where(b => b.Genre == g);
            if (minPrice.HasValue) q = q.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue) q = q.Where(b => b.Price <= maxPrice.Value);
            if (inStock == true) q = q.Where(b => b.Stock > 0);

            // Ordenação
            bool desc = string.Equals(sortDir, "desc", StringComparison.InvariantCultureIgnoreCase);
            q = (sortBy?.ToLowerInvariant()) switch
            {
                "author" => desc ? q.OrderByDescending(b => b.Author).ThenBy(b => b.Title)
                                    : q.OrderBy(b => b.Author).ThenBy(b => b.Title),
                "price" => desc ? q.OrderByDescending(b => b.Price).ThenBy(b => b.Title)
                                    : q.OrderBy(b => b.Price).ThenBy(b => b.Title),
                "stock" => desc ? q.OrderByDescending(b => b.Stock).ThenBy(b => b.Title)
                                    : q.OrderBy(b => b.Stock).ThenBy(b => b.Title),
                "createdat" => desc ? q.OrderByDescending(b => b.CreatedAt).ThenBy(b => b.Title)
                                    : q.OrderBy(b => b.CreatedAt).ThenBy(b => b.Title),
                "updatedat" => desc ? q.OrderByDescending(b => b.UpdatedAt).ThenBy(b => b.Title)
                                    : q.OrderBy(b => b.UpdatedAt).ThenBy(b => b.Title),
                _ => desc ? q.OrderByDescending(b => b.Title).ThenBy(b => b.Author)
                                    : q.OrderBy(b => b.Title).ThenBy(b => b.Author),
            };

            // Total para paginação
            var total = await q.CountAsync();

            // Paginação
            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            // Metadados de paginação no header
            Response.Headers["X-Total-Count"] = total.ToString();
            Response.Headers["X-Page"] = page.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();

            return Ok(items.Select(_mapper.Map<BookResponseDto>));
        }

        /// <summary>GET /api/books/{id} - Busca por ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookResponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity is null) return NotFound();

            return Ok(_mapper.Map<BookResponseDto>(entity));
        }

        /// <summary>PUT /api/books/{id} - Atualiza um livro</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookUpdateDto input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _db.Books.FindAsync(id);
            if (entity is null) return NotFound();

            if (!GenreHelper.TryParse(input.Genre, out var genre))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Gênero inválido",
                    Detail = $"Informe um gênero válido. Valores aceitos: {string.Join(", ", GenreHelper.ValidValues())}",
                    Status = 400
                });
            }

            bool exists = await _db.Books.AnyAsync(b =>
                b.Id != id &&
                b.Title.ToLower() == input.Title.ToLower().Trim() &&
                b.Author.ToLower() == input.Author.ToLower().Trim());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflito de dados",
                    Detail = "Já existe um livro com o mesmo título e autor.",
                    Status = 409
                });
            }

            entity.Update(input.Title.Trim(), input.Author.Trim(), genre, input.Price, input.Stock);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>DELETE /api/books/{id} - Exclui um livro</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _db.Books.FindAsync(id);
            if (entity is null) return NotFound();

            _db.Books.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
