using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace Revilations.Revit {

    public class RevilationPad {

        private FamilyInstance revitElement;
        private bool isMaster;
        private Transform transform;

        private XYZ translationVector;
        private double rotation;

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

        public XYZ Translation {
            get { return this.translationVector; }
        }

        public double Rotation {
            get { return this.rotation; }
        }

        public XYZ CenterPoint {
            get {
                var bb = this.revitElement.get_BoundingBox(null);
                return (bb.Max + bb.Min)/2.0;
            }   
        }

        public XYZ CalculatePlacementLocation(XYZ referPt, RevilationPad referPad) {
            var referPadbb = referPad.RevitElement.get_BoundingBox(null);

            var refPadRemapX = Remap(referPt.X, referPadbb.Min.X, referPadbb.Max.X, 0.0, 0.1);
            var refPadRemapY = Remap(referPt.Y, referPadbb.Min.Y, referPadbb.Max.Y, 0.0, 0.1);
            var refPadRemapZ = Remap(referPt.Z, referPadbb.Min.Z, referPadbb.Max.Z, 0.0, 0.1);
            //TaskDialog.Show("Revilation", $"{refPadRemapX}-{refPadRemapY}-{refPadRemapZ}");

            var thisbb = this.revitElement.get_BoundingBox(null);

            var thisPadRemapX = Remap(refPadRemapX, 0.0, 0.1, thisbb.Min.X, thisbb.Max.X);
            var thisPadRemapY = Remap(refPadRemapY, 0.0, 0.1, thisbb.Min.Y, thisbb.Max.Y);
            var thisPadRemapZ = Remap(refPadRemapZ, 0.0, 0.1, thisbb.Min.Z, thisbb.Max.Z);
            //TaskDialog.Show("Revilation", $"{thisPadRemapX}-{thisPadRemapY}-{thisPadRemapZ}");

            return new XYZ(thisPadRemapX, thisPadRemapY, referPt.Z);
        }

        double Remap(double value, double currMin, double currMax, double newMin, double newMax) {
            return newMin + (value - currMin) * (newMax - newMin) / (currMax - currMin);
        }

        Transform CalculateTransform(RevilationPad masterPad) {
            this.translationVector = this.CenterPoint - masterPad.CenterPoint;
            var translationTransform = Transform.CreateTranslation(this.translationVector);
            this.rotation = 0.0;
            var rotationTransform = Transform.CreateRotation(XYZ.BasisZ, this.rotation);
            return translationTransform.Multiply(rotationTransform);
        }
    }
}
