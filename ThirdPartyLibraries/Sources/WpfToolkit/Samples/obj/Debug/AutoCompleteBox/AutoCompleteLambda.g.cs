﻿#pragma checksum "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6592FF296093893FB724065E188FC556"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Windows.Controls;
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


namespace System.Windows.Controls.Samples {
    
    
    /// <summary>
    /// AutoCompleteLambda
    /// </summary>
    public partial class AutoCompleteLambda : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 54 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal System.Windows.Controls.AutoCompleteBox DepartureAirport;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal Microsoft.Windows.Controls.DatePicker DepartureDate;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal System.Windows.Controls.AutoCompleteBox ArrivalAirport;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal Microsoft.Windows.Controls.DatePicker ArrivalDate;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal System.Windows.Controls.Slider Passengers;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
        internal System.Windows.Controls.Button BookFlight;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFToolkitSamples;component/autocompletebox/autocompletelambda.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AutoCompleteBox\AutoCompleteLambda.xaml"
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
            this.DepartureAirport = ((System.Windows.Controls.AutoCompleteBox)(target));
            return;
            case 2:
            this.DepartureDate = ((Microsoft.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.ArrivalAirport = ((System.Windows.Controls.AutoCompleteBox)(target));
            return;
            case 4:
            this.ArrivalDate = ((Microsoft.Windows.Controls.DatePicker)(target));
            return;
            case 5:
            this.Passengers = ((System.Windows.Controls.Slider)(target));
            return;
            case 6:
            this.BookFlight = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

