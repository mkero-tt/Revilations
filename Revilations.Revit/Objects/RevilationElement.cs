using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revilations.Objects.Revit {

    public class RevilationElement {

        private FamilyInstance element;
        private RevilationElement parent;
        private HashSet<RevilationElement> children;

        private RevilationPad areaPad;

        public XYZ previousPosition;

        public RevilationElement(FamilyInstance element, RevilationElement parent, HashSet<RevilationElement> children = null, RevilationPad areaPad = null) {
            this.element = element;
            this.parent = parent;
            this.children = children ?? new HashSet<RevilationElement>();
            this.areaPad = areaPad;
            var bb = this.element.get_BoundingBox(null);
            this.previousPosition = (bb.Max + bb.Min) / 2.0;
        }

        public FamilyInstance FamilyInstance {
            get { return this.element; }
        }

        public RevilationElement Parent {
            get { return this.parent; }
        }

        public HashSet<RevilationElement> Children {
            get { return this.children; }
        }

        public RevilationPad AreaPad {
            get { return this.areaPad; }
        }

        public void SetDataToParameters() {
            this.element.LookupParameter("RevilationsPads").Set(this.areaPad.RevitElement.Id.IntegerValue.ToString());
            this.element.LookupParameter("RevilationsParents").Set(this.element.Id.IntegerValue.ToString());
            var childrenIds = string.Empty;
            foreach (var child in this.children) {
                childrenIds = $"{child.FamilyInstance.Id.IntegerValue};{this.children}";
            }
            this.element.LookupParameter("RevilationsChildren").Set(childrenIds.Substring(0, childrenIds.Length - 1));
        }

        public void UpdateChildren() {
            
        }
    }
}
