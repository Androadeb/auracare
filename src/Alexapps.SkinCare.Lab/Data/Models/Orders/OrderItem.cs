namespace starterkit.Data.Models.Orders;

public class OrderItem
{
  public string Id { get; set; } = string.Empty;
  public string NumberOrder { get; set; } = string.Empty;
  public string DoctorName { get; set; } = string.Empty;
  public string ServiceType { get; set; } = string.Empty;
  public string MedicalTestName { get; set; } = string.Empty;
  public string PatientName { get; set; } = string.Empty;
  public DateTime Date { get; set; }
  public string Status { get; set; } = string.Empty;
}

