using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SMCenter.NetworkTasks
{

    public partial class MapForm : Window
    {
        EnterpriseManagementGroup GetSession()
        {
            try
            {
                IServiceContainer container = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession curSession = (Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession)container.GetService(typeof(Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession));
                if (curSession == null)
                    throw new ValueUnavailableException("curSession is null");
                return curSession.ManagementGroup;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(DateTime.Now + " : " + "GetSession Error : " + e.Message);
                return null;
            }
        }

        private void UIItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                    if (SelectedUI != RoomUI && RoomMoving == false)
                    {
                        if (SelectedUI != null)
                        {
                            SelectedUI.MaxHeight = 7;
                            SelectedUI = null;
                            this.CablesTreeView.Items.Clear();
                        }
                        else
                        {
                            SelectedUI = sender as MapUIElement;
                            if (SelectedUI != RoomUI)
                            {
                                SelectedUI.MaxHeight = 15;
                                foreach (Guid NA_Id in SelectedUI.Collection_NA_IDs)
                                {
                                    this.CablesTreeView.Items.Clear();
                                    Fill_CabelsTreeView(NA_Id);
                                }
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("UIItem_MouseLeftButtonDown procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void UIItem_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(this.Map as UIElement);
                int iX = (int)currentPosition.X;
                int iY = (int)currentPosition.Y;
                txtx.Text = iX.ToString();
                txty.Text = iY.ToString();

                if (isDragging && MovingUI != null)
                {
                    //if (RoomMoving == false)
                    //{
                        //Point currentUIPosition = e.GetPosition(SelectedUI as UIElement);
                        var transform = MovingUI.RenderTransform as TranslateTransform;
                        if (transform == null)
                        {
                            transform = new TranslateTransform();
                            MovingUI.RenderTransform = transform;
                        }
                        //double dX = currentPosition.X - clickPosition.X;
                        //double dY = currentPosition.Y - clickPosition.Y;
                        //Debug.WriteLine("-" + dX + ":" + dY);
                        transform.X = currentPosition.X - clickPosition.X;
                        transform.Y = currentPosition.Y - clickPosition.Y;
                        //transform.X = currentUIPosition.X;
                        //transform.Y = currentUIPosition.Y;
                        //selectedUI.Margin = new Thickness(0);
                    //}
                    //else
                    //{   
                    //    //if (RoominPosition == false)
                    //    //{
                    //    //    RoominPosition = true;
                    //    //    clickPosition = e.GetPosition(this.Map as UIElement);
                    //    //}
                    //    RoomUI.Opacity = 10;
                    //    //Canvas.SetLeft(RoomUI, -12);
                    //    //Canvas.SetTop(RoomUI, -12);
                    //    Canvas.SetLeft(RoomUI, 0);
                    //    Canvas.SetTop(RoomUI, 0);
                    //    var transform = MovingUI.RenderTransform as TranslateTransform;
                    //    if (transform == null)
                    //    {
                    //        transform = new TranslateTransform();
                    //        MovingUI.RenderTransform = transform;
                    //    }
                    //    //double dX = currentPosition.X - clickPosition.X;
                    //    //double dY = currentPosition.Y - clickPosition.Y;
                    //    //Debug.WriteLine("-" + dX + ":" + dY);
                    //    transform.X = currentPosition.X; //- clickPosition.X;
                    //    transform.Y = currentPosition.Y; //-clickPosition.Y;
                    //}
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("UIItem_MouseMove procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void UIItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(this.Map as UIElement);
                MovingUI.ReleaseMouseCapture();
                isDragging = false;
                //RoomMoving = false;
                if (MovingUI != RoomUI)
                {
                    EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relloc = _Rel.GetSingleRelationship(emg, relConfigItemRefLocation, MovingUI.ModuleId, true);
                    relloc[relConfigItemRefLocation, "X"].Value = (int)currentPosition.X - (int)currentUIPosition.X;
                    relloc[relConfigItemRefLocation, "Y"].Value = (int)currentPosition.Y - (int)currentUIPosition.Y;
                    relloc.Commit();
                }
                else
                {
                    TreeViewItem SelectedItem = (TreeViewItem)ModulesTreeView.SelectedItem;
                    Guid RoomId = (Guid)SelectedItem.Tag;
                    EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relloc = _Rel.GetSingleRelationship(emg, relLoctoLoc, RoomId, false);
                    relloc[relLoctoLoc, "X"].Value = (int)currentPosition.X-(int)currentUIPosition.X;
                    relloc[relLoctoLoc, "Y"].Value = (int)currentPosition.Y-(int)currentUIPosition.Y;
                    relloc.Commit();
                }

                Canvas.SetLeft(MovingUI, (int)currentPosition.X - (int)currentUIPosition.X);
                Canvas.SetTop(MovingUI, (int)currentPosition.Y - (int)currentUIPosition.Y);
                MovingUI.RenderTransform = null; 
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("UIItem_MouseRightButtonUp procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                //this.Close();
            }
        }
        private void UIItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //if (RoomMoving == false)
                //{
                    MovingUI = sender as MapUIElement;
                    MovingUI.AllowDrop = true;
                    MovingUI.CaptureMouse();

                    isDragging = true;
                    clickPosition = e.GetPosition(this.Map as UIElement);
                    currentUIPosition = e.GetPosition(MovingUI as UIElement);
               // }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("UIItem_MouseRightButtonDown procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }
        private void CabelsTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItem SelectedTreeItem = (TreeViewItem)this.CablesTreeView.SelectedItem;

                if (SelectedTreeItem.Header.ToString().Contains("Cable.Footage"))
                {

                }
                else
                {
                    //Get CurrentNode

                    Guid G = new Guid(SelectedTreeItem.Tag.ToString());
                    EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(G, ObjectQueryOptions.Default);

                    //Guid Id_NA = new Guid();
                    if (EMO.IsInstanceOf(classModule))
                    {
                        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classModule);
                        IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                        Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);

                    }
                    else if (EMO.IsInstanceOf(classNodePort))
                    {
                        var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                        {
                            EMO = relobj.SourceObject;
                        }
                        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classNode);
                        IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                        Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                    }
                    else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                    {
                        var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                        {
                            EMO = relobj.SourceObject;
                        }
                        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classDevice);
                        IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                        Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                    }
                    else if (EMO.IsInstanceOf(classComputerNetworkAdapter))
                    {
                        var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                        {
                            EMO = relobj.SourceObject;
                        }
                        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classWindowsComputer);
                        IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                        Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                    }
                    //else if (EMO.IsInstanceOf(classPatchPanelPort))
                    //{
                    //    var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(G, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    //    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
                    //    {
                    //        EMO = relobj.SourceObject;
                    //    }
                    //    EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(classPatchPanel);
                    //    IDataItem itemIdentity = dataType.CreateProxyInstance(EMO);
                    //    Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(itemIdentity);
                    //}
                    else
                    {
                        System.Windows.MessageBox.Show("Unknown object!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CabelsTreeView_MouseDoubleClick procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

        }
        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


        private void ModulesTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                int X;
                int Y;
                if (Map.Children.Contains(RoomUI)) { RoomUI.Opacity = 0; }

                if (SelectedUI != null && SelectedUI.ObjectType != MapUIElement.UIType.Room)
                { SelectedUI.MaxHeight = 7; }

                TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;
                Guid Guid_Item = new Guid(SelectedItem.Tag.ToString());

                if (Guid_Item == LinkedLocationID)//При выборе корневой локации - не отрисовываем Room UI
                { return; }

                //TreeViewItem SelectedItem = (TreeViewItem)ModulesTreeView.SelectedItem;
                //Ищем координаты комнаты и отрисовываем RoomUI
                //Guid TVI_Id = (Guid)SelectedItem.Tag;

                _LogFile.Write(String.Format("Debug. Guid_Item {0}", Guid_Item), false);

                if (Guid_Item != null)
                {
                    EnterpriseManagementObject sel_emo = emg.EntityObjects.GetObject<EnterpriseManagementObject>(Guid_Item, ObjectQueryOptions.Default);
                    if (sel_emo.IsInstanceOf(classLocation))
                    {
                        //Ищем координаты связи с родительской локацией
                        var confitems = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Guid_Item, relLoctoLoc, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in confitems)
                        {

                            if (rel[relLoctoLoc, "X"].Value == null) { X = 0; }
                            else
                            {
                                int.TryParse(rel[relLoctoLoc, "X"].Value.ToString(), out X);
                                //f = true;
                            }
                            if (rel[relLoctoLoc, "Y"].Value == null) { Y = 0; }
                            else
                            {
                                int.TryParse(rel[relLoctoLoc, "Y"].Value.ToString(), out Y);
                                //f = true;
                            }
                            if (Map.Children.Contains(RoomUI))
                            {
                                Canvas.SetLeft(RoomUI, X);
                                Canvas.SetTop(RoomUI, Y);
                                RoomUI.Opacity = 1;
                                //if (X == 0 && Y == 0)
                                //{ RoomUI.Opacity = 0; }
                                //else
                                //{ RoomUI.Opacity = 1; }
                            }
                        }
                    }
                    else
                    {
                        SelectedUI = (MapUIElement)Map.Children[EMOId_UI[Guid_Item]];
                        SelectedUI.MaxHeight = 15;
                    }
                }
                
                
                

                         //private Dictionary<String, Guid> UItoConfId = new Dictionary<String, Guid>();
                        //NAtoUI.Add(port.TargetObject.Id, mapUIElement1.Uid);
                        //UItoConfId.Add(UIRack.Uid, ConfItemId);
                        //string UI_Id = EMOId_UI[Guid_Item];
                        
                        
                        //if (SelectedUI != RoomUI)
                        //{
                        //    if (SelectedUI != null)
                        //    {
                        //    SelectedUI.MaxHeight = 7;
                        //    SelectedUI = null;
                        //    this.CablesTreeView.Items.Clear();
                        //    }
                        //    else
                        //    {
                        //        SelectedUI = sender as MapUIElement;
                        //        if (SelectedUI != RoomUI)
                        //        {
                        //            SelectedUI.MaxHeight = 15;
                        //            foreach (Guid NA_Id in SelectedUI.Collection_NA_IDs)
                        //            {
                        //                this.CablesTreeView.Items.Clear();
                        //                Fill_CabelsTreeView(NA_Id);
                        //            }
                        //        }
                        //    }
                    
                        //}


                this.CablesTreeView.Items.Clear();
                if (Guid_Item != null) { Fill_CabelsTreeView(Guid_Item); }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("ModulsTreeView_SelectedItemChanged procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                //this.Close();
            }
        }


        private void brd_Links_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateLinks();
        }

        private void CabelsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //try
            //{
            //    TreeViewItem SelectedItem = (TreeViewItem)e.NewValue;
            //    if (!(SelectedItem.Header.ToString().Contains("Cable")))
            //    {
            //        Guid Guid = new Guid(SelectedItem.Tag.ToString());
            //        var items = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Guid, relLocationContainsConfigItem, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
            //        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in items)
            //        {
            //            EnterpriseManagementObject E = relobj.TargetObject;
            //            //this.txtLocation.Text = E[classLocation, "Path"].Value.ToString();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //    this.Close();
            //}
        }

        private void ModulesTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    int X;
            //    int Y;
            //    bool f = false;
            //    RoomUI.Opacity = 1;

            //    TreeViewItem SelectedItem = (TreeViewItem)ModulesTreeView.SelectedItem;
            //    //Ищем координаты комнаты и отрисовываем RoomUI
            //    //Если координат нет - то отрисовываем на точке 0-20
            //    Guid TVI_Id = (Guid)SelectedItem.Tag;
            //    //Ищем координаты связи с родительской локацией
            //    var loctolocRels = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(TVI_Id, relLoctoLoc, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
            //    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in loctolocRels)
            //    {
            //        if (rel.TargetObject.IsInstanceOf(classLocation))
            //        {
            //            //ConfItemId = rel.TargetObject.Id;
            //            //bool boolType = false;
                        
            //            if (rel[relLoctoLoc, "X"].Value == null) { X = 0; }
            //            else
            //            { 
            //                int.TryParse(rel[relLoctoLoc, "X"].Value.ToString(), out X);
            //                f = true;
            //            }
            //            if (rel[relLoctoLoc, "Y"].Value == null) { Y = 0; }
            //            else
            //            { 
            //                int.TryParse(rel[relLoctoLoc, "Y"].Value.ToString(), out Y); 
            //                f = true;
            //            }
            //            //System.Windows.MessageBox.Show("Нашли связь! Х = " + X + " Y = " + Y);
            //            Canvas.SetLeft(RoomUI, X);
            //            Canvas.SetTop(RoomUI, Y);
            //        }
            //    }
            //    if (f)
            //    {
                    
            //    }
            //    else
            //    {
            //        RoomMoving = true;
            //        isDragging = true;
            //        MovingUI = RoomUI;
            //        MovingUI.AllowDrop = true;
            //        MovingUI.CaptureMouse();
            //    }
            //}
            //catch (Exception exc)
            //{
            //    System.Windows.MessageBox.Show("ModulesTreeView_MouseDoubleClick procedure error : " + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
