using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.ViewModels;


public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    [StringLength(100, ErrorMessage = "Не больше 100 символов")]
    [Display(Name = "Имя и фамилия")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Phone(ErrorMessage = "Неверный формат телефона")]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не короче 6 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Повторите пароль")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    [Display(Name = "Повторите пароль")]
    public string ConfirmPassword { get; set; } = "";


    [Range(typeof(bool), "true", "true", ErrorMessage = "Нужно согласие на обработку персональных данных")]
    [Display(Name = "Согласен на обработку персональных данных")]
    public bool AcceptPersonalData { get; set; }
}


public class LoginViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = "";

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; }
}
