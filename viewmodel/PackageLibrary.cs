using System.Collections.Generic;

namespace viewmodel
{
    /// <summary>
    /// Stores package programs for making dependency mappings
    /// </summary>
    public class PackageLibrary
    {
        Dictionary<string, SccmPackage> _packages = new Dictionary<string, SccmPackage>();
        Dictionary<string, SccmPackageProgram> _packageprograms = new Dictionary<string, SccmPackageProgram>();
        Dictionary<string, List<SccmPackageProgram>> _pendingregistrations = new Dictionary<string, List<SccmPackageProgram>>();

        public void AddPackageProgram(SccmPackageProgram packageprogram)
        {
            SccmPackageProgram outval;
            bool ret = this._packageprograms.TryGetValue(packageprogram.ID, out outval);
            if (ret == false) { this._packageprograms.Add(packageprogram.ID, packageprogram); }

            if (string.IsNullOrWhiteSpace(packageprogram.DependentProgram) == false)
            {
                this.RegisterDependentProgram(packageprogram.DependentProgram, packageprogram);
            }

            //check for the parent package, add or create
            SccmPackage outpackage;
            ret = this._packages.TryGetValue(packageprogram.PackageID, out outpackage);
            if (ret != true)
            { 
                outpackage = new SccmPackage(packageprogram.PackageName, packageprogram.PackageID);
                this._packages.Add(outpackage.ID, outpackage);
            }
            outpackage.AddProgram(packageprogram);

            //check for pending registrations. connect the dependencies if they exist and cleanup
            List<SccmPackageProgram> pendinglist;
            ret = this._pendingregistrations.TryGetValue(packageprogram.ID, out pendinglist);
            if (ret == true)
            {
                foreach (SccmPackageProgram child in pendinglist)
                {
                    child.DependentSccmPackageProgram = packageprogram;
                }
                this._pendingregistrations.Remove(packageprogram.ID);
            }
        }

        /// <summary>
        /// Register a program for it's dependent program. Will store and add later if program doesn't exist yet
        /// </summary>
        /// <param name="dependentid"></param>
        /// <param name="child"></param>
        public void RegisterDependentProgram(string dependentid, SccmPackageProgram progam)
        {
            if (!string.IsNullOrWhiteSpace(dependentid))
            {
                //string limitingidupper = dependentid.ToUpper();
                SccmPackageProgram outval;
                bool ret = this._packageprograms.TryGetValue(dependentid, out outval);
                if (ret == true) { progam.DependentSccmPackageProgram = outval; }
                else
                {
                    List<SccmPackageProgram> pendinglist;
                    ret = this._pendingregistrations.TryGetValue(dependentid, out pendinglist);
                    if (ret == true) { pendinglist.Add(progam); }
                    else
                    {
                        List<SccmPackageProgram> newlist = new List<SccmPackageProgram>();
                        newlist.Add(progam);
                        this._pendingregistrations.Add(dependentid, newlist);
                    }
                }
            }
        }

        public SccmPackageProgram GetPackageProgram(string packageprogramid)
        {
            if (!string.IsNullOrWhiteSpace(packageprogramid))
            {
                string colidupper = packageprogramid.ToUpper();
                SccmPackageProgram outval;
                bool ret = this._packageprograms.TryGetValue(colidupper, out outval);
                if (ret == true) { return outval; }
                else { return null; }
            }
            else { return null; }
        }

        public SccmPackage GetPackage(string packageid)
        {
            if (!string.IsNullOrWhiteSpace(packageid))
            {
                string colidupper = packageid.ToUpper();
                SccmPackage outval;
                bool ret = this._packages.TryGetValue(colidupper, out outval);
                if (ret == true) { return outval; }
                else { return null; }
            }
            else { return null; }
        }

        public List<SccmPackage> GetAllPackages()
        {
            List<SccmPackage> packages = new List<SccmPackage>();
            foreach (SccmPackage package in this._packages.Values)
            {
                packages.Add(package);
            }
            return packages;
        }
    }
}
