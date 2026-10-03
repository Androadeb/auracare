namespace starterkit.Data.Models.MedicalTests;

public class MedicalTestItem
{
  public string Id { get; set; } = string.Empty;
  public string MedicalTestId { get; set; } = string.Empty;
  public string TestName { get; set; } = string.Empty;
  public string CategoryName { get; set; } = string.Empty;
  public decimal LabPrice { get; set; }
  public string Duration { get; set; } = string.Empty;
  public bool IsEnabled { get; set; }
  public string ServiceScopeName { get; set; } = string.Empty;
}

