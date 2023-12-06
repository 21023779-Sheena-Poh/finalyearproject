using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CDSS.Models;

public class PasswordDTO
{
    [Required(ErrorMessage = "Current Password Required")]
    [DataType(DataType.Password)]
    [Remote("VerifyCurrentPassword", "MedicalStaffs", ErrorMessage = "Current Password Incorrect")]
    public string CurrentPwd { get; set; } = null!;

    [Required(ErrorMessage = "New Password Required")]
    [DataType(DataType.Password)]
    [Remote("VerifyNewPassword", "MedicalStaffs",
          ErrorMessage = "New Password Invalid")]
    public string NewPwd { get; set; } = null!;

    [Required(ErrorMessage = "Confirm Password Required")]
    [DataType(DataType.Password)]
    [Compare("NewPwd", ErrorMessage = "Passwords Unmatched")]
    public string ConfirmPwd { get; set; } = null!;
}

