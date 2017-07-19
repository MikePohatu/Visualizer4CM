using System.ComponentModel;

namespace viewmodel
{
    public interface ISccmObject
    {
        event PropertyChangedEventHandler PropertyChanged;
        string ID { get; }
        string Name { get; }
        bool IsHighlighted { get; set; }
    }
}
