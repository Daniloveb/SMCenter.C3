﻿#pragma checksum "..\..\CITemplate.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "593251F5486C4EB3B56FC5FBCD4247C2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.EnterpriseManagement.UI.WpfControls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ConfigItemFormTemplate {
    
    
    /// <summary>
    /// CITemplate
    /// </summary>
    public partial class CITemplate : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\CITemplate.xaml"
        internal System.Windows.Controls.TabControl tabControl1;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\CITemplate.xaml"
        internal System.Windows.Controls.TabItem tabItemGeneral;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\CITemplate.xaml"
        internal System.Windows.Controls.TabItem tabItemRelItems;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\CITemplate.xaml"
        internal System.Windows.Controls.TabItem tabItemHistory;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ConfigItemFormTemplate;component/citemplate.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CITemplate.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tabControl1 = ((System.Windows.Controls.TabControl)(target));
            return;
            case 2:
            this.tabItemGeneral = ((System.Windows.Controls.TabItem)(target));
            return;
            case 3:
            this.tabItemRelItems = ((System.Windows.Controls.TabItem)(target));
            return;
            case 4:
            this.tabItemHistory = ((System.Windows.Controls.TabItem)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}