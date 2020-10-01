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
using Microsoft.EnterpriseManagement.ConnectorFramework;

namespace SMCenter.Forms
{
    /// <summary>
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class SCSForm : Window
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
        ManagementPackClass classRack;
        ManagementPackClass classDeviceNetworkAdapter;
        ManagementPackClass classPatchPanel;
        //ManagementPackClass classPatchPanelPort;
        ManagementPackClass classNAPort;
        

        ManagementPackRelationship relComputerRunsWindowsComputer;
        ManagementPackRelationship relComputerHostsComputerNetworkAdapter;
        ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter;
        ManagementPackRelationship relLoctoLoc;
        //ManagementPackRelationship relLocationContainsConfigItem;
        ManagementPackRelationship relConfigItemRefLocation;
        ManagementPackRelationship relNodeComposedOfNetworkAdapter;
        ManagementPackRelationship relDeviceHostNetworkAdapter;
        ManagementPackRelationship relConfigItemRefRack;

        EnterpriseManagementObject emo_ClientNA;
        EnterpriseManagementObject emo_Node;
        EnterpriseManagementObject emo_NodePort;
        EnterpriseManagementObject emo_Rack1;
        EnterpriseManagementObject emo_Rack2;
        EnterpriseManagementObject emo_PP1;
        EnterpriseManagementObject emo_PP2;
        EnterpriseManagementObject emo_PPP1;
        EnterpriseManagementObject emo_PPP2;
        EnterpriseManagementObject emo_Module;
        EnterpriseManagementObject emo_Location;

        EnterpriseManagementRelationshipObject<EnterpriseManagementObject> Current_Cable_RelationShip;

        CablesTree CablesTree;

        LogFile LogFile;

        //public ConnectionsForm(IDataItem Node)
        public SCSForm(EnterpriseManagementObject EMO)
        {
            InitializeComponent();

            //try
            //{
                emg = GetSession();
                if (emg == null)
                {
                    emg = new EnterpriseManagementGroup("SM");
                }

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
                classRack = emg.EntityTypes.GetClass("SMCenter.Rack", mpNetworkAssetLibrary);
                classDeviceNetworkAdapter = emg.EntityTypes.GetClass("SMCenter.Device.NetworkAdapter", mpNetworkAssetLibrary);
                classPatchPanel = emg.EntityTypes.GetClass("SMCenter.PatchPanel", mpNetworkAssetLibrary);
                classNAPort = emg.EntityTypes.GetClass("System.NetworkManagement.NetworkAdapter", mpNetworkLibrary);
                //classPatchPanelPort = emg.EntityTypes.GetClass("SMCenter.PatchPanelPort", mpNetworkAssetLibrary);
                relComputerRunsWindowsComputer = emg.EntityTypes.GetRelationshipClass("Microsoft.SystemCenter.ConfigurationManager.DeployedComputerRunsWindowsComputer", mpCMLibrary);
                relComputerHostsComputerNetworkAdapter = emg.EntityTypes.GetRelationshipClass("Microsoft.Windows.ComputerHostsComputerNetworkAdapter", mpWindows);
                relLoctoLoc = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpAssetCore);
                //relNetworkAdapterHasChildNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkAdapterHasChildNetworkAdapter", mpNetworkAssetLibrary);
                relChildNetworkAdapterRefParentNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.ChildNetworkAdapterRefParentNetworkAdapter", mpNetworkAssetLibrary);
                //relLocationContainsConfigItem = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsConfigItem", mpAssetCore);
                relConfigItemRefLocation = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefLocation", mpAssetCore);
                relNodeComposedOfNetworkAdapter = emg.EntityTypes.GetRelationshipClass("System.NetworkManagement.NodeComposedOfNetworkAdapter", mpNetworkLibrary);
                relDeviceHostNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.DeviceHostsNetworkAdapter", mpNetworkAssetLibrary);
                relConfigItemRefRack = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefRack", mpNetworkAssetLibrary);

                emo_Node = null;
                emo_NodePort = null;
                emo_Rack1 = null;
                emo_Rack2 = null;
                emo_PP1 = null;
                emo_PP2 = null;
                emo_PPP1 = null;
                emo_PPP2 = null;
                emo_Module = null;
                emo_Location = null;

                SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);
                this.CabelsTreeView.Items.Clear();
                TreeViewItem treeitem = new TreeViewItem();

                LogFile = new LogFile(@"C:\LogFile.txt", true);

                CablesTree = new CablesTree(emg, relChildNetworkAdapterRefParentNetworkAdapter, classModule, relConfigItemRefLocation, LogFile);
                CablesTree.SetClasses(classModule, classNAPort, classNodePort,classDeviceNetworkAdapter, classComputerNetworkAdapter);

                IDataItem Node = null;
                Guid CurrentNodeId;
                //Get CurrentNode if ConsoleMode
                if (Node != null)
                {
                    CurrentNodeId = (Guid)Node["$Id$"];
                    EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                }
                //Заполняем переменные
                #region
                CurrentNodeId = EMO.Id;
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

                        //treeitem = ConnectionsTree.Create(Id_NA, emg);
                        //if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
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
                            this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(Id_NA));
                            // treeitem = ConnectionsTree.Create(Id_NA, emg);
                            // if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                        }
                        #endregion
                    }
                    //else if (EMO.IsInstanceOf(classNode)) //?? не может быть
                    //{
                       #region
                    //    //ManagementPackEnumeration StatusType = SRLibraryMP.GetEnumeration("ServiceRequestStatusEnum.New");
                    //    //emop.Object[null, "Status"].Value = StatusType;

                    //    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    //    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    //    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    //    {
                    //        GuidCol.Add(relobj.TargetObject.Id);
                    //    }
                    //    foreach (Guid id in GuidCol)
                    //    {
                    //        this.CabelsTreeView.Items.Add(CablesTree.Go(id));
                    //        // treeitem = ConnectionsTree.Create(id, emg);
                    //        //if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    //    }
                     #endregion
                    //}
                    else if (EMO.IsInstanceOf(classNodePort))
                    {
                        this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(CurrentNodeId));
                        //treeitem = ConnectionsTree.Create(CurrentNodeId, emg);
                        //if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    }
                    else if (EMO.IsInstanceOf(classDevice))
                    {
                        #region
                        var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentNodeId, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                        {
                            Id_NA = relobj.TargetObject.Id;
                        }
                        this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(Id_NA));
                        // treeitem = ConnectionsTree.Create(Id_NA, emg);
                        //if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                        #endregion
                    }
                    else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                    {
                        this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(CurrentNodeId));
                        //treeitem = ConnectionsTree.Create(CurrentNodeId, emg);
                        // if (treeitem != null) { this.ConnectionsTreeView.Items.Add(treeitem); }
                    }
                    else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                    {
                        this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(CurrentNodeId));
                    }
                #endregion
            //======================================================================================
            //Get objects from CableTree
            //======================================================================================
            #region
                    if (CablesTree.emo_NodePort == null)
                    {
                        return;
                    }
                    //emo_Rack=CablesTree.
                    emo_NodePort = CablesTree.emo_NodePort;
                    emo_PPP1 = CablesTree.emo_PPP1;
                    emo_PPP2 = CablesTree.emo_PPP2;
                    emo_Node = _Rel.GetObjectFromSingleRelationship(emg, relNodeComposedOfNetworkAdapter, emo_NodePort.Id, false);
                    
                    if (emo_PPP1 != null)
                    {
                        emo_PP1 = _Rel.GetObjectFromSingleRelationship(emg, relNodeComposedOfNetworkAdapter, emo_PPP1.Id, false);
                        emo_Rack1 = _Rel.GetObjectFromSingleRelationship(emg, relConfigItemRefRack, emo_PP1.Id, true);
                    }
                    if (emo_PPP2 != null)
                    {
                        emo_PP2 = _Rel.GetObjectFromSingleRelationship(emg, relNodeComposedOfNetworkAdapter, emo_PPP1.Id, false);
                        emo_Rack2 = _Rel.GetObjectFromSingleRelationship(emg, relConfigItemRefRack, emo_PP2.Id, true);
                    }

                    emo_Module = CablesTree.emo_Module;
                    emo_ClientNA = CablesTree.emo_ClientNA;
       
        #endregion
             //======================================================================================
             //Fill Form
             //======================================================================================
         #region
            
            //Switch
            //===================================================
            //this.txtSwitch.Text = emo_Node.FullName;
            //this.txtSwitchPort.Text = emo_NodePort[classNodePort, "Index"].Value.ToString();
            
            //Rack
            IObjectReader<EnterpriseManagementObject> RackIObjectReader = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(classRack, ObjectQueryOptions.Default);
            List<EnterpriseManagementObject> RackList = new List<EnterpriseManagementObject>();
            foreach (EnterpriseManagementObject E in RackIObjectReader) { RackList.Add(E); }
            //Сортировка
            //var sortedRack = from item in Col orderby item.Header select item;
            var sortedRack = from item in RackList orderby item.Name select item;
            cboxRack1.ItemsSource = sortedRack;
            cboxRack1.DisplayMemberPath = "Name";
            cboxRack1.SelectedValuePath = "Id";
            if (emo_Rack1 != null)
            {
                cboxRack1.SelectedValue = emo_Rack1.Id;
            }

            cboxRack2.ItemsSource = sortedRack;
            cboxRack2.DisplayMemberPath = "Name";
            cboxRack2.SelectedValuePath = "Id";
            if (emo_Rack2 != null)
            {
                cboxRack2.SelectedValue = emo_Rack2.Id;
            }

            //PatchPanel - Заполняем только те ПП - связанные с текущим Rack
            //===================================================
            
            if (emo_Rack1 != null)
            {
                List<EnterpriseManagementObject> PPList1 = _Rel.GetCollection(emg, relConfigItemRefRack, classPatchPanel, emo_Rack1.Id, false);
                var sortedPPList1 = from item in PPList1 orderby item.Name select item;
                cboxPP1.ItemsSource = sortedPPList1;
                cboxPP1.DisplayMemberPath = "Name";
                cboxPP1.SelectedValuePath = "Id";
                if (emo_PP1 != null)
                {
                    cboxPP1.SelectedValue = emo_PP1.Id;
                }
            }
            if (emo_Rack2 != null)
            {
                List<EnterpriseManagementObject> PPList2 = _Rel.GetCollection(emg, relConfigItemRefRack, classPatchPanel, emo_Rack2.Id, false);
                var sortedPPList2 = from item in PPList2 orderby item.Name select item;
                cboxPP2.ItemsSource = sortedPPList2;
                cboxPP2.DisplayMemberPath = "Name";
                cboxPP2.SelectedValuePath = "Id";
                if (emo_PP2 != null)
                {
                    cboxPP2.SelectedValue = emo_PP2.Id;
                }
            }

            //PPPorts
            
            if (emo_PP1 != null)
            {
                List<EnterpriseManagementObject> PPPList1 = _Rel.GetCollection(emg, relNodeComposedOfNetworkAdapter, classNAPort, emo_PP1.Id, true);
                var sortedPPPList1 = from item in PPPList1 orderby item.Name select item;
                cboxPPP1.ItemsSource = sortedPPPList1;
                cboxPPP1.DisplayMemberPath = "DisplayName";
                cboxPPP1.SelectedValuePath = "Id";
                if (emo_PP1 != null)
                {
                    cboxPP1.SelectedValue = emo_PP1.Id;
                }
            }
            if (emo_PP2 != null)
            {
                List<EnterpriseManagementObject> PPPList2 = _Rel.GetCollection(emg, relNodeComposedOfNetworkAdapter, classNAPort, emo_PP2.Id, true);
                var sortedPPPList2 = from item in PPPList2 orderby item.Name select item;
                cboxPP2.ItemsSource = sortedPPPList2;
                cboxPP2.DisplayMemberPath = "DisplayName";
                cboxPP2.SelectedValuePath = "Id";
                if (emo_PP2 != null)
                {
                    cboxPP2.SelectedValue = emo_PP2.Id;
                }
            }
            //Модуль
            if (emo_Module != null)
            {
                emo_Location = _Rel.GetObjectFromSingleRelationship(emg, relConfigItemRefLocation, emo_Module.Id, true);
                this.lbLocation.Content = emo_Location.Name;
                //IObjectReader<EnterpriseManagementObject> ModuleIObjectReader = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(classModule, ObjectQueryOptions.Default);
                //List<EnterpriseManagementObject> ModuleList = new List<EnterpriseManagementObject>();
                //foreach (EnterpriseManagementObject E in ModuleIObjectReader) { ModuleList.Add(E); }
                List<EnterpriseManagementObject> ModuleList = _Rel.GetCollection(emg, relConfigItemRefLocation, classModule, emo_Location.Id, false);
                var sortedModuleList = from item in ModuleList orderby item.Name select item;
                cboxModule.ItemsSource = sortedModuleList;
                cboxModule.DisplayMemberPath = "Name";
                cboxModule.SelectedValuePath = "Id";
                cboxModule.SelectedValue = emo_Module.Id;
            }

         #endregion


            //backups
            #region
            //E54C4A48-EB81-1425-FF98-D5F51698477A 
            //Guid G = new Guid("E54C4A48-EB81-1425-FF98-D5F51698477A");
            //cboxPP.SelectedValue = G;
            //cboxPP.SelectedValue = emo_PP.Id;
            //Module

            //Linked Items
            ObservableCollection<EnterpriseManagementObject> LinkedItemsCol = new ObservableCollection<EnterpriseManagementObject>();


            //ObservableCollection<System.Net.Mail.Attachment> AttachmentsCol = new ObservableCollection<System.Net.Mail.Attachment>();
            //if (this.FilesListView.ItemsSource != null)
            //{
            //    AttachmentsCol = (ObservableCollection<System.Net.Mail.Attachment>)this.FilesListView.ItemsSource;
            //}
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.Multiselect = false;
            //Nullable<bool> result = dlg.ShowDialog();
            //if (result == true)
            //{
            //    string filename = dlg.FileName;
            //    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(filename);
            //    attachment.Name = filename;
            //    AttachmentsCol.Add(attachment);
            //}
            //this.FilesListView.ItemsSource = AttachmentsCol;
         


            //Надо сделать форму для ПП для связи с Шкафом. Вообще придумать как, но все ПП должны быть связаны со шкафами.

                    //this.cboxPP.ItemsSource = PPlist;
                    //this.cboxPP.DisplayMemberPath = "FullName";

                //ItemsSource="{Binding Path=CountrySource, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type Window}}}" 
                                //DisplayMemberPath="CountryName" 
                                //SelectedValuePath="CountryID"
                                //SelectedValue="{Binding Country.CountryID}"
            //SelectedItem="{Binding Path=Country, Mode=TwoWay}"/>
            #endregion

            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    this.Close();
            //}
        }

        private void ConnectionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //FillLocations();
            //FillNodes();

            if (CablesTree.emo_NodePort == null)
            {
                System.Windows.MessageBox.Show("Node Port not connected!");
                this.Close();
                return;
            }
        }

        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                //if (curSession == null)
                  //  throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch //(Exception e)
            {
                //System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
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
                //this.txtTargetObject.Text = SelectedItem.Header.ToString();

                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
                Guid Guid_loc = new Guid(SelectedItem.Tag.ToString());
                //this.ConnectionsTreeView.Items.Clear();
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

        

        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CabelsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;
            if (SelectedItem != null)
            {
                if (SelectedItem.Header.ToString().Contains("Cable.Footage"))
                {
                    this.txtF1.IsEnabled = true;
                    this.txtF2.IsEnabled = true;
                }
                else
                {
                    this.txtF1.IsEnabled = false;
                    this.txtF2.IsEnabled = false;
                }

                //this.txtLocation.Text = "";
                
                if (SelectedItem.Header.ToString().Contains("Cable"))
                {
                    Guid Guid = new Guid(SelectedItem.Tag.ToString());
                    Current_Cable_RelationShip = emg.EntityObjects.GetRelationshipObject<EnterpriseManagementObject>(Guid, ObjectQueryOptions.Default);
                    if (Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value != null)
                    {
                        this.txtF1.Text = Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value.ToString();
                    }
                    if (Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ChildFootage"].Value != null)
                    {
                        this.txtF2.Text = Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ChildFootage"].Value.ToString();
                    }
                  
                }
            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CabelsTreeView_SelectedItemChanged procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void CabelsTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
            EnterpriseManagementObject EMO;
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
                else if (EMO.IsInstanceOf(classNAPort))
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
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("CabelsTreeView_MouseDoubleClick procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void lbLocation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);
                TreeViewForm TWindow = new TreeViewForm(emg, classLocation, relLoctoLoc, criteria);
                TWindow.ShowDialog(); 
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("lbLocation_MouseDown procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtF1_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string Val = this.txtF1.Text;
                if (Val == "" || Val == null)
                {
                   Val = "0";
                }
                Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value = Val;
                Current_Cable_RelationShip.Commit();
                this.txtF1.Text = "";
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("txtF1_LostFocus procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtF2_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string Val = this.txtF2.Text;
                if (Val == "" || Val == null)
                {
                    Val = "0";
                }
                Current_Cable_RelationShip[relChildNetworkAdapterRefParentNetworkAdapter, "ChildFootage"].Value = Val;
                Current_Cable_RelationShip.Commit();
                this.txtF2.Text = "";
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("txtF2_LostFocus procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void brd_PPAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
            
            //Добавляем Патчпанели
            if (emo_PPP1 != null)
            {
                MessageBoxResult MSR = MessageBox.Show("Заменяем связи?", "Service Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (MSR == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                MessageBoxResult MSR = MessageBox.Show("Создаем связи?", "Service Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (MSR == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (emo_PPP1 == null && cboxPPP1.SelectedValue != null) emo_PPP1 = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(this.cboxPPP1.SelectedValue.ToString()), ObjectQueryOptions.Default);
                if (emo_PPP2 == null && cboxPPP2.SelectedValue != null) emo_PPP2 = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(this.cboxPPP2.SelectedValue.ToString()), ObjectQueryOptions.Default);
                if (emo_PPP1 == null)
                {
                    MessageBox.Show("Не найдены объекты порты Патч-панели! Прерываем выполнение!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            //Создаем связи
            deleteParentAdapterRelations(emo_NodePort);
            CreatableEnterpriseManagementRelationshipObject relObject1 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
            relObject1.SetSource(emo_PPP1);
            relObject1.SetTarget(emo_NodePort);
            relObject1.Commit();
            deleteParentAdapterRelations(emo_PPP1);
            CreatableEnterpriseManagementRelationshipObject relObject2 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
            relObject2.SetSource(emo_PPP2);
            relObject2.SetTarget(emo_PPP1);
            relObject2.Commit();
            if (emo_Module != null)
            {
                deleteParentAdapterRelations(emo_PPP2);
                CreatableEnterpriseManagementRelationshipObject relObject3 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject3.SetSource(emo_Module);
                relObject3.SetTarget(emo_PPP2);
                relObject3.Commit();
            }
            else
            {
                if (emo_ClientNA != null)
                {
                    deleteParentAdapterRelations(emo_PPP2);
                    CreatableEnterpriseManagementRelationshipObject relObject4 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                    relObject4.SetSource(emo_ClientNA);
                    relObject4.SetTarget(emo_PPP2);
                    relObject4.Commit();
                }
            }
            this.CabelsTreeView.Items.Clear();
            this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(emo_NodePort.Id));
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("brd_PPAdd_MouseDown procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void brd_ModuleAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
            //Добавляем модуль
            MessageBoxResult MSR = MessageBox.Show("Добавляем модуль?", "Service Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (MSR == MessageBoxResult.Cancel)
            {
                return;
            }
            if (cboxModule.SelectedValue == null)
            {
                MessageBox.Show("Не указан модуль?", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (emo_PPP2 == null && cboxPPP2.SelectedValue != null) emo_PPP2 = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(this.cboxPPP2.SelectedValue.ToString()), ObjectQueryOptions.Default);
            EnterpriseManagementObject emo_ModuleNew = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(this.cboxModule.SelectedValue.ToString()), ObjectQueryOptions.Default);

            if (emo_PPP2 == null) //если порта нет - связывем модуль с портом свича
            {
                CabelsTreeView.Items.Clear();
                deleteParentAdapterRelations(emo_NodePort);
                CreatableEnterpriseManagementRelationshipObject relObject1 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject1.SetSource(emo_ModuleNew);
                relObject1.SetTarget(emo_NodePort);
                relObject1.Commit();
                deleteParentAdapterRelations(emo_ModuleNew);
                CreatableEnterpriseManagementRelationshipObject relObject2 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject2.SetSource(emo_ClientNA);
                relObject2.SetTarget(emo_ModuleNew);
                relObject2.Commit();
            }
            else //При наличии порта ПП2 - связываем с ним
            {
                CabelsTreeView.Items.Clear();
                deleteParentAdapterRelations(emo_PPP2);
                CreatableEnterpriseManagementRelationshipObject relObject1 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject1.SetSource(emo_ModuleNew);
                relObject1.SetTarget(emo_PPP2);
                relObject1.Commit();
                deleteParentAdapterRelations(emo_ModuleNew);
                CreatableEnterpriseManagementRelationshipObject relObject2 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject2.SetSource(emo_ClientNA);
                relObject2.SetTarget(emo_ModuleNew);
                relObject2.Commit();
            }
            this.CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(emo_NodePort.Id));

            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("brd_ModuleAdd_MouseDown procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        void deleteParentAdapterRelations(EnterpriseManagementObject ParentAdapter)
        {
            IncrementalDiscoveryData dd = new IncrementalDiscoveryData();
            List<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>> LRO = new List<EnterpriseManagementRelationshipObject<EnterpriseManagementObject>>();

            var relatedObjectsList = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ParentAdapter.Id, relChildNetworkAdapterRefParentNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);

            int n = 0;
            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> O in relatedObjectsList)
            { LRO.Add(O); }

            while (n < LRO.Count)
            {
                dd.Remove(LRO[n]);
                LogFile.Write(String.Format("Удаляем WC {0}", LRO[n].Id), false);
                n++;
            }
            dd.Commit(emg);
        }

        void Location_In(bool Ok, EnterpriseManagementObject _LocationEMO, ManagementPackClass MP)
        {
            try
            {

                emo_Location = _LocationEMO;
                this.lbLocation.Content = emo_Location.Name;
                List<EnterpriseManagementObject> ModuleList = _Rel.GetCollection(emg, relConfigItemRefLocation, classModule, emo_Location.Id, false);
                foreach (EnterpriseManagementObject E in ModuleList) { ModuleList.Add(E); }
                cboxModule.ItemsSource = ModuleList;
                //cboxModule.DisplayMemberPath = "Name";
                //cboxModule.SelectedValuePath = "Id";
                cboxModule.SelectedValue = emo_Module.Id;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in LocIN() : " + e.Message);
            }
        }

        private void cboxRack1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
            //Меняем список ПП
            if (this.cboxRack1.SelectedValue != null)
            {
                Guid id = (Guid)this.cboxRack1.SelectedValue;
                List<EnterpriseManagementObject> PPList1 = _Rel.GetCollection(emg, relConfigItemRefRack, classPatchPanel, id, false);
                var sortedPPList1 = from item in PPList1 orderby item.Name select item;
                cboxPP1.ItemsSource = sortedPPList1;
                cboxPP1.DisplayMemberPath = "Name";
                cboxPP1.SelectedValuePath = "Id";
            }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("cboxRack1_SelectionChanged procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboxRack2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
            //Меняем список ПП
            if (this.cboxRack2.SelectedValue != null)
            {
                Guid id2 = (Guid)this.cboxRack2.SelectedValue;
                List<EnterpriseManagementObject> PPList2 = _Rel.GetCollection(emg, relConfigItemRefRack, classPatchPanel, id2, false);
                var sortedPPList2 = from item in PPList2 orderby item.Name select item;
                cboxPP2.ItemsSource = sortedPPList2;
                cboxPP2.DisplayMemberPath = "Name";
                cboxPP2.SelectedValuePath = "Id";
            }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("cboxRack2_SelectionChanged procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboxPP1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
            //Меняем список ПП портов
            if (cboxPP1.SelectedValue != null)
            {
            Guid id = (Guid)this.cboxPP1.SelectedValue;
            List<EnterpriseManagementObject> PPPList = _Rel.GetCollection(emg, relNodeComposedOfNetworkAdapter, classNAPort, id, true);
            var sortedPPPList = from item in PPPList orderby item.Name select item;
            cboxPPP1.ItemsSource = sortedPPPList;
            cboxPPP1.DisplayMemberPath = "DisplayName";
            cboxPPP1.SelectedValuePath = "Id";
            }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("cboxPP1_SelectionChanged procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboxPP2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
            //Меняем список ПП портов
            if (cboxPP2.SelectedValue != null)
            {
            Guid id2 = (Guid)this.cboxPP2.SelectedValue;
            List<EnterpriseManagementObject> PPPList2 = _Rel.GetCollection(emg, relNodeComposedOfNetworkAdapter, classNAPort, id2, true);
            var sortedPPPList2 = from item in PPPList2 orderby item.Name select item;
            cboxPPP2.ItemsSource = sortedPPPList2;
            cboxPPP2.DisplayMemberPath = "DisplayName";
            cboxPPP2.SelectedValuePath = "Id";
            }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("cboxPP2_SelectionChanged procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void brd_PPCopy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Копируем шкаф и ПП 
            //cboxPP2.ItemsSource = cboxPP1.ItemsSource;
            //cboxPPP2.DisplayMemberPath = "DisplayName";
            //cboxPPP2.SelectedValuePath = "Id";
            //cboxPP2.SelectedValue = cboxPP1.SelectedValue;
            //cboxPPP2.ItemsSource = cboxPPP1.ItemsSource;
            //cboxPPP2.DisplayMemberPath = "DisplayName";
            //cboxPPP2.SelectedValuePath = "Id";
            //if (cboxPPP1.SelectedValue != null)
            //{ cboxPPP2.SelectedValue = cboxPPP1.SelectedValue; }

        }

        private void brd_CI_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Удаляем все связи - затем связываем порт и NA
            if (MessageBox.Show("Удалить все связи?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            if (emo_NodePort != null && emo_ClientNA != null)
            {
                deleteParentAdapterRelations(emo_NodePort);
                CreatableEnterpriseManagementRelationshipObject relObject2 = new CreatableEnterpriseManagementRelationshipObject(emg, relChildNetworkAdapterRefParentNetworkAdapter);
                relObject2.SetSource(emo_ClientNA);
                relObject2.SetTarget(emo_NodePort);
                relObject2.Commit();
            }
            if (emo_Module != null) deleteParentAdapterRelations(emo_Module);
            if (emo_PPP1 != null && emo_PPP2 != null)
            {
                deleteParentAdapterRelations(emo_PPP1);
                deleteParentAdapterRelations(emo_PPP2);
            }
            CabelsTreeView.Items.Clear();
            CabelsTreeView.Items.Add(CablesTree.CreateTreeViewItem(emo_NodePort.Id));
        }
    }
}
