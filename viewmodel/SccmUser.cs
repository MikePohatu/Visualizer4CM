using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public class SccmUser : SccmResource
    {
        public override SccmItemType Type { get { return SccmItemType.User; } }
    }
}
