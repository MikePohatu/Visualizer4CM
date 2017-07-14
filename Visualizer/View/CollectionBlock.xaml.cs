using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Visualizer.View
{
    /// <summary>
    /// Interaction logic for CollectionBlock.xaml
    /// </summary>
    public partial class CollectionBlock : UserControl
    {
        public Point TopMid { get; set; }
        public Point BottomMid { get; set; }

        public CollectionBlock()
        {
            InitializeComponent();
        }

        private void OnUpdated (object sender, EventArgs e)
        {
            this.CaluculatePoints();
        }

        private void CaluculatePoints()
        {
            Point p = this.PointToScreen(new Point(0, 0));
            double midx = p.X + (this.ActualWidth / 2);
            double bottomy = p.Y + this.ActualHeight;
            this.TopMid = new Point(midx,p.Y);
            this.BottomMid = new Point(midx, bottomy);
        }
    }
}
