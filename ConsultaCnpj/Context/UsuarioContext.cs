using ConsultaCnpj.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultaCnpj.Context;

public class UsuarioContext : DbContext
{
    public UsuarioContext(DbContextOptions<UsuarioContext> options)
        : base(options)
    {
    }

    public DbSet<UsuarioItem> Usuarios { get; set; } = null!;

    public DbSet<ConsultaCnpjItem> Pedido { get; set; } = null!;
}
