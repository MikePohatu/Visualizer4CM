using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class CollectionLibrary
    {
        Dictionary<string, SccmCollection> _collections = new Dictionary<string, SccmCollection>();
        Dictionary<string, List<SccmCollection>> _pendingregistrations = new Dictionary<string, List<SccmCollection>>();

        public void AddCollection(SccmCollection collection)
        {
            SccmCollection outval;
            bool ret = this._collections.TryGetValue(collection.ID, out outval);
            if (ret == false) { this._collections.Add(collection.ID, collection); }

            //register the limiting collection
            if (string.IsNullOrWhiteSpace(collection.LimitingCollectionID) == false) { this.RegisterChildCollection(collection.LimitingCollectionID, collection); }

            //check for pending registrations. connect the limiting collections if they exist and cleanup
            List<SccmCollection> pendinglist;
            ret = this._pendingregistrations.TryGetValue(collection.ID, out pendinglist);
            if (ret == true)
            {
                foreach (SccmCollection child in pendinglist)
                {
                    child.LimitingCollection = collection;
                }
                this._pendingregistrations.Remove(collection.ID);
            }
        }

        //register a child collection for its limiting collection. store it in pending if limiting isn't added yet
        public void RegisterChildCollection(string limitingid, SccmCollection child)
        {
            if (!string.IsNullOrWhiteSpace(limitingid))
            {
                string limitingidupper = limitingid.ToUpper();
                SccmCollection outval;
                bool ret = this._collections.TryGetValue(limitingidupper, out outval);
                if (ret == true) { child.LimitingCollection = outval; }
                else
                {
                    List<SccmCollection> pendinglist;
                    ret = this._pendingregistrations.TryGetValue(limitingidupper, out pendinglist);
                    if (ret == true) { pendinglist.Add(child); }
                    else
                    {
                        List<SccmCollection> newlist = new List<SccmCollection>();
                        newlist.Add(child);
                        this._pendingregistrations.Add(limitingidupper, newlist);
                    }
                }
            }
        }

        //register a child collection for its limiting collection. store it in pending if limiting isn't added yet
        public SccmCollection GetCollection(string collectionid)
        {
            if (!string.IsNullOrWhiteSpace(collectionid))
            {
                string colidupper = collectionid.ToUpper();
                SccmCollection outval;
                bool ret = this._collections.TryGetValue(colidupper, out outval);
                if (ret == true) { return outval; }
                else { return null; }
            }
            else { return null; }
        }

        public List<SccmCollection> GetAllCollections()
        {
            List<SccmCollection> collections = new List<SccmCollection>();
            foreach (SccmCollection col in this._collections.Values)
            {
                collections.Add(col);
            }
            return collections;
        }

    }
}
