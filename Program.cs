using System;
using System.IO;
using System.Linq;

namespace FindUnuseableFile
{
    class Program
    {
        static bool CheckIgnoreDir(string path, string ignorePath)
        {
            var startIndex = path.LastIndexOf('\\');

            var folderName = path.Substring(startIndex);

            return folderName == ignorePath;
        }
        //y => Path.GetExtension(x)
        static void Main(string[] args)
        {
            //string[] _extentionIgnore = new[] { "sln", "csprj" };
            // string[] _foldersIgnore = new[] { "\\bin", "\\debug", "\\.vs", "\\.vscode", "\\.git", "\\docs" };
            var str = @"start_code();
/****
 * Common multi-line comment style.
 ****/
more_code(); 
/*
/*
 * Another common multi-line comment style.
//sfsd dsf
 */
// somthing
var str = new;
end_code();";

            var root = @"D:\projects\BSOFresh\";

            //string[] fileEntries = Directory.GetDirectories(root)
            //   .Where(x => !_foldersIgnore.Any(y=> CheckIgnoreDir(x,y))).ToArray();
            //var dd = new CallThis("newString");
            //dd.Print();
            var tt = new Finder(root);        
          
            tt.GO();


            //D:\projects\BSOFresh
            // string[] rootDirectory = Directory.GetDirectories(@"D:\projects\BSOFresh");
            //string[] rootDirectory =  Directory.GetFileSystemEntries(@"D:\projects\BSOFresh");
            //foreach (var item in fileEntries)
            //{
            //    Console.WriteLine(item);
            //}
            //string substr = "ttr";
            //string str = "asdasdasdttrdd asdas";
            //if (str.Contains(substr))
            //{
            //    Console.WriteLine("conteins!!");
            //}
           // Console.ReadKey();
        }
    }
}
