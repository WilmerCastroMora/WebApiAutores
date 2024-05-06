using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entidades;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:250)]
        public string Name { get; set; }
        public List<AutorDTO> Autores { get; set; }

    }
}
