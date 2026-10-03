using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using starterkit.Data.Models;
using starterkit.Data.Models.MedicalTests;
using starterkit.Data.Services;
using starterkit.Services;

namespace starterkit.Pages.MedicalTests
{
  public partial class MedicalTests : IDisposable
  {
    [Inject] private MedicalTestService MedicalTestService { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private ITokenProvider TokenProvider { get; set; } = null!;
    [Inject] private NotificationService Notification { get; set; } = null!;

    private List<MedicalTestItem> _tests = new();
    private PaginationMetadata _metadata = new();
    private List<MedicalTestCategory> _medicalTestCategories = new();
    private List<MedicalTest> _availableTestsForCategory = new();

    // Shared Modal State
    private CreateMedicalTest _newTest = new CreateMedicalTest();
    private string _editingId = string.Empty;

    private bool IsEditing => _editingId.Length > 0;

    // Search State
    private System.Timers.Timer? _debounceTimer;

    private string? _searchTerm;

    // Filter State
    private bool? SelectedStatus { get; set; }
    private int EnabledCount => 18;
    private int DisabledCount => 32;

    // Pagination State
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 2;
    private int TotalCount => _metadata.TotalCount;
    private int TotalPages => _metadata.TotalPages;
    private bool HasNextPage => _metadata.HasNextPage;
    private bool HasPreviousPage => _metadata.HasPreviousPage;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        if (!string.IsNullOrEmpty(TokenProvider.AccessToken))
        {
          await LoadMedicalTests();
          await LoadTestCategories();
          StateHasChanged();
        }
      }
    }

    protected override void OnInitialized()
    {
      _debounceTimer = new System.Timers.Timer(500);
      _debounceTimer.AutoReset = false;
      _debounceTimer.Elapsed += OnTimerElapsed;
    }

    private async Task LoadTestCategories()
    {
      _medicalTestCategories = await MedicalTestService.GetCategoriesAsync();
    }

    private async Task LoadMedicalTests()
    {
      try
      {
        var response = await MedicalTestService.GetTestsAsync(CurrentPage, PageSize, _searchTerm, SelectedStatus);
        if (response != null)
        {
          _tests = response.Items;
          _metadata = response.Metadata;
        }
        else
        {
          await Notification.ShowError($"Failed to load medical tests. Please try again.");
        }
      }
      catch (InvalidOperationException)
      {
      }
    }

    private async Task OnCategorySelectionChanged()
    {
      var selectedId = _newTest.TestCategoryId;
      var category = _medicalTestCategories.FirstOrDefault(c => c.Id == selectedId);

      if (!string.IsNullOrEmpty(selectedId))
      {
        _availableTestsForCategory = await MedicalTestService.GetTestsByCategoryAsync(selectedId);
      }
    }

    private void OnSearchInput(ChangeEventArgs e)
    {
      _searchTerm = e.Value?.ToString();

      _debounceTimer?.Stop();
      _debounceTimer?.Start();
    }

    private async void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
      await InvokeAsync(async () =>
      {
        CurrentPage = 1;
        await LoadMedicalTests();
        StateHasChanged();
      });
    }

    private async Task FilterByStatus(bool? status)
    {
      SelectedStatus = status;
      CurrentPage = 1;
      await LoadMedicalTests();
    }

    // --- Action Methods ---

    private void PrepareAdd()
    {
      _editingId = string.Empty;
      _newTest = new CreateMedicalTest { IsEnabled = true };
      _availableTestsForCategory.Clear();
    }

    private async Task EditTest(MedicalTestItem test)
    {
      _editingId = test.Id;
      // Map the table item back to the Create model
      var categoryId = _medicalTestCategories.FirstOrDefault(c => c.Name == test.CategoryName)?.Id;
      _newTest = new CreateMedicalTest
      {
        TestCategoryId = categoryId!,
        MedicalTestId = test.MedicalTestId,
        LabPrice = test.LabPrice,
        Duration = test.Duration,
        IsEnabled = test.IsEnabled,
        ServiceScope = test.ServiceScopeName
      };

      if (!string.IsNullOrEmpty(categoryId))
      {
        _availableTestsForCategory = await MedicalTestService.GetTestsByCategoryAsync(categoryId);
      }

      await JsRuntime.InvokeVoidAsync("HSOverlay.open", "#add-test-modal");
    }

    private async Task HandleSubmit()
    {
      bool success = IsEditing
        ? await MedicalTestService.UpdateTestAsync(_editingId, _newTest)
        : await MedicalTestService.CreateTestAsync(_newTest);
      if (success)
      {
        await LoadMedicalTests();
        await JsRuntime.InvokeVoidAsync("HSOverlay.close", "#add-test-modal");

        var msg = IsEditing ? "Test updated successfully" : "Test added successfully";
        await Notification.ShowToast(msg, 2000);

        CurrentPage = 1;
        _newTest = new();
        _editingId = string.Empty;
      }
    }

    private async Task DeleteTest(MedicalTestItem test)
    {
      if (await Notification.ConfirmDelete(test.TestName))
      {
        var success = await MedicalTestService.DeleteTestAsync(test.Id);
        if (success)
        {
          await LoadMedicalTests();
          await Notification.ShowToast("Test deleted successfully", 2000);
        }
        else
        {
          await Notification.ShowError("Could not delete the test. Please try again.");
        }
      }
    }

    // --- Helper & Pagination Methods ---

    private async Task NextPage()
    {
      if (HasNextPage)
      {
        CurrentPage++;
        await LoadMedicalTests();
      }
    }

    private async Task PrevPage()
    {
      if (HasPreviousPage)
      {
        CurrentPage--;
        await LoadMedicalTests();
      }
    }

    private async Task ChangePageSize(ChangeEventArgs e)
    {
      if (int.TryParse(e.Value?.ToString(), out int newSize))
      {
        PageSize = newSize;
        CurrentPage = 1;
        await LoadMedicalTests();
      }
    }

    public void Dispose()
    {
      _debounceTimer?.Stop();

      if (_debounceTimer != null)
      {
        _debounceTimer.Elapsed -= OnTimerElapsed;
      }

      _debounceTimer?.Dispose();
    }
  }
}
