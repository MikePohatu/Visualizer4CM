using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class CollectionNode : Node
    {       
        const int _highlightedlinewidth = 5;
        private SccmCollection _collection;
        public SccmCollection Collection
        {
            get { return this._collection; }
            set { this._collection = value; }
        }

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
                    this.ChangeInEdges(Color.Green, _highlightedlinewidth);
                }
                else {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Black;
                    this.ChangeInEdges(Color.Black,1);
                }
            }
            if (e.PropertyName.Equals("IsMemberPresent"))
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
        }

        private void ChangeInEdges(Color newcolor, int newthickness)
        {
            foreach (Edge edge in this.InEdges)
            {
                edge.Attr.Color = newcolor;
                edge.Attr.Weight = newthickness;
            }
        }
    }
}
