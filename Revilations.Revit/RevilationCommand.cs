using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows;

namespace Revilations.Revit {

    public class RevilationCommand : IExternalCommand {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            try {
                var uidoc = commandData.Application.ActiveUIDocument;

                var allViews = CollectAllViews(uidoc.Document);
                var allDetailComponents = CollectAllDetailComponents(uidoc.Document);
                var control = new RevilationsInputControl(allViews, allDetailComponents);
                while (true) {
                    control.ShowDialog();
                    if (control.SelectElements) {
                        control.SetElements(uidoc.Selection.PickObjects(ObjectType.Element).Select(e => e.ElementId));
                        control.SelectElements = false;
                    } else if (control.SelectPads) {
                        control.SetPads(uidoc.Selection.PickObjects(ObjectType.Element).Select(e => e.ElementId));
                        control.SelectPads = false;
                    } else {
                        break;
                    }
                }

                if (control.RunOnClose) {
                    var masterElements = control.SelectedElements;
                    var masterPad = new RevilationPad(masterElements.FirstOrDefault(e => e.Item1.Name.Equals(Revilations.PadFamilySymbolName)).Item1, null);
                    var selectedPads = control.SelectedPads.Select(e => new RevilationPad(e, masterPad));
                    var viewToAddComponentsTo = control.SelectedView;

                    foreach (var masterElement in masterElements) {
                        foreach (var pad in selectedPads) {
                            var createdElem = (masterElement.Item2 == null) ? 
                                CreateCopyElement(masterElement.Item1, pad) : 
                                CreateDetailObject(masterElement.Item1, masterElement.Item2, pad, viewToAddComponentsTo);
                            SetElementDatas(createdElem, masterElement.Item1, pad);
                        }
                    }
                    return Result.Succeeded;
                } else {
                    return Result.Cancelled;
                }
            } catch (Exception x) {
                TaskDialog.Show("Revilations", $"{x.Message}\n{x.StackTrace}");
                return Result.Failed;
            }
        }

        public List<View> CollectAllViews(Document doc) {
            var fec = new FilteredElementCollector(doc);
            fec.OfCategory(BuiltInCategory.OST_Views);
            var views = new List<View>();
            foreach (var e in fec.ToElements()) {
                if (e is View) {
                    var v = e as View;
                    if (v.ViewType == ViewType.EngineeringPlan || v.ViewType == ViewType.FloorPlan) {
                        views.Add(v);
                    }
                }
            }
            return views;
        }

        public List<FamilySymbol> CollectAllDetailComponents(Document doc) {
            var fec = new FilteredElementCollector(doc);
            fec.OfCategory(BuiltInCategory.OST_DetailComponents);
            fec.OfClass(typeof(FamilySymbol));
            var detailComps = new List<FamilySymbol>();
            foreach (var e in fec.ToElements()) {
                detailComps.Add(e as FamilySymbol);
            }
            return detailComps;
        }

        FamilyInstance CreateCopyElement(FamilyInstance elem, RevilationPad pad) {
            var copyElem = elem.Document.GetElement(ElementTransformUtils.CopyElement(elem.Document, elem.Id, new XYZ()).FirstOrDefault());
            var elemPt = (elem.Location is LocationPoint) ? (copyElem.Location as LocationPoint).Point : (copyElem.Location as LocationCurve).Curve.Evaluate(0.5, true);
            var transformedLocation = pad.Transform.OfPoint(elemPt);
            elem.Location.Move(transformedLocation);
            return copyElem as FamilyInstance;
        }

        FamilyInstance CreateDetailObject(FamilyInstance elem, FamilySymbol symbol, RevilationPad pad, View viewToPlaceOn) {
            var doc = elem.Document;
            var elemPt = (elem.Location is LocationPoint) ? (elem.Location as LocationPoint).Point : (elem.Location as LocationCurve).Curve.Evaluate(0.5, true);
            var transformedLocation = pad.Transform.OfPoint(elemPt);
            return doc.Create.NewFamilyInstance(transformedLocation, symbol, viewToPlaceOn);
        }

        void SetElementDatas(FamilyInstance elem, FamilyInstance parentElement, RevilationPad pad) {
            
        }
    }
}