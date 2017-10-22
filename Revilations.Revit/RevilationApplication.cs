using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Revilations.Objects.Revit;
using Revilations.Revit.Objects;

namespace Revilations.Revit {

    public class RevilationApplication : IExternalApplication {

        internal static RevilationApplication Application;
        private RevilationElements elements;

        public Result OnShutdown(UIControlledApplication application) {
            application.ControlledApplication.DocumentChanged -= this.ControlledApplicationOnDocumentChanged;
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application) {
            Application = this;
            application.ControlledApplication.DocumentChanged += this.ControlledApplicationOnDocumentChanged;
            return Result.Succeeded;
        }

        public void AddElements(List<RevilationElement> elems) {
            foreach (var elem in elems) {
                this.elements.AddElements(elems);
            }
        }

        #region event handlers
        private void ControlledApplicationOnDocumentChanged(object sender, DocumentChangedEventArgs documentChangedEventArgs) {
            var doc = documentChangedEventArgs.GetDocument();
            this.HandleDeletedElements(documentChangedEventArgs.GetDeletedElementIds());
            this.HandleChangedElements(documentChangedEventArgs.GetModifiedElementIds());
        }

        void HandleDeletedElements(ICollection<ElementId> deletedElementIds) {
            foreach (var id in deletedElementIds) {
                if (this.elements.ContainsKey(id)) {
                    var idList = this.elements[id].DeleteChildren();
                    //foreach()
                }
            }
        }

        void HandleChangedElements(ICollection<ElementId> changedElementIds) {
            foreach (var id in changedElementIds) {
                if (this.elements.ContainsKey(id)) {
                    this.elements[id].UpdateChildren();
                }
            }
        }
        #endregion
    }
}
