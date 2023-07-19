using ConsultaCnpj.Context;
using ConsultaCnpj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultaCnpj.Controllers
{
    public class PedidoController : ControllerBase
    {
        private readonly UsuarioContext _context;
        private readonly IConfiguration _configuration;

        public PedidoController(UsuarioContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("{cnpj}")]
        public async Task<ActionResult<string>> GetCnpj(string cnpj)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = _configuration["ReceitaAwsLink:Link"];

                string urlCompleta = $"{apiUrl}{cnpj}";

                var response = await httpClient.GetAsync(urlCompleta);

                if (response.IsSuccessStatusCode)
                {
                    var dadosCNPJ = await response.Content.ReadAsStringAsync();

                    var consultaCnpj = new ConsultaCnpjItem
                    {
                        Id = Guid.NewGuid(),
                        Cnpj = cnpj,
                        Resultado = dadosCNPJ
                    };
                    _context.Pedido.Add(consultaCnpj);
                    await _context.SaveChangesAsync();

                    return dadosCNPJ;
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Erro na chamada");
                }
            }
        }
    }
}
