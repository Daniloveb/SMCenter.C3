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
    //Class для создания ветки дерева кабельных соединений с определенным NetworkAdapter
    public class CablesTree
    {
        //static ManagementPack mpNetworkAssetLibrary;
        private ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter { get; set; }
        private ManagementPackClass classModule { get; set; }
        private ManagementPackRelationship relConfigItemRefLocation { get; set; }
        private EnterpriseManagementGroup emg { get; set; }
        public Guid Location_Id { get; set; }
        public TreeViewItem TreeViewItem { get; set; }
        private LogFile LogFile;

        public CablesTree(EnterpriseManagementGroup _emg, ManagementPackRelationship _relChildNetworkAdapterRefParentNetworkAdapter, ManagementPackClass _classModule, ManagementPackRelationship _relConfigItemRefLocation, LogFile _LogFile)
        {
            emg = _emg;
            relChildNetworkAdapterRefParentNetworkAdapter = _relChildNetworkAdapterRefParentNetworkAdapter;
            classModule = _classModule;
            relConfigItemRefLocation = _relConfigItemRefLocation;
            Location_Id = new Guid();
            LogFile = _LogFile;
        }

        public CablesTree(EnterpriseManagementGroup _emg, ManagementPackRelationship _relChildNetworkAdapterRefParentNetworkAdapter)
        {
            emg = _emg;
            relChildNetworkAdapterRefParentNetworkAdapter = _relChildNetworkAdapterRefParentNetworkAdapter;
            classModule = null;
            Location_Id = Guid.Empty;
        }

        public TreeViewItem CreateTreeViewItem(Guid NetworkAdapter_Guid)
        {
            TreeViewItem TVI = Go(NetworkAdapter_Guid);
            return TVI;
        }

        public TreeViewItem CreateTreeViewItemAndFindLocation(Guid NetworkAdapter_Guid, out Guid Location_Id)
        {
            Location_Id = Guid.Empty;
            TreeViewItem TVI = Go(NetworkAdapter_Guid);
            return TVI;
        }

        //Создаем ветку Treeview из подключенных адаптеров
        public TreeViewItem Go(Guid NetworkAdapter_Guid)
        {
            try
            {
                //LogFile.Write(String.Format("Debug. Go"), false);
                TreeViewItem TVI = new TreeViewItem();
                if (NetworkAdapter_Guid != Guid.Empty)
                {
                    TreeViewItem Parent_TVI = Sub_SearchParentRoot(NetworkAdapter_Guid); //Ищем рутовый NA в ветке
                    //LogFile.Write(String.Format("Debug. Нашли Parent {0}",Parent_TVI.Header), false);
                    ObservableCollection<TreeViewItem> Col = BuildCollection(Parent_TVI); //Строим от него вниз ветку подключений
                    //LogFile.Write(String.Format("Debug. построили подключения"), false);
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
                    TVI = Col2[0];
                }
                else
                {
                    System.Windows.MessageBox.Show("Id Network Adapter is Empty");
                }
                return TVI;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CableTree Create procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                Location_Id = Guid.Empty;
                return null;
            }
        }

        ObservableCollection<TreeViewItem> BuildCollection(TreeViewItem TVI)
        {
            //try
            //{
            TreeViewItem newItem = new TreeViewItem();
            TreeViewItem newItemCabel = new TreeViewItem();
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            Col.Add(TVI);
            Guid NA_Id = (Guid)TVI.Tag;
            bool t = true;
            while (t == true)
            {
                //newItem = SubChild(G);
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj = Get_Relationship(NA_Id);
                if (relobj != null)
                {
                    newItemCabel = new TreeViewItem();
                    newItemCabel.Header = "Cable.Footage:" + relobj[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value + ":" + relobj[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value;
                    newItemCabel.Tag = relobj.Id;
                    Col.Add(newItemCabel);
                    newItem = new TreeViewItem();
                    newItem.Header = relobj.SourceObject.FullName;
                    newItem.Tag = relobj.SourceObject.Id;
                    Col.Add(newItem);

                    //LogFile.Write(String.Format("Debug. relobj.SourceObject.FullName {0}", relobj.SourceObject.FullName), false);

                    NA_Id = (Guid)newItem.Tag;
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

        TreeViewItem Sub_SearchParentRoot(Guid G)
        {
           //try
           // {
            TreeViewItem newItem = null;
            Guid _G = G;
            bool x = true;
            while (x == true)
            {
                x = false;
                var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(_G, relChildNetworkAdapterRefParentNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    newItem = new TreeViewItem();
                    newItem.Header = relobj.TargetObject.FullName;
                    newItem.Tag = relobj.TargetObject.Id;
                    _G = relobj.TargetObject.Id;
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
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("Sub_SearchParentRoot procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return null;
            //}
        }


        //Ищем связанные NA. в случае нахождения модуля - проверяем связь с локацией и сохраняем Location_Id
        EnterpriseManagementRelationshipObject<EnterpriseManagementObject> Get_Relationship(Guid G)
        {
            //try
            //{
                EnterpriseManagementObject EMO = null;
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel = null; //new EnterpriseManagementRelationshipObject<EnterpriseManagementObject>();
                var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relChildNetworkAdapterRefParentNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    rel = relobj;
                    EMO = relobj.SourceObject;
                }
                if (classModule != null && EMO != null)
                {
                    if (EMO.IsInstanceOf(classModule))
                    {
                        var G_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(EMO.Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> confobj in G_objects)
                        {
                            Location_Id = confobj.TargetObject.Id;
                        }
                    }
                }
                return rel;
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("Get_Relationship procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return null;
            //}
        }
    }
}
