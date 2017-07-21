namespace viewmodel
{
    public interface IDeployment: ISccmObject
    {
        string CollectionID { get; }
        string CollectionName { get; }
    }
}
