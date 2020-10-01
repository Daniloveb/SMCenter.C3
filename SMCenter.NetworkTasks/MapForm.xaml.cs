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

        EnterpriseManagementGroup emg;
        ManagementPack mpAssetCore;
        ManagementPack mpNetworkAssetLibrary;
        ManagementPack mpNetworkLibrary;
        ManagementPack mpWindows;
        ManagementPack mpCMLibrary;

        ManagementPackClass classModule;
        ManagementPackClass classWindowsComputer;
        ManagementPackClass classDeployedComputer;
        ManagementPackClass classComputerNetworkAdapter;
        ManagementPackClass classNode;
        ManagementPackClass classNodePort;
        ManagementPackClass classLocation;
        ManagementPackClass classRack;
        ManagementPackClass classDevice;
        ManagementPackClass classDeviceNetworkAdapter;
        ManagementPackClass classPatchPanel;
        //ManagementPackClass classPatchPanelPort;
        ManagementPackClass classNetworkMap;
        ManagementPackClass targetClass;

        ManagementPackRelationship relComputerRunsWindowsComputer;
        ManagementPackRelationship relComputerHostsComputerNetworkAdapter;
        //ManagementPackRelationship relNetworkAdapterHasChildNetworkAdapter;
        ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter;
        ManagementPackRelationship relLoctoLoc;
        ManagementPackRelationship relConfigItemRefLocation;
        ManagementPackRelationship relNodeComposedOfNetworkAdapter;
        ManagementPackRelationship relDeviceHostNetworkAdapter;
        ManagementPackRelationship relConfigItemRefRack;
        ManagementPackRelationship relNetworkMapRefLocation;

        EnterpriseManagementObject EMO;
        EnterpriseManagementObject NetworkMap;
        EnterpriseManagementObject LinkedLocation;

        CablesTree CablesTree;

        private Dictionary<Guid, int> EMOId_UI = new Dictionary<Guid, int>();
        //private Dictionary<Guid, String> NAIds = new Dictionary<Guid, String>();

        //UIComputer selectedUI;
        MapUIElement MovingUI;
        MapUIElement SelectedUI;
        MapUIElement RoomUI;
        //MapUIElement CircleUI;
        protected bool isDragging;
        protected bool RoomMoving;
        private Point clickPosition;
        private Point currentUIPosition;

        bool boolShowLinks = false;

        Guid ParamObjectId;
        Guid LinkedLocationID;
        LogFile _LogFile;

        public MapForm(IDataItem IDataItem)
        {
            InitializeComponent();

            try
            {
                emg = GetSession();
                //emg = new EnterpriseManagementGroup("SM");
                mpWindows = emg.ManagementPacks.GetManagementPack("Microsoft.Windows.Library", "31bf3856ad364e35", new Version());
                mpAssetCore = emg.ManagementPacks.GetManagementPack("SMCenter.AssetManagement.Core", "75b45bd6835084b1", new Version());
                mpNetworkAssetLibrary = emg.ManagementPacks.GetManagementPack("SMCenter.NetworkAssetManagement.Library", "75b45bd6835084b1", new Version());
                mpNetworkLibrary = emg.GetManagementPack("System.NetworkManagement.Library", "31bf3856ad364e35", new Version());
                mpCMLibrary = emg.ManagementPacks.GetManagementPack("Microsoft.SystemCenter.ConfigurationManager", "31bf3856ad364e35", new Version());
                classModule = emg.EntityTypes.GetClass("SMCenter.Module", mpNetworkAssetLibrary);
                classWindowsComputer = emg.EntityTypes.GetClass("SMCenter.WindowsComputer", mpNetworkAssetLibrary);
                classDeployedComputer = emg.EntityTypes.GetClass("SMCenter.DeployedComputer", mpNetworkAssetLibrary);
                classComputerNetworkAdapter = emg.EntityTypes.GetClass("Microsoft.Windows.ComputerNetworkAdapter", mpWindows);
                classNode = emg.EntityTypes.GetClass("SMCenter.Node", mpNetworkAssetLibrary);
                classNodePort = emg.EntityTypes.GetClass("SMCenter.NodePort", mpNetworkAssetLibrary);
                classLocation = emg.EntityTypes.GetClass("SMCenter.Location", mpAssetCore);
                classRack = emg.EntityTypes.GetClass("SMCenter.Rack", mpNetworkAssetLibrary);
                classDevice = emg.EntityTypes.GetClass("SMCenter.Device", mpNetworkAssetLibrary);
                classDeviceNetworkAdapter = emg.EntityTypes.GetClass("SMCenter.Device.NetworkAdapter", mpNetworkAssetLibrary);
                classPatchPanel = emg.EntityTypes.GetClass("SMCenter.PatchPanel", mpNetworkAssetLibrary);
                //classPatchPanelPort = emg.EntityTypes.GetClass("SMCenter.PatchPanelPort", mpNetworkAssetLibrary);
                classNetworkMap = emg.EntityTypes.GetClass("SMCenter.NetworkMap", mpNetworkAssetLibrary);
                relComputerRunsWindowsComputer = emg.EntityTypes.GetRelationshipClass("Microsoft.SystemCenter.ConfigurationManager.DeployedComputerRunsWindowsComputer", mpCMLibrary);
                relComputerHostsComputerNetworkAdapter = emg.EntityTypes.GetRelationshipClass("Microsoft.Windows.ComputerHostsComputerNetworkAdapter", mpWindows);
                relLoctoLoc = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsChildLocation", mpAssetCore);
                relChildNetworkAdapterRefParentNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.ChildNetworkAdapterRefParentNetworkAdapter", mpNetworkAssetLibrary);
                //relNetworkAdapterHasChildNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkAdapterHasChildNetworkAdapter", mpNetworkAssetLibrary);
                //relLocationContainsConfigItem = emg.EntityTypes.GetRelationshipClass("SMCenter.LocationContainsConfigItem", mpAssetCore);
                relConfigItemRefLocation = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefLocation", mpAssetCore);
                relNodeComposedOfNetworkAdapter = emg.EntityTypes.GetRelationshipClass("System.NetworkManagement.NodeComposedOfNetworkAdapter", mpNetworkLibrary);
                relDeviceHostNetworkAdapter = emg.EntityTypes.GetRelationshipClass("SMCenter.DeviceHostsNetworkAdapter", mpNetworkAssetLibrary);
                relConfigItemRefRack = emg.EntityTypes.GetRelationshipClass("SMCenter.ConfigItemRefRack", mpNetworkAssetLibrary);
                relNetworkMapRefLocation = emg.EntityTypes.GetRelationshipClass("SMCenter.NetworkMapRefLocation", mpNetworkAssetLibrary);

                _LogFile = new LogFile(@"C:\MapFormLogFile.txt", true);

                CablesTree = new CablesTree(emg, relChildNetworkAdapterRefParentNetworkAdapter, classModule, relConfigItemRefLocation, _LogFile);

                SelectTreeViewObjectHandlerClass.EventHandler = new SelectTreeViewObjectHandlerClass.EMOEvent(Location_In);

                
                ParamObjectId = (Guid)IDataItem["$Id$"];
                NetworkMap = GetNetworkMap(ParamObjectId);
                RoomUI = null;

                
                //WriteTextLogHandlerClass.EventHandler = new WriteTextLogHandlerClass.WriteLog(WriteLog);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Initialize procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        

        private void Location_In(EnterpriseManagementObject _LocationEMO)
        {
            try
            {
                LinkedLocation = _LocationEMO;
                NetworkMap = GetNetworkMap(_LocationEMO.Id);

                this.txtTargetObject.Text = _LocationEMO.FullName;

                if (NetworkMap == null)
                {
                    System.Windows.MessageBox.Show("Объект не связан с Картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (LinkedLocation == null)
                {
                    System.Windows.MessageBox.Show("Карта сети не связана с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                //Очищаем Treeview
                this.ModulesTreeView.Items.Clear();

                var images = Map.Children.OfType<MapUIElement>().ToList();
                foreach (var image in images)
                {
                    Map.Children.Remove(image);
                }

                this.lbLocation.Content = LinkedLocation.DisplayName;
                FillModulesTreeView(LinkedLocation);

                FillImage(NetworkMap);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Location_In procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void FillModulesTreeView(EnterpriseManagementObject LinkedLocation)
        {
            try
            {
                LinkedLocationID = LinkedLocation.Id;
                //Создаем RoomUI
                RoomUI = new MapUIElement(MapUIElement.UIType.Room, MapUIElement.NetworkAdapterType.Computer);
                RoomUI.ObjectId = Guid.NewGuid();
                Canvas.SetLeft(RoomUI, 0);
                Canvas.SetTop(RoomUI, 0);
                RoomUI.Uid = Guid.NewGuid().ToString();
                //UIComputer.ToolTip = Item_Module.Header + ":" + Item_Computer.Header;
                //RoomUI.Description = Item_Module.Header + ":" + Item_Computer.Header;
                RoomUI.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                RoomUI.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                RoomUI.MouseMove += UIItem_MouseMove;
                RoomUI.MouseLeftButtonDown += UIItem_MouseLeftButtonDown;
                Map.Children.Add(RoomUI);
                Canvas.SetZIndex(RoomUI, 5);
                RoomUI.Opacity = 0;

                #region
                //EMOId_UI.Add(Guid.NewGuid(), Map.Children.IndexOf(RoomUI));

                ////Создаем RoomUI виде круга
                //Ellipse CircleUI = new Ellipse();
                //// Create a SolidColorBrush with a red color to fill the 
                //// Ellipse with.
                //SolidColorBrush mySolidColorBrush = new SolidColorBrush();

                //// Describes the brush's color using RGB values. 
                //// Each value has a range of 0-255.
                ////mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                ////myEllipse.Fill = mySolidColorBrush;
                //CircleUI.StrokeThickness = 2;
                //CircleUI.Stroke = Brushes.Red;
                //CircleUI.Width = 30;
                //CircleUI.Height = 30;
                //// Add the Ellipse to the StackPanel.
                //Canvas.SetLeft(CircleUI, 0);
                //Canvas.SetTop(CircleUI, 20);
                //Map.Children.Add(CircleUI);
                ////CircleUI.Opacity = 0;

                #endregion


                int XX = 0;
                int YY = 0;
                
                bool haveLocCoordinates;
                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();

                ObservableCollection<TreeViewItem> ColModules = new ObservableCollection<TreeViewItem>();
                ObservableCollection<TreeViewItem> ColNodes = new ObservableCollection<TreeViewItem>();
                ObservableCollection<TreeViewItem> ColRacks = new ObservableCollection<TreeViewItem>();

                //Работаем с основной локацией LinkedLocation
                _LogFile.Write(String.Format("Работаем с основной локацией LinkedLocation {0}", LinkedLocation.DisplayName), false);
                TreeViewItem LinkedLocationTVI = new TreeViewItem();
                LinkedLocationTVI.Header = LinkedLocation.DisplayName;
                LinkedLocationTVI.Tag = LinkedLocation.Id;
                LinkedLocationTVI.IsExpanded = true;
                //Col.Add(LinkedLocationTVI);
                this.ModulesTreeView.Items.Add(LinkedLocationTVI);

                FitCollection(LinkedLocation, false, XX, YY, out ColModules, out ColNodes, out ColRacks);

                var _sortedColModules = from item in ColModules orderby item.Header select item;
                foreach (TreeViewItem TVI in _sortedColModules)
                {
                    LinkedLocationTVI.Items.Add(TVI);
                }
                var _sortedColNodes = from item in ColNodes orderby item.Header select item;
                foreach (TreeViewItem TVI in _sortedColNodes)
                {
                    LinkedLocationTVI.Items.Add(TVI);
                }
                var _sortedColRacks = from item in ColRacks orderby item.Header select item;
                foreach (TreeViewItem TVI in _sortedColRacks)
                {
                    LinkedLocationTVI.Items.Add(TVI);
                }


                //LinkedLocation это локация соединенная с NetworkMap. К канвасу привязываем все дочерние локации.
                //========Ищем дочерние локации===================================
                var childlocations = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(LinkedLocation.Id, relLoctoLoc, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in childlocations)
                {
                    _LogFile.Write(String.Format("Локация {0}", LinkedLocation.DisplayName), false);
                    //Сохраняем координаты дочерней локации
                    haveLocCoordinates = false;
                    if (relobj[relLoctoLoc, "X"].Value == null) { XX = 0; }
                    else
                    { int.TryParse(relobj[relLoctoLoc, "X"].Value.ToString(), out XX); }
                    if (relobj[relLoctoLoc, "Y"].Value == null) { YY = 0; }
                    else
                    { int.TryParse(relobj[relLoctoLoc, "Y"].Value.ToString(), out YY); }

                    if (XX != 0 && YY != 0)  {  haveLocCoordinates = true;    }

                    //для каждой дочерней локации создаем TreeviewItem
                    TreeViewItem Root = new TreeViewItem();
                    Root.Header = relobj.TargetObject.DisplayName;
                    if (haveLocCoordinates == false) { Root.Header = Root.Header + "*"; }
                    Root.Tag = relobj.TargetObject.Id;
                    Root.IsExpanded = true;
                    Col.Add(Root);

                    FitCollection(relobj.TargetObject, haveLocCoordinates, XX, YY, out ColModules, out ColNodes, out ColRacks);

                    

                    var sortedColModules = from item in ColModules orderby item.Header select item;
                    foreach (TreeViewItem TVI in sortedColModules)
                    {
                        Root.Items.Add(TVI);
                    }
                    var sortedColNodes = from item in ColNodes orderby item.Header select item;
                    foreach (TreeViewItem TVI in sortedColNodes)
                    {
                        Root.Items.Add(TVI);
                    }
                    var sortedColRacks = from item in ColRacks orderby item.Header select item;
                    foreach (TreeViewItem TVI in sortedColRacks)
                    {
                        Root.Items.Add(TVI);
                    }
                }
                var sortedCol = from item in Col orderby item.Header select item;
                foreach (TreeViewItem TVI in sortedCol)
                {
                    this.ModulesTreeView.Items.Add(TVI);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("FillModulesTreeView procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }



        //Алгоритм рисования связей в 2 цикла по UIElements.
        //- Первый цикл по UI объектам на канвасе
        //- Смотрим на тип UI Object, заполняем словарь (NA_Id):(UIOject_Id) и внутреннюю коллекцию для UI_Object - коллекцию NA-Ids.(в случае модуля - компьютера - девайса NA_Id уже заполнен)
        //- Второй цикл. Первый UIElement
        //- В UI объекте перебираем внутренний массив NA-Ids.
        //- Находим Cable связь-NAtoNA. При переборе используем только например Target
        //- Ищем в словаре второй UIElement2  - связанный c найденным вторым NA. NA_Id:UI_Object. 
        //- Создаем объект Line 


        //==============================================================================================================
        //Первый цикл
        //==============================================================================================================
        Dictionary<Guid, String> Create_NAtoUI()
        {
            try
            {
                Dictionary<Guid, String> NAtoUI = new Dictionary<Guid, String>();
                MapUIElement mapUIElement1;
                foreach (UIElement i in Map.Children)
                {
                    try
                    { mapUIElement1 = (MapUIElement)i; }
                    catch
                    { continue; }
                    Guid NA_Id = Guid.Empty;
                    _LogFile.Write(String.Format("Заполнение словаря"), false);
                    //Заполнение словаря NAtoUI (для быстрого поиска связанного элемента)
                    switch (mapUIElement1.NetworkAdapterObjectType)
                    {
                        case MapUIElement.NetworkAdapterType.Module:
                            _LogFile.Write(String.Format("модуль"), false);
                            foreach (Guid g in mapUIElement1.Collection_NA_IDs) { if (NAtoUI.ContainsKey(g) == false) NAtoUI.Add(g, mapUIElement1.Uid); }
                            break;
                        case MapUIElement.NetworkAdapterType.Computer:
                            _LogFile.Write(String.Format("компьютер"), false);
                            foreach (Guid g in mapUIElement1.Collection_NA_IDs) { if (NAtoUI.ContainsKey(g) == false) NAtoUI.Add(g, mapUIElement1.Uid); }
                            break;
                        case MapUIElement.NetworkAdapterType.Device:
                            _LogFile.Write(String.Format("девайс"), false);
                            foreach (Guid g in mapUIElement1.Collection_NA_IDs) { if (NAtoUI.ContainsKey(g) == false) NAtoUI.Add(g, mapUIElement1.Uid); }
                            break;
                        case MapUIElement.NetworkAdapterType.Node:
                            _LogFile.Write(String.Format("нода"), false);
                            var ports = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(mapUIElement1.ObjectId, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in ports)
                            {
                                NAtoUI.Add(rel.TargetObject.Id, mapUIElement1.Uid);
                                mapUIElement1.Collection_NA_IDs.Add(rel.TargetObject.Id);
                            }
                            break;
                        case MapUIElement.NetworkAdapterType.Rack:
                            _LogFile.Write(String.Format("шкаф"), false);
                            //Перебираем содержимое шкафа
                            var confitems = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(mapUIElement1.ObjectId, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in confitems)
                            {
                                //Noda
                                if (rel.SourceObject.IsInstanceOf(classNode))
                                {
                                    ports = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(rel.SourceObject.Id, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> port in ports)
                                    {
                                        NAtoUI.Add(port.TargetObject.Id, mapUIElement1.Uid);
                                        mapUIElement1.Collection_NA_IDs.Add(port.TargetObject.Id);
                                    }
                                }//Device
                                else if (rel.SourceObject.IsInstanceOf(classDevice))
                                {
                                    ports = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(rel.SourceObject.Id, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> port in ports)
                                    {
                                        NAtoUI.Add(port.TargetObject.Id, mapUIElement1.Uid);
                                        mapUIElement1.Collection_NA_IDs.Add(port.TargetObject.Id);
                                    }
                                }

                            }
                            break;
                    }
                }
                return NAtoUI;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Create_NAtoUI procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return null;
            }
        }

        void CreateLinks()
        {
            try
            {
                //Первый цикл по UIElement. собираем словарь NAtoUI
                Dictionary<Guid, String> NAtoUI = Create_NAtoUI();
                MapUIElement mapUIElement1;
                MapUIElement mapUIElement2;
                if (boolShowLinks)
                {
                    //Удаляем все линии
                    UIElement[] UICol = new UIElement[Map.Children.Count];
                    Map.Children.CopyTo(UICol, 0);
                    foreach (UIElement i in UICol)
                    {
                        try
                        {
                            Line l = (Line)i;
                            Map.Children.Remove(i);
                        }
                        catch
                        { continue; }
                    }
                    boolShowLinks = false;
                }
                else
                {
                    //======================================================================================================================
                    //Второй раз пробегаем по объектам на канвасе и используя словарь - рисуем связи
                    //======================================================================================================================
                    mapUIElement1 = null;
                    UIElement[] UICol = new UIElement[Map.Children.Count];
                    Map.Children.CopyTo(UICol, 0);
                    foreach (UIElement i in UICol)
                    {
                        try
                        { mapUIElement1 = (MapUIElement)i; }
                        catch
                        { continue; }
                        //Перебираем внутреннюю коллекцию NA UIElement
                        foreach (Guid NA_Id in mapUIElement1.Collection_NA_IDs)
                        {
                            //Ищем парный связанный родительский NA - Cable связь
                            var links = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(NA_Id, relChildNetworkAdapterRefParentNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> link in links)
                            {
                                string UIElement2Id = null;
                                mapUIElement2 = null;
                                //Находим ID родительского UIElement2 из собранного на первом цикле словаря
                                if (NAtoUI.TryGetValue(link.TargetObject.Id, out UIElement2Id))
                                {
                                    //Убираем варианты связей Модуль-Компьютер
                                    if (mapUIElement1.Uid == UIElement2Id)
                                    {
                                        continue;
                                    }
                                    //Находим UIElement2
                                    foreach (UIElement ii in Map.Children)
                                    {
                                        if (ii.Uid == UIElement2Id)
                                        {
                                            try
                                            { mapUIElement2 = (MapUIElement)ii; }
                                            catch
                                            { continue; }
                                        }
                                    }

                                    Line line = new Line();
                                    Point PointUI1 = mapUIElement1.TransformToAncestor(Map).Transform(new Point(0, 0));
                                    line.X1 = PointUI1.X + 3;
                                    line.Y1 = PointUI1.Y + 3;
                                    Point PointUI2 = mapUIElement2.TransformToAncestor(Map).Transform(new Point(0, 0));
                                    line.X2 = PointUI2.X + 3;
                                    line.Y2 = PointUI2.Y + 3;
                                    line.Stroke = Brushes.Red;
                                    line.StrokeThickness = 0.2;
                                    Map.Children.Add(line);
                                    Canvas.SetZIndex(line, 2);
                                }
                                else //рассматриваем NA не в словаре
                                { 
                                    EnterpriseManagementObject Port = emg.EntityObjects.GetObject<EnterpriseManagementObject>(link.TargetObject.Id,ObjectQueryOptions.Default);
                                    if (Port.IsInstanceOf(classNodePort))
                                    {
                                        //Находим связанную Node
                                        EnterpriseManagementObject Node = null;
                                        var rels = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Port.Id, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in rels)
                                        {
                                            Node = rel.SourceObject;
                                        }
                                        if (Node == null)
                                        {
                                            _LogFile.Write(String.Format("Error! При добавлении внешних NA: NodePort не связан с Node. Port={0}", Port.DisplayName), true);
                                            continue;
                                        }
                                        //_LogFile.Write(String.Format("Debug! mapUIElement1.Uid={0}", mapUIElement1.Uid), true);
                                        //Ищем UI
                                        mapUIElement2=null;
                                        foreach (UIElement ui in Map.Children)
                                        {
                                            if (ui.Uid == Node.Id.ToString())
                                            {
                                                try
                                                { mapUIElement2 = (MapUIElement)ui; }
                                                catch
                                                {
                                                    _LogFile.Write(String.Format("Error! Ошибка конвертации UI to MapUIElement."), true);
                                                    continue;
                                                }
                                            }
                                        }
                                        if (mapUIElement2 == null)
                                        {
                                            
                                            //Создаем UI

                                            mapUIElement2 = new MapUIElement(MapUIElement.UIType.Node, MapUIElement.NetworkAdapterType.Node);
                                            mapUIElement2.ObjectId = Node.Id;
                                            Random rnd = new Random();
                                            Canvas.SetLeft(mapUIElement2, rnd.Next(200, 800));
                                            //Canvas.SetLeft(mapUIElement2, 50);
                                            Canvas.SetTop(mapUIElement2, 50);
                                            mapUIElement2.Uid = Node.Id.ToString();
                                            string tooltip = Node[classNode, "GeoName"].Value.ToString() + ":" + Node[classNode, "AssetKey"].Value.ToString();
                                            mapUIElement2.ToolTip = tooltip;
                                            mapUIElement2.Description = tooltip;

                                            Map.Children.Add(mapUIElement2);
                                            Canvas.SetZIndex(mapUIElement2, 5);

                                        }
                                        
                                        _LogFile.Write(String.Format("Debug! mapUIElement1.Uid={0} mapUIElement2.Uid={1}", mapUIElement1.Uid, mapUIElement2.Uid), true);
                                        
                                        //Рисуем линию
                                        Line line = new Line();
                                        Point PointUI1 = mapUIElement1.TransformToAncestor(Map).Transform(new Point(0, 0));
                                        line.X1 = PointUI1.X + 3;
                                        line.Y1 = PointUI1.Y + 3;
                                        _LogFile.Write(String.Format("Debug! PointUI1.X={0} PointUI1.Y={1}", PointUI1.X, PointUI1.Y), true);
                                        //Point PointUI2 = mapUIElement2.TransformToAncestor(Map).Transform(new Point(0, 0));
                                       
                                        //line.X2 = PointUI2.X + 3;
                                        //line.Y2 = PointUI2.Y + 3;
                                        line.X2 = Canvas.GetLeft(mapUIElement2)+3;
                                        line.Y2 = Canvas.GetTop(mapUIElement2)+3;
                                        line.Stroke = Brushes.Blue;
                                        line.StrokeThickness = 0.2;
                                        Map.Children.Add(line);
                                        Canvas.SetZIndex(line, 2);

                                    }
                                    else
                                    {
                                        _LogFile.Write(String.Format("Error! При добавлении внешних NA - объект оказался не NodePort. Object={0}", Port.DisplayName), true);
                                        continue;
                                    }
                                }

                            }
                        }


                    }
                    Debug.WriteLine("Перебор закончен");

                    boolShowLinks = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("CreateLinks procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }


        //задача - быстро найти связанную локацию и соответсвенный NetworkMap
        private EnterpriseManagementObject GetNetworkMap(Guid TargetObject_Id)
        {
            try
            {
                targetClass = null;
                NetworkMap = null;
                LinkedLocation = null;
                Guid Location_Id = new Guid();
                Guid Id_NA = new Guid();
                bool OK = true;
                SearchNetworkMap SNM = new SearchNetworkMap(emg, relConfigItemRefLocation, relNetworkMapRefLocation, relLoctoLoc, relConfigItemRefRack);
                EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(TargetObject_Id, ObjectQueryOptions.Default);
                //===============================================================================================
                //Module
                //===============================================================================================
                if (EMO.IsInstanceOf(classModule))
                {
                    #region
                    targetClass = classModule;
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Location_Id = relobj.TargetObject.Id;
                    }
                    if (Location_Id != Guid.Empty)
                    {
                        OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation);
                    }
                    if (OK)
                    {
                        //Fill CabelsTreeview
                        this.CablesTreeView.Items.Add(CablesTree.CreateTreeViewItem(TargetObject_Id));
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Модуль не связан с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);

                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                        //Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(this.sip.Instance);
                    }
                    #endregion
                }
                //===============================================================================================
                //NetworkMap
                //===============================================================================================
                else if (EMO.IsInstanceOf(classNetworkMap))
                {
                    targetClass = classNetworkMap;
                    NetworkMap = EMO;
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(EMO.Id, relNetworkMapRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        LinkedLocation = relobj.TargetObject;
                    }
                    if (LinkedLocation == null)
                    {
                        //System.Windows.MessageBox.Show("Карта сети не связана с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                }
                //======================================================================================
                //Location
                //======================================================================================
                else if (EMO.IsInstanceOf(classLocation))
                {
                    targetClass = classLocation;
                    if (!(SNM.SearchFromLocation(TargetObject_Id, out NetworkMap, out LinkedLocation)))
                    {
                        //System.Windows.MessageBox.Show("Локация не связана с картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                }
                //======================================================================================
                //WindowsComputer
                //======================================================================================
                else if (EMO.IsInstanceOf(classWindowsComputer))
                {
                    #region
                    targetClass = classWindowsComputer;
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Id_NA = relobj.TargetObject.Id;
                    }
                    //SearchLocationInTree SLIT = new SearchLocationInTree(emg,relNetworkAdapterHasChildNetworkAdapter,classModule,relLocationContainsConfigItem);
                    //TreeViewItem Item = SLIT.Start(Id_NA);
                    TreeViewItem Item = CablesTree.CreateTreeViewItemAndFindLocation(Id_NA, out Location_Id);
                    if (Location_Id != Guid.Empty)
                    { OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation); }
                    if (OK)
                    {
                        //Fill CabelsTreeview
                        this.CablesTreeView.Items.Add(Item);
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                    #endregion
                }
                //======================================================================================
                //DeployedComputer
                //======================================================================================
                else if (EMO.IsInstanceOf(classDeployedComputer))
                {
                    #region
                    targetClass = classDeployedComputer;
                    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    //Guid Temp_Loc_Id = new Guid();
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relComputerRunsWindowsComputer, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        GuidCol.Add(relobj.TargetObject.Id);
                    }
                    //Для каждого WindowsComputer ищем свой NetworkAdapter
                    foreach (Guid id in GuidCol)
                    {
                        T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(id, relComputerHostsComputerNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                        {
                            Id_NA = relobj.TargetObject.Id;
                        }
                        //this.CabelsTreeView.Items.Add(CablesTree.Create(Id_NA, emg));
                        //SearchLocationInTree SLIT = new SearchLocationInTree(emg, relNetworkAdapterHasChildNetworkAdapter, classModule, relLocationContainsConfigItem);
                        TreeViewItem Item = CablesTree.CreateTreeViewItemAndFindLocation(Id_NA, out Location_Id);
                        //TreeViewItem Item = SLIT.Start(Id_NA);
                        //Temp_Loc_Id = SLIT.Location_Id;
                    }

                    if (Location_Id != Guid.Empty)
                    { OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation); }
                    if (OK == false)
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с Картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                    #endregion
                }
                //======================================================================================
                //Node
                //======================================================================================
                else if (EMO.IsInstanceOf(classNode))
                {
                    targetClass = classNode;
                    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    if (SNM.SearchFromNode(EMO.Id, out NetworkMap, out LinkedLocation))
                    {
                        var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                        {
                            GuidCol.Add(relobj.TargetObject.Id);
                        }
                        foreach (Guid id in GuidCol)
                        {
                            this.CablesTreeView.Items.Add(CablesTree.CreateTreeViewItem(id));
                        }
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с Картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }

                }
                //======================================================================================
                //NodePort
                //======================================================================================
                else if (EMO.IsInstanceOf(classNodePort))
                {
                    targetClass = classNodePort;
                    Guid Node_Id = new Guid();
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Node_Id = relobj.SourceObject.Id;
                    }
                    ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
                    if (SNM.SearchFromNode(Node_Id, out NetworkMap, out LinkedLocation))
                    {
                        T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Node_Id, relNodeComposedOfNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                        {
                            GuidCol.Add(relobj.TargetObject.Id);
                        }
                        foreach (Guid id in GuidCol)
                        {
                            this.CablesTreeView.Items.Add(CablesTree.CreateTreeViewItem(id));
                        }
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с Картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                }
                //======================================================================================
                //Device
                //======================================================================================
                else if (EMO.IsInstanceOf(classDevice))
                {
                    #region
                    targetClass = classDevice;
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relDeviceHostNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Id_NA = relobj.TargetObject.Id;
                    }
                    //SearchLocationInTree SLIT = new SearchLocationInTree(emg, relNetworkAdapterHasChildNetworkAdapter, classModule, relLocationContainsConfigItem);

                    //TreeViewItem Item = SLIT.Start(Id_NA);
                    TreeViewItem Item = CablesTree.CreateTreeViewItemAndFindLocation(Id_NA, out Location_Id);
                    if (Location_Id != Guid.Empty)
                    { OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation); }
                    if (OK)
                    {
                        //Fill CabelsTreeview
                        this.CablesTreeView.Items.Add(Item);
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                    #endregion
                }
                //======================================================================================
                //Device NetworkAdapter
                //======================================================================================
                else if (EMO.IsInstanceOf(classDeviceNetworkAdapter))
                {
                    targetClass = classDeviceNetworkAdapter;
                    //SearchLocationInTree SLIT = new SearchLocationInTree(emg, relNetworkAdapterHasChildNetworkAdapter, classModule, relLocationContainsConfigItem);

                    //TreeViewItem Item = SLIT.Start(TargetObject_Id);
                    TreeViewItem Item = CablesTree.CreateTreeViewItemAndFindLocation(TargetObject_Id, out Location_Id);
                    if (Location_Id != Guid.Empty)
                    { OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation); }
                    if (OK)
                    {
                        //Fill CabelsTreeview
                        this.CablesTreeView.Items.Add(Item);
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Объект не связан с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                        //StartObjectWindow(EMO, targetClass);
                        //this.Close();
                    }
                }
                //======================================================================================
                //Rack
                //======================================================================================
                else if (EMO.IsInstanceOf(classRack))
                {
                    targetClass = classRack;
                    var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(TargetObject_Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Location_Id = relobj.TargetObject.Id;
                    }
                    if (Location_Id != Guid.Empty)
                    {
                        OK = SNM.SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation);
                        if (OK == false)
                        {
                            System.Windows.MessageBox.Show("Объект не связан с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                    }
                    //Дерево соединений не заполняем
                }
                return NetworkMap;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("GetNetworkMap procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return null;
            }
        }

        private void NetworkMapWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CablesTreeView.Items.Clear();

                //Get CurrentNode
                //            Guid Guid_in = new Guid("27D40675-71F9-2F70-C14B-DE554D3132CE");  //NetworkMap
                //Guid Guid_in = new Guid("6933936A-C589-EA33-3130-9830F97752B3"); //3Л-4 этаж 


                EMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>(ParamObjectId, ObjectQueryOptions.Default);
                this.txtTargetObject.Text = EMO.FullName;

                //EnterpriseManagementObject NetworkMap = GetNetworkMap(LocationId);
                if (NetworkMap == null)
                {
                    System.Windows.MessageBox.Show("Объект не связан с Картой сети!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (LinkedLocation == null)
                {
                    System.Windows.MessageBox.Show("Карта сети не связана с локацией!", "Service Manager", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                this.lbLocation.Content = LinkedLocation.DisplayName;

                FillModulesTreeView(LinkedLocation);

                FillImage(NetworkMap);

                //FillCanvas();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("NetworkMapWindow_Loaded procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }


        //перевод изображения в массив байт
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();

        }

        private void FillImage(EnterpriseManagementObject NetworkMap)
        {
            try
            {
                //Fill Image;
                //1. Создаем BinaryStream 
                BinaryStream bStream = (BinaryStream)NetworkMap[classNetworkMap, "Image"].Value;
                byte[] buffer = new byte[32768];
                //2.переводим в MemoryStream
                MemoryStream mStream = new MemoryStream();
                while (true)
                {
                    int read = bStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        break;
                    mStream.Write(buffer, 0, read);
                }
                //3.Создаем System.Drawing.Image
                System.Drawing.Image image = System.Drawing.Image.FromStream(mStream);
                //4.Конвертируем System.Drawing.Image в массив byte[]
                byte[] imageArray = imageToByteArray(image);
                //5.Создаем BitmapImage на основе массива 
                var source = new BitmapImage();
                source.BeginInit();
                source.StreamSource = new MemoryStream(imageArray);
                source.EndInit();
                this.img.Source = source;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("FillImage procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void Fill_CabelsTreeView(Guid Guid_Item)
        {
            try
            {
                _LogFile.Write(String.Format("Debug.Fill_CabelsTreeView" ), false);
                Guid Guid_NetworkAdapter = new Guid();
                ObservableCollection<TreeViewItem> Col = new ObservableCollection<TreeViewItem>();
                EnterpriseManagementObject EMO_Item = emg.EntityObjects.GetObject<EnterpriseManagementObject>(Guid_Item, ObjectQueryOptions.Default);
                if (EMO_Item.IsInstanceOf(classLocation))
                {
                    var modules = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Guid_Item, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in modules)
                    {
                        if (relobj.SourceObject.IsInstanceOf(classModule))
                        {
                            Guid_NetworkAdapter = relobj.SourceObject.Id;
                            if (Guid_NetworkAdapter != Guid.Empty)
                            {
                                Col.Add(CablesTree.CreateTreeViewItem(Guid_NetworkAdapter));
                            }
                        }
                    }
                }
                else if (EMO_Item.IsInstanceOf(classModule))
                {
                    Col.Add(CablesTree.CreateTreeViewItem(EMO_Item.Id));
                }
                else if (EMO_Item.IsInstanceOf(classComputerNetworkAdapter))
                {
                    Col.Add(CablesTree.CreateTreeViewItem(EMO_Item.Id));
                }
                else if (EMO_Item.IsInstanceOf(classDeviceNetworkAdapter))
                {
                    Col.Add(CablesTree.CreateTreeViewItem(EMO_Item.Id));
                }
                //Сортировка
                var sortedOC = from item in Col orderby item.Header select item;
                foreach (var i in sortedOC)
                {
                    TreeViewItem item = (TreeViewItem)i;
                    //item.Items.Add("*");
                    this.CablesTreeView.Items.Add(item);
                }
                _LogFile.Write(String.Format("Debug.Fill_CabelsTreeView End"), false);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Fill_CabelsTreeView procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void FitCollection(EnterpriseManagementObject objLocation, bool haveLocCoordinates, int XX, int YY, out ObservableCollection<TreeViewItem> ColModules, out ObservableCollection<TreeViewItem> ColNodes, out ObservableCollection<TreeViewItem> ColRacks)
        {
            try
            {
                int UIIndex = 0;
                //ищем связанные Conf Items.
                ColModules = new ObservableCollection<TreeViewItem>();
                ColNodes = new ObservableCollection<TreeViewItem>();
                ColRacks = new ObservableCollection<TreeViewItem>();
                //ColModules = null;
                //ColNodes = null;
                //ColRacks = null;

                Guid ConfItemId = Guid.Empty;
                //Ищем связанные с дочерними локациями ConfigItems
                var list = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(objLocation.Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                _LogFile.Write(String.Format("Локация {0}, цикл по связанным ConfItem. Количество Item = {1}", objLocation.DisplayName, list.Count), false);

                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relconfitem in list)
                {
                    //цикл по связанным конфиг итемам
                    #region
                    EnterpriseManagementObject ConfItem = relconfitem.SourceObject;
                    ConfItemId = ConfItem.Id;
                    _LogFile.Write(String.Format("Локация {0}, цикл по связанным ConfItem. ConfItem ={1} ", objLocation.DisplayName, ConfItem.DisplayName), false);
                    //ConfItemId = ConfItem.Id;
                    bool boolType = false;
                    //Находим координаты объекта
                    int X;
                    int Y;
                    if (relconfitem[relConfigItemRefLocation, "X"].Value == null) { X = 0; }
                    else
                    { int.TryParse(relconfitem[relConfigItemRefLocation, "X"].Value.ToString(), out X); }
                    if (relconfitem[relConfigItemRefLocation, "Y"].Value == null) { Y = 0; }
                    else
                    { int.TryParse(relconfitem[relConfigItemRefLocation, "Y"].Value.ToString(), out Y); }
                    //System.Windows.MessageBox.Show("Нашли связанный конфигитем - координаты X = " + X + " Y = " + Y); 
                    //Координаты
                    if (X == 0 && Y == 0 && haveLocCoordinates)
                    {
                        Random rnd = new Random();
                        X = rnd.Next(XX, XX + 12);
                        Y = rnd.Next(YY, YY + 12);
                    }
                    
                    //=================  Модули    ==============================================
                    #region
                    if (ConfItem.IsInstanceOf(classModule))
                    {
                        _LogFile.Write(String.Format("Module {0} ", ConfItem.DisplayName), false);
                        TreeViewItem Item_Module = new TreeViewItem();
                        Item_Module.Header = "Модуль:" + ConfItem.DisplayName;
                        Item_Module.Tag = ConfItem.Id;
                        Item_Module.IsExpanded = true;
                        ColModules.Add(Item_Module);
                        //Ищем связанные c модулями NA - Компьютеры или устройства
                        var globaldevices = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ConfItem.Id, relChildNetworkAdapterRefParentNetworkAdapter, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relglobaldevice in globaldevices)
                        {
                            _LogFile.Write(String.Format("Ищем связанные с Модулем {0} объекты. count = {1}",ConfItem.DisplayName, globaldevices.Count.ToString()), false);
                            //Модуль связан с Computer
                            if (relglobaldevice.SourceObject.IsInstanceOf(classComputerNetworkAdapter))
                            {
                                _LogFile.Write(String.Format("classComputerNetworkAdapter {0} ", relglobaldevice.SourceObject.DisplayName), false);
                                //Ищем родительский Windows Computer и создаем TVI
                                #region
                                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relDeployedComp = null;
                                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relWinComp = _Rel.GetSingleRelationship(emg, relComputerHostsComputerNetworkAdapter, relglobaldevice.SourceObject.Id, false);
                                _LogFile.Write(String.Format("1 "), false);
                                if (relWinComp != null) { relDeployedComp = _Rel.GetSingleRelationship(emg, relComputerRunsWindowsComputer, relWinComp.SourceObject.Id, false); }
                                //Если нашли DeployedComp
                                if (relDeployedComp != null)
                                {
                                    _LogFile.Write(String.Format("get DeployedComp {0} ", relDeployedComp.SourceObject.DisplayName), false);
                                    TreeViewItem Item_Computer = new TreeViewItem();
                                    Item_Computer.Header = "Компьютер:" + relDeployedComp.SourceObject.DisplayName + ":" + relDeployedComp.SourceObject[classDeployedComputer, "AssetKey"].Value.ToString();
                                    Item_Computer.Tag = relDeployedComp.SourceObject.Id;
                                    Item_Computer.IsExpanded = true;
                                    Item_Module.Items.Add(Item_Computer);
                                    _LogFile.Write(String.Format("2 "), false);
                                    boolType = true;
                                    //Создание UI
                                    MapUIElement UIComputer = new MapUIElement(MapUIElement.UIType.Computer, MapUIElement.NetworkAdapterType.Computer);
                                    //UIComputer.ObjectId = ConfItemId;
                                    UIComputer.ObjectId = relDeployedComp.SourceObject.Id;
                                    UIComputer.ModuleId = ConfItemId;

                                    Canvas.SetLeft(UIComputer, X);
                                    Canvas.SetTop(UIComputer, Y);
                                    _LogFile.Write(String.Format("3 "), false);
                                    UIComputer.Uid = Guid.NewGuid().ToString();
                                    UIComputer.ToolTip = Item_Module.Header + ":" + Item_Computer.Header;
                                    UIComputer.Description = Item_Module.Header + ":" + Item_Computer.Header;
                                    UIComputer.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                                    UIComputer.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                                    UIComputer.MouseMove += UIItem_MouseMove;
                                    UIComputer.MouseLeftButtonDown += UIItem_MouseLeftButtonDown;
                                    Map.Children.Add(UIComputer);
                                    Canvas.SetZIndex(UIComputer, 5);
                                    UIIndex = Map.Children.IndexOf(UIComputer);
                                    _LogFile.Write(String.Format("EMOId_UI! Добавляем relDeployedComp.SourceObject.Id ={0}, UIIndex = {1}", relDeployedComp.SourceObject.Id, UIIndex), false);
                                    EMOId_UI.Add(relDeployedComp.SourceObject.Id, UIIndex);
                                    //Добавляем Na Id in Collection. NA_ID Модуля и компьютера
                                    //_LogFile.Write(String.Format("Модуль! {0} relglobaldevice.SourceObject.Id={1}", relglobaldevice.SourceObject.DisplayName, relglobaldevice.SourceObject.Id), false);
                                    //_LogFile.Write(String.Format("Computer! {0} relglobaldevice.TargetObject.Id={1}", relglobaldevice.TargetObject.DisplayName, relglobaldevice.TargetObject.Id), false);
                                    if (UIComputer.Collection_NA_IDs.Contains(relglobaldevice.TargetObject.Id) == false) UIComputer.Collection_NA_IDs.Add(relglobaldevice.TargetObject.Id);
                                    UIComputer.Collection_NA_IDs.Add(relglobaldevice.SourceObject.Id);

                                }
                                #endregion
                            }
                            
                            //Модуль связан с Device
                            if (relglobaldevice.SourceObject.IsInstanceOf(classDeviceNetworkAdapter))
                            {
                                _LogFile.Write(String.Format("classDeviceNetworkAdapter {0} ", ConfItem.DisplayName), false);
                                //Ищем родительское устройство
                                #region
                                EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relDevice = _Rel.GetSingleRelationship(emg, relDeviceHostNetworkAdapter, relglobaldevice.SourceObject.Id, false);
                                if (relDevice != null)
                                {
                                    //_LogFile.Write(String.Format("relDevice.SourceObject.DisplayName {0}", relDevice.SourceObject.DisplayName), false);
                                    TreeViewItem Item_Device = new TreeViewItem();
                                    Item_Device.Header = "Устройство:" + relDevice.SourceObject.DisplayName;
                                    Item_Device.Tag = relDevice.SourceObject.Id;
                                    Item_Device.IsExpanded = true;
                                    Item_Module.Items.Add(Item_Device);

                                    boolType = true;

                                    //Создание UI
                                    MapUIElement UIItem = null;
                                    var DeviceType = relDevice.SourceObject[classDevice, "Type"].Value;
                                    if (DeviceType != null)
                                    {
                                        //string strDeviceType = relDevice.SourceObject[classDevice, "Type"].Value.ToString();
                                        string strDeviceType = DeviceType.ToString();
                                        switch (strDeviceType)
                                        {
                                            case "DeviceType.Desktop":
                                                UIItem = new MapUIElement(MapUIElement.UIType.Computer, MapUIElement.NetworkAdapterType.Device);
                                                break;
                                            case "DeviceType.Server":
                                                UIItem = new MapUIElement(MapUIElement.UIType.Server, MapUIElement.NetworkAdapterType.Device);
                                                break;
                                            case "DeviceType.Printer":
                                                UIItem = new MapUIElement(MapUIElement.UIType.Printer, MapUIElement.NetworkAdapterType.Device);
                                                break;
                                            case "DeviceType.NetworkDevice":
                                                UIItem = new MapUIElement(MapUIElement.UIType.Node, MapUIElement.NetworkAdapterType.Device);
                                                break;
                                            default:
                                                UIItem = new MapUIElement(MapUIElement.UIType.Server, MapUIElement.NetworkAdapterType.Device);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        //_LogFile.Write(String.Format("DeviceType = null {0}", relDevice.SourceObject.DisplayName), true);
                                        UIItem = new MapUIElement(MapUIElement.UIType.Computer, MapUIElement.NetworkAdapterType.Device);
                                    }
                                    //MapUIElement UIDevice = new MapUIElement(MapUIElement.TypeEnum.Device);
                                    UIItem.ObjectId = relDevice.SourceObject.Id;
                                    UIItem.ModuleId = ConfItemId;
                                    // UIItem.ObjectId = ConfItemId;
                                    Canvas.SetLeft(UIItem, X);
                                    Canvas.SetTop(UIItem, Y);

                                    UIItem.Uid = Guid.NewGuid().ToString();
                                    UIItem.ToolTip = Item_Module.Header + ":" + Item_Device.Header;
                                    UIItem.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                                    UIItem.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                                    UIItem.MouseMove += UIItem_MouseMove;
                                    UIItem.MouseLeftButtonDown += UIItem_MouseLeftButtonDown;

                                    Map.Children.Add(UIItem);
                                    Canvas.SetZIndex(UIItem, 5);

                                    UIIndex = Map.Children.IndexOf(UIItem);
                                    //_LogFile.Write(String.Format("EMOId_UI! Добавляем relDevice.SourceObject.Id ={0}, UIIndex = {1}", relDevice.SourceObject.Id, UIIndex), false);
                                    EMOId_UI.Add(relDevice.SourceObject.Id, UIIndex);

                                    //Добавляем Na Id in Collection
                                    if (UIItem.Collection_NA_IDs.Contains(relglobaldevice.TargetObject.Id) == false) UIItem.Collection_NA_IDs.Add(relglobaldevice.TargetObject.Id);
                                    UIItem.Collection_NA_IDs.Add(relglobaldevice.SourceObject.Id);
                                }
                                else
                                {
                                    _LogFile.Write(String.Format("Error! DeviceNetworkAdapter не связан с Device. confitem.TargetObject = {0} ", ConfItem.DisplayName), true);
                                    continue;
                                }
                                #endregion
                            }
                        }
                        //Случай когда модуль ни с кем не связан - добавляем модуль
                        if (boolType == false)
                        {
                            _LogFile.Write(String.Format("Module не связанный {0}", Item_Module.Header), false);
                            #region
                            //Создание UI
                            MapUIElement UIModule = new MapUIElement(MapUIElement.UIType.Module, MapUIElement.NetworkAdapterType.Module);
                            UIModule.ObjectId = ConfItemId;
                            UIModule.ModuleId = ConfItemId;

                            Canvas.SetLeft(UIModule, X);
                            Canvas.SetTop(UIModule, Y);

                            UIModule.Uid = Guid.NewGuid().ToString();
                            UIModule.ToolTip = Item_Module.Header;
                            //System.Windows.MessageBox.Show("Создаем UI для модуля " + Item_Module.Header); 
                            UIModule.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                            UIModule.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                            UIModule.MouseMove += UIItem_MouseMove;
                            UIModule.MouseLeftButtonDown += UIItem_MouseLeftButtonDown;
                            //_LogFile.Write(String.Format("Создали модуль"), false);
                            Map.Children.Add(UIModule);
                            //_LogFile.Write(String.Format("Добавили модуль"), false);
                            Canvas.SetZIndex(UIModule, 5);

                            UIIndex = Map.Children.IndexOf(UIModule);
                            //_LogFile.Write(String.Format("EMOId_UI! Добавляем ConfItemId ={0}, UIIndex = {1}", ConfItemId, UIIndex), false);
                            EMOId_UI.Add(ConfItemId, UIIndex);
                            //_LogFile.Write(String.Format("EMOId_UI"), false);
                            //Добавляем Na Id in Collection
                            UIModule.Collection_NA_IDs.Add(ConfItemId);
                            //_LogFile.Write(String.Format("Collection_NA_IDs"), false);
                            #endregion
                        }
                        else //в случае связи - добавляем модуль в словарь
                        {
                            //_LogFile.Write(String.Format("EMOId_UI! Добавляем ConfItemId ={0}, UIIndex = {1}", ConfItemId, UIIndex), false);
                            EMOId_UI.Add(ConfItemId, UIIndex);
                        }
                    }
                    #endregion
                    //=================  Nodes    ==============================================
                    #region
                    else if (ConfItem.IsInstanceOf(classNode))
                    {
                        //MessageBox.Show("Noda");
                        _LogFile.Write(String.Format("Noda {0}", ConfItem.DisplayName), false);
                        //В случае связи со шкафом - выходим - нода будет добавлена в составе шкафа.
                        var relRack = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(ConfItem.Id, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        if (relRack.Count > 0)
                        {
                            //("Не добавляем связаную со шкафом ноду! " + relobj.SourceObject.FullName);
                            continue;
                        }

                        TreeViewItem Item_Node = new TreeViewItem();
                        Item_Node.Header = "Node:" + ConfItem[classNode, "AssetKey"].Value + ":" + ConfItem[classNode, "GeoName"].Value;
                        Item_Node.Tag = ConfItem.Id;
                        Item_Node.IsExpanded = true;
                        ColNodes.Add(Item_Node);

                        //Создание UI
                        MapUIElement UINode = new MapUIElement(MapUIElement.UIType.Node, MapUIElement.NetworkAdapterType.Node);
                        UINode.ObjectId = ConfItem.Id;
                        UINode.ModuleId = ConfItemId;
                        
                        Canvas.SetLeft(UINode, X);
                        Canvas.SetTop(UINode, Y);

                        UINode.Uid = Guid.NewGuid().ToString();
                        UINode.ToolTip = Item_Node.Header;
                        UINode.Description = Item_Node.Header.ToString();
                        UINode.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                        UINode.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                        UINode.MouseMove += UIItem_MouseMove;

                        Map.Children.Add(UINode);
                        Canvas.SetZIndex(UINode, 5);
                        UIIndex = Map.Children.IndexOf(UINode);
                        EMOId_UI.Add(ConfItem.Id, UIIndex);

                    }
                    #endregion
                    //=================  Racks    ==============================================
                    #region
                    else if (ConfItem.IsInstanceOf(classRack))
                    {
                        _LogFile.Write(String.Format("Rack {0}", ConfItem.DisplayName), false);
                        TreeViewItem Item_Rack = new TreeViewItem();
                        Item_Rack.Header = "Шкаф:" + ConfItem.Name;
                        Item_Rack.Tag = ConfItem.Id;
                        Item_Rack.IsExpanded = true;
                        ColRacks.Add(Item_Rack);

                        ObservableCollection<TreeViewItem> ColRackItems = new ObservableCollection<TreeViewItem>();
                        ObservableCollection<Guid> ColRackItemsGuids = new ObservableCollection<Guid>();
                        //Добавляем содержимое шкафа
                        var RackItems = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ConfItem.Id, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                        foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> item in RackItems)
                        {
                            TreeViewItem Item_RackItem = new TreeViewItem();
                            Item_RackItem.Header = item.SourceObject.Name;
                            Item_RackItem.Tag = item.SourceObject.Id;
                            Item_RackItem.IsExpanded = true;
                            ColRackItems.Add(Item_RackItem);
                            ColRackItemsGuids.Add(item.SourceObject.Id);
                        }
                        var sortedColRacks = from item in ColRackItems orderby item.Header select item;
                        foreach (TreeViewItem TVI in sortedColRacks)
                        {
                            Item_Rack.Items.Add(TVI);
                        }
                        //Создание UI
                        MapUIElement UIRack = new MapUIElement(MapUIElement.UIType.Rack, MapUIElement.NetworkAdapterType.Rack);
                        UIRack.ObjectId = ConfItem.Id;
                        UIRack.ModuleId = ConfItemId;

                        Canvas.SetLeft(UIRack, X);
                        Canvas.SetTop(UIRack, Y);

                        UIRack.Uid = Guid.NewGuid().ToString();
                        UIRack.ToolTip = Item_Rack.Header;
                        UIRack.Description = Item_Rack.Header.ToString();
                        UIRack.MouseRightButtonDown += UIItem_MouseRightButtonDown;
                        UIRack.MouseRightButtonUp += UIItem_MouseRightButtonUp;
                        UIRack.MouseMove += UIItem_MouseMove;

                        Map.Children.Add(UIRack);
                        Canvas.SetZIndex(UIRack, 5);

                        UIIndex = Map.Children.IndexOf(UIRack);
                        EMOId_UI.Add(ConfItem.Id, UIIndex);
                        //Добавялем в коллекцию EMOId_UI - содержимое шкафа
                        foreach (Guid g in ColRackItemsGuids)
                        {
                            EMOId_UI.Add(g, UIIndex);
                        }
                        //EMOId_UI.Add(ConfItemId,UIIndex);
                    }
                    #endregion
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _LogFile.Write(String.Format("FitCollection procedure error : " + ex.Message), false);
                System.Windows.MessageBox.Show("FitCollection procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                ColModules = new ObservableCollection<TreeViewItem>();
                ColNodes = new ObservableCollection<TreeViewItem>();
                ColRacks = new ObservableCollection<TreeViewItem>();
                this.Close();
            }
        }

        private void lbLocation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EnterpriseManagementObjectCriteria criteria = new EnterpriseManagementObjectCriteria("LocationType = '0A607435-3D34-23D1-B38E-B89DDE0A558D'", classLocation);
                TreeViewForm TWindow = new TreeViewForm(emg, classLocation, relLoctoLoc, criteria);
                TWindow.ShowDialog();
            }
            catch (Exception exc)
            {
                System.Windows.MessageBox.Show("lbLocation_MouseDown procedure error" + exc.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
    }
    }

