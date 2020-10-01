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
    public partial class CatalogItemForm : UserControl
    {
        EnterpriseManagementGroup emg;
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;
        ObservableCollection<EnterpriseManagementObject> CIListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        ObservableCollection<EnterpriseManagementObject> CItoAddСollection= new ObservableCollection<EnterpriseManagementObject>();
        //public static Guid guidLifecycleStatus = new Guid("7159356A-9C44-37FD-FB58-70AB9FFF5EB8");
        //public static Guid guidEntityStatus = new Guid("b7efc970-9c06-798a-73b4-bef30633dae1");
        //public static Guid guidHardwareAssetType = new Guid("11ed339c-6a30-074a-bfef-50a9e3d102ad");


        public static Guid guidLifecycleStatus = new Guid("7159356A-9C44-37FD-FB58-70AB9FFF5EB8");
        public static Guid guidHardwareAssetType = new Guid("4676B5CF-14D6-97BD-5DB3-DCDD46E0440F");
        public static Guid ConfigItemClassGuid = new Guid("62f0be9f-ecea-e73c-f00d-3dd78a7422fc");// Guid System.ConfigItem
        public static Guid guidEntityStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");


        public CatalogItemForm()
        {
            InitializeComponent();
            ProjectionObjectTemplate TemplateManufacturer = new ProjectionObjectTemplate();
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
                string strlog = @"c:\Temp\SMCLog_" + userName + ".txt";
                TextWriterTraceListener TL = new TextWriterTraceListener(strlog);
                Trace.Listeners.Add(TL);
                Trace.AutoFlush = true;
                GetSession();


                ManagementPack mpCore = emg.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                //ManagementPack mpHA = emg.GetManagementPack("SMCenter.HardwareAssetManagement.Library", "75b45bd6835084b1", new Version());
                //ManagementPack mpSystem = emg.GetManagementPack("System.Library", "31bf3856ad364e35", new Version());

                //HAClass = emg.EntityTypes.GetClass("SMCenter.HardwareAsset", mpHA);
                //ConfItemClass = emg.EntityTypes.GetClass("System.ConfigItem", mpSystem);
                //LocationClass = emg.EntityTypes.GetClass("SMCenter.Location", mpCore);
                //CatalogItemClass = emg.EntityTypes.GetClass("SMCenter.HardwareCatalogItem", mpHA);



                //Fill Templates
                ManagementPackClass ManufacturerClass = emg.EntityTypes.GetClass("SMCenter.Company", mpCore);
                this.TemplateManufacturer.PathString = "Manufacturer";
                this.TemplateManufacturer.mpClass = ManufacturerClass;

                ManagementPackClass PreferredSuplierClass = emg.EntityTypes.GetClass("SMCenter.Company", mpCore);
                this.TemplatePreferredSuplier.PathString = "PreferredSupplier";
                this.TemplatePreferredSuplier.mpClass = PreferredSuplierClass;

                //Handlers
                this.AddHandler(FormEvents.PreviewSubmitEvent, new EventHandler<PreviewFormCommandEventArgs>(this.OnPreviewSubmit));

            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now + " : " + "UserControl_Loaded Error : " + ex.Message);
            }

        }

        private void OnPreviewSubmit(object sender, PreviewFormCommandEventArgs e)
        {
            try
            {
                CurrentDataItem["DisplayName"] = CurrentDataItem["MakeModel"].ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in OnPreviewSubmit() : " + ex.Message);
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
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

        }

       

    }
    

