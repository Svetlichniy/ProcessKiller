using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessKiller.Helpers
{
    public static class FileHelper
    {
        public static string filePath { get; set; }
        public static void ExportBlackList(List<string> BlackList)
        {
            using (var Sdialog = new SaveFileDialog())
            {
                Sdialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                if (Sdialog.ShowDialog() == DialogResult.OK)
                {
                    using (var SWriter = new StreamWriter(new FileStream( Sdialog.FileName, FileMode.OpenOrCreate, FileAccess.Write ), Encoding.UTF8))
                    {
                        foreach(var line in BlackList)
                            SWriter.WriteLine(line);
                    }
                }
            }
        }

        public static List<string> ImportBlackList()
        {
            var BlackList = new List<string>();
            using (var Odialog = new OpenFileDialog())
            {
                Odialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
                if (Odialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = Odialog.FileName;
                    using (var Sreader = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read), Encoding.UTF8))
                    {
                        string line;
                        while ((line = Sreader.ReadLine()) != null)
                        {
                            BlackList.Add(line);
                        }
                    }
                }
            }
            return BlackList;
        }

        public static List<string> ImportBlackList(string FilePath)
        {
            var BlackList = new List<string>();
            if (string.IsNullOrEmpty(FilePath))
                return BlackList;

            using (var Sreader = new StreamReader(new FileStream(FilePath, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                string line;
                while ((line = Sreader.ReadLine()) != null)
                {
                    BlackList.Add(line);
                }
            }
            
            return BlackList;
        }
    }
}
