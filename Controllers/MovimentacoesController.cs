using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PENTDRIVEApi.Data;

namespace PENTDRIVEApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentacoesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MovimentacoesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentacaoEstoque>>> GetMovimentacoes()
        {
            return await _context.MovimentacoesEstoque
                                    .Include(m => m.Produto)
                                    .Include(m => m.Venda)
                                    .ToListAsync();
        }
    }
}