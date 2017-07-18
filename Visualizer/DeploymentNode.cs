using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class DeploymentNode : Node
    {       
        const int _highlightedlinewidth = 5;
        private SccmDeployment _deployment;
        public SccmDeployment Deployment
        {
            get { return this._deployment; }
            set { this._deployment = value; }
        }

        public DeploymentNode(string id, SccmDeployment deployment) :base(id)
        {
            this._deployment = deployment;
            this._deployment.PropertyChanged += this.OnPropertyChanged;
            this.Attr.Shape = Shape.Box;
            this.Attr.XRadius = 3;
            this.Attr.YRadius = 3;
            this.Attr.Padding = 3;
            this.Attr.LabelMargin = 5;
            this.LabelText = deployment.TargetName + Environment.NewLine + deployment.DeploymentType.ToString();
            this.SetColor();
            //this.Attr.FillColor = Color.Green;
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

        private void SetColor()
        {
            if (this._deployment.DeploymentType == SccmDeployment.CIType.Application) { this.Attr.Color = Color.PaleGreen; }
            else if (this._deployment.DeploymentType == SccmDeployment.CIType.TaskSequence) { this.Attr.Color = Color.Orchid; }
            else if (this._deployment.DeploymentType == SccmDeployment.CIType.Package) { this.Attr.Color = Color.AliceBlue; }
            else if (this._deployment.DeploymentType == SccmDeployment.CIType.SoftwareUpdate) { this.Attr.Color = Color.Gold; }
        }
    }
}
