using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace TTT.Host
{
    public abstract class View//<T> where T : class
    {
        protected IServiceProvider _serviceProvider;
        public View(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        readonly IDictionary<object, Action<object>> _updatingObjects = new Dictionary<object, Action<object>>();
        
        public Canvas Content { get; internal set; }
        
        //public TModel Model { get; set; }

        public void Refresh()
        {
            foreach (var key in _updatingObjects.Keys)
            {
                App.Current.Dispatcher.Invoke(() => _updatingObjects[key](key));
            }
            // Clear List of updated elements 
        }

        //TODO: Is this needed?
        public abstract void SizeChanged(Size newSize);
        public abstract void Show();

        protected void RegisterUpdatingAction(object obj, Action<object> action)
        {
            _updatingObjects[obj] = action;
        }

        protected TService GetService<TService>()
            where TService : class
        {
            return _serviceProvider.GetService<TService>();
        }
    }
}
