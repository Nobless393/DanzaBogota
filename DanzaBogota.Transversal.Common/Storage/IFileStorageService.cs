namespace DanzaBogota.Transversal.Common.Storage
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
        Task<bool> DeleteFileAsync(string fileUrl);
    }
}
