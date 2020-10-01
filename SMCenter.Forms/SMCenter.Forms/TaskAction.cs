using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.UI.DataModel;
using Microsoft.EnterpriseManagement.GenericForm;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;
using System.ComponentModel.Design;



namespace SMCenter.Forms
{
    public class CreateSoftwareVersionTask : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
                //LogFile _LogFile = new LogFile(@"C:\LogForms.txt",true);
                IDataItem dataItem = null;
                //There will only ever be one item because we are going to limit this task to single select
                foreach (NavigationModelNodeBase node in nodes)
                {
                    //Check if task was started from form
                    bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);
                    //If started from form
                    if (startedFromForm)
                    {
                        dataItem = FormUtilities.Instance.GetFormDataContext(node);
                    }
                    //Else started from view
                    else
                    {
                        dataItem = node;
                    }
                }

                EnterpriseManagementGroup emg = GetSession();

                Guid CurrentNodeId = (Guid)dataItem["$Id$"];
                EnterpriseManagementObject EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                var newWindow = new CreateVersionTaskForm(EMO);
                newWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }

        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                //if (curSession == null)
                //throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }
    }
   
    //Task на открытие WC.
    public class RelatedWCFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
               // LogFile _LogFile = new LogFile(@"C:\LogForms.txt", true);
                IDataItem dataItem = null;
                //There will only ever be one item because we are going to limit this task to single select
                foreach (NavigationModelNodeBase node in nodes)
                {
                    //Check if task was started from form
                    bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);
                    //If started from form
                    if (startedFromForm)
                    {
                        dataItem = FormUtilities.Instance.GetFormDataContext(node);
                    }
                    //Else started from view
                    else
                    {
                        dataItem = node;
                    }
                }

                EnterpriseManagementGroup emg = GetSession();
                ManagementPack mpAssetCore;
                ManagementPack mpNetworkAssetLibrary;
                ManagementPack mpCMLibrary;

                ManagementPackClass classWindowsComputer;
                ManagementPackClass classDeployedComputer;


                ManagementPackRelationship relComputerRunsWindowsComputer;
                mpAssetCore = emg.ManagementPacks.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                mpCMLibrary = emg.ManagementPacks.GetManagementPack("Microsoft.SystemCenter.ConfigurationManager", "31bf3856ad364e35", new Version());
                classWindowsComputer = emg.EntityTypes.GetClass("SMCenter.WindowsComputer", mpNetworkAssetLibrary);
                classDeployedComputer = emg.EntityTypes.GetClass("SMCenter.DeployedComputer", mpNetworkAssetLibrary);
                relComputerRunsWindowsComputer = emg.EntityTypes.GetRelationshipClass("Microsoft.SystemCenter.ConfigurationManager.DeployedComputerRunsWindowsComputer", mpCMLibrary);

                EnterpriseManagementObject WC = null;
                Guid CurrentNodeId = (Guid)dataItem["$Id$"];
                EnterpriseManagementObject DC = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                //if (WC.IsInstanceOf(classWindowsComputer))
                //{
                //    var rels = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(WC.Id, relComputerRunsWindowsComputer, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                //    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in rels)
                //    {
                //        DC = rel.SourceObject;
                //    }
                //}
                //else
                //{
                //    //_LogFile.Write(String.Format("No DC"), false);
                //    System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + "Текущий объект не является Windows Computer.");
                //}

                if (DC.IsInstanceOf(classDeployedComputer))
                {
                    var rels = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(DC.Id, relComputerRunsWindowsComputer, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in rels)
                    {
                        WC = rel.TargetObject;
                    }
                }
                else
                {
                    //_LogFile.Write(String.Format("No DC"), false);
                    System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + "Текущий объект не является Deployed Computer.");
                }

                if (WC != null)
                {
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classWindowsComputer);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(WC);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else
                {
                    System.Windows.MessageBox.Show(DateTime.Now + " : Нет связанных Windows Computer!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }



        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                //if (curSession == null)
                //throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }
    }

    //Task на открытие DC.
    public class RelatedDCFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
               //LogFile _LogFile = new LogFile(@"C:\LogForms.txt",true);
                IDataItem dataItem = null;
                //There will only ever be one item because we are going to limit this task to single select
                foreach (NavigationModelNodeBase node in nodes)
                {
                    //Check if task was started from form
                    bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);
                    //If started from form
                    if (startedFromForm)
                    {
                        dataItem = FormUtilities.Instance.GetFormDataContext(node);
                    }
                    //Else started from view
                    else
                    {
                        dataItem = node;
                    }
                }

                EnterpriseManagementGroup emg = GetSession();
                ManagementPack mpAssetCore;
                ManagementPack mpNetworkAssetLibrary;
                ManagementPack mpCMLibrary;

                ManagementPackClass classWindowsComputer;
                ManagementPackClass classDeployedComputer;


                ManagementPackRelationship relComputerRunsWindowsComputer;
                mpAssetCore = emg.ManagementPacks.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                mpCMLibrary = emg.ManagementPacks.GetManagementPack("Microsoft.SystemCenter.ConfigurationManager", "31bf3856ad364e35", new Version());
                classWindowsComputer = emg.EntityTypes.GetClass("SMCenter.WindowsComputer", mpNetworkAssetLibrary);
                classDeployedComputer = emg.EntityTypes.GetClass("SMCenter.DeployedComputer", mpNetworkAssetLibrary);
                relComputerRunsWindowsComputer = emg.EntityTypes.GetRelationshipClass("Microsoft.SystemCenter.ConfigurationManager.DeployedComputerRunsWindowsComputer", mpCMLibrary);

                EnterpriseManagementObject DC = null; 
                Guid CurrentNodeId = (Guid)dataItem["$Id$"];
                EnterpriseManagementObject WC = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                if (WC.IsInstanceOf(classWindowsComputer))
                {
                    var rels = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(WC.Id, relComputerRunsWindowsComputer, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in rels)
                    {
                        DC = rel.SourceObject;
                    }
                }
                else
                {
                    //_LogFile.Write(String.Format("No DC"), false);
                    System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + "Текущий объект не является Windows Computer.");
                }
                if (DC != null)
                {
                    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classDeployedComputer);
                    IDataItem itemIdentity = dataType.CreateProxyInstance(DC);
                    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                }
                else
                {
                    System.Windows.MessageBox.Show(DateTime.Now + " : Нет связанных Deployed Computer!","Service Manager",MessageBoxButton.OK,MessageBoxImage.Information);
                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }

        

        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                //if (curSession == null)
                    //throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }
    }

    public class SCSFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
                //LogFile _LogFile = new LogFile(@"C:\LogForms.txt",true);
                IDataItem dataItem = null;
                //There will only ever be one item because we are going to limit this task to single select
                foreach (NavigationModelNodeBase node in nodes)
                {
                    //Check if task was started from form
                    bool startedFromForm = FormUtilities.Instance.IsNodeWithinForm(nodes[0]);
                    //If started from form
                    if (startedFromForm)
                    {
                        dataItem = FormUtilities.Instance.GetFormDataContext(node);
                    }
                    //Else started from view
                    else
                    {
                        dataItem = node;
                    }
                }

                EnterpriseManagementGroup emg = GetSession();

                Guid CurrentNodeId = (Guid)dataItem["$Id$"];
                EnterpriseManagementObject EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(CurrentNodeId, ObjectQueryOptions.Default);
                var newWindow = new SCSForm(EMO);
                newWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }

        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                //if (curSession == null)
                //throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }
    }


    public class ConnectionsFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {

                var newWindow = new ConnectionsForm();
                newWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }

        
    }
}
