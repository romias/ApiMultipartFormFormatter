using MultipartFormDataFormatterExtension.Models;

namespace SharedLibrary.Models
{
    public class Profile
    {
        public string Name { get; set; }
        
        public HttpFile Attachment { get; set; }
    }
}