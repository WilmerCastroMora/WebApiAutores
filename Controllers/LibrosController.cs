using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "ObtenerLibros")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {
            var libro = await context.Libros.
                Include(LibroDB => LibroDB.AutoresLibros).
                ThenInclude(AutorLibroDB => AutorLibroDB.Autor).
                FirstOrDefaultAsync((x => x.Id == id));
            if(libro == null)
            {
                return NotFound();
            }
            
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(AutLibro => AutLibro.Orden).ToList();
            return mapper.Map<LibroDTO>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if(libroCreacionDTO.AutoresIds == null) {
                return BadRequest("No se pueden crear libros sin autores");
            }
            var AutoresIds = await context.Autores
                .Where(autorId => libroCreacionDTO.AutoresIds.Contains(autorId.Id)).Select(x => x.Id).ToListAsync();

            if(AutoresIds.Count != libroCreacionDTO.AutoresIds.Count)
            {
                return BadRequest("No existe alguno de los autores");
            }
            var libro = mapper.Map<Libro>(libroCreacionDTO);
            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("ObtenerLibros", new {id = libro.Id}, libroDTO);
        }
        
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(libroId => libroId.Id == id);
            if(libroDB == null)
            {
                return NotFound();
            }
            libroDB = mapper.Map(libroCreacionDTO, libroDB);
            AsignarOrdenAutores(libroDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }
            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if(libro == null)
            {
                return NotFound();
            }
            var libroPatchDto = mapper.Map<LibroPATCHDTO>(libro);
            jsonPatchDocument.ApplyTo(libroPatchDto, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            var esValido = TryValidateModel(libroPatchDto);
            if(esValido == false)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(libroPatchDto, libro);
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
        [HttpDelete("{id:int}")] //api/autores/id
        public async Task<ActionResult> Delete(int id)
        {
            var existeAutor = await context.Libros.AnyAsync(x => x.Id == id);
            if (!existeAutor)
            {
                return NotFound();
            }

            context.Remove(new Libro { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
