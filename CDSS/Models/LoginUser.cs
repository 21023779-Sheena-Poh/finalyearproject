using System.ComponentModel.DataAnnotations;

namespace CDSS.Models;

public class LoginUser
{
    [Required(ErrorMessage = "Please enter Username")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Please enter Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

