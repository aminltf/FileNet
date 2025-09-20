namespace FileNet.WebFramework.Enums;

public enum DocumentCategory : byte
{
    RetentionAndSeparation = 0,     // ابقاء و رهایی
    PersonnelInformation = 1,       // اطلاعات پرسنلی
    InsuranceAndLoans = 2,          // امور بیمه و وام
    VeteranAffairs = 3,             // ایثارگری
    Training = 4,                   // آموزش
    PromotionsAndAppointments = 5,  // ترفیعات و انتصابات
    RewardsAndDiscipline = 6,       // تشویق و تنبیه
    Commission = 7,                 // کمیسیون
    Miscellaneous = 8,              // متفرقه
    LeaveAndDutyTravel = 9,         // مرخصی و ماموریت
    TransfersAndRelocations = 10,   // نقل و انقالات
    Other = 255                     // سایر
}
