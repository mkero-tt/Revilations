using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revilations {

    [Serializable]
    public class RevilationsFile {

        public RevilationOptions Options;
        public List<Tuple<int, int, List<int>, int>> Elements;
    }
}
