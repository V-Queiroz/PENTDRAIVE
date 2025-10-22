using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PENTDRIVEApi.Data;
using PENTDRIVEApi.DTOs;
using PENTDRIVEApi.Models;
using System.Linq;
using System;

namespace PENTDRIVEApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
        {
            _context = context;
        }

        private const int UsuarioResponsavelId = 1;

        [HttpPost]
        public async Task<IActionResult> PostVenda(VendaRequest request)
        {
            // Inicia uma transação explícita
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    decimal valorTotalVenda = 0;
                    var itensVenda = new List<ItemVenda>();
                    
                    // Lista auxiliar para armazenar os produtos modificados, que serão salvos DEPOIS
                    var produtosParaAtualizar = new List<Produto>(); 

                    foreach (var item in request.Itens)
                    {
                        // Carrega o produto, e o EF Core começa a rastreá-lo
                        var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                        if (produto == null)
                        {
                            return NotFound($"Produto com ID {item.ProdutoId} não encontrado");
                        }

                        if (produto.Estoque < item.Quantidade)
                        {
                            return BadRequest($"Estoque insuficiente para o produto '{produto.Nome}'. Disponivel: {produto.Estoque}");
                        }

                        decimal subtotal = produto.Preco * item.Quantidade;
                        valorTotalVenda += subtotal; 	

                        itensVenda.Add(new ItemVenda
                        {
                            ProdutoId = item.ProdutoId,
                            Quantidade = item.Quantidade,
                            PrecoUnitario = produto.Preco,
                            Subtotal = subtotal
                        });

                        // 1. Aplica a subtração no objeto rastreado
                        produto.Estoque -= item.Quantidade; 
                        
                        // 2. Desanexa imediatamente o produto para que a 1ª chamada SALVE APENAS A VENDA.
                        // Usamos '!' para garantir ao compilador que 'produto' não é nulo.
                        _context.Entry(produto!).State = EntityState.Detached; 
                        
                        // 3. Adiciona o produto modificado a uma lista auxiliar para salvar na 2ª chamada.
                        produtosParaAtualizar.Add(produto); 
                    }

                    var novaVenda = new Venda
                    { 	
                        CnpjCpf = request.CnpjCpf,
                        FormaPagamento = request.FormaPagamento,
                        ValorPago = request.ValorPago,
                        ValorTotal = valorTotalVenda,
                        Status = "PAGO",
                        DataHora = DateTime.Now
                    };

                    _context.Vendas.Add(novaVenda);
                    
                    // 4. PRIMEIRA CHAMADA: Salva a Venda (obtendo o ID). O Produto é ignorado (Detached).
                    await _context.SaveChangesAsync(); 

                    // 5. ANEXA NOVAMENTE: Anexa os produtos modificados e os marca para UPDATE
                    foreach(var produto in produtosParaAtualizar)
                    {
                       
                        _context.Produtos.Update(produto); 
                    }
                    
                   
                    foreach (var item in itensVenda)
                    {
                        item.VendaId = novaVenda.Id;
                        _context.ItensVenda.Add(item);
                        _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                        {
                            IdUsuario = UsuarioResponsavelId, 
                            IdProduto = item.ProdutoId,
                            Quantidade = -item.Quantidade,
                            DataHora = DateTime.Now,
                            TipoMovimentacao = "SAIDA", 
                            IdVenda = novaVenda.Id 
                        });
                    }

                    
                    await _context.SaveChangesAsync(); 

                    await transaction.CommitAsync();

                   
                    return CreatedAtAction(nameof(PostVenda), new { id = novaVenda.Id }, novaVenda); 
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Erro ao processar a venda. A transação foi desfeita.");
                }
            }
        }

    }
}