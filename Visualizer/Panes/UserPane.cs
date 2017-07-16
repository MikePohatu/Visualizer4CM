using System.Windows;
using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class UserPane: BaseResourcePane
    {
        public UserPane(SccmConnector connector): base(connector)
        {
            this.CollectionsType = CollectionType.User;
            this._header = "User Collections";
            this._findlabeltext = "User:";
            this._library = connector.UserCollectionLibrary;
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
