﻿using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class DevicePane: BaseResourcePane
    {
        public DevicePane(SccmConnector connector): base(connector)
        {
            this.CollectionsType = CollectionType.Device;
            this._header = "Device Collections";
            this._findlabeltext = "Device:";
            this._library = connector.DeviceCollectionLibrary;
        }

        protected override void Find()
        {
            this.ClearHighlightedCollections();
            if (string.IsNullOrWhiteSpace(this._findtext) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._findtext.Trim());
                this._highlightedcollections = TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs);
            }
            else
            {
                SccmCollection col = this._library.GetCollection(this._findtext);
                if (col != null) { col.IsHighlighted = true; }
            }
            this.Redraw();
        }
    }
}
