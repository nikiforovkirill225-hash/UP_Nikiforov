using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Nikiforov
{
    public static class Core
    {
        public static ShutAndKrolEntities Context = new ShutAndKrolEntities();
        public static Users CurrentUser { get; set; }
    }
}
