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
    public partial class PPForm : UserControl
    {
        EnterpriseManagementGroup emg;
        ManagementPack mpCore;
        ManagementPack mpNA;
        ManagementPack mpNetworkManagementLibrary;
        ManagementPackClass classLocation;
        ManagementPackClass classRack;
        ManagementPackClass classPatchPanel;
        //ManagementPackClass classPPPort;
        ManagementPackClass classNAPort;
        //ManagementPackClass classNetworkMap;
        ManagementPackRelationship relLoctoLoc;
        ManagementPackRelationship relConfigItemRefRack;
         ManagementPackRelationship relNodetoPort;
        IDataItem CurrentDataItem;
        EnterpriseManagementObject CurrentEMO;


        //ObservableCollection<EnterpriseManagementObject> CIListViewСollection = new ObservableCollection<EnterpriseManagementObject>();
        //ObservableCollection<EnterpriseManagementObject> CItoAddСollection= new ObservableCollection<EnterpriseManagementObject>();
        public static Guid guidAssetStatus = new Guid("DF83475D-D6D1-7236-FC6D-EC8FE52022B0");
        public static Guid guidNodeMonitorType = new Guid("1CEC75AE-B151-8FE0-FA49-358A078701FD");
        public static Guid guidNodeType = new Guid("CB43F0BC-2595-2EB2-7481-E23DEFBD3A77");
        //public static Guid guidHardwareAssetType =  new Guid("11ed339c-6a30-074a-bfef-50a9e3d102ad");
        
        private RelatedItemsPane _relatedItemsPane;
        LogFile _LogFile;
        //System.Drawing.Image DrawingImage;

        public PPForm()
        {
            try
            {

                InitializeComponent();
                _relatedItemsPane = new RelatedItemsPane(new ConfigItemRelatedItemsConfiguration());
                tabItemRelItems.Content = _relatedItemsPane;

                //Define Template
                ProjectionLocationTemplate TemplateLocation = new ProjectionLocationTemplate();
                ProjectionRackTemplate TemplateRack = new ProjectionRackTemplate();

                _LogFile = new LogFile(@"C:\LogForms2.txt", true);

                //SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Initialized Proc Error : " + e.Message);
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
                mpNetworkManagementLibrary = emg.GetManagementPack("System.NetworkManagement.Library", "31bf3856ad364e35", new Version());

                classLocation = emg.EntityTypes.GetClass("SMCenter.Location", mpCore);
                classRack = emg.EntityTypes.GetClass("SMCenter.Rack", mpNA);
                classPatchPanel = emg.EntityTypes.GetClass("SMCenter.PatchPanel", mpNA);
                //classPPPort = emg.EntityTypes.GetClass("SMCenter.PatchPanelPort", mpNA);
                classNAPort = emg.EntityTypes.GetClass("System.NetworkManagement.NetworkAdapter", mpNetworkManagementLibrary);
                //classNetworkMap = emg.EntityTypes.GetClass("SMCenter.NetworkMap", mpNA);


                relLoctoLoc = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpCore);

                relConfigItemRefRack = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefRack", mpNA);
                relNodetoPort = emg.EntityTypes.GetRelationshipClass("System.NetworkManagement.NodeComposedOfNetworkAdapter", mpNetworkManagementLibrary);

                //Fill Templates

                //this.TemplateLocationItem.emg = emg;
                //this.TemplateLocationItem.PathString = "LinkedLocation";
                //this.TemplateLocationItem.mpClass = classLocation;
                //this.TemplateLocationItem.mpRelationshipClass = relLoctoLoc;
                //this.TemplateLocationItem.criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);

                this.TemplateRackItem.PathString = "Rack";
                this.TemplateRackItem.mpClass = classRack;

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


                CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);

                var racks = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(CurrentEMO.Id, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in racks)
                {
                    int res;
                    if (!(int.TryParse(this.TemplateRackItem.Unit, out res)) && this.TemplateRackItem.Unit != null)
                    {
                        System.Windows.MessageBox.Show(DateTime.Now + " : " + "Значение Unit должно быть числовым!");
                        return;
                    }
                    if (this.TemplateRackItem.Unit != null)
                    { 
                        relobj[relConfigItemRefRack, "UnitIndex"].Value = this.TemplateRackItem.Unit;
                        relobj.Commit();
                    }
                }


                //Проверяем существуют ли порты
                var Ports = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentEMO.Id, relNodetoPort, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                if (Ports.Count > 0)
                {
                    _LogFile.Write(String.Format("Return "), false);
                    return;
                }
                //Создаем порты
                int intPortsNo = 48;
                int.TryParse(this.txtPortsNo.Text, out intPortsNo);
                if (intPortsNo == 0) intPortsNo = 48;
                _LogFile.Write(String.Format("PortsNo " + intPortsNo.ToString()), false);
                    //int[] BBnumbers = null;
                    //if (BackBonePortList != null && BackBonePortList != "")
                    //{
                    //    string[] BBlist = BackBonePortList.Split(",".ToCharArray());
                    //    BBnumbers = BBlist.Select(s => int.Parse(s)).ToArray();
                    //}
                    //int PortsNumber;
                    //if (Uplink < 27)
                    //{ PortsNumber = 26; }
                    //else
                    //{ PortsNumber = 50; }

                    //if (PN.Contains("Cisco"))
                    //{ PortsNumber = 50; }
                    //else if (PN.Contains("DES-2108"))
                    //{ PortsNumber = 8; }

                    //ManagementPackEnumeration enumNodeType = EO.mpNetworkAssetLibrary.GetEnumeration("NodeType.Hub");
                    //objNode[EO.classNode, "NodeType"].Value = enumNodeType;
                    //if (Node[EO.classNode, "NodeType"].Value == enumNodeType & Uplink == 0)
                    //{ PortsNumber = 8; }
                    for (int i = 1; i <= intPortsNo; i++)
                    {
                        _LogFile.Write(String.Format("i " + i.ToString()), false);
                        //DeviceNetworkAdapter[classDevice, "Id"].Value = Device[classDevice, "Id"].Value;
                        //DeviceNetworkAdapter[classDeviceNetworkAdapter, "DeviceID"].Value = strDeviceID;

                        CreatableEnterpriseManagementObject objPort = new CreatableEnterpriseManagementObject(emg, classNAPort);
                        _LogFile.Write(String.Format("CurrentEMO " + CurrentEMO.FullName + " port " + i.ToString()), false);
                        //objPort[EO.classNode, "DeviceKey"].Value = Node[EO.classNode, "DeviceKey"].Value;
                        objPort[classPatchPanel, "DeviceKey"].Value = CurrentEMO[classPatchPanel, "DeviceKey"].Value;
                        objPort[classNAPort, "Index"].Value = i.ToString();
                        objPort[classNAPort, "Key"].Value = "PORT-" + CurrentEMO[classPatchPanel, "DeviceKey"].Value.ToString() + "/" + i.ToString();
                        objPort[classNAPort, "DisplayName"].Value = "PORT-" + i.ToString();
                        objPort.Overwrite();
                        //CreatableEnterpriseManagementRelationshipObject relobjNodetoPort = new CreatableEnterpriseManagementRelationshipObject(emg, relNodetoPort);
                        //relobjNodetoPort.SetSource(Node);
                        //relobjNodetoPort.SetTarget((EnterpriseManagementObject)objPort);
                        //relobjNodetoPort.Commit();

                        //objPort[EO.classNodePort, "DisplayName"].Value = "PORT-" + i.ToString();
                        //objPort[EO.classNodePort, "InterfaceNumber"].Value = i.ToString();
                        //objPort[EO.classNodePort, "Key"].Value = "PORT-" + Node[EO.classNode, "SNMPAddress"].Value.ToString() + "/" + i.ToString();
                        //objPort.Overwrite();
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
                        CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);

                        var racks = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(CurrentEMO.Id, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in racks)
                        {
                            if (relobj[relConfigItemRefRack, "UnitIndex"].Value != null)
                            {
                                //System.Windows.MessageBox.Show("Unit " + relobj[relRackContainsConfigItem, "UnitIndex"].Value.ToString());
                                this.TemplateRackItem.Unit = relobj[relConfigItemRefRack, "UnitIndex"].Value.ToString();
                            }
                        }
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

