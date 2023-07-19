using Microsoft.Extensions.Configuration;

public class Service
{
    private readonly IConfiguration _configuration;

    public Service(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConectarAoBanco()
    {
        string connectionString = _configuration.GetConnectionString("ConnectionString");
    }
}
