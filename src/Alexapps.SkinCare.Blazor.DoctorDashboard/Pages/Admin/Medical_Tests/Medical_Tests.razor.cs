using Microsoft.AspNetCore.Components;
using ynex.Models.Auth;
using ynex.Models.MedicalTest;
using ynex.Services.MedicalTest;

namespace ynex.Pages.Admin.Medical_Tests
{
    public partial class Medical_Tests : ComponentBase
    {
        [Inject] public IMedicalTestService MedicalTestService { get; set; } = default!;
        [Inject] public AuthState Auth { get; set; } = default!;

        // --- تعديل: استخدام Backing Fields لإدارة الحالة ---
        private string _searchTerm = "";
        public string searchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    currentPage = 1; // تصفير الصفحة عند البحث
                }
            }
        }

        private string _activeTab = "All";
        public string activeTab
        {
            get => _activeTab;
            set
            {
                if (_activeTab != value)
                {
                    _activeTab = value;
                    currentPage = 1; // تصفير الصفحة عند تغيير التبويب
                }
            }
        }

        public List<MedicalTestOrder> medicalTests = new();
        private int currentPage = 1;
        private int pageSize = 10;

        protected override async Task OnInitializedAsync()
        {
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            try
            {
                var result = await MedicalTestService.GetOrdersAsync();
                if (result != null)
                {
                    medicalTests = result;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading orders: {ex.Message}");
            }
        }

        // --- المنطق المحدث للفلترة والترقيم ---

        // 1. القائمة المفلترة بالكامل (بدون تقطيع)
        public List<MedicalTestOrder> AllFilteredTests => medicalTests
            .Where(t => (activeTab == "All" || MapStatus(t.status).Equals(activeTab, StringComparison.OrdinalIgnoreCase))
                     && (string.IsNullOrEmpty(searchTerm)
                         || t.patientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || t.testType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        // 2. القائمة المقطوعة للعرض في الجدول فقط
        public IEnumerable<MedicalTestOrder> FilteredTests => AllFilteredTests
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize);

        // 3. حساب إجمالي الصفحات بناءً على القائمة المفلترة
        private int totalPages => Math.Max(1, (int)Math.Ceiling((double)AllFilteredTests.Count / pageSize));

        private bool hasPreviousPage => currentPage > 1;
        private bool hasNextPage => currentPage < totalPages;

        private void ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= totalPages)
            {
                currentPage = newPage;
                StateHasChanged();
            }
        }

        // Counts for Tabs
        public int CountAll => medicalTests.Count;
        public int CountRequested => medicalTests.Count(t => t.status == "1");
        public int CountInProgress => medicalTests.Count(t => t.status == "2");
        public int CountSampleCollected => medicalTests.Count(t => t.status == "3");
        public int CountResultReady => medicalTests.Count(t => t.status == "4");

        private string MapStatus(string statusId)
        {
            return statusId switch
            {
                "1" => "Requested",
                "2" => "InProgress",
                "3" => "SampleCollected",
                "4" => "ResultReady",
                _ => "Unknown"
            };
        }

        public string GetStatusStyle(string statusId)
        {
            return statusId switch
            {
                "1" => "background-color: #e0f2fe !important; color: #0ea5e9 !important;",
                "2" => "background-color: #fef3c7 !important; color: #d97706 !important;",
                "3" => "background-color: #ede9fe !important; color: #8b5cf6 !important;",
                "4" => "background-color: #dcfce7 !important; color: #22c55e !important;",
                _ => "background-color: #f1f5f9; color: #64748b;"
            };
        }
    }
}