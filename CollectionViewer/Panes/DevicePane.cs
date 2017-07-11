using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace CollectionViewer.Panes
{
    public class DevicePane: BasePane
    {

        public DevicePane(SccmConnector connector): base(connector)
        {
            this._header = "Device Collections";
            this._findlabeltext = "Device:";
            this._library = connector.DeviceCollectionLibrary;
        }

        protected override void OnFindButtonPressed(object sender, RoutedEventArgs e)
        {
            TreeBuilder.ClearHighlightedCollections(this._highlightedcollections);
            if (string.IsNullOrWhiteSpace(this._resourcetext) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._resourcetext.Trim());
                this._highlightedcollections = TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs);
            }
            this.UpdatePaneToTabControl();
        }
    }
}
