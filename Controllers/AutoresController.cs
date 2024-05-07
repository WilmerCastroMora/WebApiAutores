using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.CositasVarias;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDBContext contest;
        private readonly IMapper mapper;
        private readonly IConfiguration configuracion;

        public AutoresController(ApplicationDBContext contest, IMapper mapper, IConfiguration configuracion)
        {
            this.contest = contest;
            this.mapper = mapper;
            this.configuracion = configuracion;
        }
        [HttpGet("configuraciones")]
        public ActionResult<string> ObtenerConfiguraciones()
        {
            return configuracion["llaveJWT"];
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [HttpGet("listado")] //api/autores/listado
        [HttpGet("/listado")] //listado
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autores= await contest.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutor")]
        public async Task<ActionResult<AutorDTO>> Get(int id)
        {
            var existeAutor = await contest.Autores
                .Include(autorLibro => autorLibro.AutoresLibros)
                .ThenInclude(autorLibro => autorLibro.Libro).FirstOrDefaultAsync(x => x.Id == id);
            if(existeAutor == null)
            {
                return NotFound();  
            }
            return mapper.Map<AutorDTO>(existeAutor);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
        {
            var autor = await contest.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autor);
        }

        [HttpGet("primero")] //api/autores/PrimerAutor
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int valor, [FromQuery] string ocupacion)
        {
            return await contest.Autores.FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO){
            var existeAutorConMismoNombre = await contest.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (existeAutorConMismoNombre)
            {
                return BadRequest("Ya existe un autor con este nombre");
            }
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            contest.Add(autor);
            await contest.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("ObtenerAutor", new { id = autor.Id }, autorDTO);
        }
        [HttpPut("{id:int}")] //api/autores/algo
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var existeAutor = await contest.Autores.AnyAsync(x => x.Id == id);
            if (!existeAutor)
            {
                return NotFound();
            }
            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            contest.Update(autor);
            await contest.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")] //api/autores/id
        public async Task<ActionResult> Delete(int id) { 
            var existeAutor = await contest.Autores.AnyAsync(x => x.Id == id);
            if (!existeAutor)
            {
                return NotFound();
            }

            contest.Remove(new Autor { Id = id });
            await contest.SaveChangesAsync();
            return Ok();
        }

    }
}
