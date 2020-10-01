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
    public static class CablesTree_old
    {
        static ManagementPack mpNetworkAssetLibrary;
        static ManagementPackRelationship relNetworkAdapterHasChildNetworkAdapter;
        public static EnterpriseManagementGroup emg;

        public static TreeViewItem Create(Guid NetworkAdapter_Guid, EnterpriseManagementGroup _emg)
        {
            try
            {
                emg = _emg;
                mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                relNetworkAdapterHasChildNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkAdapterHasChildNetworkAdapter", mpNetworkAssetLibrary);
                TreeViewItem TVI = new TreeViewItem();
                if (NetworkAdapter_Guid != Guid.Empty)
                {
                    TVI = CreateTree(NetworkAdapter_Guid);
                }
                else
                {
                    System.Windows.MessageBox.Show("Id Network Adapter is Empty");
                }
                return TVI;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CabelTree Create procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        static TreeViewItem CreateTree(Guid G)
        {
            try
            {
            TreeViewItem TVI = Sub_SearchParentRoot(G);
            ObservableCollection<TreeViewItem> Col = BuildCollection(TVI);
            ObservableCollection<TreeViewItem> Col2 = new ObservableCollection<TreeViewItem>();
            if (Col.Count == 1)
            {
                Col2.Add(Col[0]);
            }
            else
            {
                TreeViewItem t1 = new TreeViewItem();
                TreeViewItem t2 = new TreeViewItem();
                int k = 0;
                while (k < Col.Count - 1)
                {
                    t1 = Col[k];
                    t2 = Col[k + 1];
                    t1.Items.Add(t2);
                    t1.IsExpanded = true;
                    Col2.Add(t1);

                    k = k + 1;
                }
            }
            return Col2[0];
           }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CreateTree procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        static ObservableCollection<TreeViewItem> BuildCollection(TreeViewItem TVI)
        {
            try
            {
                
            TreeViewItem newItem = new TreeViewItem();
            TreeViewItem newItemCabel = new TreeViewItem();
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            Col.Add(TVI);
            Guid G = (Guid)TVI.Tag;
            bool t = true;
            while (t == true)
            {
                //newItem = SubChild(G);
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj = SubChild(G);
                if (relobj != null)
                {
                    newItemCabel = new TreeViewItem();
                    newItemCabel.Header = "Cable.Footage:" + relobj[relNetworkAdapterHasChildNetworkAdapter, "ParentFootage"].Value + ":" + relobj[relNetworkAdapterHasChildNetworkAdapter, "ParentFootage"].Value;
                    newItemCabel.Tag = relobj.Id;
                    Col.Add(newItemCabel);
                    newItem = new TreeViewItem();
                    newItem.Header = relobj.TargetObject.FullName;
                    newItem.Tag = relobj.TargetObject.Id;
                    Col.Add(newItem);

                    G = (Guid)newItem.Tag;
                }
                else
                { t = false; }
            }
            return Col;
            //сортировка не нужна т.к. связь один к одному
           }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("BuildCollection procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        static TreeViewItem Sub_SearchParentRoot(Guid G)
        {
           try
            {
            TreeViewItem newItem = null;
            Guid _G = G;
            bool x = true;
            while (x == true)
            {
                x = false;
                var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(_G, relNetworkAdapterHasChildNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    newItem = new TreeViewItem();
                    newItem.Header = relobj.SourceObject.FullName;
                    newItem.Tag = relobj.SourceObject.Id;
                    _G = relobj.SourceObject.Id;
                    x = true;
                }
            }
               //В случае отсутсвия родительских NA - передаем текущий элемент.
            if (newItem == null)
            {
                EnterpriseManagementObject E = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);
                newItem = new TreeViewItem();
                newItem.Header = E.FullName;
                newItem.Tag = E.Id;
            }
            return newItem;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Sub_SearchParentRoot procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }



        static EnterpriseManagementRelationshipObject<EnterpriseManagementObject> SubChild(Guid G)
        {
            try
            {
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel = null; //new EnterpriseManagementRelationshipObject<EnterpriseManagementObject>();
                var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relNetworkAdapterHasChildNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    rel = relobj;
                }
                return rel;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("SubChild procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
