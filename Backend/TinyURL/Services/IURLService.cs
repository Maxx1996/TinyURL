using TinyURL.Models;

namespace TinyURL.Services
{
    public interface IURLService
    {
        Task<string> GetTinyUrl(URLInputModel model);
        Task<string> GetLongUrl(string codedUri);
    }
}

