using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.UI.DataModel;
using Microsoft.EnterpriseManagement.UI.Core.Connection;
//using Microsoft.EnterpriseManagement.UI.Extensions.Shared;
//using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
//using Microsoft.EnterpriseManagement.UI.ViewFramework;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Security;
//Contains ConsoleContextHelper
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common;
//Contains BindableDataItems
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel.Design;

namespace SMCenter.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LocationForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPackClass locationClass;
        ManagementPackRelationship relationClass;
        //public static Guid guidLocationType = new Guid("247A7668-980C-7048-82A9-47B634275223");
        public static Guid guidLocationType = new Guid("6E2535F9-01F6-FC10-52C9-1E77274A3794");
        //A3E35143-3D32-AA5C-5AA7-564E54695F48 - Guid MP SMCenter.AssetManagement.Core
        public LocationForm()
        {
            InitializeComponent();
        }
  
        void GetSession()
        {
            // Get the current session, more info: <span class="skimlinks-unlinked">http://blogs.technet.com/servicemanager/archive/2010/02/11/tasks-part-1-tasks-overview.aspx</span>
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                if (curSession == null)
                    throw new ValueUnavailableException("curSession is null");
                emg = curSession.ManagementGroup;
                Trace.WriteLine(DateTime.Now + " : " + "emg.Name = " + emg.Name);
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + " : " + "GetSession Error : " + e.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string userName = (System.Security.Principal.WindowsIdentity.GetCurrent().Name).Replace("\\", "_");
            //string strlog = @"c:\Temp\SMCLog_" + userName + ".txt";
            //Debug.WriteLine(strlog);
            //TextWriterTraceListener TL = new TextWriterTraceListener(strlog);
            //Trace.Listeners.Add(TL);
            //Trace.AutoFlush = true;
            GetSession();
            ManagementPack mp = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
            locationClass = emg.EntityTypes.GetClass("SMCenter.Location", mp);
            relationClass = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mp);
            //FillTreeView.Now(emg, locationClass, relationClass, LocationTreeView);
            Fill();
        }

        private void brd_Loc_Create_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LocationTreeView.SelectedItem != null)
            {
                CreatableEnterpriseManagementObject newLocation = new CreatableEnterpriseManagementObject(emg, locationClass);

                //Add some property values
                newLocation[locationClass, "DisplayName"].Value = "Add some info";
                newLocation.Commit();

                //Treeview Item - Parent Item
                TreeViewItem TR = (TreeViewItem)LocationTreeView.SelectedItem;
                // Get an object by GUID
                Guid G = new Guid(TR.Tag.ToString());
                EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                CreatableEnterpriseManagementRelationshipObject relationship = new CreatableEnterpriseManagementRelationshipObject(emg, relationClass);
                relationship.SetSource(treeviewEMO);
                relationship.SetTarget(newLocation);
                relationship.Commit();

                //FillTreeView.Now(emg, locationClass, relationClass, LocationTreeView);
                Fill();


                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(locationClass);
                IDataItem newlocationDataItem = dataType.CreateProxyInstance(newLocation);

                //Open Console form for created object
                //ConsoleContextHelper.Instance.PopoutForm(itemIdentity);
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(newlocationDataItem);
            }
            else
            {
                MessageBox.Show("Please select parent location!", "No location selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                Trace.WriteLine(DateTime.Now + " : " + "Not have Values for create Rel");
            }
        }

        private void brd_Loc_Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (LocationTreeView.SelectedItem != null)
                {

                    Guid guid = new Guid("2d30268b-c9d8-b9f5-93ff-5b95b2cd3e4d");
                    Collection<IDataItem> ItemsCol = new Collection<IDataItem>();
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.LaunchAddInstancePickerDialog(ItemsCol, guid);
                    if (ItemsCol.Count() == 1)
                    {
                        TreeViewItem TR = (TreeViewItem)LocationTreeView.SelectedItem;
                        // Get an object by GUID
                        Guid G = new Guid(TR.Tag.ToString());
                        EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                        IDataItem targetInstanceIDataItem = ItemsCol[0];
                        //IDataItem targetInstanceIDataItem = this.LocPiker.Instance;
                        Guid gId = (Guid)targetInstanceIDataItem["$Id$"];
                        Trace.WriteLine(DateTime.Now + " : " + "Id : " + gId); //+ " ID : " + targetInstanceIDataItem["ID"]);
                        EnterpriseManagementObject pickerEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(gId, ObjectQueryOptions.Default);
                        Trace.WriteLine(DateTime.Now + " : " + "Values for create Rel : " + treeviewEMO.DisplayName + " : " + pickerEMO.DisplayName + " picker.Id : " + pickerEMO.Id.ToString());
                        CreatableEnterpriseManagementRelationshipObject relationship = new CreatableEnterpriseManagementRelationshipObject(emg, relationClass);
                        relationship.SetSource(treeviewEMO);
                        relationship.SetTarget(pickerEMO);
                        relationship.Commit();

                        //FillTreeView.Now(emg, locationClass, relationClass, LocationTreeView);
                        Fill();
                    }
                    else if (ItemsCol.Count() > 1)
                    {
                        MessageBox.Show("Please select only ONE location!", "Multiselection!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (ItemsCol.Count() == 0)
                    {
                        MessageBox.Show("Please select location!", "No location selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please select location!", "No location selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Trace.WriteLine(DateTime.Now + " : " + "Not have Values for create Rel");
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Picker Dialog " + exc.Message);
            }
        }

        private void brd_Loc_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LocationTreeView.SelectedItem != null)
            {
                TreeViewItem TR = (TreeViewItem)LocationTreeView.SelectedItem;
                // Get an object by GUID
                Guid G = new Guid(TR.Tag.ToString());
                EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(locationClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(treeviewEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select location!", "No location selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_Loc_DeleteLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete RelationShip?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {
                    TreeViewItem TR = (TreeViewItem)LocationTreeView.SelectedItem;
                    // Get an object by GUID
                    Guid G = new Guid(TR.Tag.ToString());
                    //EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                    IncrementalDiscoveryData instance = new IncrementalDiscoveryData();
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                    {
                        Trace.WriteLine(DateTime.Now + " : " + "Remove Relationship " + obj.TargetObject.DisplayName);
                        instance.Remove(obj);
                        instance.Commit(emg);
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Remove Relationship void " + exc.Message);
            }
            //FillTreeView.Now(emg, locationClass, relationClass, LocationTreeView);
            Fill();
        }

        private void brd_Loc_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LocationTreeView.SelectedItem != null)
            {
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete Location?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {

                }
            }
            else
            {
                MessageBox.Show("Please select location!", "No location selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_Loc_Refresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //FillTreeView.Now(emg, locationClass, relationClass, LocationTreeView);
            Fill();
        }

        private void Fill()
        {
            LocationTreeView.Items.Clear();
            EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", locationClass);
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
                LocationTreeView.Items.Add(item);
            }
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem SelectedItem = (TreeViewItem)e.OriginalSource;
            SelectedItem.Items.Clear();

            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();

            // Get an object by GUID
            Guid G = new Guid(SelectedItem.Tag.ToString());
            var child_locations = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
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
    }
    }

