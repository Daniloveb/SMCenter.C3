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
    public partial class CreateVersionTaskForm : Window
    {
        //EnterpriseManagementObject EMO_SoftwareItem;
        EnterpriseManagementObject EMO_SoftwareTitle;
        //EnterpriseManagementObject EMO_SoftwareVersion;
        EnterpriseManagementGroup emg;

        //ManagementPack mpAssetCore;
        ManagementPack mpSoftLibrary; 
        //ManagementPack mpSystemSoftwareLibrary; 
        //ManagementPackClass classSoftwareItem;
        ManagementPackClass classSoftwareVersion; 
        ManagementPackClass classSoftwareTitle; 
        //ManagementPackClass classPublisher;

        //ManagementPackRelationship relExcludedSoftwareItemGroup;
        ManagementPackRelationship relSoftwareTitleHasSoftwareVersion;


        //LogFile LogFile;

        //public ConnectionsForm(IDataItem Node)
        public CreateVersionTaskForm(EnterpriseManagementObject _EMO) 
        {
            InitializeComponent();

            try
            {
                //LogFile = new LogFile(@"C:\LogFile.txt", true);
                
                //ManagementPackClass PreferredSuplierClass = emg.EntityTypes.GetClass("SMCenter.Company", mpAssetCore);
                ProjectionObjectTemplate TemplateSoftwareTitle = new ProjectionObjectTemplate();
                
                EMO_SoftwareTitle = _EMO;
                this.txtSoftwareTitle.Text = EMO_SoftwareTitle.DisplayName;

                //SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
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

        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

       

        private void lbOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //IDataItem DI = this.TemplateSoftwareTitle.sipInstance;
                //Guid CurrentNodeId = (Guid)DI["$Id$"];
                //EnterpriseManagementObject SoftwareTitle_EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                //MessageBox.Show(SoftwareTitle_EMO.Name);
                //MessageBox.Show(TemplateSoftwareTitle.mpClass.Name);

                if (this.txtDisplayName.Text != "" && this.txtDisplayName.Text != null)
                {
                    //ManagementPackClass classTask = ManagementGroup.EntityTypes.GetClass(“Task”, testMP)
                    CreatableEnterpriseManagementObject Version = new CreatableEnterpriseManagementObject(emg, classSoftwareVersion);
                    //Version [classTask, “Id”].Value = Guid.NewGuid().ToString();
                    Version[classSoftwareVersion, "DisplayName"].Value = this.txtDisplayName.Text;

                    CreatableEnterpriseManagementRelationshipObject relationshipObject = new CreatableEnterpriseManagementRelationshipObject(emg, relSoftwareTitleHasSoftwareVersion);
                    relationshipObject.SetSource(EMO_SoftwareTitle);
                    relationshipObject.SetTarget(Version);

                    IncrementalDiscoveryData dd = new IncrementalDiscoveryData();
                    dd.Add(Version); //target object
                    dd.Add(relationshipObject); //relationship object
                    dd.Commit(emg);
                    this.Close();
                }
                else
                { MessageBox.Show("Type SoftwareVersion DisplayName!", "Warning!!!", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ok_MouseDown void error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
  
        }

        private void AddToCatalogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                emg = GetSession();
                if (emg == null)
                {
                    emg = new EnterpriseManagementGroup("SM");
                }

                //mpAssetCore = emg.ManagementPacks.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());

                mpSoftLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.SoftwareAssetManagement.Library", "75b45bd6835084b1", new Version());
                //mpSystemSoftwareLibrary = emg.ManagementPacks.GetManagementPack("System.Software.Library", "31bf3856ad364e35", new Version());
                //classSoftwareItem = emg.EntityTypes.GetClass("System.SoftwareItem", mpSystemSoftwareLibrary);
                classSoftwareTitle = emg.EntityTypes.GetClass("SMCenter.SoftwareTitle", mpSoftLibrary);
                classSoftwareVersion = emg.EntityTypes.GetClass("SMCenter.SoftwareVersion", mpSoftLibrary);
                //classPublisher = emg.EntityTypes.GetClass("SMCenter.Company", mpAssetCore);
                //SMCenter.PublisherHasSoftwareTitle
                //SMCenter.SoftwareTitleHasSoftwareVersion
                //SMCenter.SoftwareVersionReferencesDiscoveredSoftwareItem
                relSoftwareTitleHasSoftwareVersion = emg.EntityTypes.GetRelationshipClass("SMCenter.SoftwareTitleHasSoftwareVersion", mpSoftLibrary);
                //relExcludedSoftwareItemGroup = emg.EntityTypes.GetRelationshipClass("SMCenter.ExcludedSoftwareItemGroupContainsEntities", mpSoftLibrary);

                //TemplateSoftwareTitle.PathString = "SotwareTitle";
                //TemplateSoftwareTitle.mpClass = classSoftwareTitle;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
    }
    
}
