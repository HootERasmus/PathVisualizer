using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export3DLine.ViewModels
{
    public class Export3DLineViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public Export3DLineViewModel()
        {
            Message = "View A from your Prism Module";
        }
    }
}
