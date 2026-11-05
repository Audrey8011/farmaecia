using System.ComponentModel.DataAnnotations;

namespace FarmaECia.ViewModels;

public class RegistrarViewModel
{
    [Required, StringLength(100, MinimumLength = 2)]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), DataType(DataType.Password)]
    [Display(Name = "Senha (mínimo de 8 caracteres)")]
    public string Senha { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Senha))]
    [Display(Name = "Confirme a senha")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}

public class EntrarViewModel
{
    [Required, EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;
}
