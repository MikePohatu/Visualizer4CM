using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmSoftwareUpdate : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.SoftwareUpdate; } }
        public SccmSoftwareUpdate() : base()
        { }
        public SccmSoftwareUpdate(IResultObject resource) : base(resource)
        { }
    }
}
