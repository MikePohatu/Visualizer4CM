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
            double radius = 20;
            this.Attr.Shape = Shape.Circle;
            this.Attr.Color = Color.Red;
            this.Attr.FillColor = Color.Red;
            this.Attr.XRadius = radius;
            this.Attr.YRadius = radius;
        }
    }
}
