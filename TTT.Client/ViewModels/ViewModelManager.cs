﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace TTT.Client
{
    public class ViewModelManager
    {
        Dictionary<Type, Type> _viewModelLookup = new Dictionary<Type, Type>();
        IServiceProvider _serviceProvider;
        public ViewModelManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RegisterViewModel<TModel, TViewModel>() 
            where TModel : class 
            where TViewModel : ViewModel<TModel>
        {
            _viewModelLookup[typeof(TModel)] = typeof(TViewModel);
        }

        
        public ViewModel<TModel> CreateViewModel<TModel>(TModel model) 
            where TModel : class
        {
            var viewModelType = _viewModelLookup[model.GetType()];
            var viewModel = Activator.CreateInstance(viewModelType, new object[] { model }) as ViewModel<TModel>;
            viewModel.Init(_serviceProvider);
            return viewModel;
        }
    }
}