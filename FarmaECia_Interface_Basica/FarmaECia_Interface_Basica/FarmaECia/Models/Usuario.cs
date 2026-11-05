using System.ComponentModel.DataAnnotations;

namespace FarmaECia.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(180)]
    public string Email { get; set; } = string.Empty;

    // Nunca guardar a senha em texto puro. Este campo armazena um hash.
    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Perfil { get; set; } = "Cliente";

    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
}
