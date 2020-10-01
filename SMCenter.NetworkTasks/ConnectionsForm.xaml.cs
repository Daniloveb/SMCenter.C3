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
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;

namespace SMCenter.NetworkTasks
{
    /// <summary>
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class ConnectionsForm : Window
    {
        EnterpriseManagementGroup emg;
        ManagementPack mpAssetCore;
        ManagementPack mpNetworkAssetLibrary;
        ManagementPack mpNetworkLibrary;
        ManagementPack mpWindows;
        ManagementPack mpCMLibrary;

        ManagementPackClass classModule;
        ManagementPackClass classWindowsComputer;
        ManagementPackClass classDeployedComputer;
        ManagementPackClass classComputerNetworkAdapter;
        ManagementPackClass classNode;
        ManagementPackClass classNodePort;
        ManagementPackClass classLocation;
        ManagementPackClass classDevice;
        ManagementPackClass classDeviceNetworkAdapter;
        ManagementPackClass classPatchPanel;
        ManagementPackClass classPatchPanelPort;
        

        ManagementPackRelationship relComputerRunsWindowsComputer;
        ManagementPackRelationship relComputerHostsComputerNetworkAdapter;
        ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter;
        ManagementPackRelationship relLoctoLoc;
        //ManagementPackRelationship relLocationContainsConfigItem;
        ManagementPackRelationship relConfigItemRefLocation;
        ManagementPackRelationship relNodeComposedOfNetworkAdapter;
        ManagementPackRelationship relDeviceHostNetworkAdapter;

        EnterpriseManagementObject EMO;
        CablesTree CablesTree;

        LogFile LogFile;

        public ConnectionsForm(IDataItem Node)
        {
            InitializeComponent();

            try
            {

                emg = GetSession();
                mpWindows = emg.ManagementPacks.GetManagementPack("Microsoft.Windows.Library", "31bf3856ad364e35", new Version());
                mpAssetCore = emg.ManagementPacks.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                mpNetworkLibrary = emg.GetManagementPack("System.NetworkManagement.Library", "31bf3856ad364e35", new Version());
                mpCMLibrary = emg.ManagementPacks.GetManagementPack("Microsoft.SystemCenter.ConfigurationManager", "31bf3856ad364e35", new Version());
                classModule = emg.EntityTypes.GetClass("SMCenter.Module", mpNetworkAssetLibrary);
                classWindowsComputer = emg.EntityTypes.GetClass("SMCenter.WindowsComputer", mpNetworkAssetLibrary);
                classDeployedComputer = emg.EntityTypes.GetClass("SMCenter.DeployedComputer", mpNetworkAssetLibrary);
                classComputerNetworkAdapter = emg.EntityTypes.GetClass("Microsoft.Windows.ComputerNetworkAdapter", mpWindows);
                classNode = emg.EntityTypes.GetClass("SMCenter.Node", mpNetworkAssetLibrary);
                classNodePort = emg.EntityTypes.GetClass("SMCenter.NodePort", mpNetworkAssetLibrary);
                classLocation = emg.EntityTypes.GetClass("SMCenter.Location", mpAssetCore);
                classDevice = emg.EntityTypes.GetClass("SMCenter.Device", mpNetworkAssetLibrary);
                classDeviceNetworkAdapter = emg.EntityTypes.GetClass("SMCenter.Device.NetworkAdapter", mpNetworkAssetLibrary);
                classPatchPanel = emg.EntityTypes.GetClass("SMCenter.PatchPanel", mpNetworkAssetLibrary);
                classPatchPanelPort = emg.EntityTypes.GetClass("SMCenter.PatchPanelPort", mpNetworkAssetLibrary);
                relComputerRunsWindowsComputer = emg.EntityTypes.GetRelationshipClass("Microsoft.SystemCenter.ConfigurationManager.DeployedComputerRunsWindowsComputer", mpCMLibrary);
                relComputerHostsComputerNetworkAdapter = emg.EntityTypes.GetRelationshipClass("Microsoft.Windows.ComputerHostsComputerNetworkAdapter", mpWindows);
                relLoctoLoc = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpAssetCore);
                //relNetworkAdapterHasChildNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkAdapterHasChildNetworkAdapter", mpNetworkAssetLibrary);
                relChildNetworkAdapterRefParentNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.ChildNetworkAdapterRefParentNetworkAdapter", mpNetworkAssetLibrary);
                //relLocationContainsConfigItem = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsConfigItem", mpAssetCore);
                relConfigItemRefLocation = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefLocation", mpAssetCore);
                relNodeComposedOfNetworkAdapter = emg.EntityTypes.GetRelationshipClass("System.NetworkManagement.NodeComposedOfNetworkAdapter", mpNetworkLibrary);
                relDeviceHostNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.DeviceHostsNetworkAdapter", mpNetworkAssetLibrary);

                this.ConnectionsTreeView.Items.Clear();
                this.CabelsTreeView.Items.Clear();
                TreeViewItem treeitem = new TreeViewItem();

                LogFile = new LogFile(@"C:\LogFile.txt", true);

                CablesTree = new CablesTree(emg, relChildNetworkAdapterRefParentNetworkAdapter, classModule, relConfigItemRefLocation, LogFile);

                //Get CurrentNode
                Guid CurrentNodeId = (Guid)Node["$Id$"];
                EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);

                this.txtTargetObject.Text = EMO.FullName;

                Guid Id_NA = new Guid();
                if (EMO.IsInstanceOf(classModule))
                {
                    this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(CurrentNodeId));
                }
                else if (EMO.IsInstanceOf(classWindowsComputer))
                {
                    #region
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Id_NA = relobj.TargetObject.Id;
                    }
                    this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(Id_NA));

                    treeitem = ConnectionsTree.Create(Id_NA, emg);
                    if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    #endregion
                }
                else if (EMO.IsInstanceOf(classDeployedComputer))
                {
                    #region
                    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relComputerRunsWindowsComputer, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        GuidCol.Add(relobj.TargetObject.Id);
                    }
                    //Для каждого WindowsComputer ищем свой NetworkAdapter
                    foreach (Guid id in GuidCol)
                    {
                        T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(id, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                        {
                            Id_NA = relobj.TargetObject.Id;
                        }
                        this.CabelsTreeView.Items.Add(CablesTree.Go(Id_NA));
                        treeitem = ConnectionsTree.Create(Id_NA, emg);
                        if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    }
                    #endregion

                }
                else if (EMO.IsInstanceOf(classNode))
                {
                    #region
                    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        GuidCol.Add(relobj.TargetObject.Id);
                    }
                    foreach (Guid id in GuidCol)
                    {
                        this.CabelsTreeView.Items.Add(CablesTree.Go(id));
                        treeitem = ConnectionsTree.Create(id, emg);
                        if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    }
                    #endregion
                }
                else if (EMO.IsInstanceOf(classNodePort))
                {
                    this.CabelsTreeView.Items.Add(CablesTree.Go(CurrentNodeId));
                    treeitem = ConnectionsTree.Create(CurrentNodeId, emg);
                    if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                }
                else if (EMO.IsInstanceOf(classDevice))
                {
                    #region
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Id_NA = relobj.TargetObject.Id;
                    }
                    this.CabelsTreeView.Items.Add(CablesTree.Go(Id_NA));
                    treeitem = ConnectionsTree.Create(Id_NA, emg);
                    if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    #endregion
                }
                else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                {
                    this.CabelsTreeView.Items.Add(CablesTree.Go(CurrentNodeId));
                    treeitem = ConnectionsTree.Create(CurrentNodeId, emg);
                    if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ConnectionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillLocations();
            FillNodes();
  
        }

        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                if (curSession == null)
                    throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }

        public void FillLocations()
        {
            try
            {
            //LocationType находим тип Building
            EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);
            //EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = 'LocationType.Building'", classLocation);
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
                LocationsTreeView.Items.Add(item);
            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("FillLocations procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        public void FillNodes()
        {
            try
            {
            //LocationType находим тип Building
            //EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("GeoName = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);
            //EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = 'LocationType.Building'", classLocation);
            //IObjectReader<EnterpriseManagementObject> reader = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(criteria, ObjectQueryOptions.Default);
            IObjectReader<EnterpriseManagementObject> reader = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(classNode, ObjectQueryOptions.Default);
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            foreach (EnterpriseManagementObject loc in reader)
            {
                if (loc[classNode, "GeoName"].Value != null && loc[classNode, "GeoName"].Value.ToString().Contains("xFREE") == false)
                {
                    TreeViewItem Root = new TreeViewItem();
                    Root.Header = loc.DisplayName;
                    Root.Tag = loc.Id;
                    Col.Add(Root);
                }
            }
            //Сортировка
            var sortedOC = from item in Col orderby item.Header select item;

            foreach (var i in sortedOC)
            {
                TreeViewItem item = (TreeViewItem)i;
                item.Items.Add("*");
                NodesTreeView.Items.Add(item);
            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("FillNodes procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LocationsTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem SelectedItem = (TreeViewItem)e.OriginalSource;

                SelectedItem.Items.Clear();

                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();

                // Get an object by GUID
                Guid G = new Guid(SelectedItem.Tag.ToString());
                var child_locations = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relLoctoLoc, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("LocationsTreeView_Expanded procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void NodesTreeView_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem SelectedItem = (TreeViewItem)e.OriginalSource;

                SelectedItem.Items.Clear();

                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();

                // Get an object by GUID
                Guid G = new Guid(SelectedItem.Tag.ToString());
                var ports = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in ports)
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
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("NodesTreeView_Expanded procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

        }

        private void LocationsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                Guid Guid_NetworkAdapter = new Guid();
                TreeViewItem Item_Module = new TreeViewItem();
                TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;
                //TextBox
                this.txtTargetObject.Text = SelectedItem.Header.ToString();

                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
                Guid Guid_loc = new Guid(SelectedItem.Tag.ToString());
                this.ConnectionsTreeView.Items.Clear();
                this.CabelsTreeView.Items.Clear();
                var modules = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Guid_loc, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in modules)
                {
                    Item_Module.Header = relobj.SourceObject.DisplayName;
                    Item_Module.Tag = relobj.SourceObject.Id;
                    Guid_NetworkAdapter = relobj.SourceObject.Id;
                    if (Guid_NetworkAdapter != Guid.Empty)
                    {
                        Col.Add(CablesTree.CreateTreeViewItem(Guid_NetworkAdapter));
                    }
                }
                //Сортировка
                var sortedOC = from item in Col orderby item.Header select item;
                foreach (var i in sortedOC)
                {
                    TreeViewItem item = (TreeViewItem)i;
                    //item.Items.Add("*");
                    this.CabelsTreeView.Items.Add(item);
                }
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("LocationsTreeView_SelectedItemChanged procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void NodesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;

                //TextBox
                this.txtTargetObject.Text = SelectedItem.Header.ToString();
                this.txtLocation.Text = "";

                TreeViewItem NewItem = new TreeViewItem();
                Guid SelectedItemId = new Guid(SelectedItem.Tag.ToString());
                this.ConnectionsTreeView.Items.Clear();
                this.CabelsTreeView.Items.Clear();
                ObservableCollection<TreeViewItem> ColCables = new ObservableCollection<TreeViewItem>();
                ObservableCollection<TreeViewItem> ColConnections = new ObservableCollection<TreeViewItem>();
                if (SelectedItem.Header.ToString().Contains("PORT"))
                {
                    this.CabelsTreeView.Items.Add(CablesTree.Go(SelectedItemId));
                    TreeViewItem treeitem = ConnectionsTree.Create(SelectedItemId, emg);
                    if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                }
                else
                {
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(SelectedItemId, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        Guid Guid_NetworkAdapter = relobj.TargetObject.Id;
                            ColCables.Add(CablesTree.CreateTreeViewItem(Guid_NetworkAdapter));

                            NewItem = ConnectionsTree.Create(Guid_NetworkAdapter, emg);
                            if (NewItem != null) { ColConnections.Add(NewItem); }
                    }
                    //Сортировка
                    var sortedCables = from item in ColCables orderby item.Header select item;
                    foreach (var i in sortedCables)
                    {
                        TreeViewItem TVitem = (TreeViewItem)i;
                        this.CabelsTreeView.Items.Add(TVitem);
                    }
                    if (ColConnections.Count > 0)
                    {
                        var sortedConnections = from item in ColConnections orderby item.Header select item;
                        foreach (var i in sortedConnections)
                        {
                            TreeViewItem TVitem = (TreeViewItem)i;
                            if (TVitem != null) { this.ConnectionsTreeView.Items.Add(TVitem); }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("NodesTreeView_SelectedItemChanged procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ConnectionsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            
            


        }

        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CabelsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                this.txtLocation.Text = "";

                TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;
                if (!(SelectedItem.Header.ToString().Contains("Cable")))
                {
                    Guid Guid = new Guid(SelectedItem.Tag.ToString());
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Guid, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        EnterpriseManagementObject E = relobj.TargetObject;
                        this.txtLocation.Text = E[classLocation, "Path"].Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void CabelsTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            TreeViewItem SelectedTreeItem = (TreeViewItem)this.CabelsTreeView.SelectedItem;

            if (SelectedTreeItem.Header.ToString().Contains("Cable.Footage"))
            {

            }
            else
            {
                //Get CurrentNode

                Guid G = new Guid(SelectedTreeItem.Tag.ToString());
                EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                //Guid Id_NA = new Guid();
                if (EMO.IsInstanceOf(classModule))
                {
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classModule);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);

                }
                else if (EMO.IsInstanceOf(classNodePort))
                {
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        EMO = relobj.SourceObject;
                    }
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classNode);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                {
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        EMO = relobj.SourceObject;
                    }
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classDevice);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else if (EMO.IsInstanceOf(classComputerNetworkAdapter))
                {
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        EMO = relobj.SourceObject;
                    }
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classWindowsComputer);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else if (EMO.IsInstanceOf(classPatchPanelPort))
                {
                    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    {
                        EMO = relobj.SourceObject;
                    }
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classPatchPanel);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else
                {
                    System.Windows.MessageBox.Show("Unknown object!");
                }
            }


        }
    }
}
