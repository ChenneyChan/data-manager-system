using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Tools
{
    internal class TestEventArgs : EventArgs
    {
        public object? obj { set; get; } = null;
    }
}
