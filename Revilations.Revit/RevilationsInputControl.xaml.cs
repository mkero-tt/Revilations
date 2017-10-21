using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Revilations.Revit {

    /// <summary>
    /// Interaction logic for RevilationsInputControl.xaml
    /// </summary>
    public partial class RevilationsInputControl : System.Windows.Window {

        private Document doc;
        private Dictionary<string, FamilyInstance> selectedElements;
        private List<FamilyInstance> selectedPads;
        private View selectedView;

        private List<View> allViews;
        private List<FamilySymbol> allSymbols;

        private bool runOnClose, selectElements, selectPads;

        public RevilationsInputControl(List<View> allViews, List<FamilySymbol> allSymbols) {
            InitializeComponent();
            this.doc = allViews[0].Document;
            this.selectedElements = new Dictionary<string, FamilyInstance>();
            this.selectedPads = new List<FamilyInstance>();
            this.allViews = allViews;
            this.allSymbols = allSymbols;
            this.selectedView = null;
            this.runOnClose = false;
            this.selectElements = false;
            this.selectPads = false;
        }

        public bool SelectElements {
            get { return this.selectElements; }
            set { this.selectElements = value; }
        }

        public bool SelectPads {
            get { return this.selectPads; }
            set { this.selectPads = value; }
        }
        
        public bool RunOnClose {
            get { return this.runOnClose; }
        }

        public List<Tuple<FamilyInstance, FamilySymbol>> SelectedElements {
            get {
                var selElems = new List<Tuple<FamilyInstance, FamilySymbol>>();
                foreach (var e in this.stackPanel_ElementMap.Children) {
                    var stackPanelPair = e as StackPanel;
                    var label = stackPanelPair.Children[0] as Label;
                    var symbolComboBox = stackPanelPair.Children[1] as ComboBox;
                    var inst = this.selectedElements[label.Content.ToString()];
                    var symbolToUse = (symbolComboBox.SelectedIndex == 0) ? null : this.allSymbols[symbolComboBox.SelectedIndex - 1];
                    selElems.Add(new Tuple<FamilyInstance, FamilySymbol>(inst, symbolToUse));
                }
                return selElems;
            }
        }

        public List<FamilyInstance> SelectedPads {
            get { return this.selectedPads; }
        }

        public View SelectedView {
            get { return this.selectedView; }
        }

        #region methods called back by command
        public void SetElements(IEnumerable<ElementId> ids) {
            foreach (var id in ids) {
                var elem = this.doc.GetElement(id);
                if (elem is FamilyInstance) {
                    var inst = elem as FamilyInstance;
                    var instName = $"{inst.Name}-{inst.Id.IntegerValue}";

                    var label = new Label();
                    label.Content = instName;

                    var dropDown = new ComboBox();
                    dropDown.Items.Add("");
                    foreach (var view in this.allSymbols) {
                        dropDown.Items.Add(view.Name);
                    }
                    dropDown.SelectedIndex = 0;

                    var elemPanel = new StackPanel();
                    elemPanel.Orientation = Orientation.Horizontal;
                    elemPanel.Children.Add(label);
                    elemPanel.Children.Add(dropDown);
                    this.stackPanel_ElementMap.Children.Add(elemPanel);

                    this.selectedElements.Add(instName, inst);
                }
            }
        }
        
        public void SetPads(IEnumerable<ElementId> ids) {
            foreach (var id in ids) {
                var elem = this.doc.GetElement(id);
                if (elem is FamilyInstance) {
                    var inst = elem as FamilyInstance;
                    if (inst.Name.Equals(Revilations.PadFamilySymbolName)) {
                        this.selectedPads.Add(inst);
                    }
                }
            }
        }
        #endregion

        private void buttonOK_Click(object sender, RoutedEventArgs e) {
            this.runOnClose = true;
            this.Close();
        }
        
        private void buttonSelectElements_Click(object sender, RoutedEventArgs e) {
            this.selectElements = true;
            this.Hide();
        }

        private void buttonSelectPads_Click(object sender, RoutedEventArgs e) {
            this.selectPads = true;
            this.Hide();
        }

        private void buttonSelectView_Click(object sender, RoutedEventArgs e) {

        }
    }
}
