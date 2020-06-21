using Prism.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Lib.SharedModels;
using Microsoft.Win32;
using PipelineService;
using Prism.Commands;
using Prism.Events;

namespace Export3DLine.ViewModels
{
    public class Export3DLineViewModel : BindableBase
    {
        public DelegateCommand Export3DLineCommand { get; set; }
        
        private readonly IEventAggregator _eventAggregator;
        private Tag _tag;
        
        public Export3DLineViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);

            Export3DLineCommand = new DelegateCommand(Export3DLineAction);
        }


        private void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            _tag = history.Values.Last();
        }

        private async void Export3DLineAction()
        {
            if (_tag == null)
            {
                MessageBox.Show("No tag was selected");
                return;
            }

            var dialog = new SaveFileDialog
            {
                DefaultExt = ".obj", Filter = "*.obj|*.OBJ", AddExtension = true, FileName = $"{_tag.Id}"
            };
            dialog.ShowDialog(Application.Current.MainWindow);

            if(string.IsNullOrEmpty(dialog.FileName)) return;

            await ConvertInto3D(_tag.TimeCoordinates, dialog.FileName);
        }

        public static string DrawLine(TimeCoordinate first, TimeCoordinate second, int number)
        {
            var stringBuilder = new StringBuilder();
            var constant = 1;

            if (number != 0)
                constant = 8;

            var size = 0.1;

            var constant3D = 0.0005;
            var first3D = number * constant3D;
            var second3D = first3D + constant3D;

            number = number + 1;

            //bottom - first end
            stringBuilder.Append($"v {first.X}          {first.Y}      {first3D}\n"); // start
            stringBuilder.Append($"v {first.X + size}     {first.Y}      {first3D}\n"); // x + 1

            stringBuilder.Append($"v {second.X}         {second.Y}     {second3D}\n"); // y + 1
            stringBuilder.Append($"v {second.X + size}    {second.Y}     {second3D}\n"); // y + 1

            stringBuilder.Append($"v {first.X}          {first.Y}      {first3D + size}\n"); // start
            stringBuilder.Append($"v {first.X + size}     {first.Y}      {first3D + size}\n"); // x + 1

            stringBuilder.Append($"v {second.X}         {second.Y}     {second3D + size}\n"); // # y + 1
            stringBuilder.Append($"v {second.X + size}    {second.Y}     {second3D + size}\n"); // # y + 1


            number = number - 1;
            stringBuilder.Append($"f {(number * constant) + 1} {(number * constant) + 2} {(number * constant) + 4} {(number * constant) + 3}\n"); // # bottom
            stringBuilder.Append($"f {(number * constant) + 5} {(number * constant) + 6} {(number * constant) + 8} {(number * constant) + 7}\n"); // top
            stringBuilder.Append($"f {(number * constant) + 1} {(number * constant) + 5} {(number * constant) + 6} {(number * constant) + 2}\n"); //side 1
            stringBuilder.Append($"f {(number * constant) + 2} {(number * constant) + 6} {(number * constant) + 8} {(number * constant) + 4}\n"); // side 2
            stringBuilder.Append($"f {(number * constant) + 4} {(number * constant) + 8} {(number * constant) + 7} {(number * constant) + 3}\n"); // side 3
            stringBuilder.Append($"f {(number * constant) + 3} {(number * constant) + 7} {(number * constant) + 5} {(number * constant) + 1}\n"); // side 4

            return stringBuilder.ToString();
        }

        public async Task ConvertInto3D(List<TimeCoordinate> dataPoints, string fileName)
        {
            await Task.Run(() =>
            {
                var length = dataPoints.Count - 1;
                var stringBuilder = new StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    stringBuilder.Append(DrawLine(dataPoints[i], dataPoints[i + 1], i));
                }

                StringToFile(stringBuilder.ToString(), fileName);
            });
        }

        public static void StringToFile(string input, string fileName)
        {
            using var file = new StreamWriter(fileName);
            file.Write(input);
        }
    }
}
