namespace dotnet_folder_sync.Interfaces
{
    public interface ISyncService
    {
        Task Sync(string source, string target, TimeSpan interval);
    }
}
