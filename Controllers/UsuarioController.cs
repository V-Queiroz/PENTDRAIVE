using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using PENTDRIVEApi.Models;
using PENTDRIVEApi.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt; 
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace PENTDRIVEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // Variável para configuração JWT

        
        public UsuariosController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioRegistroRequest request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("O Email fornecido já está em uso.");
            }
            if (await _context.Usuarios.AnyAsync(u => u.CNPJ == request.CNPJ))
            {
                return BadRequest("O CNPJ fornecido já está em uso.");
            }

            var senhaHash = CreatePasswordHash(request.Senha);
            var novoUsusario = new Usuario
            {
                Nome = request.Nome,
                Email = request.Email,
                CNPJ = request.CNPJ,
                SenhaHash = senhaHash,
                Role = "PADRAO"
            };

            _context.Usuarios.Add(novoUsusario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Registrar), new { id = novoUsusario.Id },
                                     new { Message = "Usuario Registrado com Sucesso.", Id = novoUsusario.Id });
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            //Busca o usuário
            var usuario = await _context.Usuarios // Use 'Usuarios' (plural) se for a DbSet
                                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                // Mensagem caso os dados estejam incorretos
                return Unauthorized(new { Message = "Email ou Senha incorretos." });
            }

            // 2. Verifica a Senha (Usando a lógica de comparação SHA256)
            if (!VerifyPasswordHash(request.Senha, usuario.SenhaHash))
            {
                return Unauthorized(new { Message = "Email ou Senha incorretos." });
            }

            // 3. Gera o Token JWT
            var token = CreateToken(usuario);

            // 4. Retorna o token
            return Ok(new { Token = token, Message = "Login realizado com sucesso." });
        }

        
        private byte[] CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // HASHING - Verificação do Hash SHA256
        private bool VerifyPasswordHash(string password, byte[] passwordHash)
        {
            // Gera o hash da senha fornecida
            var computedHash = CreatePasswordHash(password);

            // Compara byte por byte os hashes
            return computedHash.SequenceEqual(passwordHash);
        }

        // JWT - Geração do Token
        private string CreateToken(Usuario usuario)
        {
           
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Role) // Adiciona a Role
            };

    
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!.Trim())
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Pega o tempo de expiração do appsettings.json 10080 minutos   
            int expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes), // Usa o tempo configurado
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}