using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public class SccmDeployment
    {
        public string CollectionID { get; set; }
        public string CollectionName { get; set; }
        public string DeploymentID { get; set; }
        public int DeploymentIntent { get; set; }
        public string DeploymentName { get; set; }
        public int DeploymentType { get; set; }
        public int DeploymentTypeID { get; set; }
        public string TargetID { get; set; }
        public string TargetName { get; set; }
        public int TargetSecurityTypeID { get; set; }
        public string TargetSubName { get; set; }
    }
}
