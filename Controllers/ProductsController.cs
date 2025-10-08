using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using PENTDRIVEApi.Models;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()

    {
        return await _context.Produtos.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> PostProduto(Produto produto)    

    {
    _context.Produtos.Add(produto);

    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto); 


    }
    
}

