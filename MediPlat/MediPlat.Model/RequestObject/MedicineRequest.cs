using System.ComponentModel.DataAnnotations;

namespace MediPlat.Model.RequestObject
{
    public class MedicineRequest
    {
        [Required(ErrorMessage = "Tên thuốc là bắt buộc.")]
        [MaxLength(255, ErrorMessage = "Tên thuốc không được vượt quá 255 ký tự.")]
        public string Name { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "Dạng thuốc không được vượt quá 100 ký tự.")]
        public string DosageForm { get; set; }

        [Required(ErrorMessage = "Hàm lượng là bắt buộc.")]
        [MaxLength(50, ErrorMessage = "Hàm lượng không được vượt quá 50 ký tự.")]
        [RegularExpression(@"^\d+(\.\d+)?\s*(mg|g|ml|IU|mcg|%)$", ErrorMessage = "Hàm lượng phải có định dạng hợp lệ (ví dụ: 500mg, 5ml, 10%)")]
        public string Strength { get; set; } = null!;

        [MaxLength(1000, ErrorMessage = "Tác dụng phụ không được vượt quá 1000 ký tự.")]
        public string? SideEffects { get; set; }
    }
}
