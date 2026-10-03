using Microsoft.AspNetCore.Components;
using starterkit.Data.Models;
using starterkit.Data.Models.Orders;
using starterkit.Data.Services;
using starterkit.Services;

namespace starterkit.Pages.Orders;

public partial class Orders : IDisposable
{
  [Inject] private OrderService OrderService { get; set; } = null!;
  [Inject] private NotificationService NotificationService { get; set; } = null!;
  [Inject] private NavigationManager Navigation { get; set; } = null!;
  private List<OrderItem> _orderItems = new();
  private PaginationMetadata _metadata = new();

  // Search State
  private System.Timers.Timer? _debounceTimer;
  private string? _searchTerm;

  // Filter State
  private DiagnosticTestStatus? SelectedStatus { get; set; }
  private int NewCount => 5;
  private int InProgressCount => 10;
  private int ResultReadyCount => 5;
  private LabServiceType? SelectedLabServiceType { get; set; }

  private DateTime? RequestDate { get; set; }

  // Pagination State
  private int CurrentPage { get; set; } = 1;
  private int PageSize { get; set; } = 6;
  private int TotalCount => _metadata.TotalCount;
  private int TotalPages => _metadata.TotalPages;
  private bool HasNextPage => _metadata.HasNextPage;
  private bool HasPreviousPage => _metadata.HasPreviousPage;


  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      await LoadOrders();
      StateHasChanged();
    }
  }

  protected override void OnInitialized()
  {
    _debounceTimer = new System.Timers.Timer(500);
    _debounceTimer.AutoReset = false;
    _debounceTimer.Elapsed += OnTimerElapsed;
  }

  private async Task LoadOrders()
  {
    try
    {
      var response = await OrderService.GetOrdersAsync(CurrentPage, PageSize, _searchTerm, null,
        SelectedLabServiceType, RequestDate);
      if (response != null)
      {
        _orderItems = response.Items;
        _metadata = response.Metadata;
      }
      else
      {
        await NotificationService.ShowError($"Failed to load orders. Please try again.");
      }
    }
    catch (InvalidOperationException)
    {
    }
  }

  private void ViewOrder(string id)
  {
    Navigation.NavigateTo($"/order-details/{id}");
  }

  private async Task HandleFilterChange()
  {
    CurrentPage = 1;
    await LoadOrders();
  }

  private async Task HandleDateChange()
  {
    CurrentPage = 1;
    await LoadOrders();
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
      await LoadOrders();
      StateHasChanged();
    });
  }

  private async Task FilterByStatus(DiagnosticTestStatus? status)
  {
    SelectedStatus = status;
    CurrentPage = 1;
    await LoadOrders();
  }


  // --- Helper & Pagination Methods ---

  private async Task NextPage()
  {
    if (HasNextPage)
    {
      CurrentPage++;
      await LoadOrders();
    }
  }

  private async Task PrevPage()
  {
    if (HasPreviousPage)
    {
      CurrentPage--;
      await LoadOrders();
    }
  }

  private async Task ChangePageSize(ChangeEventArgs e)
  {
    if (int.TryParse(e.Value?.ToString(), out int newSize))
    {
      PageSize = newSize;
      CurrentPage = 1;
      await LoadOrders();
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

  private string SplitCamelCase(string input)
  {
    return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1").Trim();
  }
}

