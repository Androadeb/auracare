public interface IAppStateRepository
{
    Task<AppState> GetAppStateAsync();
    Task SaveAppStateAsync(AppState appState);
}
