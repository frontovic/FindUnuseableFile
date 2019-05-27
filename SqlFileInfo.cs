using System;
using System.Collections.Generic;
using System.Text;

namespace FindUnuseableFile
{
    public class SqlFileInfo
    {      
        public string FullPath { get; set; }
        public string SqlTag { get; set; }
       // public bool IsChecked { get; set; } // тру когда все пары перебраны
        public bool IsUseCS { get; set; } // тру если файл точно используется в CS
        public IList<string> UseFilesCollection { get; set; } = new List<string>();

    }
}
