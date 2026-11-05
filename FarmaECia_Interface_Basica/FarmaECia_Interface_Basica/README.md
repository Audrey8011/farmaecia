# Farma&Cia — loja funcional

Projeto ASP.NET Core MVC da Farma&Cia com a interface visual integrada, responsiva e pronta para uso local.

## O que já está pronto

- Layout completo com logo, busca, navegação, cesta e rodapé.
- Página inicial com banner, filtros por categoria e cartões de produto.
- Página de ofertas com os produtos em promoção.
- Login, cadastro de cliente, cesta, meus pedidos e painel administrativo estilizados.
- Controle de estoque, pedidos e vendas gravados no SQL Server LocalDB.
- CSS próprio em `FarmaECia/wwwroot/css/site.css`, sem depender de frameworks externos.
- Imagem da marca em `FarmaECia/wwwroot/images/logo-farma.png`.

O protótipo HTML original que estava na área de trabalho foi movido para `Prototipo-HTML/`, na raiz deste pacote, apenas como referência. A versão funcional é este projeto ASP.NET Core.

## Como executar

### Pelo Visual Studio Community

1. Abra `FarmaECia.sln`.
2. Aguarde a restauração do pacote `Microsoft.EntityFrameworkCore.SqlServer` pelo NuGet.
3. Verifique se o SQL Server LocalDB está instalado e disponível.
4. Confira `FarmaECia/appsettings.json`. A conexão incluída é:

   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FarmaECiaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   ```

5. Pressione `F5` (ou use o perfil **FarmaECia - http**).
6. Na primeira execução, `Program.cs` chama `InicializadorBanco`, que cria o banco/tabelas e insere os produtos demonstrativos e a conta inicial de administrador.

O site abre em `http://localhost:5186` no perfil HTTP (ou `https://localhost:7186` no perfil HTTPS).

### Pelo terminal

Na pasta que contém `FarmaECia.sln`:

```powershell
cd FarmaECia
dotnet run --launch-profile "FarmaECia - http"
```

Ou, na raiz deste pacote, execute `iniciar-site.bat` (para encerrar a execução local, use `parar-site.bat`).

### Página já em execução

Se o site já estiver aberto em `http://localhost:5186` antes de você apertar `F5`, execute `parar-site.bat` primeiro para liberar a porta.

## Conta administrativa de demonstração

- E-mail: `admin@farmaecia.local`
- Senha: `Admin@12345`

Use somente para testes locais e troque essa senha se adaptar o projeto para outro uso. Clientes podem criar uma conta pela página **Cadastre-se**.

## Páginas principais

| Página | Rota |
| --- | --- |
| Loja / catálogo | `/` |
| Ofertas | `/Home/Ofertas` |
| Entrar | `/Conta/Entrar` |
| Criar conta | `/Conta/Registrar` |
| Minha cesta | `/Carrinho` |
| Meus pedidos | `/Carrinho/MeusPedidos` |
| Painel administrativo | `/Admin` |

## Onde personalizar

- `FarmaECia/Views/Shared/_Layout.cshtml`: cabeçalho, busca, navegação e rodapé.
- `FarmaECia/Views/Home/Index.cshtml`: página inicial.
- `FarmaECia/Views/Home/_ProdutoCard.cshtml`: cartão de produto reutilizado na loja e nas ofertas.
- `FarmaECia/Views/Home/Ofertas.cshtml`: página de ofertas.
- `FarmaECia/Views/Conta/`: login e cadastro.
- `FarmaECia/Views/Carrinho/`: cesta e pedidos do cliente.
- `FarmaECia/Views/Admin/`: painel administrativo, estoque, produtos, pedidos e vendas.
- `FarmaECia/wwwroot/css/site.css`: todo o visual do site.
- `FarmaECia/wwwroot/images/`: imagens estáticas.

## Cuidados ao personalizar

Preserve os atributos Razor (`asp-controller`, `asp-action`, `asp-route-*`, `asp-for`) e os nomes de campos usados pelos controladores, como `id`, `status`, `quantidades[ID]` e os formulários com `method="post"`. Alterar esses pontos pode manter a tela visível, mas quebrar a função correspondente.

## Banco de dados

- O banco é criado automaticamente na primeira execução.
- Para inspecionar os dados, conecte o SQL Server Management Studio à instância `(localdb)\MSSQLLocalDB` e abra o banco `FarmaECiaDb`.
- Para recriar os dados demonstrativos, apague o banco `FarmaECiaDb` e execute a aplicação novamente.
