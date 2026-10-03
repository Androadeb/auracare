using System.Collections.Generic;
using System.Threading.Tasks;
using ynex.Models.Chat;
using ynex.Models.MedicalTest;

namespace ynex.Services.MedicalTest
{
    public interface IMedicalTestService
    {
     

        Task<List<MedicalCategory>> GetCategoriesAsync();

        Task<List<MedicalTestItem>> GetTestsByCategoryAsync(string categoryId);

        Task<List<SampleTypeItem>> GetSampleTypesByTestIdAsync(string medicalTestId);

        Task<ChatMessage?> CreateMedicalTestAsync(object postData);

        Task<List<MedicalTestOrder>> GetOrdersBySessionIdAsync(string sessionId);

        Task<MedicalTestOrderDetail?> GetOrderDetailsAsync(string id);

        Task<List<MedicalTestOrder>> GetOrdersAsync();

        Task<bool> SendTestResultToChatAsync(string orderId, string fileUrl, string fileName);
    }
}