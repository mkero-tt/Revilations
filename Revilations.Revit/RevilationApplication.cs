using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

namespace Revilations.Revit {
    public class RevilationApplication : IExternalApplication {
        public Result OnShutdown(UIControlledApplication application) {
            application.ControlledApplication.DocumentChanged -= this.ControlledApplicationOnDocumentChanged;
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application) {
            application.ControlledApplication.DocumentChanged += this.ControlledApplicationOnDocumentChanged;
            return Result.Succeeded;
        }

        private void ControlledApplicationOnDocumentChanged(object sender, DocumentChangedEventArgs documentChangedEventArgs) {
            var doc = documentChangedEventArgs.GetDocument();
            this.HandleDeletedElements(doc, documentChangedEventArgs.GetDeletedElementIds());
            this.HandleChangedElements(doc, documentChangedEventArgs.GetModifiedElementIds());
        }

        void HandleDeletedElements(Document doc, ICollection<ElementId> deletedElementIds) {
            foreach (var id in deletedElementIds) {
                var elem = doc.GetElement(id);
            }
        }

        void HandleChangedElements(Document doc, ICollection<ElementId> changedElementIds) {
            foreach (var id in changedElementIds) {
                var elem = doc.GetElement(id);
            }
        }
    }
}
