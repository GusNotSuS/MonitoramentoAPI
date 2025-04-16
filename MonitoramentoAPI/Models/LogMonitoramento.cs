namespace MonitoramentoAPI.Models
{
    public class LogMonitoramento
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        public DateTime DataVerificacao { get; set; } = DateTime.Now;
        public string Status { get; set; } = "";
        public int? TempoResposta { get; set; }
        public string? Erro { get; set; }
    }
}
