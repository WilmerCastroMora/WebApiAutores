using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.AutoMaperProfile
{
    public class AutoMapperProfiler:Profile
    {
        public AutoMapperProfiler()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>().
                ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));
            
            CreateMap<LibroCreacionDTO, Libro>().
                ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapeoAutoresLibros));
            CreateMap<Libro, LibroDTO>().
                ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<Libro, LibroPATCHDTO>().ReverseMap();

            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();
            if (autor.AutoresLibros == null)
            {
                return resultado;
            }
            foreach (var autorLibro in autor.AutoresLibros) {
                resultado.Add(
                    new LibroDTO()
                    {
                        Id = autorLibro.LibroId,
                        Name = autorLibro.Libro.Name
                    });
            }
            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();
            if (libro.AutoresLibros == null)
            {
                return resultado;
            }
            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }
            return resultado;
        }

        private List<AutorLibro> MapeoAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado= new List<AutorLibro>();
            if(libroCreacionDTO.AutoresIds == null)
            {
                return resultado;
            }
            foreach(var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }
            return resultado;
        }
    }
}
