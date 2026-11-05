namespace FarmaECia.Models;

public class HomeViewModel
{
    public List<Produto> Produtos { get; set; } = new();
    public List<string> Categorias { get; set; } = new();
    public string? Busca { get; set; }
    public string? CategoriaSelecionada { get; set; }
}
