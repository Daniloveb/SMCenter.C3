using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;

namespace SMCenter.NetworkTasks
{
    //Class для создания TreeviewItem 
    public static class CollectionTree
    {
        //static ManagementPack mpNetworkAssetLibrary;
        static ManagementPackRelationship relNetworkAdapterHasChildNetworkAdapter;
        public static EnterpriseManagementGroup emg;

        public static TreeViewItem Create(Guid NetworkAdapter_Guid, EnterpriseManagementGroup _emg, ManagementPackRelationship _relNetworkAdapterHasChildNetworkAdapter)
        {
            emg = _emg;
            relNetworkAdapterHasChildNetworkAdapter = _relNetworkAdapterHasChildNetworkAdapter;
            //mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
            //relNetworkAdapterHasChildNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkAdapterHasChildNetworkAdapter", mpNetworkAssetLibrary);
            TreeViewItem TVI = new TreeViewItem();
            if (NetworkAdapter_Guid != Guid.Empty)
            {
                TVI = CreateChildTree(NetworkAdapter_Guid);
            }
            return TVI;
        }

        static TreeViewItem CreateChildTree(Guid G)
        {

            TreeViewItem TVI = Sub_SearchParentRoot(G);
            ObservableCollection<TreeViewItem> Col = BuildCollection(TVI);
            ObservableCollection<TreeViewItem> Col2 = new ObservableCollection<TreeViewItem>();

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
            return Col2[0];
            //this.ModuleTreeView.Items.Add(Col2[0]);
        }

        static ObservableCollection<TreeViewItem> BuildCollection(TreeViewItem TVI)
        {
            TreeViewItem newItem = new TreeViewItem();
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            Col.Add(TVI);
            Guid G = (Guid)TVI.Tag;
            //ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            bool t = true;
            while (t == true)
            {
                newItem = SubChild(G);
                if (newItem != null)
                {
                    Col.Add(newItem);
                    //q_parent.Push(newItem);
                    G = (Guid)newItem.Tag;
                }
                else
                { t = false; }
            }
            return Col;
            //сортировка не нужна т.к. связь один к одному
        }

        static TreeViewItem Sub_SearchParentRoot(Guid G)
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
            return newItem;
        }


        static TreeViewItem SubChild(Guid G)
        {
            TreeViewItem newItem = null;
            newItem = null;
            var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relNetworkAdapterHasChildNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
            {
                newItem = new TreeViewItem();
                newItem.Header = relobj.TargetObject.FullName;
                newItem.Tag = relobj.TargetObject.Id;
            }
            return newItem;
        }

    }
}
