using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revilations.Revit {

    public class RevilationPad {

        private FamilyInstance revitElement;
        private bool isMaster;
        private Transform transform;

        public RevilationPad(FamilyInstance revitElement, RevilationPad masterPad) {
            this.revitElement = revitElement;
            this.isMaster = masterPad == null || this.revitElement == masterPad.RevitElement;
            this.transform = (this.isMaster) ? Transform.Identity : this.CalculateTransform(masterPad);
        }

        public FamilyInstance RevitElement {
            get { return this.revitElement; }
        }

        public Transform Transform {
            get { return this.transform; }
        }

        public XYZ CenterPoint {
            get {
                var bb = this.revitElement.get_BoundingBox(null);
                return (bb.Max + bb.Min)/2.0;
            }   
        }

        Transform CalculateTransform(RevilationPad masterPad) {
            var translation = Transform.CreateTranslation(masterPad.CenterPoint - this.CenterPoint);
            var rotation = Transform.CreateRotation(XYZ.BasisZ, 0.0);
            return translation.Multiply(rotation);
        }
    }
}
