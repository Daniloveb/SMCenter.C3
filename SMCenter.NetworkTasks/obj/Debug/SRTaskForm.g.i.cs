﻿#pragma checksum "..\..\SRTaskForm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "86A0CB58FFC66784E62697F5E5B7ED47"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.EnterpriseManagement.UI.FormsInfra;
using Microsoft.EnterpriseManagement.UI.WpfControls;
using Microsoft.EnterpriseManagement.UI.WpfToolbox;
using Microsoft.Windows.Controls;
using Rimera.SRComplete;
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


namespace Rimera.SRComplete {
    
    
    /// <summary>
    /// SRTask
    /// </summary>
    public partial class SRTask : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Rimera.SRComplete.SRTask SRTaskForm;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblDisplayName;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.EnterpriseManagement.UI.WpfControls.ListPicker lpResult;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblComment;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbOK;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\SRTaskForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/SMCenter.NetworkTasksdll;component/srtaskform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SRTaskForm.xaml"
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
            this.SRTaskForm = ((Rimera.SRComplete.SRTask)(target));
            
            #line 10 "..\..\SRTaskForm.xaml"
            this.SRTaskForm.Loaded += new System.Windows.RoutedEventHandler(this.SRTaskForm_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblDisplayName = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lpResult = ((Microsoft.EnterpriseManagement.UI.WpfControls.ListPicker)(target));
            return;
            case 4:
            this.lblComment = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.txt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.lbOK = ((System.Windows.Controls.Label)(target));
            
            #line 43 "..\..\SRTaskForm.xaml"
            this.lbOK.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbOK_MouseDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lbCancel = ((System.Windows.Controls.Label)(target));
            
            #line 44 "..\..\SRTaskForm.xaml"
            this.lbCancel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbCancel_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

