using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonitoramentoAPI.Data;
using MonitoramentoAPI.Models;

namespace MonitoramentoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Services.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Service service)

        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = service.Id }, service);
        }

        [HttpPut("{id}/ativar")]
        public async Task<IActionResult> AtivarServico(int id)
        {
            var servico = await _context.Services.FindAsync(id);

            if (servico == null)
                return NotFound();

            servico.Ativo = true;
            await _context.SaveChangesAsync();

            return Ok(servico);
        }

        [HttpPut("{id}/desativar")]
        public async Task<IActionResult> DesativarServico(int id)
        {
            var servico = await _context.Services.FindAsync(id);

            if (servico == null)
                return NotFound();

            servico.Ativo = false;
            await _context.SaveChangesAsync();

            return Ok(servico);
        }

    }
}
