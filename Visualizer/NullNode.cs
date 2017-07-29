using System;
using System.ComponentModel;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace Visualizer
{
    public class NullNode: Node
    {
        public NullNode(string id) :base(id)
        {
            this.Attr.Shape = Shape.Box;
            this.Attr.Color = Color.Red;
            this.Attr.FillColor = Color.Red;
            this.Attr.LabelMargin = 3;
            this.Attr.Padding = 20;
            this.Label.FontColor = Color.White;
            this.Label.FontSize = 8;
        }
    }
}
