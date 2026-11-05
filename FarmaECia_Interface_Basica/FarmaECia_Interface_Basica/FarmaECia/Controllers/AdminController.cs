using FarmaECia.Data;
using FarmaECia.Models;
using FarmaECia.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly FarmaDbContext _banco;
    public AdminController(FarmaDbContext banco) => _banco = banco;

    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            ProdutosAtivos = await _banco.Produtos.CountAsync(p => p.Ativo),
            UnidadesEmEstoque = await _banco.Produtos.Where(p => p.Ativo).SumAsync(p => (int?)p.Estoque) ?? 0,
            PedidosAbertos = await _banco.Pedidos.CountAsync(p => p.Status != "Concluído" && p.Status != "Cancelado"),
            TotalVendas = await _banco.Vendas.Where(v => v.Status != "Cancelada").SumAsync(v => (decimal?)v.Total) ?? 0m,
            ClientesCadastrados = await _banco.Usuarios.CountAsync(u => u.Perfil == "Cliente")
        };
        return View(model);
    }

    public async Task<IActionResult> Produtos()
    {
        var produtos = await _banco.Produtos.AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
        return View(produtos);
    }

    [HttpGet]
    public IActionResult CriarProduto() => View("ProdutoForm", new Produto { Ativo = true, Estoque = 0, Ilustracao = "✚" });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarProduto([Bind("Nome,Descricao,Categoria,Ilustracao,Selo,Preco,PrecoAnterior,Estoque,Ativo")] Produto produto)
    {
        if (!ModelState.IsValid) return View("ProdutoForm", produto);
        _banco.Produtos.Add(produto);
        await _banco.SaveChangesAsync();
        TempData["Mensagem"] = "Produto cadastrado.";
        return RedirectToAction(nameof(Produtos));
    }

    [HttpGet]
    public async Task<IActionResult> EditarProduto(int id)
    {
        var produto = await _banco.Produtos.FindAsync(id);
        return produto == null ? NotFound() : View("ProdutoForm", produto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarProduto(int id, [Bind("Id,Nome,Descricao,Categoria,Ilustracao,Selo,Preco,PrecoAnterior,Estoque,Ativo")] Produto form)
    {
        if (id != form.Id) return BadRequest();
        if (!ModelState.IsValid) return View("ProdutoForm", form);
        var produto = await _banco.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        produto.Nome = form.Nome; produto.Descricao = form.Descricao; produto.Categoria = form.Categoria;
        produto.Ilustracao = form.Ilustracao; produto.Selo = form.Selo; produto.Preco = form.Preco;
        produto.PrecoAnterior = form.PrecoAnterior; produto.Estoque = form.Estoque; produto.Ativo = form.Ativo;
        await _banco.SaveChangesAsync();
        TempData["Mensagem"] = "Produto atualizado.";
        return RedirectToAction(nameof(Produtos));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DesativarProduto(int id)
    {
        var produto = await _banco.Produtos.FindAsync(id);
        if (produto == null) return NotFound();
        produto.Ativo = !produto.Ativo;
        await _banco.SaveChangesAsync();
        TempData["Mensagem"] = produto.Ativo ? "Produto reativado." : "Produto desativado e ocultado da loja.";
        return RedirectToAction(nameof(Produtos));
    }

    public async Task<IActionResult> Pedidos()
    {
        var pedidos = await _banco.Pedidos.AsNoTracking().Include(p => p.Usuario).Include(p => p.Itens).OrderByDescending(p => p.DataPedido).ToListAsync();
        return View(pedidos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarStatusPedido(int id, string status)
    {
        var permitidos = new[] { "Recebido", "Em preparação", "Enviado", "Concluído", "Cancelado" };
        if (!permitidos.Contains(status)) return BadRequest();
        var pedido = await _banco.Pedidos.Include(p => p.Itens).ThenInclude(i => i.Produto).Include(p => p.Venda).FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();
        if (pedido.Status == "Cancelado" && status != "Cancelado")
        {
            TempData["Mensagem"] = "Um pedido cancelado não pode ser reaberto neste protótipo.";
            return RedirectToAction(nameof(Pedidos));
        }
        if (pedido.Status != "Cancelado" && status == "Cancelado")
        {
            foreach (var item in pedido.Itens) item.Produto.Estoque += item.Quantidade;
            if (pedido.Venda != null) pedido.Venda.Status = "Cancelada";
        }
        pedido.Status = status;
        await _banco.SaveChangesAsync();
        TempData["Mensagem"] = $"Pedido #{pedido.Id} atualizado para: {status}.";
        return RedirectToAction(nameof(Pedidos));
    }

    [HttpGet]
    public async Task<IActionResult> EditarPedido(int id)
    {
        var pedido = await _banco.Pedidos.AsNoTracking()
            .Include(p => p.Usuario)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id);
        return pedido == null ? NotFound() : View(pedido);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SalvarEdicaoPedido(int id, string status, Dictionary<int, int>? quantidades)
    {
        var permitidos = new[] { "Recebido", "Em preparação", "Enviado", "Concluído", "Cancelado" };
        if (!permitidos.Contains(status)) return BadRequest();

        await using var transacao = await _banco.Database.BeginTransactionAsync();
        var pedido = await _banco.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Itens).ThenInclude(i => i.Produto)
            .Include(p => p.Venda).ThenInclude(v => v!.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido == null) return NotFound();
        if (pedido.Status == "Cancelado")
        {
            await transacao.RollbackAsync();
            TempData["Mensagem"] = "Um pedido cancelado não pode ser editado nem reaberto.";
            return RedirectToAction(nameof(Pedidos));
        }

        try
        {
            if (status == "Cancelado")
            {
                foreach (var item in pedido.Itens)
                    item.Produto.Estoque += item.Quantidade;
                if (pedido.Venda != null) pedido.Venda.Status = "Cancelada";
                pedido.Status = "Cancelado";
            }
            else
            {
                foreach (var item in pedido.Itens.ToList())
                {
                    var novaQuantidade = quantidades != null && quantidades.TryGetValue(item.Id, out var informada)
                        ? informada : item.Quantidade;
                    if (novaQuantidade < 0 || novaQuantidade > 100000)
                    {
                        await transacao.RollbackAsync();
                        TempData["Mensagem"] = "Informe quantidades entre 0 e 100.000.";
                        return RedirectToAction(nameof(EditarPedido), new { id });
                    }

                    var diferenca = novaQuantidade - item.Quantidade;
                    if (diferenca > 0 && item.Produto.Estoque < diferenca)
                    {
                        await transacao.RollbackAsync();
                        TempData["Mensagem"] = $"Estoque insuficiente para aumentar {item.NomeProduto}. Disponível: {item.Produto.Estoque}.";
                        return RedirectToAction(nameof(EditarPedido), new { id });
                    }

                    // Se a quantidade aumenta, consome o estoque; se diminui, devolve a diferença.
                    item.Produto.Estoque -= diferenca;
                    if (novaQuantidade == 0)
                    {
                        var itemVenda = pedido.Venda?.Itens.FirstOrDefault(v => v.ProdutoId == item.ProdutoId);
                        if (itemVenda != null) _banco.ItensVenda.Remove(itemVenda);
                        pedido.Itens.Remove(item);
                        _banco.ItensPedido.Remove(item);
                    }
                    else
                    {
                        item.Quantidade = novaQuantidade;
                        var itemVenda = pedido.Venda?.Itens.FirstOrDefault(v => v.ProdutoId == item.ProdutoId);
                        if (itemVenda != null) itemVenda.Quantidade = novaQuantidade;
                    }
                }

                if (!pedido.Itens.Any())
                {
                    await transacao.RollbackAsync();
                    TempData["Mensagem"] = "O pedido precisa manter pelo menos um item. Use Cancelar pedido para invalidá-lo.";
                    return RedirectToAction(nameof(EditarPedido), new { id });
                }

                pedido.Total = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
                if (pedido.Venda != null) pedido.Venda.Total = pedido.Total;
                pedido.Status = status;
            }

            await _banco.SaveChangesAsync();
            await transacao.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transacao.RollbackAsync();
            TempData["Mensagem"] = "O estoque mudou durante a edição. Reabra o pedido, confira as quantidades e tente novamente.";
            return RedirectToAction(nameof(Pedidos));
        }

        TempData["Mensagem"] = $"Pedido #{pedido.Id} e seus itens foram atualizados.";
        return RedirectToAction(nameof(Pedidos));
    }

    public async Task<IActionResult> Vendas()
    {
        var vendas = await _banco.Vendas.AsNoTracking().Include(v => v.Usuario).Include(v => v.Pedido).Include(v => v.Itens).OrderByDescending(v => v.DataVenda).ToListAsync();
        ViewBag.TotalAtivo = vendas.Where(v => v.Status != "Cancelada").Sum(v => v.Total);
        return View(vendas);
    }
}
