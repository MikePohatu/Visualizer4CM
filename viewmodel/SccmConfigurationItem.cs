using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmConfigurationItem : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.ConfigurationItem; } }
        public SccmConfigurationItem(): base() { }
        public SccmConfigurationItem(IResultObject resource) : base(resource) { }
    }
}
