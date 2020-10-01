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
using Microsoft.EnterpriseManagement.UI.Core.Connection;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.Security;
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;
using Microsoft.EnterpriseManagement.UI.Controls;
using Microsoft.EnterpriseManagement.UI.Controls.Internal;
using Microsoft.EnterpriseManagement.UI.Extensions;
using Microsoft.EnterpriseManagement.UI.WpfControls;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel.Design;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using System.ComponentModel;
using System.Windows.Markup;
using Microsoft.EnterpriseManagement.UI.FormsInfra;
using Microsoft.EnterpriseManagement.GenericForm;

namespace SMCenter.Forms
{
    /// <summary>
    /// Interaction logic for ProjectionTemplate.xaml
    /// </summary>
    public partial class ProjectionObjectTemplate : UserControl
    {

        //EnterpriseManagementGroup emg;
        
        //EnterpriseManagementObject EMO;
        IDataItem CurrentDataItem;
        public ManagementPackClass mpClass { get; set; }
        //public Guid guidClass { get; set; } //= new Guid("11ED339C-6A30-074A-BFEF-50A9E3D102AD");
        public string PathString { get; set; }
        public ProjectionObjectTemplate()
        {
            InitializeComponent();
        }
      

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                sip.BaseClassId = mpClass.Id;
                Binding b = new Binding()
                {
                    Mode = BindingMode.TwoWay,
                    Path = new PropertyPath(PathString),
                };
                sip.SetBinding(SingleInstancePicker.InstanceProperty, b);

                //Trace.WriteLine(DateTime.Now + " : " + "U.OriginalString = " + U.OriginalString);
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in UserControl_Loaded() " + exc.Message);
            }

        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (this.DataContext != null && this.DataContext is IDataItem)
                {
                    CurrentDataItem = this.DataContext as IDataItem;
                    if (!((bool)CurrentDataItem["$IsNew$"]))
                    {
                        //CurrentEMO = emg.EntityObjects.GetObject<EnterpriseManagementObject>((Guid)CurrentDataItem["$Id$"], ObjectQueryOptions.Default);
                    }
                }
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in DataContextChanged() " + exc.Message);
            }
        }

        private void brd_Edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.sip.Instance != null)
            {
                //EnterpriseManagementObject SelectEMO = (EnterpriseManagementObject)ListViewCI.SelectedItem;
                //Convert EnterpriseManagementObject to IDataItem
                //EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(ConfItemClass);
                //IDataItem itemIdentity = dataType.CreateProxyInstance(SelectEMO);

                //Open Console form for created object
                Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.PopoutForm(this.sip.Instance);
            }
            else
            {
                MessageBox.Show("Please select object!", "No object selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void brd_Add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //NavigationModelNodeBase nodeIn = null;
                NavigationModelNodeBase NBNew;
                NavigationModelNodeTask nmntNew = NavigationTasksHelper.CreateNewInstanceLink(mpClass);
                Microsoft.EnterpriseManagement.GenericForm.GenericCommon.MonitorCreatedForm(null, nmntNew, out NBNew);
            }
            catch (Exception exc)
            {
                Trace.WriteLine(DateTime.Now + " : " + "Error in brd_Add_MouseDown() " + exc.Message);
            }
        }

        private void sip_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("DataContext changed");
        }

        private void brd_Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.sip.Instance = null;
            TextBox tb = (TextBox)sip.FindName("instanceNameTextBox");
            tb.Text = "";
        }
  
    }
}
