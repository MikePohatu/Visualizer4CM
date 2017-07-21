using System.Collections.Generic;

namespace viewmodel
{
    public abstract class SccmResource: SccmDeployableItem
    {
        protected List<string> _collectionids = new List<string>();
        public List<string> CollectionIDs
        {
            get { return this._collectionids; }
            set { this._collectionids = value; }
        }
    }
}
