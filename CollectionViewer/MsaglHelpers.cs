using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using model;

namespace CollectionViewer
{
    public static class MsaglHelpers
    {
        public static void ConfigureNode(Node a, SccmCollection col)
        {
            a.Attr.Shape = Shape.Box;
            a.Attr.XRadius = 3;
            a.Attr.YRadius = 3;
            a.LabelText = col.Name;
            //a.Attr.FillColor = Color.Green;
            a.Attr.LineWidth = 2;

            a.UserData = col;
        }
    }
}
