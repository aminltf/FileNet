using System.ComponentModel.DataAnnotations;

namespace FileNet.Domain.Enums;

public enum DocumentCategory : byte
{
    [Display(Name = "ابقاء و رهایی")]
    RetentionAndSeparation = 0,

    [Display(Name = "اطلاعات پرسنلی")]
    PersonnelInformation = 1,

    [Display(Name = "امور بیمه و وام")]
    InsuranceAndLoans = 2,

    [Display(Name = "ایثارگری")]
    VeteranAffairs = 3,

    [Display(Name = "آموزش")]
    Training = 4,

    [Display(Name = "ترفیعات و انتصابات")]
    PromotionsAndAppointments = 5,

    [Display(Name = "تشویق و تنبیه")]
    RewardsAndDiscipline = 6,

    [Display(Name = "کمیسیون")]
    Commission = 7,

    [Display(Name = "متفرقه")]
    Miscellaneous = 8,

    [Display(Name = "مرخصی و ماموریت")]
    LeaveAndDutyTravel = 9,

    [Display(Name = " نقل و انقالات")]
    TransfersAndRelocations = 10,

    [Display(Name = "سایر")]
    Other = 99
}
