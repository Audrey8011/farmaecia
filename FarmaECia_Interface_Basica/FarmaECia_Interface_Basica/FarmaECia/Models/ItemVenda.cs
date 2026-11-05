namespace FarmaECia.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public Venda Venda { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
