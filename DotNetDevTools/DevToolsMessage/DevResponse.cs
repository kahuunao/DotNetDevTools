using DevToolsMessage.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevToolsMessage
{
    public class DevResponse
    {
        public bool IsHandled { get; set; }
        public DevIdentificationResponse Identification { get; set; }
    }
}
