using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FindUnuseableFile
{    
    public class Finder
    {
        private string _pahtRoot = @"D:\projects\FindUnuseableFile\testSolutionDir\";
       // public string _pahtTargetFileDirectory = @"\database\PriceBooksWebside\dbo\Stored Procedures\";
        private readonly List<string> _solutionFiles;
        private List<SqlFileInfo> _sqlFiles { get; set; }
      
        private string[] _extentionIgnore = new [] { ".sln", ".csproj", ".dll"};
        private string[] _foldersIgnore = new[] { "\\bin", "\\obj", "\\Debug", "\\.vs", "\\.vscode", "\\.git", "\\docs", "\\database" };
       // private string[] _foldersIgnore = new[] { "\\.vs", "\\.vscode", "\\.git", "\\docs", "\\database" };
        public Finder(string rootDirectory): this()
        {
            _pahtRoot = rootDirectory;
          //  _foldersIgnore = new[] { _pahtRoot+"database", _pahtRoot+".git", _pahtRoot + ".vs", _pahtRoot + ".vscode" };
          
        }
        public Finder()
        {   
            //todo: use defult dir getsolution directory
            _solutionFiles = new List<string>();          
            _sqlFiles = new List<SqlFileInfo>();
           
        }
        public void GO()
        {
            ProcessDirectory(null);
            FindSql();
            PrintList();
        }
        public void PrintList()
        {           
            Console.WriteLine("_SQLFiles:");
            var res = _sqlFiles.Where(x=> !x.IsUseCS).GroupBy(x => Path.GetDirectoryName(x.FullPath));
            var lines = new List<string>();
            lines.Add("unUsed files");
            foreach (var section in res)
            {
             
                lines.Add($"SECTION - {section.Key}");
                foreach (var file in section)
                {
                    lines.Add($"{file.SqlTag}");                   
                }
            }

            File.WriteAllLines(@"D:\projects\FindUnuseableFile\UnUsedFiles.txt", lines);

            res = _sqlFiles.Where(x => x.IsUseCS).GroupBy(x => Path.GetDirectoryName(x.FullPath));
            lines.Clear();           
            lines.Add("used files");
            foreach (var section in res)
            {               
                lines.Add($"SECTION - {section.Key}");
                foreach (var file in section)
                {
                    lines.Add($"{file.SqlTag}");

                    file.UseFilesCollection[0] = $"[{file.UseFilesCollection[0]}";
                    file.UseFilesCollection[file.UseFilesCollection.Count()-1] = $"{file.UseFilesCollection.Last()}]";

                    lines.AddRange(file.UseFilesCollection);
                }
            }
            File.WriteAllLines(@"D:\projects\FindUnuseableFile\UsedFiles.txt", lines);

        }
        private void FindSql()
        {
            ProcessSQLDirectory();

            SetStatusCS();           

            var validCollection = _sqlFiles.Where(x => x.IsUseCS).ToArray();
            var initCollection = _sqlFiles.Where(x => !x.IsUseCS).ToArray();            

            ProcessSqlFile(initCollection, validCollection);           

        }      
        private void SetStatusCS()
        {           
            foreach (var file in _solutionFiles)
            {
                string text = DeleteComments(File.ReadAllText(file));
                //text =  File.read
              //  var investFiles = _sqlFiles.Where(x=> !x.IsUseCS);

               // if (investFiles.Count() == 0) break;

                foreach (var fileInfo in _sqlFiles)
                {
                    if (text.Contains(fileInfo.SqlTag))
                    {
                        fileInfo.IsUseCS = true;
                      //  fileInfo.IsChecked = true;
                        fileInfo.UseFilesCollection.Add(file);
                    }                   
                    
                }
            }
          
        }
       
        public void ProcessSqlFile(IEnumerable<SqlFileInfo> initCollection, IEnumerable<SqlFileInfo> validCollection)
        {
           
            // пробегаю и ищу входждения в контрольной группе, если нахожу, выставляю ему Юзе
            foreach (var fileInfo in initCollection)
            {
                //  fileInfo.IsUseCS = CheckSQL(fileInfo.SqlTag, validCollection);
                CheckSQL(fileInfo, validCollection);
            }
           
            var nextValidCollection = initCollection.Where(x => x.IsUseCS).ToArray();
            var nextInitCollection = initCollection.Where(x => !x.IsUseCS).ToArray();
           
            if (nextValidCollection.Count()>0)
            {
                ProcessSqlFile(nextInitCollection, nextValidCollection);
            }
           
        }

        private void AddSolutionFiles(string path)
        {           
            // todo: predicate
            string[] fileEntries = Directory.GetFiles(path).Where(x=> !_extentionIgnore.Contains(Path.GetExtension(x))).ToArray();
            _solutionFiles.AddRange(fileEntries);            
           
        }
        private void AddSqlFiles(string path)
        {
            // todo: predicate
           var fileEntries = Directory.GetFiles(path)
                .Where(x => Path.GetExtension(x)==".sql")
                .Select(x => new SqlFileInfo
            {
                SqlTag = Path.GetFileNameWithoutExtension(x),
                FullPath = x,
               // IsChecked = false,
                IsUseCS = false
            }).ToArray();
            _sqlFiles.AddRange(fileEntries);          


        }
        static bool CheckIgnoreDir(string path, string ignorePath)
        {
            var startIndex = path.LastIndexOf('\\');

            var folderName = path.Substring(startIndex);

            return folderName == ignorePath;
        }
        //private void ProcessRootDirectory()
        //{
        //    AddSolutionFiles(_pahtRoot);

        //    //string[] rootDirectory = Directory.GetDirectories(_pahtRoot)
        //    //    .Where(x => !_folderIgnore.Contains(x))
        //    //  //  .Where(x => !x.Equals(_pahtRoot + @"\database"))                
        //    //    .ToArray();

        //    string[] rootDirectory = Directory.GetDirectories(_pahtRoot)
        //       .Where(x => !_foldersIgnore.Any(y => CheckIgnoreDir(x, y))).ToArray();

        //    //.Select(x=>x).ToArray();
        //    foreach (string subdirectory in rootDirectory)
        //        ProcessDirectory(subdirectory);
        //}
        private void ProcessSQLDirectory()
        {
            var sqlRoot = _pahtRoot + "database";
            AddSqlFiles(sqlRoot);

            string[] rootDirectory = Directory.GetDirectories(sqlRoot)
               // .Where(x => !x.Equals(sqlRoot+@"\azure")) 
                //  .Where(x => !x.Equals(_pahtRoot + @"\database"))                
                .ToArray();
            
            foreach (string subdirectory in rootDirectory)
                ProcessSqlDirectory(subdirectory);
        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        private  void ProcessDirectory(string targetDirectory )
        {
            if (string.IsNullOrEmpty(targetDirectory)) targetDirectory = _pahtRoot;

            AddSolutionFiles(targetDirectory);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory)
                .Where(x => !_foldersIgnore.Any(y => CheckIgnoreDir(x, y))).ToArray(); ///// curent
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        //todo: add Func(string)
        private void ProcessSqlDirectory(string targetDirectory)
        {
            AddSqlFiles(targetDirectory);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessSqlDirectory(subdirectory);
        }
        private string DeleteComments(string fileText)
        {
            var reg = new Regex(@"((?:\/\*(?:[^*]|(?:\*+[^*\/]))*\*+\/)|(?:\/\/.*))");


            return reg.Replace(fileText, string.Empty);
        }

        private void CheckSQL(SqlFileInfo fileInfo, IEnumerable<SqlFileInfo> validCollection)
        {
          //  bool result = false;
            foreach (var file in validCollection)
            {
                string text = File.ReadAllText(file.FullPath);
                if (text.Contains(fileInfo.SqlTag))
                {
                    fileInfo.IsUseCS = true;
                    // add
                    fileInfo.UseFilesCollection.Add(file.FullPath);
                    //break;
                }
            }
           // return result;

        }



    }
}
