using System.Security.Claims;
using FarmaECia.Data;
using FarmaECia.Models;
using FarmaECia.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaECia.Controllers;

public class ContaController : Controller
{
    private readonly FarmaDbContext _banco;
    private readonly PasswordHasher<Usuario> _hasher = new();
    public ContaController(FarmaDbContext banco) => _banco = banco;

    [HttpGet]
    public IActionResult Registrar() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View(new RegistrarViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(RegistrarViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var email = model.Email.Trim().ToLowerInvariant();
        if (await _banco.Usuarios.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
            return View(model);
        }
        var usuario = new Usuario { Nome = model.Nome.Trim(), Email = email, Perfil = "Cliente", DataCadastro = DateTime.Now };
        usuario.SenhaHash = _hasher.HashPassword(usuario, model.Senha);
        _banco.Usuarios.Add(usuario);
        await _banco.SaveChangesAsync();
        await EntrarComCookie(usuario);
        TempData["Mensagem"] = "Conta criada com sucesso. Boas-vindas à Farma&Cia!";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Entrar(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new EntrarViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrar(EntrarViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(model);
        var email = model.Email.Trim().ToLowerInvariant();
        var usuario = await _banco.Usuarios.SingleOrDefaultAsync(u => u.Email == email);
        if (usuario == null || _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
            return View(model);
        }
        await EntrarComCookie(usuario);
        TempData["Mensagem"] = "Login realizado com sucesso.";
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return usuario.Perfil == "Administrador" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Mensagem"] = "Você saiu da sua conta.";
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AcessoNegado() => View();

    private async Task EntrarComCookie(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil)
        };
        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidade));
    }
}
