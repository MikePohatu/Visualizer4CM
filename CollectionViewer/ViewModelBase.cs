using System.ComponentModel;

namespace CollectionViewer
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        //Setup the INotifyPropertyChanged interface 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
    }
}