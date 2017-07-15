using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class ApplicationNode : Node
    {       
        const int _highlightedlinewidth = 5;
        private SccmApplication _application;
        public SccmApplication Application
        {
            get { return this._application; }
            set { this._application = value; }
        }

        public ApplicationNode(string id, SccmApplication application) :base(id)
        {          
            application.PropertyChanged += this.OnPropertyChanged;
            this.Attr.Shape = Shape.Box;
            this.Attr.XRadius = 3;
            this.Attr.YRadius = 3;
            this.Attr.Padding = 3;
            this.Attr.LabelMargin = 5;
            this.LabelText = application.Name + Environment.NewLine + application.CIID;
            this._application = application;
            //this.Attr.FillColor = Color.Green;
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsHighlighted"))
            {
                SccmApplication col = (SccmApplication)sender;
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
