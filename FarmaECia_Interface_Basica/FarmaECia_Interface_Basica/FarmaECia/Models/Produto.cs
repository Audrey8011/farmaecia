using System.ComponentModel.DataAnnotations;

namespace FarmaECia.Models;

public class Produto
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Categoria { get; set; } = string.Empty;

    [StringLength(20)]
    public string Ilustracao { get; set; } = "✚";

    [StringLength(40)]
    public string Selo { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Preco { get; set; }

    [Range(0, 1000000)]
    public decimal? PrecoAnterior { get; set; }

    [Range(0, 1000000)]
    public int Estoque { get; set; }

    public bool Ativo { get; set; } = true;

    // SQL Server atualiza este valor e permite detectar alterações concorrentes no estoque.
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
    public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
}
