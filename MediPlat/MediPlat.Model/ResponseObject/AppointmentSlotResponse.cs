using MediPlat.Model.ResponseObject;
using System.Text.Json.Serialization;

public class AppointmentSlotResponse
{
    public Guid Id { get; set; }
    public Guid SlotId { get; set; }
    public Guid ProfileId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? Notes { get; set; }
    public virtual ICollection<AppointmentSlotMedicineResponse> AppointmentSlotMedicines { get; set; } = new List<AppointmentSlotMedicineResponse>();

    [JsonIgnore]
    public virtual ICollection<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();

    public virtual SlotResponse? Slot { get; set; }
    public virtual ProfileResponse? Profile { get; set; }
    public virtual DoctorResponse? Doctor => Slot?.Doctor;
}
