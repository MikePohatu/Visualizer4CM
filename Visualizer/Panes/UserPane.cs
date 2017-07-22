using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class UserPane: BaseCollectionPane
    {
        public UserPane(SccmConnector connector): base(connector)
        {
            this.CollectionsType = CollectionType.User;
            this._header = "User Collections";
            this._findlabeltext = "User:";
            this._library = connector.GetCollectionLibrary(CollectionType.User); ;
        }

        protected override void Find()
        {
            this.ClearHighlightedCollections();
            if (string.IsNullOrWhiteSpace(this._findtext) == false)
            {
                SccmUser user = this._connector.GetUser(this._findtext.Trim());
                if (user != null) { this._highlightedcollections = TreeBuilder.HighlightCollectionMembers(this._graph, user.CollectionIDs); }
            }
            else
            {
                SccmCollection col = this._library.GetCollection(this.@_findtext);
                if (col != null) { col.IsHighlighted = true; }
            }
            this.Redraw();
        }
    }
}
