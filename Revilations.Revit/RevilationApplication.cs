using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace Revilations.Revit {
    public class RevilationApplication : IExternalApplication {
        public Result OnShutdown(UIControlledApplication application) {
            throw new NotImplementedException();
        }

        public Result OnStartup(UIControlledApplication application) {
            throw new NotImplementedException();
        }
    }
}
