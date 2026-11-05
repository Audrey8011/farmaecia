namespace FarmaECia.Models;

public class LinhaCarrinhoViewModel
{
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal Subtotal => Produto.Preco * Quantidade;
}

public class CarrinhoViewModel
{
    public List<LinhaCarrinhoViewModel> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(item => item.Subtotal);
    public int QuantidadeTotal => Itens.Sum(item => item.Quantidade);
}
