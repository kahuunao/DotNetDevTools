using System;
using System.Collections.Generic;
using System.Text;

namespace DevToolsConnector
{
    public interface IDevSocketFactory
    {
        IDevSocket BuildSocket();
    }
}
