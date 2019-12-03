using System.Collections.Generic;
using MultipartFormDataFormatterExtension.Models;
using SharedLibrary.Models;

namespace SharedLibrary.ViewModels
{
    public class UploadAttachmentListViewModel
    {
        #region Properties

        /// <summary>
        /// Author information.
        /// </summary>
        public User Author { get; set; }

        /// <summary>
        /// List of attachments that will be uploaded to server.
        /// </summary>
        public List<HttpFile> Attachments { get; set; }

        #endregion
    }
}