namespace ZyxLogistics.Api.Models
{
    public class Permissao
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }
}
