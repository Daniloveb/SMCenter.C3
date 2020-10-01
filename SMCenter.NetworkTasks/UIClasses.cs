using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace SMCenter.NetworkTasks
{
    class MapUIElement : Image
    {
        //private EnterpriseManagementGroup emg { get; set; }
        public Guid ObjectId { get; set; }
        public Guid ModuleId { get; set; }

        public enum UIType
        {
            Module, Computer, Rack, Node, Server, Printer, Room
        }

        UIType privateObjectType;

        public UIType ObjectType
        {
            get
            {
                return privateObjectType;
            }
        }

        public enum NetworkAdapterType
        {
            Module, Computer, Device, Rack, Node
        }

        NetworkAdapterType privateNetworkAdapterType;

        public NetworkAdapterType NetworkAdapterObjectType
        {
            get
            {
                return privateNetworkAdapterType;
            }
        }

        string privateDescription;

        public String Description
        {
            get
            {
                return privateDescription;
            }
            set
            {
                privateDescription = value; 
            }
        }

        ObservableCollection<Guid> privatCol_NA_IDs = new ObservableCollection<Guid>();

        public ObservableCollection<Guid> Collection_NA_IDs
        {
            get   {  return privatCol_NA_IDs;    }
            set   {  privatCol_NA_IDs = value;   }
        }

        public MapUIElement(UIType ObjectType, NetworkAdapterType NetworkAdapterType)
        {
            privateObjectType = ObjectType;
            privateNetworkAdapterType = NetworkAdapterType;

            if (ObjectType == UIType.Module)
            { 
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/Module48x48.png");
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Computer)
            {
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/monitor_48.png");
                //Uri uri = new Uri("/application:,,component/Resources/monitor_48.png");
                BitmapImage bitmap = new BitmapImage(uri);

                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Rack)
            {
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/rack2.png");
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Node)
            {
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/Node48x48.png");
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Server)
            {
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/server_48.png");
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Printer)
            {
                //Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/monitor_48.png");
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/printer_48.png");
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 7;
            }
            else if (ObjectType == UIType.Room)
            {
                Uri uri = new Uri("pack://application:,,,/SMCenter.NetworkTasksLib;component/Resources/Circle.png");
                //Uri uri = new Uri("/Resources/square_48.png", UriKind.Relative);
                BitmapImage bitmap = new BitmapImage(uri);
                this.Source = bitmap;
                this.MaxHeight = 25;
            }
            //computer.Source = bitmap;
        }
    }

    //class MapUILine : Image
    //{
    //    //private EnterpriseManagementGroup emg { get; set; }
    //    public Guid ObjectId { get; set; }

    //    public enum TypeEnum
    //    {
    //        Module, Computer, Rack, Node, Server, Printer
    //    }

    //    TypeEnum privateObjectType;

    //    public TypeEnum ObjectTypeEnum
    //    {
    //        get
    //        {
    //            return privateObjectType;
    //        }
    //    }

        

    //}
    //class UIModule : Image
    //{
    //    //private EnterpriseManagementGroup emg { get; set; }
    //    public Guid ObjectId { get; set; }

    //    private enum TypeEnum
    //    {
    //        Ok, Cancel
    //    }


    //    public UIModule()
    //    {
    //        Uri uri = new Uri("pack://application:,,,/Resources/Module48x48.png", UriKind.Relative);
    //        BitmapImage bitmap = new BitmapImage(uri);

    //        //Image MyImage = new Image();
    //        this.Source = bitmap;
    //        //computer.Source = bitmap;
            
    //    }
    //}

    //class UINode : Image
    //{
    //    public Guid ObjectId { get; set; }

    //    public UINode()
    //    {
    //        Uri uri = new Uri("pack://application:,,,/Resources/Node48x48.png", UriKind.Relative);
    //        BitmapImage bitmap = new BitmapImage(uri);
    //        this.Source = bitmap;

    //    }
    //}

    //class UIRack : Image
    //{
    //    public Guid ObjectId { get; set; }

    //    public UIRack()
    //    {
    //        Uri uri = new Uri("pack://application:,,,/Resources/Rack48x48.png", UriKind.Relative);
    //        BitmapImage bitmap = new BitmapImage(uri);

    //        this.Source = bitmap;
    //    }
    //}

    //class UIComputer : Image
    //{
    //    public Guid ObjectId { get; set; }

    //    public UIComputer()
    //    {
    //        //Uri uri = new Uri("Images/monitor_48.png", UriKind.Relative);
    //        Uri uri = new Uri("pack://application:,,,/Resources/monitor_48.png");
    //        //Uri uri = new Uri("/application:,,component/Resources/monitor_48.png");
    //        BitmapImage bitmap = new BitmapImage(uri);
            
    //        this.Source = bitmap;
    //        this.MaxHeight = 20;

    //    }
    //}

    //class UIObject : Image
    //{
    //    //private EnterpriseManagementGroup emg { get; set; }
    //    public Guid ObjectId { get; set; }
    //    public String ObjectType { get; set; }
    //    //public Image Image { get; set; }

    //    public UIObject()
    //    {
    //        Uri uri = new Uri("pack://application:,,,/Resources/monitor_48.png");
    //        //Uri uri = new Uri("/application:,,component/Resources/monitor_48.png");
    //        BitmapImage bitmap = new BitmapImage(uri);

    //        Image _Image = new Image();
    //        _Image.Source = bitmap;
    //        this.Source = bitmap;

    //        ////Uri uri = new Uri("Images/monitor_48.png", UriKind.Relative);
    //        //Uri uri = new Uri("pack://application:,,,/Resources/monitor_48.png");
    //        ////Uri uri = new Uri("/application:,,component/Resources/monitor_48.png");
    //        //BitmapImage bitmap = new BitmapImage(uri);

    //        //Image _Image = new Image();
    //        //_Image.Source = bitmap;
    //        //Image = _Image;
    //        ////computer.Source = bitmap;

    //    }
    //}

    //class UIDevice : Image
    //{
    //    public Guid ObjectId { get; set; }

    //    public UIDevice()
    //    {
    //        Uri uri = new Uri("\\Images\\logo64x64.png", UriKind.Relative);
    //        BitmapImage bitmap = new BitmapImage(uri);

    //        this.Source = bitmap;
    //    }
    //}

    // class UIPrinter : Image
    //{
    //    public Guid ObjectId { get; set; }

    //    public UIPrinter()
    //    {
    //        Uri uri = new Uri("\\Images\\printer_48.png", UriKind.Relative);
    //        BitmapImage bitmap = new BitmapImage(uri);

    //        this.Source = bitmap;
    //    }
    //}
    
}
