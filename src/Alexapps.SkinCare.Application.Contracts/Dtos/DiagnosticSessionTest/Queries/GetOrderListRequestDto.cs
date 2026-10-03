using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries
{
    public class GetOrderListRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public DateTime? RequestDate { get; set; }
        public LabServiceType? ServiceType { get; set; }
        public DiagnosticTestStatus? Status { get; set; }

        // لإضافة pagination افتراضي
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }

    // 2. Output DTO: للرد الذي يحتوي على القائمة والإحصائيات
    public class LabOrdersPagedResultDto<T> : PagedResultDto<T>
    {
        // نضع الأرقام داخل كلاس فرعي يمثل الميتا داتا
        public LabOrdersMetadata MetaData { get; set; }

        public LabOrdersPagedResultDto(IReadOnlyList<T> items, long totalCount, LabOrdersMetadata metaData)
            : base(totalCount, items)
        {
            MetaData = metaData;
        }
    }

    public class LabOrdersMetadata
    {
        public int AllCount { get; set; }
        public int RequestedCount { get; set; }
        public int InProgressCount { get; set; }
        public int SampleCollectedCount { get; set; }
        public int ResultReadyCount { get; set; }
    }
}
