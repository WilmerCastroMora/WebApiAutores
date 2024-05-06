namespace WebApiAutores.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
