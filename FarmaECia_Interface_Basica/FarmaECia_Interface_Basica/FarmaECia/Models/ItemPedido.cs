namespace FarmaECia.Models;

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
    public decimal Subtotal => Quantidade * PrecoUnitario;
}
