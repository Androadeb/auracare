using ynex.Models.Chat;
using ynex.Models.Treatment;

namespace ynex.Services.Treatment
{
    public interface ITreatmentService
    {
        Task<List<MedicationItem>> GetMedicationsAsync(string token);

     
        Task<bool> AddTreatmentItemAsync(object postData, string token);

        
        Task<List<TreatmentPlanDetail>> GetAddedItemsAsync(string sessionId, string token);

        Task<bool> DeleteTreatmentItemAsync(string itemId, string token);

       
        Task<ChatMessage?> SendTreatmentPlanToPatientAsync(string sessionId, string token);
    }
}
