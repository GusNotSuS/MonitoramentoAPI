using Microsoft.EntityFrameworkCore;
using MonitoramentoAPI.Models;

namespace MonitoramentoAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Service> Services { get; set; }
        public DbSet<LogMonitoramento> LogsMonitoramento { get; set; }
    }
}
