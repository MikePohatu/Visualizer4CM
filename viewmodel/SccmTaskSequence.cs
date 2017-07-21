using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmTaskSequence : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.TaskSequence; } }
        public SccmTaskSequence() { }
        public SccmTaskSequence(IResultObject resource)
        {
            this.ID = resource["CI_ID"].IntegerValue.ToString();
            this.Name = resource["LocalizedDisplayName"].StringValue;
        }

        public new string ToString()
        {
            return this._name;
        }
    }
}
