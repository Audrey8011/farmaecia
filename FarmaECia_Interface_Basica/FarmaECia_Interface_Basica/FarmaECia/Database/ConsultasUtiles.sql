USE FarmaECiaDb;
GO

-- 1. Produtos ativos e saldo atual de estoque
SELECT Id, Nome, Categoria, Preco, Estoque, Ativo
FROM dbo.Produtos
WHERE Ativo = 1
ORDER BY Nome;
GO

-- 2. Histórico dos pedidos, com dados do cliente
SELECT p.Id AS Pedido, u.Nome AS Cliente, u.Email,
       p.DataPedido, p.Status, p.Total
FROM dbo.Pedidos AS p
INNER JOIN dbo.Usuarios AS u ON u.Id = p.UsuarioId
ORDER BY p.DataPedido DESC;
GO

-- 3. Registro de vendas, excluindo as canceladas
SELECT v.Id AS Venda, v.PedidoId AS Pedido, u.Nome AS Cliente,
       v.DataVenda, v.Total, v.Status
FROM dbo.Vendas AS v
INNER JOIN dbo.Usuarios AS u ON u.Id = v.UsuarioId
WHERE v.Status <> 'Cancelada'
ORDER BY v.DataVenda DESC;
GO

-- 4. Produtos vendidos em cada venda
SELECT v.Id AS Venda, iv.NomeProduto, iv.Quantidade,
       iv.PrecoUnitario, iv.Quantidade * iv.PrecoUnitario AS Subtotal
FROM dbo.ItensVenda AS iv
INNER JOIN dbo.Vendas AS v ON v.Id = iv.VendaId
ORDER BY v.DataVenda DESC, iv.NomeProduto;
GO

-- 5. Produtos com estoque baixo (cinco unidades ou menos)
SELECT Id, Nome, Estoque
FROM dbo.Produtos
WHERE Ativo = 1 AND Estoque <= 5
ORDER BY Estoque, Nome;
GO
