using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Context;
using WebAPI.Entities;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IConfiguration _config;

        public UsuariosController(AppDbContext dbContext, IConfiguration config)
        {
            _config = config;
            this.dbContext = dbContext;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            var usuarioExiste = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (usuarioExiste is not null)
            {
                return BadRequest("Ya existe un usuario con este email");
            }

            dbContext.Usuarios.Add(usuario);
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            var usuarioActual = await dbContext.Usuarios.FirstOrDefaultAsync(u =>
                                     u.Email == usuario.Email && u.Password == usuario.Password);

            if (usuarioActual is null)
            {
                return NotFound("Usuario no encontrado");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email , usuario.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new ObjectResult(new
            {
                accesstoken = jwt,
                tokentype = "bearer",
                usuarioid = usuarioActual.Id,
                usuarionombre = usuarioActual.Nombre
            });
        }

        [Authorize]
        [HttpPost("uploadfoto")]
        public async Task<IActionResult> UploadFotoUsuario(IFormFile imagen)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario is null)
            {
                return NotFound("Usuario no encontrado");
            }

            if (imagen is not null)
            {
                // arquivo unico para la imagen enviada 
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imagen.FileName;
                string filePath = Path.Combine("wwwroot/userimages", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                // Atualiza la propiedad de UrlImage

                usuario.UrlImagen = "/userimages/" + uniqueFileName;

                await dbContext.SaveChangesAsync();
                return Ok("Imagen enviada con éxito");
            }

            return BadRequest("Ninguna imagen enviada");
        }

        [Authorize]
        [HttpGet("imagenperfil")]
        public async Task<IActionResult> ImagenPerfilUsuario()
        {
            //verifica si el usuario esta autenticado
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var usuario = await dbContext.Usuarios.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (usuario is null)
                return NotFound("Usuario no encontrado");

            var imagenPerfil = await dbContext.Usuarios
                .Where(x => x.Email == userEmail)
                .Select(x => new
                {
                    x.UrlImagen,
                })
                .SingleOrDefaultAsync();

            return Ok(imagenPerfil);
        }
    }
}
