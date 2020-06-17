using System;
using System.Collections.Generic;
using System.Text;

namespace DataPanel.Models
{
    public class DataGridTagModel
    {
        public DataGridTagModel(string tagId, int count)
        {
            TagId = tagId;
            Count = count;
        }

        public string TagId { get; set; }
        public int Count { get; set; }
    }
}
