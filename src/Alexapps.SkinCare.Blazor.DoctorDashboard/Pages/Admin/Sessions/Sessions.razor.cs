using Microsoft.AspNetCore.Components;
using System.Drawing.Printing;
using ynex.Models.Sessions;
using ynex.Services.Sessions;

namespace ynex.Pages.Admin.Sessions
{
    public partial class Sessions : ComponentBase
    {
        [Inject] public IDiagnosticSessionService SessionService { get; set; } = default!;

        protected List<SessionData> sessionList = new();
        protected string searchTerm = "";
        protected string activeTab = "All";
        protected bool isLoading = true;
        protected string? errorMessage;
        protected int currentPage = 1;
        protected int pageSize = 10;
        protected int totalPages = 1;
        protected bool hasNextPage;
        protected bool hasPreviousPage;
        // --- منطق العدادات (Counters) ---
        protected int CountAll => sessionList.Count;
        protected int CountInProgress => sessionList.Count(s => s.status.Equals("InProgress", StringComparison.OrdinalIgnoreCase));
        protected int CountPending => sessionList.Count(s => s.status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        protected int CountComplete => sessionList.Count(s => s.status.Equals("Complete", StringComparison.OrdinalIgnoreCase) || s.status.Equals("Completed", StringComparison.OrdinalIgnoreCase));

        // --- تصفية القائمة (Filtering Logic) ---
        protected IEnumerable<SessionData> FilteredSessions => sessionList
            .Where(s => (activeTab == "All" || s.status.Equals(activeTab, StringComparison.OrdinalIgnoreCase))
                     && (string.IsNullOrEmpty(searchTerm) || s.clientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                isLoading = true;
                var response = await SessionService.GetSessionsAsync(currentPage, pageSize);

                if (response != null)
                {
                    sessionList = response.Items;
                    // تحديث بيانات الترقيم من الـ metadata
                    totalPages = response.Metadata.TotalPages;
                    hasNextPage = response.Metadata.HasNextPage;
                    hasPreviousPage = response.Metadata.HasPreviousPage;
                }
            }
            catch (Exception ex)
            {
                errorMessage = Loc["ErrorLoadingData"] + ": " + ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }
        protected async Task ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= totalPages)
            {
                currentPage = newPage;
                await LoadDataAsync();
            }
        }
        protected string GetStatusStyle(string status) => status?.ToLower().Replace(" ", "") switch
        {
            "inprogress" => "background-color: #e0faff !important; color: #0ea5e9 !important;",
            "pending" => "background-color: #fef3c7 !important; color: #d97706 !important;",
            "Complete" or "completed" => "background-color: #dcfce7 !important; color: #15803d !important;",
            _ => "background-color: #f1f5f9; color: #64748b;"
        };
    }
}