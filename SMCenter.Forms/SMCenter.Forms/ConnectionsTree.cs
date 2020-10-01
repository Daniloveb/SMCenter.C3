using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;

namespace SMCenter.Forms
{
    public static class ConnectionsTree
    {
        static ManagementPack mpNetworkLibrary;
        static ManagementPackClass classNetworkConnection;
        static ManagementPackRelationship relNetworkConnectionConnectedToNetworkAdapter;
        public static EnterpriseManagementGroup emg;

        public static TreeViewItem Create(Guid NetworkAdapter_Guid, EnterpriseManagementGroup _emg)
        {
            try
            {
                emg = _emg;
                mpNetworkLibrary = emg.GetManagementPack("System.NetworkManagement.Library", "31bf3856ad364e35", new Version());
                classNetworkConnection = emg.EntityTypes.GetClass("System.NetworkManagement.NetworkConnection", mpNetworkLibrary);
                relNetworkConnectionConnectedToNetworkAdapter = emg.EntityTypes.GetRelationshipClass("System.NetworkManagement.NetworkConnectionConnectedToNetworkAdapter", mpNetworkLibrary);
                EnterpriseManagementObject E = emg.EntityObjects.GetObject<EnterpriseManagementObject>(NetworkAdapter_Guid, ObjectQueryOptions.Default);
                TreeViewItem mainItem = new TreeViewItem();
                mainItem.Header = E.FullName;
                mainItem.Tag = E.Id;
                mainItem.IsExpanded = true;
                TreeViewItem NCItem = null;
                Guid NCItemGuid = new Guid();
                if (NetworkAdapter_Guid != Guid.Empty)
                {
                    //Create NetworkConnection Treeview Item
                    var S_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(NetworkAdapter_Guid, relNetworkConnectionConnectedToNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in S_objects)
                    {
                        NCItem = new TreeViewItem();
                        NCItem.Header = relobj.SourceObject[classNetworkConnection,"Key"];
                        NCItem.Tag = relobj.SourceObject.Id;
                        NCItem.IsExpanded = true;
                        mainItem.Items.Add(NCItem);

                        NCItemGuid = relobj.SourceObject.Id;
                    }
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(NCItemGuid, relNetworkConnectionConnectedToNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        //newItem = new TreeViewItem();
                        if (relobj.TargetObject.Id != NetworkAdapter_Guid && NCItem != null)
                        {
                            TreeViewItem NAItem = new TreeViewItem();
                            NAItem.Header = relobj.TargetObject.FullName;
                            NAItem.Tag = relobj.TargetObject.Id;
                            NAItem.IsExpanded = true;
                            NCItem.Items.Add(NAItem);
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Id Network Adapter is Empty");
                }
                if (NCItem == null)
                { return null; }
                else
                {
                    return mainItem;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Connections Create procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


    }
}
