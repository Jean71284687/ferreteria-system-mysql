using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FerreteriaAPI.Data;
using FerreteriaAPI.Models;
using BCrypt.Net;

namespace FerreteriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly FerreteriaContext _context;

        public UsuariosController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.Where(u => u.Activo).ToListAsync();
        }

        // POST: api/Usuarios/registro
        [HttpPost("registro")]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Hash de la contraseña
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            usuario.FechaCreacion = DateTime.Now;
            usuario.Activo = true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);
        }

        // POST: api/Usuarios/login
        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login(LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Activo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
            {
                return Unauthorized("Credenciales inválidas");
            }

            return Ok(new { 
                Id = usuario.Id, 
                Nombre = usuario.Nombre, 
                Email = usuario.Email, 
                Rol = usuario.Rol 
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}