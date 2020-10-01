using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.Diagnostics;
using System.Windows.Controls;

namespace SMCenter.Forms
{
    public static class FillTreeView
    {
        public static void Now(EnterpriseManagementGroup emg, ManagementPackClass objectClass, ManagementPackRelationship relationClass, TreeView LocalTreeView)
        {

            //System.Windows.MessageBox.Show("FillTreeView Call");
            //return;

            //Trace.WriteLine(DateTime.Now + " : " + "Enter FillTreeView.Now");
            ConfClass CC = new ConfClass();

            //Delete old Items
            LocalTreeView.Items.Clear();

            //=========================================================================================================
            //Create Root
            //=========================================================================================================
            try
            {
                List<EnterpriseManagementObject> arr = new List<EnterpriseManagementObject>();
                //Создаем массив локаций arr
                foreach (var loc in emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(objectClass, ObjectQueryOptions.Default))
                {
                    //Trace.WriteLine("Перебираем локации " + loc.DisplayName);
                    var parentloc = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(loc.Id, relationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    // Ищем корневые узлы
                    if (parentloc.Count == 0)
                    {
                        //Trace.WriteLine("Нашли Root " + loc.DisplayName);
                        TreeViewItem Root = new TreeViewItem();
                        Root.Header = loc.DisplayName;
                        Root.Tag = loc.Id;
                        LocalTreeView.Items.Add(Root);
                    }
                    else
                    { arr.Add(loc); }
                }
                //Trace.WriteLine(DateTime.Now + " : " + "Массив некорневых узлов arr.Count= " + arr.Count);
                if (arr.Count > 0)
                {
                    //==========foreach locations========================
                    int k = 0;
                    do
                    {
                        //Trace.WriteLine("k=" + k + " arr.Count = " + arr.Count);
                        //============= search in arr parent elements ================
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(arr[k].Id, relationClass, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                        {
                            //Trace.WriteLine("Элемент массива " + arr[k].DisplayName + " - Нашли родительский элемент " + obj.SourceObject.DisplayName);

                            //Ищем среди существующих TreeViewItems родительский элемент Pi и цепляем к нему новый
                            foreach (TreeViewItem Pi in LocalTreeView.Items)
                            {
                                if ((System.Guid)Pi.Tag == obj.SourceObject.Id)
                                {
                                    TreeViewItem Ci = new TreeViewItem();
                                    Ci.Header = obj.TargetObject.DisplayName;
                                    Ci.Tag = obj.TargetObject.Id;
                                    //Trace.WriteLine("В теле добавляем к " + Pi.Header + " дочернюю ноду - " + Ci.Header);
                                    Pi.Items.Add(Ci);
                                    //Удаляем обработанный элемент из массива
                                    EnterpriseManagementObject delItem = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(arr[k].Id.ToString()), ObjectQueryOptions.Default);
                                    arr.Remove(delItem);
                                }
                                else if (Pi.HasItems)
                                {
                                    if (CC.ChildItems(Pi, obj.TargetObject.DisplayName, obj.TargetObject.Id, obj.SourceObject.Id))
                                    {
                                        EnterpriseManagementObject delItem = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(arr[k].Id.ToString()), ObjectQueryOptions.Default);
                                        arr.Remove(delItem);
                                    }
                                }
                            }
                        }
                        if (k >= arr.Count - 1) { k = 0; }
                        else { k++; }
                    } while (arr.Count > 0);
                }
                //Trace.WriteLine(DateTime.Now + " : " + "End void " + LocalTreeView.Items.Count);
            }
            catch (Exception e)
            { System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in FillTreeview : " + e.Message); }

        }

    }

    class ConfClass
    {
        public bool ChildItems(TreeViewItem Ci, string DisplayName, System.Guid TargetID, System.Guid SourceID)
        {
            try
            {
                bool r2 = false;
                bool r = false;
                foreach (TreeViewItem CCi in Ci.Items)
                {
                    if (CCi.Tag.ToString() == SourceID.ToString())
                    {
                        TreeViewItem Ni = new TreeViewItem();
                        Ni.Header = DisplayName;
                        Ni.Tag = TargetID.ToString();
                        //Trace.WriteLine("В ConfClass добавляем к " + CCi.Header + " дочернюю ноду - " + Ni.Header);
                        CCi.Items.Add(Ni);
                        //Trace.WriteLine("After === В ConfClass добавляем к " + CCi.Header + " дочернюю ноду - " + Ni.Header);
                        r2 = true;
                    }
                    else if (CCi.HasItems)
                    {
                        //Trace.WriteLine("Создаем вложенный ConfClass для  " + CCi.Header);
                        ConfClass CClass = new ConfClass();
                        r2 = CClass.ChildItems(CCi, DisplayName, TargetID, SourceID);
                    }
                    if (r2) { r = true; }
                }
                return r;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "Error in ConfClass.ChildItems : " + e.Message);
                return false;
            }
        }
    }
}
