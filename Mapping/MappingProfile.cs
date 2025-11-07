using AutoMapper;
using GerenciadorDeLivraria.Domain;
using GerenciadorDeLivraria.DTOs;

namespace GerenciadorDeLivraria.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookResponseDto>()
                .ForMember(d => d.Genre, o => o.MapFrom(s => s.Genre.ToString()));
        }
    }
}
