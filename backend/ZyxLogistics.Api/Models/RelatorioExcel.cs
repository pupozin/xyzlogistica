namespace ZyxLogistics.Api.Models
{
    public class RelatorioExcel
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
