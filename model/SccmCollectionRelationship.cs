using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class SccmCollectionRelationship
    {
        public enum RelationShipType { Limiting, Include, Exclue }
        public string DependentCollectionID { get; set; }
        public RelationShipType Type { get; set; }
        public string SourceCollectionID { get; set; }

        /// <summary>
        /// Set the type based on int - 1=LIMITING,2=INCLUDE,3=EXCLUDE
        /// </summary>
        /// <param name="type"></param>
        public void SetType(int type)
        {
            if (type == 1) { this.Type = RelationShipType.Limiting; }
            else if(type == 2) { this.Type = RelationShipType.Include; }
            else if (type == 3) { this.Type = RelationShipType.Exclue; }
        }
    }
}
