using System;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    public static class TaskExtensions
    {
        public static async void RunSafe(this Task pThis, Action<Exception> pErrorHandler = null)
        {
            try
            {
                await pThis;
            }
            catch (Exception ex)
            {
                pErrorHandler?.Invoke(ex);
            }
        }
    }
}
