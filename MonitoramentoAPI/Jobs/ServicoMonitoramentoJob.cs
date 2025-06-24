using Microsoft.EntityFrameworkCore;
using MonitoramentoAPI.Data;
using MonitoramentoAPI.Models;
using MonitoramentoAPI.Services;
using System.Collections.Concurrent;

namespace MonitoramentoAPI.Jobs
{
    public class ServicoMonitoramentoJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServicoMonitoramentoJob> _logger;
        private readonly ConcurrentDictionary<int, Timer> _timers = new();

        public ServicoMonitoramentoJob(IServiceProvider serviceProvider, ILogger<ServicoMonitoramentoJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var servicosAtivos = await context.Services
                    .Where(s => s.Ativo)
                    .ToListAsync(stoppingToken);

                foreach (var servico in servicosAtivos)
                {
                    if (!_timers.ContainsKey(servico.Id))
                    {
                        var intervalo = TimeSpan.FromSeconds(servico.IntervaloMonitoramento);
                        var timer = new Timer(async _ => await ExecutarMonitoramento(servico.Id), null, TimeSpan.Zero, intervalo);
                        _timers.TryAdd(servico.Id, timer);

                        _logger.LogInformation($"Iniciado monitoramento para o serviço '{servico.Nome}' (ID: {servico.Id})");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Espera 1 minuto para buscar novos serviços
            }
        }


        private async Task ExecutarMonitoramento(int servicoId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>(); // nome usado aqui

            var servico = await context.Services.FindAsync(servicoId);
            if (servico == null || !servico.Ativo)
                return;

            try
            {
                var httpClient = new HttpClient();
                var inicio = DateTime.Now;
                var response = await httpClient.GetAsync(servico.URL);
                var tempo = (int)(DateTime.Now - inicio).TotalMilliseconds;

                var status = response.IsSuccessStatusCode ? "Sucesso" : "Falha";

                context.LogsMonitoramento.Add(new LogMonitoramento
                {
                    ServiceId = servico.Id,
                    Status = status,
                    TempoResposta = tempo,
                    Erro = response.IsSuccessStatusCode ? null : response.ReasonPhrase
                });

                servico.StatusUltimaVerificacao = status;

                if (!response.IsSuccessStatusCode)
                {
                    await emailService.EnviarEmailFalha("email aqui", servico.Nome, response.ReasonPhrase ?? "Erro desconhecido");
                }
            }
            catch (Exception ex)
            {
                context.LogsMonitoramento.Add(new LogMonitoramento
                {
                    ServiceId = servico.Id,
                    Status = "Erro",
                    Erro = ex.Message
                });

                servico.StatusUltimaVerificacao = "Erro";

                await emailService.EnviarEmailFalha("email aqui", servico.Nome, ex.Message);
            }

            await context.SaveChangesAsync();
        }

        public override void Dispose()
        {
            foreach (var timer in _timers.Values)
            {
                timer?.Dispose();
            }
            base.Dispose();
        }
    }
}
