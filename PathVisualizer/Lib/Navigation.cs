﻿using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Lib
{
    public abstract class Navigation : BindableBase
    {
        protected readonly IRegionManager RegionManager;

        protected Navigation(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            NavigateCommand = new DelegateCommand(NavigateAction);
        }

        public ICommand NavigateCommand { get; }

        public abstract string ButtonContent { get; set; }
        public abstract Uri NavigationView { get; set; }

        public abstract void NavigateAction();
    }
}
