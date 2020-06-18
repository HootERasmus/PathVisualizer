using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filters.Views;

namespace Filters.ViewModels
{
    public class MenuToolsFiltersViewModel : BindableBase
    {
        public DelegateCommand OpenFilterWindowCommand { get; set; }

        public MenuToolsFiltersViewModel()
        {
            OpenFilterWindowCommand = new DelegateCommand(OpenFilterWindowAction);
        }

        private void OpenFilterWindowAction()
        {
            var window = new FiltersWindow {DataContext = new FiltersViewModel()};
            window.Show();
        }
    }
}
