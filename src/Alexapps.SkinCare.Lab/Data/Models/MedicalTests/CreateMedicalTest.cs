using System.ComponentModel.DataAnnotations;

namespace starterkit.Data.Models.MedicalTests;

public class CreateMedicalTest
{
	[Required(ErrorMessage = "Test Category is required")]
	public string TestCategoryId { get; set; } = string.Empty;

	[Required(ErrorMessage = "Test Name is required")]
	public string MedicalTestId { get; set; } = string.Empty;

	[Required]
	[Range(1, 10000, ErrorMessage = "Price must be greater than 0")]
	public decimal LabPrice { get; set; }

	[Required(ErrorMessage = "Test Duration is required")]
	public string Duration { get; set; } = string.Empty;

	[Required(ErrorMessage = "Specify the status of the test")]
	public bool IsEnabled { get; set; } = true;

	[Required(ErrorMessage = "Please select the location where the test is accessible.")]
	public string ServiceScope { get; set; } = string.Empty;
}

