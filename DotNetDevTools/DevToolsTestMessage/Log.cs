﻿using System;

namespace DevToolsTestMessage
{
    public class Log
    {
        public DateTime? Date { get; set; }
        public EnumLogLevel Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
