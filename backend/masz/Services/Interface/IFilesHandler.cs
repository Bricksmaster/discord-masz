namespace MASZ.Services
{
    public interface IFilesHandler
    {
        byte[] ReadFile(string path);
        string GetContentType(string path);
        string RemoveSpecialCharacters(string str);
        FileInfo[] GetFilesByDirectory(string directory);
        Task<string> SaveFile(IFormFile file, string directory);
        void DeleteFile(string path);
        bool FileExists(string path);
        void DeleteDirectory(string directory);
    }
}
