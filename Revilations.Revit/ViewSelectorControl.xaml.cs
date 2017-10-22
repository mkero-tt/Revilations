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
    /// Interaction logic for ViewSelectorControl.xaml
    /// </summary>
    public partial class ViewSelectorControl : System.Windows.Window {

        private bool ok;

        public ViewSelectorControl(List<View> views) {
            InitializeComponent();
            this.ok = false;
            foreach (var v in views) {
                this.listBox_Views.Items.Add(v.ViewName);
            }
        }

        public bool WasViewSelected {
            get { return this.ok; }
        }

        public int SelectedIndex {
            get { return this.listBox_Views.SelectedIndex; }
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            this.ok = true;
            this.Close();
        }
    }
}
