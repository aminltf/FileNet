using System.ComponentModel.DataAnnotations;

namespace FileNet.Domain.Enums;

public enum Gender : byte
{
    [Display(Name = "مرد")]
    Male = 0,
    
    [Display(Name = "زن")]
    Female = 1
}
