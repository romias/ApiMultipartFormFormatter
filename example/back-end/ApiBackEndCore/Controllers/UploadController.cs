using System.Collections.Generic;
using ApiBackEndCore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiBackEndCore.Controllers
{
    [Route("api/upload")]
    public class UploadController : Controller
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
        
        #endregion
    }
}