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
    //Class для создания ветки дерева кабельных соединений с определенным NetworkAdapter
    public class CablesTree
    {
        //static ManagementPack mpNetworkAssetLibrary;
        private ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter { get; set; }
        private ManagementPackClass classModule { get; set; }
        private ManagementPackClass classNAPort { get; set; }
        //private ManagementPackClass classPPP { get; set; }
        private ManagementPackClass classNodePort { get; set; }
        private ManagementPackClass classDeviceNA { get; set; }
        private ManagementPackClass classComputerNA { get; set; }
        private ManagementPackRelationship relConfigItemRefLocation { get; set; }
        private EnterpriseManagementGroup emg { get; set; }
        public Guid Location_Id { get; set; }
        public TreeViewItem TreeViewItem { get; set; }
        private LogFile LogFile;
        public EnterpriseManagementObject emo_ClientNA { get; set; }
        public EnterpriseManagementObject emo_NodePort { get; set; }
        public EnterpriseManagementObject emo_ParentPort { get; set; }
        public EnterpriseManagementObject emo_PPP1 { get; set; }
        public EnterpriseManagementObject emo_PPP2 { get; set; }
        public EnterpriseManagementObject emo_Module { get; set; }

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

        public void SetClasses(ManagementPackClass _classModule, ManagementPackClass _classNAPort, ManagementPackClass _classNodePort, ManagementPackClass _classDeviceNA, ManagementPackClass _classComputerNA)
        {
            classModule = _classModule;
            classNAPort = _classNAPort;
            //classPPP = _classPPP;
            classNodePort = _classNodePort;
            classDeviceNA = _classDeviceNA;
            classComputerNA = _classComputerNA;
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
        private TreeViewItem Go(Guid NetworkAdapter_Guid)
        {
            //try
            //{
                //LogFile.Write(String.Format("Debug. Go"), false);
                TreeViewItem TVI = new TreeViewItem();
                if (NetworkAdapter_Guid != Guid.Empty)
                {
                    TreeViewItem Parent_TVI = Sub_SearchParentRoot(NetworkAdapter_Guid); //Ищем рутовый NA в ветке
                    //LogFile.Write(String.Format("Debug. Нашли Parent {0}",Parent_TVI.Header), false);
                    //Строим от него вниз ветку подключений
                    
                    ObservableCollection<TreeViewItem> Col = BuildCollection(Parent_TVI);
                    //Col - коллекция TVI которая содержит упорядоченные по порядковому номеру элементы, которые мы, используя Col2,
                    // упорядоченно подключаем друг к другу.  
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
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("CableTree Create procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Location_Id = Guid.Empty;
            //    return null;
            //}
        }

        ObservableCollection<TreeViewItem> BuildCollection(TreeViewItem ParentTVI)
        {
            //try
            //{
            TreeViewItem newItem = new TreeViewItem();
            TreeViewItem newItemCabel = new TreeViewItem();
            ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
            Col.Add(ParentTVI);
            Guid NA_Id = (Guid)ParentTVI.Tag;
            bool t = true;
            while (t == true)
            {
                //newItem = SubChild(G);
                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj = Get_Relationship(NA_Id);
                if (relobj != null)
                {
                    newItemCabel = new TreeViewItem();
                    newItemCabel.Header = "Cable.Footage:" + relobj[relChildNetworkAdapterRefParentNetworkAdapter, "ParentFootage"].Value + ":" + relobj[relChildNetworkAdapterRefParentNetworkAdapter, "ChildFootage"].Value;
                    newItemCabel.Tag = relobj.Id;
                    Col.Add(newItemCabel);
                    newItem = new TreeViewItem();
                    newItem.Header = relobj.SourceObject.FullName;
                    newItem.Tag = relobj.SourceObject.Id;
                    Col.Add(newItem);

                    //Set Objects
                    if (classNodePort != null)
                    {
                        if (relobj.SourceObject.IsInstanceOf(classModule)) emo_Module = relobj.SourceObject;
                        else if (relobj.SourceObject.IsInstanceOf(classNAPort))
                        {
                            if (emo_PPP1 == null) emo_PPP1 = relobj.SourceObject;
                            else emo_PPP2 = relobj.SourceObject;
                        }
                        else if (relobj.SourceObject.IsInstanceOf(classNodePort)) emo_NodePort = relobj.SourceObject; // логике этот вариант никогда не наступит
                    }
                    if (relobj.SourceObject.IsInstanceOf(classDeviceNA) || relobj.SourceObject.IsInstanceOf(classComputerNA)) emo_ClientNA = relobj.SourceObject;

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

            //if (newItem == null)
            //{
                emo_ParentPort = emg.EntityObjects.GetObject<EnterpriseManagementObject>(_G, ObjectQueryOptions.Default);
                newItem = new TreeViewItem();
                newItem.Header = emo_ParentPort.FullName;
                newItem.Tag = emo_ParentPort.Id;
            //}
            //Сохраняем родительский элемент как NodePort. В обычной ситуации это должно быть NodePort.
            if (classNodePort != null)
            {
                if (emo_ParentPort.IsInstanceOf(classNodePort)) emo_NodePort = emo_ParentPort;
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
