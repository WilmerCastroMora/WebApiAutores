using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var resultado =await signInManager.PasswordSignInAsync(credencialesUsuarioDTO.Email, credencialesUsuarioDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded) {
                return generarToken(credencialesUsuarioDTO);
            }
            else
            {
                //return BadRequest("Ya existe un autor con este nombre");
                return generarToken(credencialesUsuarioDTO);
            }

        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO){
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);
            if (resultado.Succeeded)
            {
                return generarToken(credencialesUsuarioDTO);
            }
            else
            {
                return generarToken(credencialesUsuarioDTO);
            }
        }
        private RespuestaAutenticacionDTO generarToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var claims = new List<Claim>() {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("cualquier cosa en la llave","cualquier cosa en el valor")
            };
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJWT"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddMinutes(20);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);
            
            return new RespuestaAutenticacionDTO() { Token = new JwtSecurityTokenHandler().WriteToken(securityToken), FechaVencimiento = expiracion };

        }
    }
}
