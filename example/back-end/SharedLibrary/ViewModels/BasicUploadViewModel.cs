using MultipartFormDataFormatterExtension.Models;
using System;
using SharedLibrary.Models;

namespace SharedLibrary.ViewModels
{
    public class BasicUploadViewModel
    {
        #region Properties

        public Guid Id { get; set; }

        public Guid? AttachmentId { get; set; }

        /// <summary>
        /// Author information.
        /// </summary>
        public User Author { get; set; }

        public HttpFile Attachment { get; set; }

        #endregion
    }
}