using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
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
    public partial class SoftwareTitleForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPack mpCore;
        ManagementPack mpNA;
        ManagementPack mpSystem;
        ManagementPack mpSoft;
        ManagementPackClass classCompany;
        ManagementPackClass classVersion;
        ManagementPackRelationship relPublisherHasSoftwareTitle;//SMCenter.PublisherHasSoftwareTitle
        ManagementPackRelationship relSoftwareTitleHasSoftwareVersion;//SMCenter.SoftwareTitleHasSoftwareVersion
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;

        ObservableCollection<EnterpriseManagementObject> VersionListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        ObservableCollection<EnterpriseManagementObject> VersiontoAddСollection = new ObservableCollection<EnterpriseManagementObject>();
        public static Guid guidAssetStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");
        public static Guid guidStatus = new Guid("7FEA7861-30C1-8050-2F29-889C7BC7D96B"); //Заменить надо!!!!
        public static Guid guidCategory = new Guid("476a7840-7373-d5d9-9589-1ee0f9078615");
        //public static Guid guidHardwareAssetType =  new Guid("11ed339c-6a30-074a-bfef-50a9e3d102ad");
        //public static Guid ConfigItemClassGuid = new Guid("62f0be9f-ecea-e73c-f00d-3dd78a7422fc");
        public static Guid VersionClassGuid = new Guid("ec378d0d-e819-b08c-cb9c-d0bc3fa9714f");
         
        private RelatedItemsPane _relatedItemsPane;

        //System.Drawing.Image DrawingImage;

        public SoftwareTitleForm()
        {
            try
            {
                InitializeComponent();
                _relatedItemsPane = new RelatedItemsPane(new ConfigItemRelatedItemsConfiguration());
                tabItemRelItems.Content = _relatedItemsPane;

                //Define Template
                //ProjectionLocationTemplate TemplateLocation = new ProjectionLocationTemplate();
                ProjectionObjectTemplate TemplatePublisher = new ProjectionObjectTemplate();

                //SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Initialized Proc Error : " + e.Message);
            }
        }

        //void Location_In(bool _Ok, EnterpriseManagementObject _LocationEMO, ManagementPackClass _mpClass)
        //{
        //    try
        //    {
        //        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(_mpClass);
        //        IDataItem itemIdentity = dataType.CreateProxyInstance(_LocationEMO);
        //        this.TemplatePublisher.sip.Instance = itemIdentity;
        //    }
        //    catch (Exception e)
        //    {
        //        System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in LocIN() : " + e.Message);
        //    }
        //}

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

                mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());
                mpCore = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNA = emg.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                mpSoft = emg.GetManagementPack("SMCenter.SoftwareAssetManagement.Library", "75b45bd6835084b1", new Version());

                classCompany = emg.EntityTypes.GetClass("SMCenter.Company", mpCore);
                classVersion = emg.EntityTypes.GetClass("SMCenter.SoftwareVersion", mpSoft);
                relPublisherHasSoftwareTitle = emg.EntityTypes.GetRelationshipClass("SMCenter.PublisherHasSoftwareTitle", mpSoft);//SMCenter.PublisherHasSoftwareTitle
                relSoftwareTitleHasSoftwareVersion = emg.EntityTypes.GetRelationshipClass("SMCenter.SoftwareTitleHasSoftwareVersion", mpSoft); ;//SMCenter.SoftwareTitleHasSoftwareVersion

                //Fill Templates

                //this.TemplatePublisher.eemg = emg;
                this.TemplatePublisher.PathString = "Publisher";
                this.TemplatePublisher.mpClass = classCompany;
                //this.TemplatePublisher.mpRelationshipClass = relLoctoLoc;
                //this.TemplateLocationItem.criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);


                //////Handlers
                this.AddHandler(FormEvents.SubmittedEvent, new EventHandler<FormCommandExecutedEventArgs>(this.OnSubmitted));
                this.AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnPreviewSubmit));

                FillListViewCI();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "UserControl_Loaded Error : " + ex.Message);
            }

        }


        private void OnSubmitted(object sender, FormCommandExecutedEventArgs e)
        {
            //try
            //{
            //    CurrentDataItem = this.DataContext as IDataItem;

            //    if (!((bool)CurrentDataItem["$IsNew$"]))
            //    {
            //        CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);

                    
            //    }
            //    else
            //    {
                   
            //    }
            //}
            //catch (Exception exc)
            //{
            //    System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in OnSubmitted() " + exc.Message);
            //}


            try
            {
                //CurrentDataItem = this.DataContext as IDataItem;
                //CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                ////Связываем добавленные CI 
                //if (VersiontoAddСollection.Count > 0)
                //{

                //    foreach (EnterpriseManagementObject Ver in VersiontoAddСollection)
                //    {
                //        MessageBox.Show("CurrentEMO " + CurrentEMO.DisplayName);
                //        MessageBox.Show("Ver " + Ver.DisplayName);
                //        CreatableEnterpriseManagementRelationshipObject rel = new CreatableEnterpriseManagementRelationshipObject(emg, relSoftwareTitleHasSoftwareVersion);
                //        rel.SetSource(CurrentEMO);
                //        rel.SetTarget(Ver);
                //        rel.Commit();
                //    }
                //}
                
            }
            catch (Exception exc)
            {
                MessageBox.Show(DateTime.Now + " : " + "Error in OnSubmitted() " + exc.Message);
            }


        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            try
            {
                //DisplayName
                //String strDisplayName = CurrentDataItem["AssetKey"].ToString() + " – " + this.TemplateCatalogItem.sip.Instance["MakeModel"].ToString();
                //CurrentDataItem["DisplayName"] = strDisplayName;


            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in OnPreviewSubmit() " + exc.Message);
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
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

        private void FillListViewCI()
        {
            try
            {
                LVVersions.ItemsSource = null;
                //ListViewCI.ItemsSource = null;
                //ListViewCI.Items.Clear();
                VersionListViewСollection.Clear();
                //ObservableCollection<EnterpriseManagementObject> RelEMOcol = new ObservableCollection<EnterpriseManagementObject>();
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>((Guid)CurrentEMO.Id, relSoftwareTitleHasSoftwareVersion, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                {
                    this.VersionListViewСollection.Add(obj.TargetObject);
                }
                Trace.WriteLine(DateTime.Now + " : " + "Count in CIListViewcollection " + VersionListViewСollection.Count());
                //this.ListViewCI.ItemsSource = CIListViewСollection;
                LVVersions.ItemsSource = VersionListViewСollection;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in FillListView() " + exc.Message);
            }
        }

        //private void brd_CI_Add_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    NavigationModelNodeBase NBNew;
        //    NavigationModelNodeTask nmntNew = NavigationTasksHelper.CreateNewInstanceLink(classVersion);
        //    Microsoft.EnterpriseManagement.GenericForm.GenericCommon.MonitorCreatedForm(null, nmntNew, out NBNew);
        //    //VersionListViewСollection.Add(CI_EMO);
        //    //this.VersiontoAddСollection.Add(CI_EMO);
        //    //this.LVVersions.ItemsSource = VersionListViewСollection;
        //}

        private void brd_CI_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LVVersions.SelectedItem != null)
            {
                //ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());
                //ConfItemClass = emg.EntityTypes.GetClass("System.ConfigItem", mpSystem);
                //ListViewItem LVItem = (ListViewItem)ListViewCI.SelectedItem;
                // Get an object by GUID
                //Guid G = new Guid(LVItem.Tag.ToString());
                //Guid G = (Guid)((EnterpriseManagementObject)ListViewCI.SelectedItem).Id;
                EnterpriseManagementObject ListViewEMO = (EnterpriseManagementObject)LVVersions.SelectedItem;
                //EnterpriseManagementObject ListViewEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                //Convert EnterpriseManagementObject to IDataItem
                EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classVersion);
                IDataItem itemIdentity = dataType.CreateProxyInstance(ListViewEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
            }
            else
            {
                MessageBox.Show("Please select Version!", "No Version selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_CI_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EnterpriseManagementObject VersionSelected = (EnterpriseManagementObject)LVVersions.SelectedItem;
                if (System.Windows.MessageBoxResult.OK == MessageBox.Show("Delete?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning))
                {
                    if ((bool)CurrentDataItem["$IsNew$"])
                    {
                        VersionListViewСollection.Remove(VersionSelected);
                    }
                    else
                    {
                        if (VersiontoAddСollection.Contains(VersionSelected))
                        {
                            //NotExisting Relationship - Remove only from Collections
                            VersionListViewСollection.Remove(VersionSelected);
                            VersiontoAddСollection.Remove(VersionSelected);
                        }
                        else
                        {
                            //Remove existing Relationship
                            Guid G = (Guid)VersionSelected.Id;
                            IncrementalDiscoveryData instance = new IncrementalDiscoveryData();
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relSoftwareTitleHasSoftwareVersion, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                            {
                                Trace.WriteLine(DateTime.Now + " : " + "Remove Relationship " + obj.TargetObject.DisplayName);
                                instance.Remove(obj);
                                instance.Commit(emg);

                                this.VersiontoAddСollection.Remove(VersionSelected);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in Remove Relationship void " + exc.Message);
            }
        }

        internal static void EditCIFromListView(ListView listView)
        {
            if (listView.ItemsSource == null || listView.SelectedItems == null || listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select Version for Edit!", "No Version selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                BindableCachedDataItem<IComposableProjection> bcdItem = (BindableCachedDataItem<IComposableProjection>)listView.SelectedItem;
                IDataItem item = (IDataItem)bcdItem;
                ConsoleContextHelper.Instance.PopoutForm(item);
            }
        }

        //private void brd_CI_Find_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        //Guid ConfigItemClassGuid = new Guid("62f0be9f-ecea-e73c-f00d-3dd78a7422fc");// Guid System.ConfigItem
        //        Collection<IDataItem> ItemsCol = new Collection<IDataItem>();
        //        Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.LaunchAddInstancePickerDialog(ItemsCol, VersionClassGuid);
        //        if (ItemsCol.Count() > 0)
        //        {
        //            foreach (IDataItem I in ItemsCol)
        //            {
        //                Guid CI_Id = (Guid)I["$Id$"];
        //                EnterpriseManagementObject CI_EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CI_Id, ObjectQueryOptions.Default);
        //                VersionListViewСollection.Add(CI_EMO);
        //                this.VersiontoAddСollection.Add(CI_EMO);
        //            }
        //            this.LVVersions.ItemsSource = VersionListViewСollection;
        //            MessageBox.Show("Find void VersiontoAddСollection.Count " + VersiontoAddСollection.Count);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        Trace.WriteLine(DateTime.Now + " : " + "Error in lbHACIAdd_MouseDown " + exc.Message);
        //    }
        //}


    }
    }

