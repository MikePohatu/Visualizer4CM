using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using model;

namespace CollectionViewer
{
    public class CollectionNode : Node
    {
        private SccmCollection _collection;

        public CollectionNode(string id, SccmCollection collection) :base(id)
        {
            this._collection = collection;
            this._collection.PropertyChanged += this.OnPropertyChanged;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsHighlighted"))
            {
                SccmCollection col = (SccmCollection)sender;
                if (col.IsHighlighted == true) { this.Attr.Color = Color.Green; }
                else { this.Attr.Color = Color.Gray; }
            }
        }
    }
}
