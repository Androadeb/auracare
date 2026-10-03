namespace ynex.Pages.Auth
{
    public partial class NewPassword
    {
        // 1. تعريف الموديل اللي الـ HTML هيشوفه
        private ResetPasswordModel resetModel = new ResetPasswordModel();

        // 2. تعريف الموديل نفسه كـ Class داخلية أو خارجية
        public class ResetPasswordModel
        {
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        // 3. الميثود اللي هتتنفذ عند الضغط على الزرار
        private async Task HandleResetPassword()
        {
            // كود الحفظ هنا
            await Task.CompletedTask;
        }
    }
}