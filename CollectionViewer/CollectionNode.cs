using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using model;

namespace CollectionViewer
{
    public class CollectionNode : Node
    {
        private SccmCollection _collection;
        const int _highlightedlinewidth = 5;

        public CollectionNode(string id, SccmCollection collection) :base(id)
        {
            this._collection = collection;
            this._collection.PropertyChanged += this.OnPropertyChanged;
            this.Attr.Shape = Shape.Box;
            this.Attr.XRadius = 3;
            this.Attr.YRadius = 3;
            this.Attr.Padding = 3;
            this.Attr.LabelMargin = 5;
            this.LabelText = collection.Name + Environment.NewLine + collection.ID;
            //this.Attr.FillColor = Color.Green;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsHighlighted"))
            {
                SccmCollection col = (SccmCollection)sender;
                if (col.IsHighlighted == true) {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Green;
                }
                else {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Gray;
                }
            }
            if (e.PropertyName.Equals("IsMemberPresent"))
            {
                SccmCollection col = (SccmCollection)sender;
                if (col.IsHighlighted == true)
                {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Orange;
                }
                else
                {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Gray;
                }
            }
        }
    }
}
