using FarmaECia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Data;

public static class InicializadorBanco
{
    public static async Task InicializarAsync(IServiceProvider servicos)
    {
        using var escopo = servicos.CreateScope();
        var banco = escopo.ServiceProvider.GetRequiredService<FarmaDbContext>();
        await banco.Database.EnsureCreatedAsync();

        if (!await banco.Usuarios.AnyAsync(u => u.Perfil == "Administrador"))
        {
            var admin = new Usuario
            {
                Nome = "Administrador Farma&Cia",
                Email = "admin@farmaecia.local",
                Perfil = "Administrador",
                DataCadastro = DateTime.Now
            };
            admin.SenhaHash = new PasswordHasher<Usuario>().HashPassword(admin, "Admin@12345");
            banco.Usuarios.Add(admin);
        }

        if (!await banco.Produtos.AnyAsync())
        {
            banco.Produtos.AddRange(
                new Produto { Nome = "Protetor Solar FPS 50", Descricao = "Toque seco para o cuidado diário", Categoria = "Cuidados com a pele", Ilustracao = "☀", Selo = "MAIS VENDIDO", Preco = 49.90m, PrecoAnterior = 64.90m, Estoque = 35 },
                new Produto { Nome = "Hidratante Facial", Descricao = "Hidratação leve para todos os dias", Categoria = "Cuidados com a pele", Ilustracao = "✿", Selo = "OFERTA", Preco = 36.90m, PrecoAnterior = 44.90m, Estoque = 24 },
                new Produto { Nome = "Shampoo Nutritivo", Descricao = "Limpeza suave para os seus cabelos", Categoria = "Cabelos", Ilustracao = "♧", Selo = "-15%", Preco = 25.90m, PrecoAnterior = 30.50m, Estoque = 40 },
                new Produto { Nome = "Sabonete Líquido", Descricao = "Sensação de frescor no banho", Categoria = "Higiene", Ilustracao = "❋", Preco = 18.90m, Estoque = 55 },
                new Produto { Nome = "Creme para as Mãos", Descricao = "Textura leve e rápida absorção", Categoria = "Cuidados com a pele", Ilustracao = "♡", Selo = "OFERTA", Preco = 16.90m, PrecoAnterior = 21.90m, Estoque = 30 },
                new Produto { Nome = "Condicionador Suave", Descricao = "Cuidado diário para os fios", Categoria = "Cabelos", Ilustracao = "〰", Selo = "NOVIDADE", Preco = 27.90m, Estoque = 22 },
                new Produto { Nome = "Gelatina Salon Line #todecacho", Descricao = "Gelatina definidora de cachos Salon Line 550 g", Categoria = "Cabelos", Ilustracao = "🧴", Selo = "NOVIDADE", Preco = 39.90m, Estoque = 25 },
                new Produto { Nome = "Kit de Higiene Bucal", Descricao = "Uma rotina prática para o seu sorriso", Categoria = "Higiene", Ilustracao = "✦", Selo = "KIT ESPECIAL", Preco = 22.90m, Estoque = 18 },
                new Produto { Nome = "Álcool em Gel 70%", Descricao = "Frasco compacto para levar com você", Categoria = "Bem-estar", Ilustracao = "❖", Selo = "PRÁTICO", Preco = 9.90m, Estoque = 60 }
            );
        }
        await banco.SaveChangesAsync();
    }
}
