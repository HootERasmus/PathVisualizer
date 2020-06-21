using System.Windows;
using Lib.Events;
using Lib.SharedModels;
using LinePlot.Events;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;

namespace LinePlot.ViewModels
{
    public class MenuFileExportViewModel
    {
        public DelegateCommand ExportCommand { get; set; }
        private readonly IEventAggregator _eventAggregator;
        private Tag _tag;
        public MenuFileExportViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TagSelectionEvent>().Subscribe(tag => _tag = tag);
            ExportCommand = new DelegateCommand(ExportAction);
        }

        private void ExportAction()
        {
            if (_tag == null)
            {
                MessageBox.Show("No tag was selected");
                return;
            }

            var dialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = "*.png|*.PNG",
                AddExtension = true,
                FileName = $"{_tag.Id}"
            };
            dialog.ShowDialog(Application.Current.MainWindow);

            if (string.IsNullOrEmpty(dialog.FileName)) return;

            _eventAggregator.GetEvent<ExportPlotEvent>().Publish(new ExportPlotEventModel(this, dialog.FileName));
        }
    }
}
