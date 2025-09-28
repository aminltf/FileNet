using System.ComponentModel.DataAnnotations;

namespace FileNet.Domain.Enums;

public enum Relation : byte
{
    [Display(Name = "همسر")]
    Wife = 0,

    [Display(Name = "شوهر")]
    Husband = 1,

    [Display(Name = "فرزند")]
    Child = 2,

    [Display(Name = "پدر")]
    Father = 3,

    [Display(Name = "مادر")]
    Mother = 4,

    [Display(Name = "برادر")]
    Brother = 5,

    [Display(Name = "خواهر")]
    Sister = 6,

    [Display(Name = "سایر")]
    Other = 99
}
