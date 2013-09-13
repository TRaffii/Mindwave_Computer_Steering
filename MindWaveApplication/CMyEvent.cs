using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MindWaveApplication
{
    public class ListEventArgs : EventArgs
    {
        public TextBox TextBoxName { get; set; }
        public ListEventArgs(TextBox v_TextBoxName)
        {
            TextBoxName = v_TextBoxName;
        }
        
    }
}
