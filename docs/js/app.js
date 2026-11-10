/* Farma&Cia — versão estática (GitHub Pages)
   Dados demonstrativos salvos no navegador do visitante. */

(function () {
    "use strict";

    var CHAVES = {
        usuarios: "farmaeciaUsuarios",
        sessao: "farmaeciaSessao",
        cesta: "farmaeciaCesta",
        pedidos: "farmaeciaPedidos"
    };

    var PRODUTOS = [
        { id: 1, nome: "Protetor Solar FPS 50", descricao: "Toque seco para o cuidado diário", categoria: "Cuidados com a pele", selo: "MAIS VENDIDO", preco: 49.90, precoAnterior: 64.90, estoque: 35, imagem: "images/produtos/protetor-solar-fps50.jpeg" },
        { id: 2, nome: "Loção deo-hidratante", descricao: "Hidratação leve para todos os dias", categoria: "Cuidados com a pele", selo: "OFERTA", preco: 36.90, precoAnterior: 44.90, estoque: 24, imagem: "images/produtos/locao-deo-hidratante.jpeg" },
        { id: 3, nome: "Shampoo Nutritivo", descricao: "Limpeza suave para os seus cabelos", categoria: "Cabelos", selo: "-15%", preco: 25.90, precoAnterior: 30.50, estoque: 40, imagem: "images/produtos/shampoo-nutritivo.jpeg" },
        { id: 4, nome: "Sabonete Líquido", descricao: "Sensação de frescor no banho", categoria: "Higiene", selo: "", preco: 18.90, precoAnterior: null, estoque: 55, imagem: "images/produtos/sabonete-liquido.jpeg" },
        { id: 5, nome: "Creme para as Mãos", descricao: "Textura leve e rápida absorção", categoria: "Cuidados com a pele", selo: "OFERTA", preco: 16.90, precoAnterior: 21.90, estoque: 30, imagem: "images/produtos/creme-para-maos.jpeg" },
        { id: 6, nome: "Condicionador Suave", descricao: "Cuidado diário para os fios", categoria: "Cabelos", selo: "NOVIDADE", preco: 27.90, precoAnterior: null, estoque: 21, imagem: "images/produtos/condicionador-suave.jpeg" },
        { id: 7, nome: "Kit de Higiene Bucal", descricao: "Uma rotina prática para o seu sorriso", categoria: "Higiene", selo: "KIT ESPECIAL", preco: 22.90, precoAnterior: null, estoque: 18, imagem: "images/produtos/kit-higiene-bucal.jpeg" },
        { id: 8, nome: "Álcool em Gel 70%", descricao: "Frasco compacto para levar com você", categoria: "Bem-estar", selo: "PRÁTICO", preco: 9.90, precoAnterior: null, estoque: 55, imagem: "images/produtos/alcool-em-gel-70.jpeg" },
        { id: 9, nome: "Gelatina Salon Line #todecacho", descricao: "Gelatina definidora de cachos Salon Line 550 g", categoria: "Cabelos", selo: "NOVIDADE", preco: 39.90, precoAnterior: null, estoque: 25, imagem: "images/produtos/gelatina-salon-line.jpeg" }
    ];

    /* ============ utilidades ============ */

    function ler(chave, padrao) {
        try {
            var bruto = localStorage.getItem(chave);
            return bruto ? JSON.parse(bruto) : padrao;
        } catch (e) {
            return padrao;
        }
    }

    function gravar(chave, valor) {
        localStorage.setItem(chave, JSON.stringify(valor));
    }

    function e(ev, ref) { return ev.querySelector(ref); }
    function ea(ev, ref) { return Array.prototype.slice.call(ev.querySelectorAll(ref)); }
    function produtoPorId(id) {
        for (var i = 0; i < PRODUTOS.length; i++) if (PRODUTOS[i].id === id) return PRODUTOS[i];
        return null;
    }
    function dinheiro(n) {
        return "R$ " + n.toFixed(2).replace(".", ",");
    }
    function escaping(t) {
        return String(t).replace(/[&<>"']/g, function (c) {
            return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c];
        });
    }
    function icone(categoria) {
        switch (categoria) {
            case "Cuidados com a pele": return "🧴";
            case "Cabelos": return "✂️";
            case "Higiene": return "🧼";
            case "Bem-estar": return "💊";
            default: return "✚";
        }
    }

    /* ============ conta / sessão ============ */

    function contas() {
        var lista = ler(CHAVES.usuarios, null);
        if (!lista) {
            lista = [{ nome: "Administrador", email: "admin@farmaecia.local", senha: "Admin@12345" }];
            gravar(CHAVES.usuarios, lista);
        }
        return lista;
    }

    function usuarioAtual() {
        var email = localStorage.getItem(CHAVES.sessao);
        if (!email) return null;
        var lista = contas();
        for (var i = 0; i < lista.length; i++)
            if (lista[i].email.toLowerCase() === email.toLowerCase()) return lista[i];
        return null;
    }

    function sair() {
        localStorage.removeItem(CHAVES.sessao);
        location.href = "index.html";
    }

    /* ============ cesta ============ */

    function cesta() {
        return ler(CHAVES.cesta, {});
    }

    function guardarCesta(c) { gravar(CHAVES.cesta, c); }

    function quantidadeNaCesta() {
        var c = cesta(), total = 0;
        for (var id in c) total += c[id];
        return total;
    }

    function adicionarACesta(id) {
        var c = cesta();
        c[id] = (c[id] || 0) + 1;
        guardarCesta(c);
        atualizarBadge();
        var p = produtoPorId(id);
        aviso("“" + p.nome + "” foi adicionado à sua cesta!");
    }

    function removerDaCesta(id) {
        var c = cesta();
        if (!c[id]) return;
        c[id] -= 1;
        if (c[id] <= 0) delete c[id];
        guardarCesta(c);
        atualizarBadge();
    }

    function atualizarBadge() {
        var badge = document.getElementById("badge-carrinho");
        var qtd = quantidadeNaCesta();
        if (!badge) return;
        if (qtd > 0) {
            badge.textContent = qtd;
            badge.style.display = "";
        } else {
            badge.style.display = "none";
        }
    }

    function aviso(texto) {
        var caixa = document.getElementById("aviso-fugaz");
        if (!caixa) {
            caixa = document.createElement("div");
            caixa.id = "aviso-fugaz";
            caixa.className = "mensagem aviso-fugaz";
            document.body.appendChild(caixa);
        }
        caixa.textContent = texto;
        caixa.classList.add("aparecendo");
        clearTimeout(caixa._t);
        caixa._t = setTimeout(function () { caixa.classList.remove("aparecendo"); }, 3200);
    }

    /* ============ cartões de produto ============ */

    function cardProduto(p) {
        var desconto = p.precoAnterior && p.precoAnterior > p.preco
            ? Math.round((1 - p.preco / p.precoAnterior) * 100)
            : 0;

        var topo = p.imagem
            ? '<img class="produto-foto" src="' + escaping(p.imagem) + '" alt="' + escaping(p.nome) + '" loading="lazy" />'
            : '<span class="produto-ilustracao" aria-hidden="true">' + icone(p.categoria) + '</span>';

        var html = '<article class="produto-card">'
            + '<div class="produto-topo' + (p.imagem ? ' produto-com-imagem' : '') + '">'
            + topo
            + (p.selo ? '<span class="produto-selo">' + escaping(p.selo) + '</span>' : '')
            + '</div>'
            + '<p class="produto-categoria">' + escaping(p.categoria) + '</p>'
            + '<h3>' + escaping(p.nome) + '</h3>'
            + '<p class="produto-descricao">' + escaping(p.descricao) + '</p>'
            + '<div class="produto-precos">'
            + (desconto ? '<span class="preco-anterior">' + dinheiro(p.precoAnterior) + '</span>' : '')
            + '<span class="produto-preco">' + dinheiro(p.preco) + '</span>'
            + (desconto ? '<span class="produto-desconto">-' + desconto + '%</span>' : '')
            + '</div>';

        if (p.estoque > 0) {
            html += '<p class="produto-estoque disponivel">Disponível: ' + p.estoque + ' un.</p>'
                + '<button type="button" class="botao botao-bloco" data-acao="adicionar" data-id="' + p.id + '">Adicionar à cesta</button>';
        } else {
            html += '<p class="produto-estoque esgotado">Produto sem estoque</p>'
                + '<button type="button" class="botao botao-bloco" disabled>Indisponível</button>';
        }

        return html + '</article>';
    }

    function ligarBotoesCesta() {
        document.addEventListener("click", function (e) {
            var b = e.target.closest ? e.target.closest('[data-acao="adicionar"]') : null;
            if (b) adicionarACesta(parseInt(b.getAttribute("data-id"), 10));
        });
    }

    /* ============ páginas ============ */

    var app = {};
    var params = new URLSearchParams(location.search);

    app.iniciarIndex = function () {
        app.cabecalho();
        app.rodape();
        var q = (params.get("q") || "").trim().toLowerCase();
        var categoria = params.get("categoria") || "";
        var categorias = CATEGORIAS();

        var marcas = '<a class="filtro-pill' + (categoria ? '' : ' ativo') + '" href="index.html">Todas</a>';
        categorias.forEach(function (cat) {
            marcas += '<a class="filtro-pill' + (cat === categoria ? ' ativo' : '') + '" '
                + 'href="index.html?categoria=' + encodeURIComponent(cat) + '">' + escaping(cat) + '</a>';
        });
        var box = document.getElementById("filtros");
        if (box) box.innerHTML = marcas;

        var termos = q ? q.split(/\s+/) : [];
        var achados = PRODUTOS.filter(function (p) {
            var okCat = !categoria || p.categoria === categoria;
            var okBusca = termos.every(function (t) {
                return (p.nome + " " + p.descricao + " " + p.categoria).toLowerCase().indexOf(t) >= 0;
            });
            return okCat && okBusca;
        });

        var r = document.getElementById("resultado");
        if (r) r.textContent = achados.length + (achados.length === 1 ? " produto encontrado" : " produtos encontrados");

        var grade = document.getElementById("grade-produtos");
        var vazia = document.getElementById("estado-vazio");
        if (!grade) return;
        if (!achados.length) {
            grade.style.display = "none";
            if (vazia) vazia.style.display = "";
        } else {
            grade.style.display = "";
            if (vazia) vazia.style.display = "none";
            grade.innerHTML = achados.map(cardProduto).join("");
        }
    };

    app.iniciarOfertas = function () {
        app.cabecalho();
        app.rodape();
        var ofertas = PRODUTOS.filter(function (p) { return p.precoAnterior && p.precoAnterior > p.preco; });
        var grade = document.getElementById("grade-produtos");
        if (grade) grade.innerHTML = ofertas.map(cardProduto).join("");
    };

    app.iniciarCesta = function () {
        app.cabecalho();
        app.rodape();
        var area = document.getElementById("area-cesta");
        var c = cesta();
        var ids = Object.keys(c);
        var u = usuarioAtual();
        if (u) { console.log("Sessão ativa para listar pedidos"); }

        if (!ids.length) {
            area.innerHTML = estadoVazio("🛒", "Sua cesta está vazia",
                "Explore nossos produtos e adicione seus favoritos à cesta.",
                '<a class="botao" href="index.html">Ver produtos</a>');
            return;
        }

        var total = 0, itensTotal = 0;
        var linhas = ids.map(function (idTxt) {
            var id = parseInt(idTxt, 10);
            var p = produtoPorId(id);
            var qtd = c[idTxt];
            var sub = p.preco * qtd;
            total += sub; itensTotal += qtd;
            return '<tr><td><strong>' + escaping(p.nome) + '</strong><br /><small>' + escaping(p.categoria) + '</small></td>'
                + '<td>' + dinheiro(p.preco) + '</td>'
                + '<td>' + qtd + '</td>'
                + '<td><strong>' + dinheiro(sub) + '</strong></td>'
                + '<td><button type="button" class="botao botao-contorno botao-pequeno" data-acao="remover" data-id="' + id + '">Remover uma unidade</button></td></tr>';
        }).join("");

        var acao = u
            ? '<button type="button" class="botao" id="botao-finalizar">Confirmar pedido</button>'
            : '<a class="botao" href="entrar.html?return=cesta.html">Entrar para finalizar</a>';
        var textoLogin = u ? '' : '<p class="texto-auxiliar">Para confirmar o pedido, entre na sua conta. Depois, volte à cesta para finalizar.</p>';

        area.innerHTML = '<div class="tabela-responsiva"><table class="tabela">'
            + '<thead><tr><th>Produto</th><th>Preço unitário</th><th>Quantidade</th><th>Subtotal</th><th>Ação</th></tr></thead>'
            + '<tbody>' + linhas + '</tbody></table></div>'
            + '<div class="resumo-compra"><p class="total">' + itensTotal + (itensTotal === 1 ? " item" : " itens")
            + ' na cesta &nbsp;|&nbsp; Total: <strong>' + dinheiro(total) + '</strong></p>' + acao + '</div>' + textoLogin;

        area.addEventListener("click", function (ev) {
            var botaoRemover = ev.target.closest ? ev.target.closest('[data-acao="remover"]') : null;
            if (botaoRemover) {
                removerDaCesta(parseInt(botaoRemover.getAttribute("data-id"), 10));
                location.reload();
            }
            if (ev.target.id === "botao-finalizar") finalizarCompra();
        });
    };

    function estadoVazio(iconeTxt, titulo, texto, acao) {
        return '<div class="estado-vazio"><span class="icone" aria-hidden="true">' + iconeTxt + '</span>'
            + '<h2>' + titulo + '</h2><p>' + texto + '</p>' + (acao || "") + '</div>';
    }

    function finalizarCompra() {
        var c = cesta();
        var ids = Object.keys(c);
        if (!ids.length) return;
        var pedidos = ler(CHAVES.pedidos, []);
        var total = 0;
        var itens = ids.map(function (idTxt) {
            var p = produtoPorId(parseInt(idTxt, 10));
            total += p.preco * c[idTxt];
            return { nome: p.nome, preco: p.preco, quantidade: c[idTxt] };
        });
        pedidos.unshift({
            numero: String(Date.now()).slice(-6),
            data: new Date().toLocaleString("pt-BR"),
            itens: itens,
            total: total
        });
        gravar(CHAVES.pedidos, pedidos);
        guardarCesta({});
        location.href = "pedidos.html?ok=1";
    }

    app.iniciarPedidos = function () {
        app.cabecalho();
        app.rodape();
        var area = document.getElementById("area-pedidos");
        var pedidos = ler(CHAVES.pedidos, []);

        if ((params.get("ok") || "") === "1") {
            area.insertAdjacentHTML("beforebegin", '<div class="navegacao"><div class="mensagem" role="status">Pedido confirmado com sucesso! Obrigado por comprar na Farma&amp;Cia.</div></div>');
        }

        if (!pedidos.length) {
            area.innerHTML = estadoVazio("📦", "Nenhum pedido ainda",
                "Seus pedidos aparecerão aqui depois da primeira compra.",
                '<a class="botao" href="index.html">Ver produtos</a>');
            return;
        }

        area.innerHTML = pedidos.map(function (pe) {
            var linhas = pe.itens.map(function (it) {
                return '<li>' + it.quantidade + '× ' + escaping(it.nome) + ' — ' + dinheiro(it.preco * it.quantidade) + '</li>';
            }).join("");
            return '<section class="cartao pedido-cartao">'
                + '<h3>Pedido nº ' + escaping(pe.numero) + ' <small class="pedido-data">' + escaping(pe.data) + '</small></h3>'
                + '<ul class="pedido-itens">' + linhas + '</ul>'
                + '<p class="pedido-total">Total: <strong>' + dinheiro(pe.total) + '</strong></p>'
                + '</section>';
        }).join("");
    };

    app.iniciarEntrar = function () {
        app.cabecalho();
        app.rodape();
        var dados = contas();
        var u = usuarioAtual();
        if (u) { location.href = "index.html"; return; }
        var destino = params.get("return") || "index.html";
        var form = document.getElementById("form-entrar");
        form.addEventListener("submit", function (ev) {
            ev.preventDefault();
            var email = form.email.value.trim().toLowerCase();
            var senha = form.senha.value;
            var achou = null;
            for (var i = 0; i < dados.length; i++)
                if (dados[i].email.toLowerCase() === email) achou = dados[i];
            if (!achou || achou.senha !== senha) {
                var msg = document.getElementById("validacao-entrar");
                msg.textContent = "E-mail ou senha incorretos. Verifique e tente de novo.";
                return;
            }
            localStorage.setItem(CHAVES.sessao, achou.email);
            location.href = destino;
        });
    };

    app.iniciarCadastro = function () {
        app.cabecalho();
        app.rodape();
        var form = document.getElementById("form-cadastro");
        form.addEventListener("submit", function (ev) {
            ev.preventDefault();
            var msg = document.getElementById("validacao-cadastro");
            var nome = form.nome.value.trim();
            var email = form.email.value.trim().toLowerCase();
            var senha = form.senha.value;
            var conf = form.confirmar.value;

            if (senha.length < 8) { msg.textContent = "A senha precisa ter pelo menos 8 caracteres."; return; }
            if (senha !== conf) { msg.textContent = "As senhas digitadas não conferem."; return; }
            var dados = contas();
            for (var i = 0; i < dados.length; i++) {
                if (dados[i].email.toLowerCase() === email) {
                    msg.textContent = "Esse e-mail já possui conta. Tente entrar.";
                    return;
                }
            }
            dados.push({ nome: nome, email: email, senha: senha });
            gravar(CHAVES.usuarios, dados);
            localStorage.setItem(CHAVES.sessao, email);
            location.href = "index.html";
        });
    };

    /* ============ cabeçalho e rodapé ============ */

    app.cabecalho = function () {
        var u = usuarioAtual();
        var pagina = location.pathname.split("/").pop() || "index.html";
        function ativo(qual) { return pagina === qual ? " ativo" : ""; }
        var menu = '<li><a class="menu-link' + ativo("index.html") + '" href="index.html">Início</a></li>'
            + '<li><a class="menu-link' + ativo("ofertas.html") + '" href="ofertas.html">Ofertas</a></li>'
            + '<li><a class="menu-link link-carrinho' + ativo("cesta.html") + '" href="cesta.html">Minha cesta'
            + ' <span class="badge-carrinho" id="badge-carrinho" style="display:none"></span></a></li>';

        if (u) {
            menu += '<li><a class="menu-link' + ativo("pedidos.html") + '" href="pedidos.html">Meus pedidos</a></li>'
                + '<li class="menu-usuario">Olá, <strong>' + escaping(u.nome.split(" ")[0]) + '</strong></li>'
                + '<li><button type="button" class="botao botao-contorno botao-pequeno" id="botao-sair">Sair</button></li>';
        } else {
            menu += '<li><a class="menu-link' + ativo("entrar.html") + '" href="entrar.html">Entrar</a></li>'
                + '<li><a class="botao botao-pequeno" href="cadastro.html">Cadastre-se</a></li>';
        }

        var alvo = document.getElementById("cabecalho");
        if (!alvo) return;
        alvo.innerHTML =
            '<div class="faixa-topo"><div class="navegacao faixa-topo-conteudo">'
            + '<span>Entrega rápida e atendimento de confiança</span>'
            + '<span>Parcele suas compras em até 3x</span></div></div>'
            + '<nav class="navegacao navegacao-principal" aria-label="Navegação principal">'
            + '<a class="marca" href="index.html" aria-label="Farma&Cia - Página inicial">'
            + '<img src="images/logo-farma.png" alt="" class="logo-farma" />'
            + '<span>Farma<strong>&amp;Cia</strong></span></a>'
            + '<form class="form-pesquisa" action="index.html" method="get" role="search">'
            + '<label class="visualmente-oculto" for="busca">Buscar produtos</label>'
            + '<input id="busca" type="search" name="q" class="barra-pesquisa" placeholder="O que você procura?" />'
            + '<button type="submit" class="botao botao-pequeno">Buscar</button></form>'
            + '<ul class="menu-acoes">' + menu + '</ul></nav>';

        atualizarBadge();
        var sa = document.getElementById("botao-sair");
        if (sa) sa.addEventListener("click", sair);
        var busca = document.getElementById("busca");
        if (busca && buscaGlobal()) busca.value = buscaGlobal();
    };

    function buscaGlobal() {
        return new URLSearchParams(location.search).get("q") || "";
    }

    function CATEGORIAS() {
        return ["Cuidados com a pele", "Cabelos", "Higiene", "Bem-estar"];
    }

    app.rodape = function () {
        var alvo = document.getElementById("rodape");
        if (!alvo) return;
        var ano = new Date().getFullYear();
        alvo.innerHTML = '<div class="rodape-conteudo">'
            + '<div class="rodape-marca"><img src="images/logo-farma.png" alt="" />'
            + '<div><strong>Farma&amp;Cia</strong><p>Um carinho a mais para você.</p></div></div>'
            + '<div><h3>Atendimento</h3><p>Segunda a sábado, das 8h às 20h</p><p>contato@farmaecia.local</p></div>'
            + '<div><h3>Sobre o projeto</h3><p>Projeto fictício desenvolvido para fins acadêmicos.</p>'
            + '<p>Produtos e preços são apenas demonstrativos.</p></div></div>'
            + '<div class="rodape-base"><p>© ' + ano + ' Farma&amp;Cia — Todos os direitos reservados.</p></div>';
    };

    ligarBotoesCesta();
    window.farmaApp = app;
})();
