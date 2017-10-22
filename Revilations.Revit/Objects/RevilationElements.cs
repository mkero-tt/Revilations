using Autodesk.Revit.DB;
using Revilations.Objects.Revit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revilations.Revit.Objects {
    public class RevilationElements : Dictionary<ElementId, RevilationElement> {

        public void AddElements(IEnumerable<RevilationElement> elems) {
            foreach (var elem in elems) {
                this.Add(elem.FamilyInstance.Id, elem);
                elem.SetDataToParameters();
            }
        }

        public void DeleteElements(IEnumerable<RevilationElement> elems) {
            foreach (var e in elems) {
                this.DeleteElement(e);
            }
        }

        private void DeleteElement(RevilationElement elem) {
            this.Remove(elem.FamilyInstance.Id);
            foreach (var e in elem.Children) {
                e.FamilyInstance.Document.Delete(e.FamilyInstance.Id);
                this.DeleteElement(e);
            }
        }
    }
}
