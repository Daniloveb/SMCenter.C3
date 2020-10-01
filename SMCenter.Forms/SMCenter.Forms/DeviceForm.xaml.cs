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
    public partial class DeviceForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPack mpCore;
        ManagementPack mpNA;
        ManagementPackClass classLocation;
        //ManagementPackClass classNetworkMap;
        ManagementPackRelationship relLoctoLoc;
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;


        //ObservableCollection<EnterpriseManagementObject> CIListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        //ObservableCollection<EnterpriseManagementObject> CItoAddСollection= new ObservableCollection<EnterpriseManagementObject>();
        public static Guid guidAssetStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");
        public static Guid guidDeviceType = new Guid("d247f079-d252-a0db-ca93-9f32172b9509");
        //public static Guid guidHardwareAssetType =  new Guid("11ed339c-6a30-074a-bfef-50a9e3d102ad");
        
        private RelatedItemsPane _relatedItemsPane;

        //System.Drawing.Image DrawingImage;

        public DeviceForm()
        {
            try
            {

                InitializeComponent();
                _relatedItemsPane = new RelatedItemsPane(new ConfigItemRelatedItemsConfiguration());
                tabItemRelItems.Content = _relatedItemsPane;

                //Define Template
                //ProjectionLocationTemplate TemplateLocation = new ProjectionLocationTemplate();

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
        //        this.TemplateLocationItem.sip.Instance = itemIdentity;
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

                mpCore = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNA = emg.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());


                classLocation = emg.EntityTypes.GetClass("SMCenter.Location", mpCore);
                //classNetworkMap = emg.EntityTypes.GetClass("SMCenter.NetworkMap", mpNA);

                relLoctoLoc = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpCore);

                //Fill Templates

                //this.TemplateLocationItem.emg = emg;
                //this.TemplateLocationItem.PathString = "LinkedLocation";
                //this.TemplateLocationItem.mpClass = classLocation;
                //this.TemplateLocationItem.mpRelationshipClass = relLoctoLoc;
                //this.TemplateLocationItem.criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);


                //////Handlers
                this.AddHandler(FormEvents.SubmittedEvent, new EventHandler<FormCommandExecutedEventArgs>(this.OnSubmitted));
                this.AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnPreviewSubmit));
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

                if (!((bool)CurrentDataItem["$IsNew$"]))
                {
                    CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);

                    
                }
                else
                {
                   
                }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in OnSubmitted() " + exc.Message);
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
                        //CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                        //this.FillListViewCI();
                    }
                }
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

    }
    }

