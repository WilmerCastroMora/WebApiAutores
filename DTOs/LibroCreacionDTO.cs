using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:250)]
        [Required]
        public string Name { get; set; }
        public List<int> AutoresIds { get; set; }
    }
}
