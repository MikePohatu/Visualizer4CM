using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public interface ISccmObject
    {
        string ID { get; }
        string Name { get; }
        bool IsHighlighted { get; set; }
    }
}
