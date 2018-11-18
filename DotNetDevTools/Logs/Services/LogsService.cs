using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logs.Services
{
    class LogsService
    {
        public ObservableCollection<object> Logs { get; private set; }
    }
}
