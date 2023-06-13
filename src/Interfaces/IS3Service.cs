namespace SACA.Interfaces
{
    public interface IS3Service
    {
        public Task<string> UploadCommonFileAsync(IFormFile file);
        public Task<string> UploadCommonSeedFileAsync(string base64Url, string keyName, string Extension);
        public Task<string> UploadUserFileAsync(IFormFile file, string keyName);
        public Task RemoveFileAsync(string keyName);
        public Task RemoveFolderAsync(string keyName);
    }
}
