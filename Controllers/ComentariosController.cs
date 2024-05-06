using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]

    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name ="ObtenerComentarios")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int id)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == id);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = await context.Comentarios.
                Where(idLibro => idLibro.LibroId == id).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro= await context.Libros.AnyAsync(libroDB => libroDB.Id == id);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.LibroId = id;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentarios", new {id = comentario.Id}, comentarioDTO)  ;
        }

        [HttpPut("{id:int}")] //api/libros/libroId/comentarios/id
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }
            var existeComentario = await context.Comentarios.AnyAsync(comentario => comentario.Id == id);
            if (!existeComentario)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
            
        }
    }
}
