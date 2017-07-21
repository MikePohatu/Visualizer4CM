namespace viewmodel
{
    public class SccmPackage : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.Package; } }
    }
}
