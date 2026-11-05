using FarmaECia.Data;
using FarmaECia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Controllers;

public class HomeController : Controller
{
    private readonly FarmaDbContext _banco;
    public HomeController(FarmaDbContext banco) => _banco = banco;

    public async Task<IActionResult> Index(string? q, string? categoria)
    {
        var consulta = _banco.Produtos.AsNoTracking().Where(p => p.Ativo);
        if (!string.IsNullOrWhiteSpace(q))
            consulta = consulta.Where(p => p.Nome.Contains(q) || p.Descricao.Contains(q) || p.Categoria.Contains(q));
        if (!string.IsNullOrWhiteSpace(categoria))
            consulta = consulta.Where(p => p.Categoria == categoria);

        var model = new HomeViewModel
        {
            Produtos = await consulta.OrderBy(p => p.Nome).ToListAsync(),
            Categorias = await _banco.Produtos.AsNoTracking().Where(p => p.Ativo).Select(p => p.Categoria).Distinct().OrderBy(c => c).ToListAsync(),
            Busca = q,
            CategoriaSelecionada = categoria
        };
        return View(model);
    }

    public async Task<IActionResult> Ofertas()
    {
        var produtos = await _banco.Produtos.AsNoTracking()
            .Where(p => p.Ativo && p.PrecoAnterior != null && p.PrecoAnterior > p.Preco)
            .OrderBy(p => p.Nome)
            .ToListAsync();

        return View(produtos);
    }

    public IActionResult Error() => View();
}
