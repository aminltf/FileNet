using System.ComponentModel.DataAnnotations;

namespace FileNet.WebFramework.Enums;

public enum Gender
{
    [Display(Name = "مرد")] Male = 0,
    [Display(Name = "زن")] Female = 1
}
