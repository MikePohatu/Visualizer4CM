namespace viewmodel
{
    public class SccmDevice: SccmResource
    {
        public override SccmItemType Type { get { return SccmItemType.Device; } }
    }
}
