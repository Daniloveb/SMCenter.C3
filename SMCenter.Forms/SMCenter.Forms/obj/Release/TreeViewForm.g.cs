﻿#pragma checksum "..\..\TreeViewForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9BFFBED94B22055629395F0D53939C286D13A80CA0FA44A375939B231B85546F"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// TreeViewForm
    /// </summary>
    public partial class TreeViewForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\TreeViewForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SMCenter.Forms.TreeViewForm TreeViewWindow;
        
        #line default
        #line hidden
        
        
        #line 6 "..\..\TreeViewForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView MainTreeView;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\TreeViewForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image Logo;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\TreeViewForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbOK;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\TreeViewForm.xaml"
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
            System.Uri resourceLocater = new System.Uri("/SMCenter.Forms;component/treeviewform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TreeViewForm.xaml"
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
            this.TreeViewWindow = ((SMCenter.Forms.TreeViewForm)(target));
            
            #line 4 "..\..\TreeViewForm.xaml"
            this.TreeViewWindow.Loaded += new System.Windows.RoutedEventHandler(this.TreeViewWindow_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainTreeView = ((System.Windows.Controls.TreeView)(target));
            
            #line 6 "..\..\TreeViewForm.xaml"
            this.MainTreeView.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.MainTreeView_MouseDoubleClick);
            
            #line default
            #line hidden
            
            #line 6 "..\..\TreeViewForm.xaml"
            this.MainTreeView.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.MainTreeView_SelectedItemChanged);
            
            #line default
            #line hidden
            
            #line 6 "..\..\TreeViewForm.xaml"
            this.MainTreeView.AddHandler(System.Windows.Controls.TreeViewItem.ExpandedEvent, new System.Windows.RoutedEventHandler(this.MainTreeView_Expanded));
            
            #line default
            #line hidden
            return;
            case 3:
            this.Logo = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.lbOK = ((System.Windows.Controls.Label)(target));
            
            #line 11 "..\..\TreeViewForm.xaml"
            this.lbOK.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbOK_MouseDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.lbCancel = ((System.Windows.Controls.Label)(target));
            
            #line 12 "..\..\TreeViewForm.xaml"
            this.lbCancel.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.lbCancel_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

