using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevToolsMessage
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
