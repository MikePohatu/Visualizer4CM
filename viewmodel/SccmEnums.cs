using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public enum SccmItemType {
        Package = 2,
        Application = 31,
        SoftwareUpdate = 37,
        TaskSequence = 7,
        SoftwareUpdateGroup = 5,
        ConfigurationBaseline = 6,
        Device,
        User
    }
}
