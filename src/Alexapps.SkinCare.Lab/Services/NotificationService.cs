using Microsoft.JSInterop;

namespace starterkit.Services
{
  public class NotificationService
  {
    private readonly IJSRuntime _js;

    private const string customSuccessIcon = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""36"" height=""36"" fill=""currentColor"" viewBox=""0 0 24 24""><path d=""M12 22C6.49 22 2 17.51 2 12S6.49 2 12 2s10 4.49 10 10-4.49 10-10 10m0-18c-4.41 0-8 3.59-8 8s3.59 8 8 8 8-3.59 8-8-3.59-8-8-8""></path><path d=""M10 16c-.26 0-.51-.1-.71-.29l-3-3L7.7 11.3l2.29 2.29 5.29-5.29 1.41 1.41-6 6c-.2.2-.45.29-.71.29Z""></path></svg>";



    public NotificationService(IJSRuntime js)
    {
      _js = js;
    }

    public async Task ShowToast(string message, int timer, string icon = "success")
    {
      await _js.InvokeVoidAsync("Swal.fire", new
      {
        toast = true,
        position = "top-right",
        customClass = new
        {
          popup = "colored-toast"
        },
        showConfirmButton = false,
        timer = timer,
        title = message,
        icon = icon,
        width = "auto",
        iconHtml = customSuccessIcon,
        timerProgressBar = true
      });
    }

    public async Task<bool> ConfirmDelete(string itemName)
    {
      var result = await _js.InvokeAsync<SweetAlertResult>("Swal.fire", new
      {
        title = "Are you sure?",
        text = $"You want to delete '{itemName}'? This cannot be undone.",
        icon = "warning",
        showCancelButton = true,
        cancelButtonColor = "#6b7280",
        confirmButtonText = "Yes, delete it!",
        cancelButtonText = "Cancel",
        customClass = new
        {
          popup = "confirm-delete"
        }
      });

      return result.IsConfirmed;
    }

    public async Task<bool> ConfirmLogout()
    {
      var result = await _js.InvokeAsync<SweetAlertResult>("Swal.fire", new
      {
        title = "Log Out?",
        text = "Are you sure you want to log out?",
        icon = "question",
        showCancelButton = true,
        confirmButtonColor = "#ef4444",
        cancelButtonColor = "#6b7280",
        confirmButtonText = "Yes, Log out",
        cancelButtonText = "Stay logged in",
        customClass = new
        {
          popup = "logout"
        }
      });

      return result.IsConfirmed;
    }

    public async Task ShowError(string message)
    {
      await _js.InvokeVoidAsync("Swal.fire", "Error", message, "error");
    }
  }

  public class SweetAlertResult
  {
    public bool IsConfirmed { get; set; }
    public bool IsDenied { get; set; }
    public bool IsDismissed { get; set; }
  }
}
