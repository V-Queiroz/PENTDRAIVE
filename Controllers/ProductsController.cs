using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using PENTDRIVEApi.Models;
using PENTDRIVEApi.DTOs;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace PENTDRIVEApi.Controllers
{

[Authorize]
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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Produto>> GetProduto (int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if(produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }


    [HttpPost]
    public async Task<ActionResult<Produto>> PostProduto(Produto produto)

    {
        _context.Produtos.Add(produto);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);




    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduto(int id, Produto produto)
    {
        if (id != produto.Id)
        {
            return BadRequest();
        }

        _context.Entry(produto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }

        catch (DbUpdateConcurrencyException)

        {
            if (!_context.Produtos.Any(e => e.Id == id))
            {
                return NotFound();
            }

            else
            {
                throw;
            }
        }
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduto(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null)
        {
            return NotFound();
        }

        _context.Produtos.Remove(produto);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private const int UsuarioResponsavelId = 1;

    [HttpPost("ReceberEstoque")]
    public async Task<IActionResult> PostRecebimentoEstoque (RecebimentoEstoqueRequest request)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.CodigoBarras == request.CodigoBarras);
                if (produto == null)
                {
                    return NotFound($"Produto com Código de Barras {request.CodigoBarras} não encontrado.");
                }

                if (request.Quantidade <= 0)
                {
                    return BadRequest($"A quantidade de recebinento deve ser positiva.");
                }

                produto.Estoque += request.Quantidade;

                _context.Produtos.Update(produto);

                // Registra as entradas
                _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    IdUsuario = UsuarioResponsavelId,
                    IdProduto = produto.Id,
                    Quantidade = request.Quantidade,
                    DataHora = DateTime.Now,
                    TipoMovimentacao = "ENTRADA"

                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = $"Recebimento  de {request.Quantidade} unidades do produto {produto.Nome} registrado com sucesso. Novo estoque: {produto.Estoque}" });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Erro ao registrar o recebimento do estoque. A transação foi desfeita.");
            }

            }
        }
    }

}

 