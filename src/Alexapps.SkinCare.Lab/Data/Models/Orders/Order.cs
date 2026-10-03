namespace starterkit.Data.Models.Orders;

public class Order
{
  public string Id { get; set; } = string.Empty;
  public string NumberOrder { get; set; } = string.Empty;
  public string TestName { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;

  public string PatientName { get; set; } = string.Empty;
  public string? PatientImage { get; set; } = string.Empty;
  public string PatientGender { get; set; } = string.Empty;
  public string PatientEmail { get; set; } = string.Empty;
  public string PatientPhone { get; set; } = string.Empty;
  public string PatientAddress { get; set; } = string.Empty;
  public double? Latitude { get; set; }
  public double? Longitude { get; set; }
  public LabServiceType ServiceType { get; set; }
  public string DateOfBirth { get; set; } = string.Empty;


  public string DoctorName { get; set; } = string.Empty;
  public string DoctorContact { get; set; } = string.Empty;
  public string Specialization { get; set; } = string.Empty;
}

