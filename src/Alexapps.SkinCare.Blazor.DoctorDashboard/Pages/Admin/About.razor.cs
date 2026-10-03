namespace ynex.Pages.Admin
{
    public partial class About
    {
        // List to store the workflow steps shown in the UI
        public List<StepItem> WorkflowSteps { get; set; } = new();

        protected override void OnInitialized()
        {
            LoadWorkflowSteps();
        }

        private void LoadWorkflowSteps()
        {
            // Initializing the steps based on the image_aa0df6.png content
            WorkflowSteps = new List<StepItem>
        {
            new ("SignIn", "SignInDescription"),
            new ("ViewSessions", "ViewSessionsDescription"),
            new ("ReviewMedicalTests", "ReviewMedicalTestsDescription"),
            new ("DiagnoseCase", "DiagnoseCaseDescription"),
            new ("RequestAdditionalTests", "RequestTestsDescription"),
            new ("FollowUp", "FollowUpDescription")
        };
        }
    }

    public record StepItem(string Title, string Description);
}
