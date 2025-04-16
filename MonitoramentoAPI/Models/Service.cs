namespace MonitoramentoAPI.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string URL { get; set; }
        public bool Ativo { get; set; }
        public int IntervaloMonitoramento { get; set; }
        public string StatusUltimaVerificacao { get; set; } = "Aguardando";
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
