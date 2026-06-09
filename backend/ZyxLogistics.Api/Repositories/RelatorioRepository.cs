using System.Data;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using ZyxLogistics.Api.Database;
using ZyxLogistics.Api.DTOs;
using ZyxLogistics.Api.Models;

namespace ZyxLogistics.Api.Repositories
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        private static readonly IReadOnlyDictionary<string, RelatorioDefinition> Definitions =
            new Dictionary<string, RelatorioDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                ["agendamentos-geral"] = new("dbo.sp_Relatorio_AgendamentosGeral", "Agendamentos Geral", "relatorio-agendamentos-geral"),
                ["estoque"] = new("dbo.sp_Relatorio_EstoqueAtual", "Estoque Atual", "relatorio-estoque"),
                ["agendas-finalizadas"] = new("dbo.sp_Relatorio_AgendasFinalizadas", "Agendas Finalizadas", "relatorio-agendas-finalizadas"),
                ["movimentacao-estoque"] = new("dbo.sp_Relatorio_MovimentacaoEstoque", "Movimentacao Estoque", "relatorio-movimentacao-estoque"),
                ["cargas-recebidas-enviadas"] = new("dbo.sp_Relatorio_CargasRecebidasEnviadas", "Cargas Recebidas Enviadas", "relatorio-cargas-recebidas-enviadas"),
                ["performance-operacional"] = new("dbo.sp_Relatorio_PerformanceOperacional", "Performance Operacional", "relatorio-performance-operacional")
            };

        public RelatorioRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RelatorioExcel> GerarAsync(string tipo, RelatorioFiltroRequest filtro)
        {
            if (!Definitions.TryGetValue(tipo, out var definition))
            {
                throw new ArgumentException("Relatorio invalido.", nameof(tipo));
            }

            if (filtro.DataFim.Date < filtro.DataInicio.Date)
            {
                throw new ArgumentException("Data final deve ser maior ou igual a data inicial.", nameof(filtro));
            }

            var table = await ExecuteAsync(definition.ProcedureName, filtro);
            var content = BuildExcel(table, definition.SheetName);
            var fileName = $"{definition.FileNamePrefix}-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return new RelatorioExcel
            {
                FileName = fileName,
                Content = content
            };
        }

        private async Task<DataTable> ExecuteAsync(string procedureName, RelatorioFiltroRequest filtro)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@DataInicio", SqlDbType.Date).Value = filtro.DataInicio.Date;
            command.Parameters.Add("@DataFim", SqlDbType.Date).Value = filtro.DataFim.Date;
            command.Parameters.Add("@OperacaoId", SqlDbType.Int).Value = filtro.OperacaoId.HasValue ? filtro.OperacaoId.Value : DBNull.Value;
            command.Parameters.Add("@StatusId", SqlDbType.Int).Value = filtro.StatusId.HasValue ? filtro.StatusId.Value : DBNull.Value;
            command.Parameters.Add("@Produto", SqlDbType.NVarChar, 150).Value = string.IsNullOrWhiteSpace(filtro.Produto) ? DBNull.Value : filtro.Produto.Trim();

            await connection.OpenAsync();
            await using var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        private static byte[] BuildExcel(DataTable table, string sheetName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            if (table.Columns.Count == 0)
            {
                worksheet.Cell(1, 1).Value = "Nenhum dado encontrado";
            }
            else
            {
                for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                {
                    var cell = worksheet.Cell(1, columnIndex + 1);
                    cell.Value = table.Columns[columnIndex].ColumnName;
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFCC00");
                    cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                }

                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
                    {
                        var value = table.Rows[rowIndex][columnIndex];
                        worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = value == DBNull.Value ? string.Empty : XLCellValue.FromObject(value);
                    }
                }

                var range = worksheet.Range(1, 1, Math.Max(table.Rows.Count + 1, 1), table.Columns.Count);
                range.CreateTable();
                worksheet.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private sealed record RelatorioDefinition(string ProcedureName, string SheetName, string FileNamePrefix);
    }
}
