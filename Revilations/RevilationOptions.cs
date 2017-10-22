using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revilations {

    public enum DeleteOptions {
        None,
        DeleteChildren,
        KeepChildren
    }

    public class RevilationOptions {

        public DeleteOptions DeleteOption;
    }
}
