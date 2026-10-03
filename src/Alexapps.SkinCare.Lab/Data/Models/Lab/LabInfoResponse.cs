namespace starterkit.Data.Models.Lab;

public class LabInfoResponse
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public List<Branch> Branches { get; set; } = new();
}

public class Branch
{
}