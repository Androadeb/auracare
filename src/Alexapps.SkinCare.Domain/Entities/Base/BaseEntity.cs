using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Alexapps.SkinCare.Entities.Base;

public abstract class BaseEntity : FullAuditedEntity<Guid>
{
    protected BaseEntity()
    {
        HandleGuidPrimaryKeyGeneration();
    }

    private void HandleGuidPrimaryKeyGeneration()
    {
        GetType().GetProperty(nameof(Id))?.SetValue(this, Guid.NewGuid());
    }
}

