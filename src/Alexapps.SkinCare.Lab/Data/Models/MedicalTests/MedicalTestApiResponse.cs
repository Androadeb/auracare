namespace starterkit.Data.Models.MedicalTests;

public class MedicalTestApiResponse
{
  public List<MedicalTestItem> Items { get; set; } = new();
  public PaginationMetadata Metadata { get; set; } = new();
}

