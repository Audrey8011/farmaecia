using System.ComponentModel.DataAnnotations;

namespace FarmaECia.Models;

public class Pedido
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.Now;
    [StringLength(30)] public string Status { get; set; } = "Recebido";
    public decimal Total { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    public Venda? Venda { get; set; }
}
