using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class SccmCollection: ViewModelBase
    {
        private string _name;
        private string _id;
        private string _comment;

        public string LimitingCollectionID { get; set; }
        public SccmCollection LimitingCollection { get; set; }
        public string Comment
        {
            get { return this._comment; }
            set
            {
                this._comment = value;
                this.OnPropertyChanged(this, "Comment");
            }
        }
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.OnPropertyChanged(this, "Name");
            }
        }
        public string ID
        {
            get { return this._id; }
            set
            {
                this._id = value;
                this.OnPropertyChanged(this, "ID");
            }
        }

        public SccmCollection() { }

        public SccmCollection(string id, string name, string limitingcollectionid)
        {
            this._name = name;
            this._id = id;
            this.LimitingCollectionID = limitingcollectionid;         
        }
    }
}
