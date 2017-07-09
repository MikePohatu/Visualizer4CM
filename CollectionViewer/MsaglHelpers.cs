using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
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
            a.Attr.Padding = 3;
            a.Attr.LabelMargin = 5;
            a.LabelText = col.Name + Environment.NewLine + col.ID;
            //a.Attr.FillColor = Color.Green;
            a.Attr.LineWidth = 2;

            a.UserData = col;
        }

        public static void ConfigureGViewer(GViewer viewer )
        {
            viewer.EdgeInsertButtonVisible = false;
            viewer.NavigationVisible = false;
            viewer.UndoRedoButtonsVisible = false;
            viewer.CurrentLayoutMethod = LayoutMethod.MDS;
            //viewer.ToolBarIsVisible = false;
        }
    }
}
