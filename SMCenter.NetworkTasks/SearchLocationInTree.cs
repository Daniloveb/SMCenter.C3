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

namespace SMCenter.NetworkTasks
{
    //старый класс - использовали для поиска связанной локации, но поззже этот функционал интеграировали в основной класс CableTree
    public class SearchLocationInTree2
    {
        //private ManagementPack mpNetworkAssetLibrary;
        private Guid NetworkAdapter_Guid;
        private ManagementPackRelationship relNetworkAdapterHasChildNetworkAdapter {get; set;}
        private ManagementPackClass classModule { get; set; }
        private ManagementPackRelationship relLocationContainsConfigItem { get; set; }
        private EnterpriseManagementGroup emg;
        //public EnterpriseManagementObject Module;
        //public EnterpriseManagementObject Location;
        public Guid Location_Id {get; set;}
        public TreeViewItem TreeViewItem {get; set;}

        public SearchLocationInTree2(EnterpriseManagementGroup _emg, ManagementPackRelationship relNetworkAdapterHasChildNetworkAdapter, ManagementPackClass classModule, ManagementPackRelationship relLocationContainsConfigItem)
        {
            emg = _emg;
            
            Location_Id = new Guid();
        }

        public TreeViewItem Start(Guid _NetworkAdapter_Guid)
        {
            //try
            //{
                NetworkAdapter_Guid = _NetworkAdapter_Guid;
                //TreeViewItem TVI = new TreeViewItem();
                if (NetworkAdapter_Guid != Guid.Empty)
                {
                    TreeViewItem = CreateTree(NetworkAdapter_Guid);
                }
                else
                {
                    System.Windows.MessageBox.Show("Id Network Adapter is Empty");
                }
                return TreeViewItem;
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("CabelTree Create procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return null;
            //}
        }

        TreeViewItem CreateTree(Guid G)
        {
            //try
            //{
            TreeViewItem TVI = Sub_SearchRootNA(G);
            ObservableCollection<TreeViewItem> Col = BuildCollection(TVI);
            System.Windows.MessageBox.Show("33");
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
           //}
           // catch (Exception ex)
           // {
           //     System.Windows.MessageBox.Show("CreateTree procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
           //     return null;
           // }
        }

        ObservableCollection<TreeViewItem> BuildCollection(TreeViewItem TVI)
        {
            //try
            //{
                if (TVI == null)
                { System.Windows.MessageBox.Show("TVI is null"); }
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
                    newItemCabel.Tag = "Cabel";//relobj.TargetObject.Id;
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
           //}
           // catch (Exception ex)
           // {
           //     System.Windows.MessageBox.Show("BuildCollection procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
           //     return null;
           // }
        }

        //Ищем корневой родительский Network Adapter
        //Возвращаем созданный на его базе TreeViewItem, без вложенных Items
        TreeViewItem Sub_SearchRootNA(Guid G)
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
        //Ищем связанные NA. в случае нахождения модуля - проверяем связь с локацией и сохраняем Location_Id
        EnterpriseManagementRelationshipObject<EnterpriseManagementObject> SubChild(Guid G)
        {
            try
            {
                EnterpriseManagementObject EMO = null;
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel = null; //new EnterpriseManagementRelationshipObject<EnterpriseManagementObject>();
                var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(G, relNetworkAdapterHasChildNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    rel = relobj;
                    EMO = relobj.TargetObject;
                }
                if (EMO.IsInstanceOf(classModule))
                {
                    var G_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(EMO.Id, relLocationContainsConfigItem, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> confobj in G_objects)
                    {
                        Location_Id = confobj.SourceObject.Id;
                    }
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
