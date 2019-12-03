using System.ComponentModel.DataAnnotations;
using MultipartFormDataFormatterExtension.Models;
using SharedLibrary.Models;

namespace SharedLibrary.ViewModels
{
    public class UploadNestedInfoViewModel
    {
        [Required]
        public HttpFile Attachment { get; set; }

        public Profile Profile { get; set; }

        public bool IsActive { get; set; }
    }
}