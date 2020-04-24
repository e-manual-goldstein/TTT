using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TTT.Host
{
    public abstract class View//<TModel> where TModel : IUpdatingElement
    {
        public Canvas Content { get; internal set; }

        //public TModel Model { get; set; }

        internal void Refresh()
        {
           // Get List of elements that need to be updated
           // Update elements with new values
           // Clear List of updated elements 
        }

        //TODO: Is this needed?
        public abstract void SizeChanged(Size newSize);
        public abstract void Show();
    }
}
