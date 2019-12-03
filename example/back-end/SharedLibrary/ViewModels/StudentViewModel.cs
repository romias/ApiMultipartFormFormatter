using System.Collections.Generic;
using MultipartFormDataFormatterExtension.Models;

namespace SharedLibrary.ViewModels
{
    public class StudentViewModel
    {
        public string FullName { get; set; }

        public int Age { get; set; }

        public List<HttpFile> Attachments { get; set; }
    }
}