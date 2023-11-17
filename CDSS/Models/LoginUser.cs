using System.ComponentModel.DataAnnotations;

namespace CDSS.Models;

public class LoginUser
{
    [Required(ErrorMessage = "User ID cannot be empty!")]
    public string MedicalStaffId { get; set; } = null!;

    [Required(ErrorMessage = "Empty password not allowed!")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public int Id { get; set; }
    public string UserName { get; set; } = null!;
}

