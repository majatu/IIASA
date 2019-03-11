using IIASA.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IIASA.Api.Controllers
{
    public abstract class BaseController<T> : ControllerBase
        where T : BaseController<T>
    {

        private ILogger _logger;

        protected ILogger Logger => _logger ?? (_logger = GlobalLogging.CreateLogger<T>());
    }
}
