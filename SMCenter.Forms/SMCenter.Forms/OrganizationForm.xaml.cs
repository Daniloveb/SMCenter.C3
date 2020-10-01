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
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Security;
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.FormsInfra;

namespace SMCenter.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OrganizationForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPackClass organizationClass;
        ManagementPackClass UserClass;
        ManagementPackRelationship relationClass;
        ManagementPackRelationship userrelationClass;
        //public static Guid guidEntityStatus = new Guid("b7efc970-9c06-798a-73b4-bef30633dae1");
        //public static Guid guidorganizationType = new Guid("247A7668-980C-7048-82A9-47B634275223");
        public static Guid guidOrganizationType = new Guid("6309E172-4724-CDDD-9CA3-5E6192889ED1");
        public static Guid guidAssetStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");
        ObservableCollection<EnterpriseManagementObject> ListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        ObservableCollection<EnterpriseManagementObject> toAddСollection = new ObservableCollection<EnterpriseManagementObject>();
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;
        public OrganizationForm()
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
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                GetSession();
                ManagementPack mp = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());
                organizationClass = emg.EntityTypes.GetClass("SMCenter.Organization", mp);
                UserClass = emg.EntityTypes.GetClass("System.Domain.User", mpSystem);
                relationClass = emg.EntityTypes.GetRelationshipClass("SMCenter.OrganizationContainsChildOrganization", mp);
                userrelationClass = emg.EntityTypes.GetRelationshipClass("SMCenter.UserHasOrganization", mp);
                FillTreeView.Now(emg, organizationClass, relationClass, OrganizationTreeView);

                this.AddHandler(FormEvents.SubmittedEvent, new EventHandler<FormCommandExecutedEventArgs>(this.OnSubmitted));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "UserControl_Loaded Error : " + ex.Message);
            }
        }

        private void OnSubmitted(object sender, FormCommandExecutedEventArgs e)
        {
            try
            {
                CurrentDataItem = this.DataContext as IDataItem;
                CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                //Связываем добавленные User Objects
                Trace.WriteLine(DateTime.Now + " : " + "toAddCollection.Count " + toAddСollection.Count.ToString());
                if (toAddСollection.Count > 0)
                {
                    foreach (EnterpriseManagementObject User_EMO in toAddСollection)
                    {
                        CreatableEnterpriseManagementRelationshipObject userrelation = new CreatableEnterpriseManagementRelationshipObject(emg, userrelationClass);
                        userrelation.SetSource(User_EMO);
                        userrelation.SetTarget(CurrentEMO);
                        userrelation.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in OnSubmitted() " + exc.Message);
            }
        }

        private void FillListView()
        {
            try
            {
                ListView.ItemsSource = null;
                ListView.Items.Clear();
                ListViewСollection.Clear();
                //ObservableCollection<EnterpriseManagementObject> RelEMOcol = new ObservableCollection<EnterpriseManagementObject>();
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>((Guid)CurrentEMO.Id, userrelationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                {
                    this.ListViewСollection.Add(obj.SourceObject);
                }
                Trace.WriteLine(DateTime.Now + " : " + "Count in ListViewcollection " + ListViewСollection.Count());
                this.ListView.ItemsSource = ListViewСollection;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in FillListView() " + exc.Message);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (this.DataContext != null && this.DataContext is IDataItem)
                {
                    CurrentDataItem = this.DataContext as IDataItem;
                    if (!((bool)CurrentDataItem["$IsNew$"]))
                    {
                        CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                        this.FillListView();
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

        private void brd_User_Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Guid UserClassGuid = UserClass.Id;
                Collection<IDataItem> ItemsCol = new Collection<IDataItem>();
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.LaunchAddInstancePickerDialog(ItemsCol, UserClassGuid);
                if (ItemsCol.Count() > 0)
                {
                    foreach (IDataItem I in ItemsCol)
                    {
                        Guid Object_Id = (Guid)I["$Id$"];
                        EnterpriseManagementObject Object_EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(Object_Id, ObjectQueryOptions.Default);
                        ListViewСollection.Add(Object_EMO);
                        this.toAddСollection.Add(Object_EMO);
                    }
                    this.ListView.ItemsSource = ListViewСollection;
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in brd_User_Add_MouseDown() " + exc.Message);
            }
        }

        private void brd_User_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ListView.SelectedItem != null)
            {
                EnterpriseManagementObject SelectEMO = (EnterpriseManagementObject)ListView.SelectedItem;
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(UserClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(SelectEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select object!", "No object selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_User_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EnterpriseManagementObject SelectedEMO = (EnterpriseManagementObject)ListView.SelectedItem;
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete RelationShip?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {
                    if ((bool)CurrentDataItem["$IsNew$"])
                    {
                        ListViewСollection.Remove(SelectedEMO);
                    }
                    else
                    {
                        if (toAddСollection.Contains(SelectedEMO))
                        {
                            //NotExisting Relationship - Remove only from Collections
                            ListViewСollection.Remove(SelectedEMO);
                            toAddСollection.Remove(SelectedEMO);
                        }
                        else
                        {
                            //Remove existing Relationship
                            Guid G = (Guid)SelectedEMO.Id;
                            IncrementalDiscoveryData instance = new IncrementalDiscoveryData();
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, userrelationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                            {
                                Trace.WriteLine(DateTime.Now + " : " + "Remove Relationship " + obj.TargetObject.DisplayName);
                                instance.Remove(obj);
                                instance.Commit(emg);
                                ListViewСollection.Remove(SelectedEMO);
                            }
                        }
                    }
                    this.ListView.ItemsSource = ListViewСollection;
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Remove Relationship void " + exc.Message);
            }
        }

        private void brd_Org_Create_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OrganizationTreeView.SelectedItem != null)
            {
                CreatableEnterpriseManagementObject newOrganization = new CreatableEnterpriseManagementObject(emg, organizationClass);

                //Add some property values
                newOrganization[organizationClass, "DisplayName"].Value = "Add some info";
                newOrganization.Commit();

                //Treeview Item - Parent Item
                TreeViewItem TR = (TreeViewItem)OrganizationTreeView.SelectedItem;
                // Get an object by GUID
                Guid G = new Guid(TR.Tag.ToString());
                EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                CreatableEnterpriseManagementRelationshipObject relationship = new CreatableEnterpriseManagementRelationshipObject(emg, relationClass);
                relationship.SetSource(treeviewEMO);
                relationship.SetTarget(newOrganization);
                relationship.Commit();

                FillTreeView.Now(emg, organizationClass, relationClass, OrganizationTreeView);

                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(organizationClass);
                IDataItem newOrganizationDataItem = dataType.CreateProxyInstance(newOrganization);

                //Open Console form for created object
                //ConsoleContextHelper.Instance.PopoutForm(itemIdentity);
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(newOrganizationDataItem);
            }
            else
            {
                MessageBox.Show("Please select parent organization!", "No organization selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                Trace.WriteLine(DateTime.Now + " : " + "Not have Values for create Rel");
            }  
        }

        private void brd_Org_Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (OrganizationTreeView.SelectedItem != null)
                {
                    Guid guid = organizationClass.Id;
                    Collection<IDataItem> ItemsCol = new Collection<IDataItem>();
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.LaunchAddInstancePickerDialog(ItemsCol, guid);
                    if (ItemsCol.Count() == 1)
                    {
                        TreeViewItem TR = (TreeViewItem)OrganizationTreeView.SelectedItem;
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

                        FillTreeView.Now(emg, organizationClass, relationClass, OrganizationTreeView);
                    }
                    else if (ItemsCol.Count() > 1)
                    {
                        MessageBox.Show("Please select only ONE organization!", "Multiselection!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (ItemsCol.Count() == 0)
                    {
                        MessageBox.Show("Please select organization!", "No organization selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please select organization!", "No organization selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Trace.WriteLine(DateTime.Now + " : " + "Not have Values for create Rel");
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Picker Dialog " + exc.Message);
            }
        }

        private void brd_Org_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OrganizationTreeView.SelectedItem != null)
            {
                TreeViewItem TR = (TreeViewItem)OrganizationTreeView.SelectedItem;
                // Get an object by GUID
                Guid G = new Guid(TR.Tag.ToString());
                EnterpriseManagementObject treeviewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(organizationClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(treeviewEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select organization!", "No organization selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_Org_DeleteLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine(DateTime.Now + " : " + "Enter Delete void");
            try
            {
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete RelationShip?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {
                    TreeViewItem TR = (TreeViewItem)OrganizationTreeView.SelectedItem;
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
            FillTreeView.Now(emg, organizationClass, relationClass, OrganizationTreeView);
        }

        private void brd_Org_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OrganizationTreeView.SelectedItem != null)
            {
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete Organization?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {

                }
            }
            else
            {
                MessageBox.Show("Please select organization!", "No organization selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_Org_Refresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FillTreeView.Now(emg, organizationClass, relationClass, OrganizationTreeView);
        }



        
    }
    }

