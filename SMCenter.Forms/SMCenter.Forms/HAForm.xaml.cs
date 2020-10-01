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
using Microsoft.EnterpriseManagement.UI.Controls;
using Microsoft.EnterpriseManagement.UI.Controls.Internal;
using Microsoft.EnterpriseManagement.UI.Extensions;
using Microsoft.EnterpriseManagement.UI.WpfControls;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using System.ComponentModel;
using System.Windows.Markup;
using Microsoft.EnterpriseManagement.UI.FormsInfra;

namespace SMCenter.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class HAForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPackClass HAClass;
        ManagementPackClass ConfItemClass;
        ManagementPackClass CatalogItemClass;
        ManagementPackClass LocationClass;
        ManagementPackRelationship relationLocationClass;
        ManagementPackRelationship relationHAtoCIClass;
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;
        //EnterpriseManagementObject CurrentModelEMO;
        String strModel;

        ObservableCollection<EnterpriseManagementObject> CIListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        ObservableCollection<EnterpriseManagementObject> CItoAddСollection= new ObservableCollection<EnterpriseManagementObject>();
        public static Guid guidLifecycleStatus = new Guid("7159356A-9C44-37FD-FB58-70AB9FFF5EB8");
        public static Guid guidHardwareAssetType = new Guid("4676B5CF-14D6-97BD-5DB3-DCDD46E0440F");
        public static Guid ConfigItemClassGuid = new Guid("62f0be9f-ecea-e73c-f00d-3dd78a7422fc");// Guid System.ConfigItem
        public static Guid guidAssetStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");
        

        private RelatedItemsPane _relatedItemsPane;

        public HAForm()
        {
            try
            {
                InitializeComponent();
                _relatedItemsPane = new RelatedItemsPane(new ConfigItemRelatedItemsConfiguration());
                tabItemRelItems.Content = _relatedItemsPane;

                //Define Template
                ProjectionObjectTemplate TemplateCatalogItem = new ProjectionObjectTemplate();
                ProjectionObjectTemplate TemplateWarranty = new ProjectionObjectTemplate();
                ProjectionObjectTemplate TemplateSupportContract = new ProjectionObjectTemplate();
                ProjectionObjectTemplate TemplateOrganization = new ProjectionObjectTemplate();
                ProjectionLocationTemplate TemplateLocation = new ProjectionLocationTemplate();


                SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Initialied Error : " + e.Message);
            }
        }

        void Location_In(bool _Ok, EnterpriseManagementObject _LocationEMO, ManagementPackClass _mpClass)
        {
            try
            {
                //Trace.WriteLine(DateTime.Now + " : " + "Enter LocIN()");
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(_mpClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(_LocationEMO);
                this.TemplateLocationItem.sip.Instance = itemIdentity;
                strModel = itemIdentity["$DisplayName$"].ToString();
            }
            catch (Exception e)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in LocIN() : " + e.Message);
            }
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
                Trace.WriteLine(DateTime.Now + " : " + "GetSession Error : " + e.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string userName = (System.Security.Principal.WindowsIdentity.GetCurrent().Name).Replace("\\", "_");
                //string strlog = @"c:\Temp\SMCLog_" + userName + ".txt";
                //TextWriterTraceListener TL = new TextWriterTraceListener(strlog);
                //Trace.Listeners.Add(TL);
                //Trace.AutoFlush = true;
                GetSession();

                ManagementPack mpCore = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                ManagementPack mpHA = emg.GetManagementPack("SMCenter.HardwareAssetManagement.Library", "75b45bd6835084b1", new Version());
                ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());

                HAClass = emg.EntityTypes.GetClass("SMCenter.HardwareAsset", mpHA);
                ConfItemClass = emg.EntityTypes.GetClass("System.ConfigItem", mpSystem);
                LocationClass = emg.EntityTypes.GetClass("SMCenter.Location", mpCore);
                CatalogItemClass = emg.EntityTypes.GetClass("SMCenter.HardwareCatalogItem", mpHA);

                relationHAtoCIClass = emg.EntityTypes.GetRelationshipClass("SMCenter.HardwareAssetReferencesConfigItem", mpHA);
                relationLocationClass = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpCore);

                //Fill Templates
                this.TemplateCatalogItem.PathString = "CatalogItem";
                this.TemplateCatalogItem.mpClass = CatalogItemClass;

                ManagementPackClass WarrantyClass = emg.EntityTypes.GetClass("SMCenter.WarrantyContract", mpCore);
                this.TemplateWarranty.PathString = "Warranty";
                this.TemplateWarranty.mpClass = WarrantyClass;

                ManagementPackClass SupportContractClass = emg.EntityTypes.GetClass("SMCenter.SupportContract", mpCore);
                this.TemplateSupportContract.PathString = "SupportContract";
                this.TemplateSupportContract.mpClass = SupportContractClass;

                ManagementPackClass OrganizationClass = emg.EntityTypes.GetClass("SMCenter.Organization", mpCore);
                this.TemplateOrganization.PathString = "Organization";
                this.TemplateOrganization.mpClass = OrganizationClass;

                this.TemplateLocationItem.emg = emg;
                this.TemplateLocationItem.PathString="Location";
                this.TemplateLocationItem.mpClass = LocationClass;
                this.TemplateLocationItem.mpRelationshipClass = relationLocationClass;
                this.TemplateLocationItem.criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", LocationClass);

                ManagementPackClass CostCenterClass = emg.EntityTypes.GetClass("SMCenter.CostCenter", mpCore);
                this.TemplateCostCenter.PathString = "CostCenter";
                this.TemplateCostCenter.mpClass = CostCenterClass;

                ManagementPackClass LeaseClass = emg.EntityTypes.GetClass("SMCenter.LeaseContract", mpCore);
                this.TemplateLease.PathString = "Lease";
                this.TemplateLease.mpClass = LeaseClass;

                ManagementPackClass SupplierClass = emg.EntityTypes.GetClass("SMCenter.Company", mpCore);
                this.TemplateSupplier.PathString = "Supplier";
                this.TemplateSupplier.mpClass = SupplierClass;

                //Handlers
                this.AddHandler(FormEvents.SubmittedEvent, new EventHandler<FormCommandExecutedEventArgs>(this.OnSubmitted));
                this.AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnPreviewSubmit));
                //this.AddHandler(FormEvents.SubmittedEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnSubmitted));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now + " : " + "UserControl_Loaded Error : " + ex.Message);
            }

        }

        private void lbOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ListViewCI.SelectedItem != null)
            {
                //ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());
                //ConfItemClass = emg.EntityTypes.GetClass("System.ConfigItem", mpSystem);
                //ListViewItem LVItem = (ListViewItem)ListViewCI.SelectedItem;
                // Get an object by GUID
                //Guid G = new Guid(LVItem.Tag.ToString());
                //Guid G = (Guid)((EnterpriseManagementObject)ListViewCI.SelectedItem).Id;
                EnterpriseManagementObject ListViewEMO = (EnterpriseManagementObject)ListViewCI.SelectedItem;
                //EnterpriseManagementObject ListViewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(ConfItemClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(ListViewEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select Configuration Item!", "No Configuration Item selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FillListViewCI()
        {
            try
            {
                ListViewCI.ItemsSource = null;
                ListViewCI.Items.Clear();
                CIListViewСollection.Clear();
                //ObservableCollection<EnterpriseManagementObject> RelEMOcol = new ObservableCollection<EnterpriseManagementObject>();
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>((Guid)CurrentEMO.Id, relationHAtoCIClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                {
                    this.CIListViewСollection.Add(obj.TargetObject);
                }
                Trace.WriteLine(DateTime.Now + " : " + "Count in CIListViewcollection " + CIListViewСollection.Count());
                this.ListViewCI.ItemsSource = CIListViewСollection;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in FillListView() " + exc.Message);
            }
        }

        //После создания HA - привязываем к нему CI из коллекции CItoAddCollection
        private void OnSubmitted(object sender, FormCommandExecutedEventArgs e)
        {
            try
            {
                CurrentDataItem = this.DataContext as IDataItem;
                CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                //Связываем добавленные CI 
                if (CItoAddСollection.Count > 0)
                {
                    foreach (EnterpriseManagementObject CI_EMO in CItoAddСollection)
                    {
                        CreatableEnterpriseManagementRelationshipObject relationHAtoCI = new CreatableEnterpriseManagementRelationshipObject(emg, relationHAtoCIClass);
                        relationHAtoCI.SetSource(CurrentEMO);
                        relationHAtoCI.SetTarget(CI_EMO);
                        relationHAtoCI.Commit();
                    }
                }

                


                //CurrentEMO.DisplayName = CurrentEMO[null,"AssetKey"].Value.ToString() + this.TemplateCatalogItem.sip.Instance["MakeModel"].ToString();
                //CurrentEMO.Commit();


                //Link Model
                //ManagementPack mpHA = emg.GetManagementPack("SMCenter.HardwareAssetManagement.Library", "75b45bd6835084b1", new Version());
                //HAClass = emg.EntityTypes.GetClass("SMCenter.HardwareAsset", mpHA);
                //ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());
                //ConfItemClass = emg.EntityTypes.GetClass("System.ConfigItem", mpSystem);
                //ManagementPackRelationship relationHAtoModelClass = emg.EntityTypes.GetRelationshipClass("SMCenter.HardwareAssetIsHardwareCatalogItem", mpHA);
                //CreatableEnterpriseManagementRelationshipObject relationHAtoModel = new CreatableEnterpriseManagementRelationshipObject(emg, relationHAtoModelClass);
                //Trace.WriteLine(DateTime.Now + " : " + "CurrentEmo " + CurrentEMO.FullName);
                //Trace.WriteLine(DateTime.Now + " : " + "CurrentModelEmo " + CurrentModelEMO.FullName);
                //relationHAtoModel.SetSource(CurrentEMO);
                //relationHAtoModel.SetTarget(CurrentModelEMO);
                //relationHAtoModel.Commit();
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in OnSubmitted() " + exc.Message);
            }
        }
        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            try
            {
                //DisplayName
                String strDisplayName = CurrentDataItem["AssetKey"].ToString() + " – " + this.TemplateCatalogItem.sip.Instance["MakeModel"].ToString();
                CurrentDataItem["DisplayName"] = strDisplayName;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in OnPreviewSubmit() " + exc.Message);
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
                        this.FillListViewCI();
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

        private void txtAssetKey_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            this.txtDisplayName.Text = this.txtAssetKey.Text + "-" + strModel;
        }

        private void brd_CI_Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Guid ConfigItemClassGuid = new Guid("62f0be9f-ecea-e73c-f00d-3dd78a7422fc");// Guid System.ConfigItem
                Collection<IDataItem> ItemsCol = new Collection<IDataItem>();
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.LaunchAddInstancePickerDialog(ItemsCol, ConfigItemClassGuid);
                if (ItemsCol.Count() > 0)
                {
                    foreach (IDataItem I in ItemsCol)
                    {
                        Guid CI_Id = (Guid)I["$Id$"];
                        EnterpriseManagementObject CI_EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CI_Id, ObjectQueryOptions.Default);
                        CIListViewСollection.Add(CI_EMO);
                        this.CItoAddСollection.Add(CI_EMO);
                    }
                    this.ListViewCI.ItemsSource = CIListViewСollection;
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in lbHACIAdd_MouseDown " + exc.Message);
            }
        }

        private void brd_CI_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ListViewCI.SelectedItem != null)
            {
                EnterpriseManagementObject SelectEMO = (EnterpriseManagementObject)ListViewCI.SelectedItem;
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(ConfItemClass);
                IDataItem itemIdentity = dataType.CreateProxyInstance(SelectEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select object!", "No object selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_CI_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
            EnterpriseManagementObject CISelected = (EnterpriseManagementObject)ListViewCI.SelectedItem;
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete RelationShip?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {
                    if ((bool)CurrentDataItem["$IsNew$"])
                    {
                        CIListViewСollection.Remove(CISelected);
                    }
                    else
                    {
                        if (CItoAddСollection.Contains(CISelected))
                        {
                            //NotExisting Relationship - Remove only from Collections
                            CIListViewСollection.Remove(CISelected);
                            CItoAddСollection.Remove(CISelected);
                        }
                        else
                        {
                            //Remove existing Relationship
                            Guid G = (Guid)CISelected.Id;
                            IncrementalDiscoveryData instance = new IncrementalDiscoveryData();
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relationHAtoCIClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                            {
                                Trace.WriteLine(DateTime.Now + " : " + "Remove Relationship " + obj.TargetObject.DisplayName);
                                instance.Remove(obj);
                                instance.Commit(emg);

                                this.CItoAddСollection.Remove(CISelected);
                            }
                        }
                    }
                    FillListViewCI();
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Remove Relationship void " + exc.Message);
            }
        }


        
    }
    }

