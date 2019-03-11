using System;
using System.Collections.Generic;
using System.Text;
using IIASA.Common;
using Microsoft.Extensions.Logging;

namespace IIASA.Services.Core
{
    public abstract class BaseService<T>
        where T : BaseService<T>
    {
        private ILogger _logger;
        protected ILogger Logger => _logger ?? (_logger = GlobalLogging.CreateLogger<T>());
    }
}
