using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IS3Service
    {
        public Task<string> UploadSharedFileAsync(string base64Url);
        public Task<string> UploadSharedFileAsync(string base64Url, string keyName);
        public Task<string> UploadUserFileAsync(string base64Url, string keyName);
        public Task RemoveFileAsync(string keyName);
        public Task RemoveFolderAsync(string keyName);
    }
}
