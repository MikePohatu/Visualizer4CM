using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public class SccmApplicationRelationship
    {
        public enum RelationShipType { Install, DontInstall }

        public int ToDeploymentTypeCIID { get; set; }
        public int ToApplicationCIID { get; set; }
        public RelationShipType Type { get; set; }
        public int FromApplicationCIID { get; set; }
        public int FromDeploymentTypeCIID { get; set; }

        /// <summary>
        /// Set the type based on int - 1=LIMITING,2=INCLUDE,3=EXCLUDE
        /// </summary>
        /// <param name="type"></param>
        public void SetType(int type)
        {
            //if (type == 1) { this.Type = RelationShipType.Limiting; }
            //else if(type == 2) { this.Type = RelationShipType.Include; }
            //else if (type == 3) { this.Type = RelationShipType.Exclude; }
        }
    }
}
