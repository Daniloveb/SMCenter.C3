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
namespace SMCenter.NetworkTasks
{
    
    public class ConnectionsFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
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

                //var newWindow = new ConnectionsForm(dataItem);
                var newWindow = new ConnectionsForm(dataItem);
                newWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }
    }

    public class MapFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {

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

                //var newWindow = new ConnectionsForm(dataItem);
                var newWindow = new MapForm(dataItem);
                newWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }
    }

    public class RelatedFormAction : Microsoft.EnterpriseManagement.UI.SdkDataAccess.ConsoleCommand
    {
        public override void ExecuteCommand(IList<Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeBase> nodes, Microsoft.EnterpriseManagement.ConsoleFramework.NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {

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

                //var newWindow = new ConnectionsForm(dataItem);
                var newWindow = new ConnectionsForm(dataItem);
                newWindow.Show();

                // Guid G = new Guid(SelectedTreeItem.Tag.ToString());
                //EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                ////Guid Id_NA = new Guid();
                //if (EMO.IsInstanceOf(classModule))
                //{
                //    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classModule);
                //    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                //    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                //}

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "ExecuteCommand Error : " + ex.Message);
            }
        }
    }
}
