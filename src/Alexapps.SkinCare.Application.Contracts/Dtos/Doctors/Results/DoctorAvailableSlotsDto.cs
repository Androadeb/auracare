using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class SlotItemDto
    {
        public string TimeLabel { get; set; } 
        public DateTime ActualDateTime { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class DoctorAvailableSlotsDto
    {
        public DateTime Date { get; set; }
        public List<SlotItemDto> Slots { get; set; } = new List<SlotItemDto>();
    }
}
