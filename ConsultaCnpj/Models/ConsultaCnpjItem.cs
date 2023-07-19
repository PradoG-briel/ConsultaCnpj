namespace ConsultaCnpj.Models
{
    public class ConsultaCnpjItem
    {
        public Guid Id { get; set; }

        public string Cnpj { get; set; }

        public string? Resultado { get; set; }
    }
}
