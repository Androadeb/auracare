using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.Notifications
{
    public class GetNotificationsInputDto : PagedAndSortedResultRequestDto
    {
        // Optional filter to get only read or unread notifications
        public bool? IsRead { get; set; }
    }
}
