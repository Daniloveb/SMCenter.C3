﻿#pragma checksum "..\..\CreateVersionTaskForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4078F3B82DE7FD3CC6CC0413AC302F1C3508A9DA95C68FC8866356E5DC0CA04A"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using SMCenter.Forms;
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


namespace SMCenter.Forms {
    
    
    /// <summary>
    /// CreateVersionTaskForm
    /// </summary>
    public partial class CreateVersionTaskForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SMCenter.Forms.CreateVersionTaskForm CreateSoftwareVersion;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblSoftwareTitle;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSoftwareTitle;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblDisplayName;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtDisplayName;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Logo;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbCancel;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\CreateVersionTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbOk;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SMCenter.Forms;component/createversiontaskform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CreateVersionTaskForm.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CreateSoftwareVersion = ((SMCenter.Forms.CreateVersionTaskForm)(target));
            
            #line 5 "..\..\CreateVersionTaskForm.xaml"
            this.CreateSoftwareVersion.Loaded += new System.Windows.RoutedEventHandler(this.AddToCatalogWindow_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblSoftwareTitle = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.txtSoftwareTitle = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.lblDisplayName = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.txtDisplayName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.Logo = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.lbCancel = ((System.Windows.Controls.Label)(target));
            
            #line 23 "..\..\CreateVersionTaskForm.xaml"
            this.lbCancel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbCancel_MouseDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.lbOk = ((System.Windows.Controls.Label)(target));
            
            #line 24 "..\..\CreateVersionTaskForm.xaml"
            this.lbOk.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbOk_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

