using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using ynex.Models.Auth;
using ynex.Models.MedicalTest;
using ynex.Services.MedicalTest;
using static System.Net.WebRequestMethods;

namespace ynex.Pages.Admin.Medical_Tests
{
    public partial class Medical_Tests_Details : ComponentBase
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IMedicalTestService MedicalTestService { get; set; } = default!;
        [Inject] private AuthState Auth { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        public MedicalTestOrderDetail? OrderDetail { get; private set; }
        private bool _isLoading = false;
        private bool _isSending = false;
        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Nav.NavigateTo("/admin/medical-tests");
                return;
            }
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                // تم حذف الـ token من هنا، الخدمة ستتعامل معه داخلياً
                var data = await MedicalTestService.GetOrderDetailsAsync(Id);

                if (data != null)
                {
                    OrderDetail = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MedicalTest] Error fetching ID {Id}: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        private async Task HandleDownloadFile()
        {
            if (OrderDetail != null && !string.IsNullOrEmpty(OrderDetail.FullFileUrl))
            {
                // فتح الرابط في نافذة جديدة سيبدأ التحميل تلقائياً للملفات أو يعرضها
                await JSRuntime.InvokeVoidAsync("open", OrderDetail.FullFileUrl, "_blank");
            }
        }
        private async Task HandleSendResult()
        {
            if (OrderDetail == null || _isSending || string.IsNullOrEmpty(OrderDetail.resultFileUrl))
                return;

            _isSending = true;
            try
            {
                // تم حذف التوكن من هنا أيضاً (الباراميتر الرابع سابقاً)
                bool isSuccess = await MedicalTestService.SendTestResultToChatAsync(
                    OrderDetail.id,
                    OrderDetail.resultFileUrl,
                    OrderDetail.resultFileName
                );

                if (isSuccess)
                {
                    // يمكن إضافة Notification هنا للنجاح
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>>> Error sending result: {ex.Message}");
            }
            finally
            {
                _isSending = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
