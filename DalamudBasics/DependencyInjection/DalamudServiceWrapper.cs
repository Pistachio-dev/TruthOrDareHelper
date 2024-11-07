using Dalamud.IoC;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudBasics.DependencyInjection
{
    internal class DalamudServiceWrapper<T>
    {
        [PluginService]
        public T Service { get; private set; } = default!;

        internal DalamudServiceWrapper(IDalamudPluginInterface pi)
        {
            pi.Inject(this);
        }
    }
}
