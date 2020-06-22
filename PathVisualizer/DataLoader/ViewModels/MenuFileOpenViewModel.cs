using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLoaderService;
using Lib;
using Lib.Events;
using Lib.SharedModels;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace DataLoader.ViewModels
{
    public class MenuFileOpenViewModel : BindableBase
    {
        public List<Tag> Tags { get; set; }

        public DelegateCommand OpenCommand { get; set; }
        private readonly IEventAggregator _eventAggregator;
        private readonly IDataLoader _dataLoader;

        public MenuFileOpenViewModel(IEventAggregator eventAggregator, IDataLoader dataLoader)
        {
            Tags = new List<Tag>();
            OpenCommand = new DelegateCommand(OpenAction);
            _eventAggregator = eventAggregator;
            _dataLoader = dataLoader;
        }

        private async void OpenAction()
        {
            var openFileDialog = new OpenFileDialog { Multiselect = true };

            if (openFileDialog.ShowDialog() != true) return;
            
            await _dataLoader.LoadFiles(openFileDialog.FileNames);
        }
    }
}
