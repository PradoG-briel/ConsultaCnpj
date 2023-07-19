using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultaCnpj.Models;
using ConsultaCnpj.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Net.Http.Json;
using ConsultaCnpj.Context;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography;
using System.Net.Http;
using System.Security.Cryptography;

namespace ConsultaCnpj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioController(UsuarioContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private bool TodoItemExists(Guid id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        [HttpGet("GetAll")]
        public ActionResult<List<UsuarioItem>> GetAllUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();

            if (usuarios == null)
            {
                return NoContent();
            }

            return usuarios;
        }

        [HttpGet("{id}")]
        public ActionResult<UsuarioItem> GetUsuario(Guid id)
        {
            var usuarios = _context.Usuarios.Find(id);

            if (usuarios == null)
            {
                return NoContent();
            }

            return usuarios;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(UsuarioDTO usuarioDTO)
        {
            var usuarioItem = new UsuarioItem
            {
                Email = usuarioDTO.Email,
                HashSenha = usuarioDTO.Senha
            };

            usuarioItem.Id = Guid.NewGuid();

            // Gerar o hash da senha usando PBKDF2
            byte[] salt = GenerateSalt();
            byte[] hashSenha = GeneratePasswordHash(usuarioItem.HashSenha, salt);

            // Converter o hash em uma string base64 para armazenamento
            string hashSenhaString = Convert.ToBase64String(hashSenha);

            // Armazenar o hash no objeto UsuarioItem
            usuarioItem.HashSenha = hashSenhaString;

            _context.Usuarios.Add(usuarioItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllUsuarios),
                new { usuarioItem.Id },
                UsuarioItemDTO(usuarioItem));
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] GeneratePasswordHash(string senha, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, 10000))
            {
                return pbkdf2.GetBytes(32); // Defina o tamanho do hash desejado
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(Guid id, UsuarioDTO updateUsuario)
        {
            var consultaItem = await _context.Usuarios.FindAsync(id);

            if (consultaItem == null)
            {
                return NotFound();
            }

            consultaItem.Email = updateUsuario.Email;
            consultaItem.HashSenha = updateUsuario.Senha;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            var consultaItem = await _context.Usuarios.FindAsync(id);

            if (consultaItem == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(consultaItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static UsuarioItem UsuarioItemDTO(UsuarioItem usuarioItem) => new UsuarioItem
        {
            Id = usuarioItem.Id,
            Email = usuarioItem.Email,
            HashSenha = usuarioItem.HashSenha
        };
}
}
