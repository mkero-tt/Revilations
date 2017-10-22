using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows;
using Revilations.Revit;

namespace Revilations.Objects.Revit {

    [Transaction(TransactionMode.Manual)]
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

                    using (var trans = new Transaction(uidoc.Document)) {
                        trans.Start("Revilations");
                        foreach (var masterElement in masterElements) {
                            if (masterElement.Item1.Id.IntegerValue != masterPad.RevitElement.Id.IntegerValue) {
                                var masterElem = new RevilationElement(masterElement.Item1, null, null, masterPad);
                                var createdElems = new List<RevilationElement>();
                                foreach (var pad in selectedPads) {
                                    var createdElem = (masterElement.Item2 == null) ? this.CreateCopyElement(masterElement.Item1, pad) : this.CreateDetailObject(masterElement.Item1, masterElement.Item2, pad, viewToAddComponentsTo);
                                    var revElem = new RevilationElement(createdElem, masterElem, null, pad);
                                    createdElems.Add(revElem);
                                }
                                foreach (var e in createdElems) {
                                    masterElem.Children.Add(e);
                                }
                                RevilationApplication.Application.AddElements(createdElems);
                            }
                        }
                        trans.Commit();
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
            return elem.Document.GetElement(ElementTransformUtils.CopyElement(elem.Document, elem.Id, pad.Translation).FirstOrDefault()) as FamilyInstance;
        }

        FamilyInstance CreateDetailObject(FamilyInstance elem, FamilySymbol symbol, RevilationPad pad, View viewToPlaceOn) {
            var doc = elem.Document;
            var elemPt = (elem.Location is LocationPoint) ? (elem.Location as LocationPoint).Point : (elem.Location as LocationCurve).Curve.Evaluate(0.5, true);
            //var transformedLocation = pad.Transform.OfPoint(elemPt);
            return doc.Create.NewFamilyInstance(elemPt + pad.Translation, symbol, viewToPlaceOn);
        }

        void SetElementDatas(FamilyInstance elem, FamilyInstance parentElement, RevilationPad pad) {
            //set the pad id
            elem.LookupParameter("RevilationsPadId").Set(pad.RevitElement.Id.IntegerValue.ToString());

            //set the parent ids
            var parentIdsParam = elem.LookupParameter("RevilationsParents");
            var parentIdsString = parentIdsParam.AsString();
            var parentIdString = (parentIdsString == null || parentIdsString.Equals(string.Empty)) ? $"{parentElement.Id.IntegerValue}" : $"{parentIdsString};{parentElement.Id.IntegerValue}";
            parentIdsParam.Set(parentIdString);

            //set the child ids
            var childrenIdsParam = parentElement.LookupParameter("RevilationsChildren");
            var childrenIdsString = childrenIdsParam.AsString();
            var childrenIdString = (childrenIdsString == null || childrenIdsString.Equals(string.Empty)) ? $"{elem.Id.IntegerValue}" : $"{childrenIdsString};{elem.Id.IntegerValue}";
            childrenIdsParam.Set(childrenIdString);
        }
    }
}