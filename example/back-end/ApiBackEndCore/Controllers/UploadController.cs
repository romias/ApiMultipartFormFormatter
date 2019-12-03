using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SharedLibrary.ViewModels;

namespace ApiBackEndCore.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        #region Properties
        
        private readonly ILogger<UploadController> _logger;

        #endregion
        
        #region Constructor
        
        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        #endregion
        
        #region Methods
        
        [HttpPost]
        public IEnumerable<string> UploadAsync(BasicUploadViewModel command)
        {
            return new[] {"Hello", "World"};
        }

        /// <summary>
        /// Upload nested file.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("nested-info-upload")]
        [HttpPost]
        public IActionResult UploadNestedInfo(UploadNestedInfoViewModel info)
        {
            var messages = new List<string>();
            var attachment = info.Attachment;
            messages.Add($"Root attachment information: (Mime) {attachment.MediaType} - (File name) {attachment.Name}");

            var profile = info.Profile;
            if (profile != null)
            {
                messages.Add($"Profile has been uploaded.");
                messages.Add($"Profile name : {profile.Name}");

                var profileAttachment = profile.Attachment;
                messages.Add(
                    $"Profile attachment information: (Mime) {profileAttachment.MediaType} - (File name) {profileAttachment.Name}");
            }
            else
                messages.Add("No profile is added");

            return Ok(new ClientResponseViewModel(messages));
        }


        #endregion
    }
}