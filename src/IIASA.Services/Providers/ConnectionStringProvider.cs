using System;
using System.Collections.Generic;
using System.Text;
using IIASA.Services.Providers.Interfaces;

namespace IIASA.Services.Providers
{
    public class ConnectionStringProvider: IConnectionStringProvider
    {
        public string ConnStringDefault { get; set; }
    }
}
