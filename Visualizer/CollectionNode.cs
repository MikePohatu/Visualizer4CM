using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class CollectionNode : SccmNode
    {
        protected SccmCollection _collection;
        public SccmCollection Collection
        {
            get { return this._collection; }
            set { this._collection = value; }
        }

        public CollectionNode(string id, SccmCollection collection) :base(id, collection)
        {
            this._sccmobject.PropertyChanged += this.OnCollectionPropertyChanged;
            this._collection = collection;
        }

        public void OnCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsLimitingCollection"))
            {
                SccmCollection col = (SccmCollection)sender;
                if (col.IsLimitingCollection == true)
                {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Blue;
                }
                else
                {
                    this.Attr.LineWidth = _normallinewidth;
                    this.Attr.Color = Color.Black;
                }
            }

            else if (e.PropertyName.Equals("IsMemberPresent"))
            {
                SccmCollection col = (SccmCollection)sender;
                if (col.IsMemberPresent == true)
                {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Orange;
                }
                else
                {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Black;
                }
            }

            else { base.OnPropertyChanged(sender, e); }
        }
    }
}
