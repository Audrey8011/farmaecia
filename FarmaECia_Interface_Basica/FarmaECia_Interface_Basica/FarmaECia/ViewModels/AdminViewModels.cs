using FarmaECia.Models;

namespace FarmaECia.ViewModels;

public class AdminDashboardViewModel
{
    public int ProdutosAtivos { get; set; }
    public int UnidadesEmEstoque { get; set; }
    public int PedidosAbertos { get; set; }
    public decimal TotalVendas { get; set; }
    public int ClientesCadastrados { get; set; }
}

public class ProdutoListaViewModel
{
    public List<Produto> Produtos { get; set; } = new();
}
