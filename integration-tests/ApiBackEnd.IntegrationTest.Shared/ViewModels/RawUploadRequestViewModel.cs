﻿using System.Collections.Generic;
using ApiMultiPartFormData.Models;

namespace ApiBackEnd.IntegrationTest.Shared.ViewModels
{
    public class RawUploadRequestViewModel
    {
        #region Properties

        public string Id { get; set; }

        public string AttachmentId { get; set; }

        /// <summary>
        ///     Author information.
        /// </summary>
        public RawProfileViewModel Profile { get; set; }

        /// <summary>
        ///     Attachment.
        /// </summary>
        public HttpFileBase Attachment { get; set; }

        public List<HttpFile> Attachments { get; set; }

        public string NonNullableQuality { get; set; }

        public string NullableQuality { get; set; }

        public List<string> Qualities { get; set; }

        public List<string> Ids { get; set; }

        #endregion
    }
}