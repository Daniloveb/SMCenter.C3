using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.UI.DataModel;
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.ConsoleFramework;

namespace SMCenter.NetworkTasks
{
    /// <summary>
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class TreeViewForm : Window
    {
        EnterpriseManagementGroup emg;
        ManagementPackClass mpClass;
        ManagementPackRelationship RelationshipClass;
        EnterpriseManagementObject EMO;
        EnterpriseManagementObjectCriteria criteria;

        public TreeViewForm(EnterpriseManagementGroup _emg, ManagementPackClass _mpClass, ManagementPackRelationship _RelationshipClass, EnterpriseManagementObjectCriteria _criteria)
        {
            InitializeComponent();
            emg = _emg;
            mpClass = _mpClass;
            RelationshipClass = _RelationshipClass;
            criteria = _criteria;
        }

        private void TreeViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //FillTreeView.Now(emg, mpClass, RelationshipClass, this.MainTreeView);
            Fill(criteria);
        }

        private void Fill(EnterpriseManagementObjectCriteria criteria)
        {
            MainTreeView.Items.Clear();
            //EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", locationClass);
            IObjectReader<EnterpriseManagementObject> reader = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(criteria, ObjectQueryOptions.Default);
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            foreach (EnterpriseManagementObject loc in reader)
            {
                TreeViewItem Root = new TreeViewItem();
                Root.Header = loc.DisplayName;
                Root.Tag = loc.Id;
                Col.Add(Root);
            }
            //Сортировка
            var sortedOC = from item in Col orderby item.Header select item;

            foreach (var i in sortedOC)
            {
                TreeViewItem item = (TreeViewItem)i;
                item.Items.Add("*");
                MainTreeView.Items.Add(item);
            }
        }

        public EnterpriseManagementObject ReturnEMO()
        {
            return EMO;
        }

        private void Send_Data()
        {
            try
            {
                if (this.MainTreeView.SelectedItem != null)
                {

                    TreeViewItem TR = (TreeViewItem)this.MainTreeView.SelectedItem;
                    // Get an object by GUID
                    Guid G = new Guid(TR.Tag.ToString());

                    EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                    SelectTreeViewObjectHandlerClass.EventHandler(EMO);
                }
                this.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(DateTime.Now + " : " + "Error in TreeViewForm Send_data " + exc.Message);
            }
        }

        private void lbOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Send_Data();
        }

        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //SelectTreeViewObjectHandlerClass.EventHandler(EMO);
            this.Close();
        }

        private void MainTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainTreeView.SelectedItem != null)
            {
                Send_Data();
            }
        }


        private void MainTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {

                TreeViewItem SelectedItem = (TreeViewItem)e.OriginalSource;

                SelectedItem.Items.Clear();

                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();

                // Get an object by GUID
                Guid G = new Guid(SelectedItem.Tag.ToString());
                var child_locations = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, RelationshipClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in child_locations)
                {
                    TreeViewItem newItem = new TreeViewItem();
                    newItem.Header = relobj.TargetObject.DisplayName;
                    newItem.Tag = relobj.TargetObject.Id;
                    Col.Add(newItem);
                }
                //Сортировка
                var sortedOC = from item in Col orderby item.Header select item;
                foreach (var i in sortedOC)
                {
                    TreeViewItem item = (TreeViewItem)i;
                    item.Items.Add("*");
                    SelectedItem.Items.Add(item);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(DateTime.Now + " : " + "TreeViewForm MainTreeView_Expanded Error! " + exc.Message);
            }
        }

    }
    public static class SelectTreeViewObjectHandlerClass
    {
        public delegate void EMOEvent(EnterpriseManagementObject _EMO);
        public static EMOEvent EventHandler;
    }
    public static class TestHandlerClass
    {
        public delegate void TestEvent(string TestString);
        public static TestEvent EventHandler;
    }
}
