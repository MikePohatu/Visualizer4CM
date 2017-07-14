using System.Windows;
using viewmodel;

namespace Visualizer.Panes
{
    public class ApplicationPane : BasePane
    {
        
        public ApplicationPane(SccmConnector connector): base(connector)
        {
            this._header = "User Collections";
            this._findlabeltext = "User:";
            this._library = connector.UserCollectionLibrary;
        }

        protected override void Find()
        {
            this.ClearHighlightedCollections();
            if (string.IsNullOrWhiteSpace(this._resourcetext) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._resourcetext.Trim());
                this._highlightedcollections = TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs);
            }
            else
            {
                SccmCollection col = this._library.GetCollection(this.CollectionText);
                if (col != null) { col.IsHighlighted = true; }
            }
            this.Redraw();
        }
    }
}
