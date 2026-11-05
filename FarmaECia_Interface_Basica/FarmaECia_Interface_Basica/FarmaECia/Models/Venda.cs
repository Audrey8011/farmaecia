namespace FarmaECia.Models;

// Registro de venda criado quando o cliente confirma um pedido no protótipo.
public class Venda
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string Status { get; set; } = "Registrada";
    public Pedido Pedido { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}
