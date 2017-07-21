namespace viewmodel
{
    public class SccmTaskSequence : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.TaskSequence; } }

        public new string ToString()
        {
            return this._name;
        }
    }
}
