using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.UI.DataModel;
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using System.ComponentModel;
using System.Windows.Markup;
using System.Diagnostics;

namespace SMCenter.Forms
{
    /// <summary>
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class SelectImageForm : Window
    {
        EnterpriseManagementGroup emg;

        public SelectImageForm(EnterpriseManagementGroup _emg)
        {
            InitializeComponent();
            emg = _emg;
        }

        private void TreeViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FillListView();
        }

        private void FillListView()
        {
            try
            {
                ListViewImages.ItemsSource = null;
                ListViewImages.Items.Clear();
                ObservableCollection<ManagementPackImage> ImagesCol = new ObservableCollection<ManagementPackImage>();
                ManagementPack mp = emg.GetManagementPack("SMCenter.AssetManagement.Resources", "75b45bd6835084b1", new Version());

                //ManagementPackImage Im = new ManagementPackImage(mp, "ItamHardwareFolder48x48", ManagementPackAccessibility.Internal);

                foreach (ManagementPackResource Res in mp.GetResources<ManagementPackResource>())
                {
                    if (Res is Microsoft.EnterpriseManagement.Configuration.ManagementPackImage)
                    {
                        ManagementPackImage Im = (ManagementPackImage)Res;
                        ImagesCol.Add(Im);
                        //System.Drawing.Image II = System.Drawing.Image.FromStream(Im.ImageData);
                    }
                }
                this.ListViewImages.ItemsSource = ImagesCol;
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in FillListViewImages() " + exc.Message);
            }
        }

        private void lbOK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ListViewImages.SelectedItem != null)
            {
                ManagementPackImage Image = (ManagementPackImage)this.ListViewImages.SelectedItem;
                SelectImageHandlerClass.EventHandler(true, Image);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select Image!", "No Image selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void lbCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectImageHandlerClass.EventHandler(false, null);
            this.Close();
        }


        private void ListViewImages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewImages.SelectedItem != null)
            {
                ManagementPackImage Image = (ManagementPackImage)this.ListViewImages.SelectedItem;
                SelectImageHandlerClass.EventHandler(true, Image);
                this.Close();
            }
        }

    }
    public static class SelectImageHandlerClass
    {
        public delegate void EMOEvent(bool Ok,ManagementPackImage _Image);
        public static EMOEvent EventHandler;
    }

}
