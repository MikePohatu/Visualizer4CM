using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class SccmNode : Node
    {       
        const int _highlightedlinewidth = 3;
        private ISccmObject _sccmobject;
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
            if (e.PropertyName.Equals("Type"))
            {
                SccmDeployment deployment = (SccmDeployment)sender;
                if (deployment.DeploymentType == SccmDeployment.CIType.Application) {
                    this.Attr.LineWidth = _highlightedlinewidth;
                    this.Attr.Color = Color.Aquamarine;
                    //this.ChangeOutEdges(Color.Green, _highlightedlinewidth);
                }
                else {
                    this.Attr.LineWidth = 1;
                    this.Attr.Color = Color.Black;
                    //this.ChangeOutEdges(Color.Black,1);
                }
            }
        }

        private void SetLayout()
        {
            string type = string.Empty;
            if (this._sccmobject is SccmApplication) { this.Attr.Color = Color.PaleGreen; type = "Application"; }
            else if (this._sccmobject is SccmDeployment) { this.Attr.Color = Color.LightBlue; type = "Deployment"; }
            else if (this._sccmobject is SccmCollection) { this.Attr.Color = Color.Cyan; type = "Collection"; }
            else if (this._sccmobject is SccmSoftwareUpdate) { this.Attr.Color = Color.Gold; type = "Software Update"; }

            this.LabelText = this._sccmobject.Name + Environment.NewLine + type + ": " + this._sccmobject.ID;
        }
    }
}
