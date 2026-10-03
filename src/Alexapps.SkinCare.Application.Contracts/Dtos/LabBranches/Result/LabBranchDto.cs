using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.LabBranches.Result;

public class LabBranchDto : EntityDto<Guid>
{
    public Guid LabId { get; set; }
    public string Name { get; set; } 
    public string ContactNumber { get; set; }

    public string FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string GoogleMapsUrl { get; set; }
    public bool IsPrimary { get; set; } 
    public bool IsActive { get; set; }
}