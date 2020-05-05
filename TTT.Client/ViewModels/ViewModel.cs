using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;

namespace TTT.Client
{
    public abstract class ViewModel<TModel> where TModel : class
    {
        private IServiceProvider _serviceProvider;

        protected ActivityManager ActivityManager { get; private set; }
        protected ViewModel(TModel model)
        {
            Model = model;
        }

        public void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ActivityManager = GetService<ActivityManager>();
        }

        protected TModel Model { get; set; }

        public abstract void Show();

        protected TService GetService<TService>()
        {
            return _serviceProvider.GetService<TService>();
        }        
    }
}