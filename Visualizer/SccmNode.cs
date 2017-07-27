using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class SccmNode : Node
    {
        protected int _highlightedlinewidth = 4;
        protected int _normallinewidth = 3;

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
            this.Attr.LineWidth = this._normallinewidth;

            this.SetLayout();
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsHighlighted"))
            {
                ISccmObject col = (ISccmObject)sender;
                if (col.IsHighlighted == true)
                {
                    this.Attr.LineWidth = this._highlightedlinewidth;
                    this.Attr.Color = Color.Green;
                    //this.Attr.FillColor = Color.LightGreen;
                }
                else
                {
                    this.Attr.LineWidth = this._normallinewidth;
                    this.Attr.Color = Color.Black;
                    //this.Attr.Color = Color.White;
                }
            }
        }

        protected void SetLayout()
        {
            string prefix = string.Empty;

            switch (this._sccmobject.Type)
            {
                case SccmItemType.Application:
                    //this.Attr.Color = Color.SandyBrown;
                    prefix = "Application: ";
                    break;
                case SccmItemType.Collection:
                    //this.Attr.Color = Color.RoyalBlue;
                    prefix = "Collection: ";
                    break;
                case SccmItemType.SoftwareUpdateGroup:
                    //this.Attr.Color = Color.Gold;
                    prefix = "SUG: ";
                    break;
                case SccmItemType.TaskSequence:
                    //this.Attr.Color = Color.Thistle;
                    prefix = "Task Sequence: ";
                    break;
                case SccmItemType.Package:
                    //this.Attr.Color = Color.Chocolate;
                    prefix = "Package: ";
                    break;
                case SccmItemType.PackageProgram:
                    this.Attr.Color = Color.RosyBrown;
                    prefix = "Program: ";
                    break;
                case SccmItemType.ConfigurationBaseline:
                    //this.Attr.Color = Color.CornflowerBlue;
                    prefix = "Baseline: ";
                    break;
                default:
                    break;

            }

            this.LabelText = this._sccmobject.Name + Environment.NewLine + prefix + this._sccmobject.ID;
        }
    }
}
