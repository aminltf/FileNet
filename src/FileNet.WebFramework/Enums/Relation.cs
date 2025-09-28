using System.ComponentModel.DataAnnotations;

namespace FileNet.WebFramework.Enums;

public enum Relation : byte
{
    [Display(Name = "همسر")]
    Wife,

    [Display(Name = "شوهر")]
    Husband,

    [Display(Name = "فرزند")]
    Child,

    [Display(Name = "پدر")]
    Father,

    [Display(Name = "مادر")]
    Mother,

    [Display(Name = "برادر")]
    Brother,

    [Display(Name = "خواهر")]
    Sister,

    [Display(Name = "سایر")]
    Other
}
