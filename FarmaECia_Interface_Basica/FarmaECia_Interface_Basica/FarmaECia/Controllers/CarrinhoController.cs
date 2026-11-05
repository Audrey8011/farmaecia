using System.Security.Claims;
using FarmaECia.Data;
using FarmaECia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Controllers;

public class CarrinhoController : Controller
{
    private const string ChaveCarrinho = "CarrinhoIds";
    private readonly FarmaDbContext _banco;
    public CarrinhoController(FarmaDbContext banco) => _banco = banco;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(int id, string? returnUrl = null)
    {
        var produto = await _banco.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
        if (produto == null) return NotFound();
        if (produto.Estoque <= 0)
        {
            TempData["Mensagem"] = "Este produto está sem estoque no momento.";
            return RedirectToAction("Index", "Home");
        }

        var ids = LerIds();
        ids.Add(id);
        GravarIds(ids);
        TempData["Mensagem"] = "Produto adicionado à sua cesta.";
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Index()
    {
        var ids = LerIds();
        var quantidades = ids.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());
        // Produtos desativados continuam visíveis na cesta para que o cliente possa removê-los.
        var produtos = await _banco.Produtos.AsNoTracking()
            .Where(p => quantidades.Keys.Contains(p.Id))
            .ToListAsync();
        var model = new CarrinhoViewModel
        {
            Itens = produtos.Select(p => new LinhaCarrinhoViewModel
            {
                Produto = p,
                Quantidade = quantidades[p.Id]
            }).ToList()
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Remover(int id)
    {
        var ids = LerIds();
        ids.Remove(id); // Remove somente uma unidade do produto.
        GravarIds(ids);
        TempData["Mensagem"] = "Uma unidade foi removida da cesta.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> FinalizarCompra()
    {
        var ids = LerIds();
        if (ids.Count == 0)
        {
            TempData["Mensagem"] = "Sua cesta está vazia.";
            return RedirectToAction(nameof(Index));
        }

        var usuarioIdTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(usuarioIdTexto, out var usuarioId)) return Challenge();
        var quantidades = ids.GroupBy(id => id).ToDictionary(g => g.Key, g => g.Count());

        await using var transacao = await _banco.Database.BeginTransactionAsync();
        try
        {
            var produtos = await _banco.Produtos
                .Where(p => quantidades.Keys.Contains(p.Id) && p.Ativo)
                .ToListAsync();

            if (produtos.Count != quantidades.Count)
            {
                await transacao.RollbackAsync();
                TempData["Mensagem"] = "Um ou mais produtos não estão mais disponíveis. Remova-os da cesta e tente novamente.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var produto in produtos)
            {
                if (produto.Estoque < quantidades[produto.Id])
                {
                    await transacao.RollbackAsync();
                    TempData["Mensagem"] = $"Estoque insuficiente para {produto.Nome}. Disponível: {produto.Estoque}.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                DataPedido = DateTime.Now,
                Status = "Recebido"
            };

            foreach (var produto in produtos)
            {
                var quantidade = quantidades[produto.Id];
                produto.Estoque -= quantidade;
                pedido.Itens.Add(new ItemPedido
                {
                    ProdutoId = produto.Id,
                    NomeProduto = produto.Nome,
                    Quantidade = quantidade,
                    PrecoUnitario = produto.Preco
                });
            }

            pedido.Total = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
            _banco.Pedidos.Add(pedido);
            await _banco.SaveChangesAsync();

            // Neste protótipo, confirmar o pedido também grava a venda demonstrativa.
            // Não há pagamento eletrônico real.
            var venda = new Venda
            {
                PedidoId = pedido.Id,
                UsuarioId = usuarioId,
                DataVenda = DateTime.Now,
                Total = pedido.Total,
                Status = "Registrada",
                Itens = pedido.Itens.Select(i => new ItemVenda
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.NomeProduto,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
            _banco.Vendas.Add(venda);
            await _banco.SaveChangesAsync();
            await transacao.CommitAsync();

            HttpContext.Session.Remove(ChaveCarrinho);
            TempData["Mensagem"] = $"Pedido #{pedido.Id} registrado. O estoque foi atualizado.";
            return RedirectToAction(nameof(MeusPedidos));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transacao.RollbackAsync();
            TempData["Mensagem"] = "O estoque foi alterado por outra compra ao mesmo tempo. Confira a cesta e tente novamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize]
    public async Task<IActionResult> MeusPedidos()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pedidos = await _banco.Pedidos.AsNoTracking()
            .Include(p => p.Itens)
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();
        return View(pedidos);
    }

    private List<int> LerIds()
    {
        var dados = HttpContext.Session.GetString(ChaveCarrinho);
        return string.IsNullOrWhiteSpace(dados)
            ? new List<int>()
            : dados.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.TryParse(x, out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();
    }

    private void GravarIds(List<int> ids) => HttpContext.Session.SetString(ChaveCarrinho, string.Join(',', ids));
}
