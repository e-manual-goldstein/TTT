using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using TTT.Common;

namespace TTT.Client
{
    public abstract class ViewModel<TModel> where TModel : class
    {
        private IServiceProvider _serviceProvider;

        protected ActivityManager ActivityManager { get; private set; }
        protected Logger Logger { get; private set; }
        protected ViewModel(TModel model)
        {
            Model = model;
        }

        public void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ActivityManager = GetService<ActivityManager>();
            Logger = GetService<Logger>();
            Draw();
        }

        protected TModel Model { get; private set; }

        protected abstract void Draw();

        protected TService GetService<TService>()
        {
            return _serviceProvider.GetService<TService>();
        }
        
        protected DisplayMetrics DisplayMetrics
        {
            get
            {
                return ActivityManager.CurrentContext().Resources.DisplayMetrics;
            }
        }
    }
}