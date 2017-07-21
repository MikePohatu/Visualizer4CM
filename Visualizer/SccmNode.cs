using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class SccmNode : Node
    {
        protected int _highlightedlinewidth = 3;

        protected ISccmObject _sccmobject;
        public ISccmObject SccmObject
        {
            get { return this._sccmobject; }
            set { this._sccmobject = value; }
        }

        public SccmNode(string id, ISccmObject sccmobject) :base(id)
        {
            this._sccmobject = sccmobject;
            this._sccmobject.PropertyChanged += this.OnPropertyChanged;
            this.Attr.Shape = Shape.Box;
            this.Attr.XRadius = 3;
            this.Attr.YRadius = 3;
            this.Attr.Padding = 3;
            this.Attr.LabelMargin = 5;
            
            this.SetLayout();
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsHighlighted"))
            {
                ISccmObject col = (ISccmObject)sender;
                if (col.IsHighlighted == true)
                {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Green;
                }
                else
                {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Black;
                }
            }
        }

        protected void SetLayout()
        {
            string prefix = string.Empty;
            if (this._sccmobject is SccmApplication) { this.Attr.Color = Color.SandyBrown; prefix = "Application: "; }
            else if (this._sccmobject is SMS_DeploymentSummary) { this.Attr.Color = Color.LightBlue; prefix = "Deployment: "; }
            else if (this._sccmobject is SccmCollection) { this.Attr.Color = Color.RoyalBlue; prefix = "Collection: "; }
            else if (this._sccmobject is SccmSoftwareUpdateGroup) { this.Attr.Color = Color.Gold; prefix = "SUG: "; }
            else if (this._sccmobject is SccmTaskSequence) { this.Attr.Color = Color.Thistle; prefix = "Task Sequence: "; }
            else if (this._sccmobject is SccmPackage) { this.Attr.Color = Color.Chocolate; prefix = "Package: "; }
            else if (this._sccmobject is SccmConfigurationBaseline) { this.Attr.Color = Color.CornflowerBlue; prefix = "Baseline: "; }

            this.LabelText = this._sccmobject.Name + Environment.NewLine + prefix + this._sccmobject.ID;
        }
    }
}
