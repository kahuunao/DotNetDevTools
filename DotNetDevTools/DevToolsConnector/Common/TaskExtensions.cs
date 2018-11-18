using NLog;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DevToolsConnector.Common
{
    public static class TaskExtensions
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public static async void RunSafe(this Task pThis, Action<Exception> pErrorHandler = null, [CallerFilePath] string pCaller = null, [CallerMemberName] string pMethodName = null)
        {
            try
            {
                await pThis;
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex, "Error from: File {0}, Method {1}", pCaller, pMethodName);
                pErrorHandler?.Invoke(ex);
            }
        }
    }
}
