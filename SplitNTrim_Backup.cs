using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace NugetCleanupTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] allfileslist;
            string strRootpath = @"E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\src";
            string tfsRootpath = @"E:\OneESBaseline\SCOM\private\product";
            string oneesOutRootpath = @"E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out";
            string strPath = @"E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\src\product\sdk\server\RuntimeService";
            //string directoryCmdPath = String.Empty;

            //Task1 
            List<string> allresultsExternal = new List<string>();
            List<string> allresultsInternal = new List<string>();
            List<string> allRootfilesItemList1 = new List<string>();
            List<string> allRootfilesItemList2 = new List<string>();

            string[] Outlines;
            List<string> allRootfilesItemList = new List<string>();
            List<string> allCsprojOfNugetsList = new List<string>();
            if (args.Length != 0)
            {
                strRootpath = args[0];
            }
            //GetCSprojlistIntoTextFile(strRootpath, allRootfilesItemList);
            //allfileslist = GetCSprojlistIntoTextFile(strRootpath, allRootfilesItemList);
            //Findcorrespondingcsproj(strPath, allCsprojOfNugetsList);

            //////Task 1
            // Below Code reads the entry from dirs.proj as for migrated projects we create csproj entry in dirs.proj 
            // CSV List of migrated .csproj,.vcxproj,.mpproj,.wixproj projects
            //InternalBinaryList(strRootpath);


            //////Task 2
            // List of .inc files under a migrated project folders
            //IncFilelist(strRootpath);


            //////Task 3
            // List of files in placefile
            // Take the Key, Value pairs from each line of placefile Which do not start ';' character
            // Read the Value seprated by ':' character into below format
            // Example: 
            // A.dll path1:path2:path3
            // <Xxx.dll> <path1>:<path2>:<path3>
            //FilesinPlaceFileList(strRootpath);

            //////Task 2
            // Sources, Makefile, and Makefile.inc cleanup: These files should be deleted from all migrated projects. 
            // This task should also remove the files.inc related files.
            // Getting the list of all migrated csproj,mpproj,vcxproj etc.
            //allRootfilesItemList2 = MigratedProjectsList(strRootpath);
            //foreach (var directoryCmdPath in allRootfilesItemList2)
            //{
            //    CleanUpTFSFile(strRootpath, directoryCmdPath);
            //    GetFilesList(strRootpath, directoryCmdPath);
            //}
            //// check file are there in coresponding csproj or not, Write script which can return list of all files.inc
            ////list of directory of Migrated projects, if my files.inc search directory matches to the list I will delete otherwise keep as it is
            //List<string> migratedProjectItemList = MigratedProjectsList(strRootpath);
            ////List<string> filesIncList = new List<string>();
            //foreach (var directoryCmdPath in migratedProjectItemList)
            //{
            //    DeleteFiles(directoryCmdPath, "files.inc");
            //    //filesIncList = GetFilesList(strRootpath, directoryCmdPath);
            //    //FilesIncHitList(strRootpath, directoryCmdPath);

            //}

            ///// Task 1
            //NugetList();

            allRootfilesItemList1 = PlacefileMigration(tfsRootpath);
            //System.Diagnostics.Process.Start("CMD.exe", "/C ipconfig");
            //string command = @"robocopy E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\src\product\uX\Core\Agent.CSMScripting\objd\amd64 $OutDir\uX\Core Agent.CSMScripting.dll";
            //string command = @"robocopy $(INETROOT)\out\debug-amd64\Agent.CSMScripting $(INETROOT)\out\placefiled\uX\Core Agent.CSMScripting.dll";
            //ExecuteCommandSync(command);

            //System.Diagnostics.Process.Start(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\test.bat");
            //GetBinariesFromOutPath(oneesOutRootpath, allRootfilesItemList1);
            List<string> allItemList = new List<string>();
            foreach (var strs in allRootfilesItemList1)
            {
                test(oneesOutRootpath, strs, allItemList);
            }
            File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\placeFileList.txt", allItemList.ToList());
            Console.ReadLine();
        }

        private static void test(string oneesOutRootpath, string strs, List<string> allItemList)
        {
            //string str = @"Microsoft.EnterpriseManagement.Utility.WorkflowExpansion.dll E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out\placefiled,E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out\placefiledTest_Dependency\bin\";
            string file = string.Empty;
            string inputFilePath = string.Empty;
            string item1 = string.Empty;
            List<string> targetPaths = new List<string>();
            //string[] separators = { ".dll", ".exe", ".xsd", ".gif", ".pdb" };
            string[] separators = { " ", "\t" };
            string[] str1 = strs.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            bool flag = true;

            foreach (var word in str1)
            {
                if (flag)
                {
                    //Console.WriteLine("DLL");
                    //Console.WriteLine(word);
                    file = word;
                    flag = false;
                    bool flag1 = false;
                    inputFilePath = getFilePathOfDLL(file, oneesOutRootpath, flag1);
                    if (flag1)
                    {
                        flag1 = false;
                        break;                        
                    }
                }
                else
                {
                    //Console.WriteLine("Target Path/Paths");
                    targetPaths.Add(word);
                    //Console.WriteLine("........");

                    string[] separators2 = { "," };
                    string[] str2 = word.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                    string str3 = string.Empty;
                    foreach (var item in str2.ToArray())
                    {
                        char last = item[item.Length - 1];
                        if (last.Equals('\\'))
                        {
                            item1 = item.Remove(item.Length - 1);
                            str3 = "robocopy " + Path.GetDirectoryName(inputFilePath) + " " + item1.Trim() + " " + file;
                            Console.WriteLine(str3);
                            allItemList.Add(str3);
                        }
                        else
                        {
                            str3 = "robocopy " + Path.GetDirectoryName(inputFilePath) + " " + item.Trim() + @" " + file;
                            Console.WriteLine(str3);
                            allItemList.Add(str3);
                        }
                    }
                }
            }

        }

        private static string getFilePathOfDLL(string file, string oneesOutRootpath, bool flag1)
        {
            string filepath1 = string.Empty;
            filepath1 = Directory.GetFiles(oneesOutRootpath, file, SearchOption.AllDirectories).FirstOrDefault();
            if (filepath1 == null)
            {
                flag1 = true;
                Console.WriteLine("------------ Empty-------------");
            }
            return filepath1;
        }

        private static void GetBinariesFromOutPath(string oneesOutRootpath, List<string> allRootfilesItemList1)
        {
            //string line1 = allRootfilesItemList1[i];
            string str1, strHit, strStatus1, strStatus2;


            // Add the below line for INETROOT or any other detinations
            // Replacing "target\%BUILDTYPE%\%CPUTYPE%\ " with "$OutDir"
            //string[] replacedLines = lines.Select(x => x.Replace(@"target\%BUILDTYPE%\%CPUTYPE%\", @"$(INETROOT)\out\placefiled\")).ToArray();


            if (Directory.Exists(oneesOutRootpath))
            {
                List<string> locallist = new List<string>();
                //string[] allfileslist1 = Directory.GetFiles(oneesOutRootpath, "*dirs.*", SearchOption.AllDirectories);

                //string[] lines = System.IO.File.ReadAllLines(line1);
                //string[] lines =

                // string str1, str2;
                // Display the file contents by using a foreach loop.
                foreach (var item in allRootfilesItemList1)
                {
                    if (item != "")
                    {
                        StringBuilder stringBuilder = new StringBuilder(item.Length);
                        int j = 0;
                        foreach (char c in item)
                        {
                            if (c != ' ' || j == 0 || item[j - 1] != ' ')
                                stringBuilder.Append(c);
                            j++;
                        }
                        str1 = stringBuilder.ToString();
                        if (!str1.Contains(";"))
                        {
                            str1 = str1.Replace(":", ",");
                            locallist.Add(str1);
                        }
                    }
                }


            }
        }

        private static List<string> PlacefileMigration(string tfsRootpath)
        {
            List<string> allRootfilesItemList2 = new List<string>();
            allRootfilesItemList2 = MasterPlaceFileList(tfsRootpath);
            return allRootfilesItemList2;
        }
        private static List<string> MasterPlaceFileList(string tfsRootpath)
        {
            List<string> allRootfilesItemList1 = new List<string>();
            List<string> targetFilesItemList = new List<string>();

            if (Directory.Exists(tfsRootpath))
            {
                int i = 0;
                string[] allfileslist = Directory.GetFiles(tfsRootpath, "*placefile*", SearchOption.AllDirectories);
                string strHit, str1 = string.Empty;
                bool flag = true;
                //foreach (var item in allfileslist)
                //{
                //    i = i + 1;
                //    str1 = i.ToString() + "," + item;
                //    System.Console.WriteLine(str1);
                //    allRootfilesItemList.Add(str1);
                //}
                foreach (string line1 in allfileslist)
                {
                    string[] lines = System.IO.File.ReadAllLines(line1);

                    string[] replacedLines = lines.Select(x => x.Replace(":", ",")).ToArray();
                    // Add the below line for INETROOT or any other detinations
                    // Replacing "target\%BUILDTYPE%\%CPUTYPE%\ " with "$OutDir"
                    //string[] replacedLines = lines.Select(x => x.Replace(@"target\%BUILDTYPE%\%CPUTYPE%\", @"$(INETROOT)\out\placefiled\")).ToArray();                    
                    string[] replacedLines1 = replacedLines.Select(x => x.Replace(@"target\%BUILDTYPE%\%CPUTYPE%\", @"E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out\placefiled")).ToArray();

                    // string str1, str2;
                    // Display the file contents by using a foreach loop.
                    foreach (var item in replacedLines1)
                    {
                        if (item != "" & item != ";")
                        {
                            StringBuilder stringBuilder = new StringBuilder(item.Length);
                            int j = 0;
                            foreach (char c in item)
                            {
                                if (c != ' ' || j == 0 || item[j - 1] != ' ')
                                    stringBuilder.Append(c);
                                j++;
                            }
                            str1 = stringBuilder.ToString();
                            if (!str1.Contains(";"))
                            {
                                //str1 = str1.Replace(":", ",");


                                //strHit = line2.Split('.dll')[line2.Split('\\').Length - 1].Replace("</HintPath>", string.Empty);

                                allRootfilesItemList1.Add(str1);
                            }
                        }
                    }
                }
                allfileslist = null;
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\masterPlaceFileList.txt", allRootfilesItemList1.ToList());
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(ass.Location);
                //System.IO.File.Copy(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", @path + @"\InternalBinaryList.txt", true);
            }
            //Console.ReadLine();
            return allRootfilesItemList1;
        }

        private static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }

        private static void NugetList()
        {
            // string strRootpath = @"\\WIN201613\1ES.SCOMMigration_071718\SystemCenter\Migration\SCOM\src\";
            string strRootpath = ConfigurationManager.AppSettings["Rootpath"];
            string[] allfileslist;
            List<string> allInternalItemList = new List<string>();
            List<string> allExternal_dllPath = new List<string>();
            List<string> allresultsExternal = new List<string>();
            List<string> allresultsInternal = new List<string>();
            // string strCDMBuild_dllPath = @"D:\CDM_SFE\Branches\OnPrem\1ES_Baseline\OnPrem\SCOM\private\product\external\";
            string strExternal_dllPath = ConfigurationManager.AppSettings["External_dllPath"];
            if (Directory.Exists(strExternal_dllPath))
            {
                allfileslist = Directory.GetFiles(strExternal_dllPath, "*.dll", SearchOption.AllDirectories);
                foreach (string line1 in allfileslist)
                {
                    allExternal_dllPath.Add(line1);
                }
            }
            List<string> allfileslist1 = Directory.GetFiles(strRootpath, "*.*", SearchOption.AllDirectories)
      .Where(file => new string[] { ".csproj" /*, ".mpproj"*/ }
      .Contains(Path.GetExtension(file)))
      .ToList();
            for (int i = 0; i < allfileslist1.Count; i++)

            {
                string line1 = allfileslist1[i];
                string s1, strHit, strStatus1, strStatus2;
                string[] Internal_lines = System.IO.File.ReadAllLines(line1);
                foreach (string line2 in Internal_lines)
                {
                    if (line2.Contains("HintPath"))
                    {
                        s1 = line2.Replace("<HintPath>", string.Empty).Replace("</HintPath>", string.Empty);
                        strHit = line2.Split('\\')[line2.Split('\\').Length - 1].Replace("</HintPath>", string.Empty);
                        strStatus1 = allExternal_dllPath.Where(x => x.Contains(strHit)).FirstOrDefault();
                        if (strStatus1 != null)
                        {
                            //External
                            strStatus2 = allresultsExternal.Where(x => x.Contains(strHit)).FirstOrDefault();
                            if (strStatus2 == null)
                            {
                                //allresultsExternal.Add(strHit + ",External");
                                allresultsExternal.Add(strHit);
                            }
                        }
                        else
                        {
                            strStatus2 = allresultsInternal.Where(x => x.Contains(strHit)).FirstOrDefault();
                            //Internal
                            if (strStatus2 == null)
                            {
                                //allresultsInternal.Add(strHit + ",Internal");
                                allresultsInternal.Add(strHit);
                            }
                        }
                    }
                }
            }
            Assembly ass = Assembly.GetExecutingAssembly();
            string path = System.IO.Path.GetDirectoryName(ass.Location);
            //File.WriteAllLines(path + "\\NuGetListforExternal.txt", allresultsExternal.ToList());
            //File.WriteAllLines(path + "\\NuGetListforInternal.txt", allresultsInternal.ToList());

            File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\NuGetListforExternal.txt", allresultsExternal.ToList());
            File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\NuGetListforInternal.txt", allresultsInternal.ToList());

            Console.WriteLine(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\NuGetListforExternal.txt", allresultsInternal.ToList());
            Console.WriteLine(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\NuGetListforInternal.txt", allresultsInternal.ToList());
        }

        private static void FilesIncHitList(string strRootpath, string directoryCmdPath)
        {
            DeletedFilesIncLst(directoryCmdPath, "files.inc");
            throw new NotImplementedException();
        }

        private static void DeletedFilesIncLst(string directoryCmdPath, string v)
        {
            throw new NotImplementedException();
        }

        // Sources, Makefile, and Makefile.inc & files.inc related files
        private static void CleanUpTFSFile(string strRootpath, string directoryCmdPath)
        {
            DeleteFiles(directoryCmdPath, "sources");
            DeleteFiles(directoryCmdPath, "Makefile");
            DeleteFiles(directoryCmdPath, "makefile.inc");
            //DeleteFiles(directoryCmdPath, "files.inc");
        }
        private static void DeleteFiles(string rootpath, string format, Func<string, bool> searchFilterFunc = null)
        {
            List<string> lstDelete = new List<string>();
            var csprojfiles = (searchFilterFunc == null) ? Directory.GetFiles(rootpath, format, SearchOption.AllDirectories) :
                Directory.GetFiles(rootpath, format, SearchOption.AllDirectories).Where(searchFilterFunc);
            foreach (string csprojfile in csprojfiles)
            {
                Console.WriteLine("Deleting {0}", csprojfile);
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\DeleteLogs.txt", lstDelete.ToList());
                ForceDeleteFile(csprojfile);
            }
        }
        private static void ForceDeleteFile(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }
            RemoveReadOnlyFlag(filename);
            File.Delete(filename);
        }
        private static void RemoveReadOnlyFlag(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }
            FileAttributes attrs = File.GetAttributes(filename);
            if (attrs.HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(filename, attrs & ~FileAttributes.ReadOnly);
            }
        }

        //
        private static List<string> GetFilesList(string strRootpath, string directoryCmdPath)
        {
            List<string> lst1 = new List<string>();
            return lst1 = GetFilesLst(directoryCmdPath, "files.inc");
        }
        private static List<string> GetFilesLst(string rootpath, string format, Func<string, bool> searchFilterFunc = null)
        {
            List<string> lst = new List<string>();
            int i = 0;
            var csprojfiles = (searchFilterFunc == null) ? Directory.GetFiles(rootpath, format, SearchOption.AllDirectories) :
                Directory.GetFiles(rootpath, format, SearchOption.AllDirectories).Where(searchFilterFunc);
            foreach (string csprojfile in csprojfiles)
            {
                Console.WriteLine("List of files.incs:");
                Console.WriteLine("{0} : {1}", i++, csprojfile);

            }
            File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\filesInc.txt", lst.ToList());
            return lst;
        }

        private static void FilesinPlaceFileList(string strRootpath)
        {
            List<string> allRootfilesItemList = new List<string>();

            if (Directory.Exists(strRootpath))
            {
                int i = 0;
                string[] allfileslist = Directory.GetFiles(strRootpath, "*placefile*", SearchOption.AllDirectories);
                string str1 = string.Empty;
                bool flag = true;
                //foreach (var item in allfileslist)
                //{
                //    i = i + 1;
                //    str1 = i.ToString() + "," + item;
                //    System.Console.WriteLine(str1);
                //    allRootfilesItemList.Add(str1);
                //}
                foreach (string line1 in allfileslist)
                {
                    string[] lines = System.IO.File.ReadAllLines(line1);

                    // string str1, str2;
                    // Display the file contents by using a foreach loop.
                    foreach (var item in lines)
                    {
                        StringBuilder stringBuilder = new StringBuilder(item.Length);
                        int j = 0;
                        foreach (char c in item)
                        {
                            if (c != ' ' || j == 0 || item[j - 1] != ' ')
                                stringBuilder.Append(c);
                            j++;
                        }
                        str1 = stringBuilder.ToString();
                        if (!str1.Contains(";"))
                        {
                            str1 = str1.Replace(":", ",");
                            allRootfilesItemList.Add(str1);
                        }
                    }
                }
                allfileslist = null;
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\filesInPlaceFileList.txt", allRootfilesItemList.ToList());
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(ass.Location);
                //System.IO.File.Copy(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", @path + @"\InternalBinaryList.txt", true);
            }
            Console.ReadLine();
        }

        private static void IncFilelist(string strRootpath)
        {
            List<string> allRootfilesItemList = new List<string>();

            if (Directory.Exists(strRootpath))
            {
                int i = 0;
                string[] allfileslist = Directory.GetFiles(strRootpath, "*.inc", SearchOption.AllDirectories);
                string str1;
                foreach (var item in allfileslist)
                {
                    i = i + 1;
                    str1 = i.ToString() + "," + item;
                    System.Console.WriteLine(str1);
                    allRootfilesItemList.Add(str1);
                }
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\IncFileList.txt", allRootfilesItemList.ToList());
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(ass.Location);
                //System.IO.File.Copy(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", @path + @"\InternalBinaryList.txt", true);
            }
            Console.ReadLine();
        }

        private static List<string> MigratedProjectsList(string strInputFolderPath)
        {
            List<string> allRootfilesItemList = new List<string>();
            //strInputFolderPath = strInputFolderPath.Remove(strInputFolderPath.Length - 1);

            if (Directory.Exists(strInputFolderPath))
            {
                int i = 0;
                string[] allfileslist = Directory.GetFiles(strInputFolderPath, "*dirs.*", SearchOption.AllDirectories);
                foreach (string line1 in allfileslist)
                {
                    string[] lines = System.IO.File.ReadAllLines(line1);
                    // string str1, str2;
                    // Display the file contents by using a foreach loop.
                    foreach (string line in lines)
                    {
                        if (line.Contains(@"<ProjectReference") && !line.Contains(@"!--") && !line.Contains(@"-->") && !line.Contains(@"dirs.proj") && line.Contains(@".csproj"))
                        {
                            var line0 = line.Replace(@"<ProjectReference Include=", "").Replace("\t", "").Replace(" ", "").Replace("\"", "").Replace(@"$(SRCROOT)", strInputFolderPath).Replace(@"/>", "");
                            if (File.Exists(line0))
                            {
                                string appPath = System.IO.Path.GetDirectoryName(line0);
                                allRootfilesItemList.Add(appPath);
                            }
                            else
                            {
                                System.Console.WriteLine(line0);
                            }
                        }
                    }
                    allfileslist = null;
                    //File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\MigratedProjectsList.txt", allRootfilesItemList.ToList());
                }
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(ass.Location);
                //System.IO.File.Copy(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", @path + @"\InternalBinaryList.txt", true);
            }
            //Console.ReadLine();
            return allRootfilesItemList;
        }

        private static void InternalBinaryList(string strInputFolderPath)
        {
            List<string> allRootfilesItemList = new List<string>();
            //strInputFolderPath = strInputFolderPath.Remove(strInputFolderPath.Length - 1);

            if (Directory.Exists(strInputFolderPath))
            {
                int i = 0;
                string[] allfileslist = Directory.GetFiles(strInputFolderPath, "*dirs.*", SearchOption.AllDirectories);
                foreach (string line1 in allfileslist)
                {
                    string[] lines = System.IO.File.ReadAllLines(line1);
                    // string str1, str2;
                    // Display the file contents by using a foreach loop.
                    foreach (string line in lines)
                    {
                        if (line.Contains(@"<ProjectReference") && !line.Contains(@"!--") && !line.Contains(@"-->") && !line.Contains(@"dirs.proj") && line.Contains(@".csproj"))
                        {
                            var line0 = line.Replace(@"<ProjectReference Include=", "").Replace("\t", "").Replace(" ", "").Replace("\"", "").Replace(@"$(SRCROOT)", strInputFolderPath).Replace(@"/>", "");
                            if (File.Exists(line0))
                            {
                                string[] lines1 = System.IO.File.ReadAllLines(line0);
                                string str1, str2 = "", str3;
                                i++;
                                string strAssembly = string.Empty, strGu = string.Empty;
                                string strAssemblyline = lines1.Where(x => x.Contains(@"<AssemblyName>")).FirstOrDefault();
                                if (!string.IsNullOrEmpty(strAssemblyline))
                                {
                                    strAssembly = strAssemblyline.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "").Trim();

                                    string strGuideline = lines1.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(strGuideline))
                                    {
                                        strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
                                    }
                                    str2 = i.ToString() + "," + strAssembly.Trim() + "," + line0.Trim() + "," + strGu + "," + ".csproj";
                                    System.Console.WriteLine(str2);
                                    allRootfilesItemList.Add(str2);
                                }
                            }
                            else
                            {
                                System.Console.WriteLine(line0);
                            }
                        }
                        if (line.Contains(@"<ProjectReference") && !line.Contains(@"!--") && !line.Contains(@"-->") && !line.Contains(@"dirs.proj") && line.Contains(@".vcxproj"))
                        {
                            var line0 = line.Replace(@"<ProjectReference Include=", "").Replace("\t", "").Replace(" ", "").Replace("\"", "").Replace(@"$(SRCROOT)", strInputFolderPath).Replace(@"/>", "");
                            if (File.Exists(line0))
                            {
                                string[] lines1 = System.IO.File.ReadAllLines(line0);
                                string str1, str2 = "", str3;
                                i++;
                                string strAssembly = string.Empty, strGu = string.Empty;
                                string strAssemblyline = lines1.Where(x => x.Contains(@"<AssemblyName>")).FirstOrDefault();
                                if (!string.IsNullOrEmpty(strAssemblyline))
                                {
                                    strAssembly = strAssemblyline.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "").Trim();

                                    string strGuideline = lines1.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(strGuideline))
                                    {
                                        strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
                                    }
                                    str2 = i.ToString() + "," + strAssembly.Trim() + "," + line0.Trim() + "," + strGu + "," + ".vcxproj";
                                    System.Console.WriteLine(str2);
                                    allRootfilesItemList.Add(str2);
                                }
                            }
                            else
                            {
                                System.Console.WriteLine(line0);
                            }
                        }
                        if (line.Contains(@"<ProjectReference") && !line.Contains(@"!--") && !line.Contains(@"-->") && !line.Contains(@"dirs.proj") && line.Contains(@".mpproj"))
                        {
                            var line0 = line.Replace(@"<ProjectReference Include=", "").Replace("\t", "").Replace(" ", "").Replace("\"", "").Replace(@"$(SRCROOT)", strInputFolderPath).Replace(@"/>", "");
                            if (File.Exists(line0))
                            {
                                string[] lines1 = System.IO.File.ReadAllLines(line0);
                                string str1, str2 = "", str3;
                                i++;
                                string strAssembly = string.Empty, strGu = string.Empty;
                                string strAssemblyline = lines1.Where(x => x.Contains(@"<AssemblyName>")).FirstOrDefault();
                                if (!string.IsNullOrEmpty(strAssemblyline))
                                {
                                    strAssembly = strAssemblyline.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "").Trim();

                                    string strGuideline = lines1.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(strGuideline))
                                    {
                                        strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
                                    }
                                    str2 = i.ToString() + "," + strAssembly.Trim() + "," + line0.Trim() + "," + strGu + "," + ".mpproj";
                                    System.Console.WriteLine(str2);
                                    allRootfilesItemList.Add(str2);
                                }
                            }
                            else
                            {
                                System.Console.WriteLine(line0);
                            }
                        }
                        if (line.Contains(@"<ProjectFile") && !line.Contains(@"!--") && !line.Contains(@"-->") && !line.Contains(@"dirs.proj") && line.Contains(@".wixproj"))
                        {
                            var line0 = line.Replace(@"<ProjectFile Include=", "").Replace("\t", "").Replace(" ", "").Replace("\"", "").Replace(@"$(SRCROOT)", strInputFolderPath).Replace(@"/>", "");
                            line0 = line1.Replace("dirs.proj", "") + line0;
                            if (File.Exists(line0))
                            {
                                string[] lines1 = System.IO.File.ReadAllLines(line0);
                                string str1, str2 = "", str3;
                                i++;
                                string strAssembly = string.Empty, strGu = string.Empty;
                                string strAssemblyline = lines1.Where(x => x.Contains(@"<Name>")).FirstOrDefault();
                                if (!string.IsNullOrEmpty(strAssemblyline))
                                {
                                    strAssembly = strAssemblyline.Replace(@"<Name>", "").Replace(@"</Name>", "").Trim();

                                    string strGuideline = lines1.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
                                    if (!string.IsNullOrEmpty(strGuideline))
                                    {
                                        strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
                                    }
                                    str2 = i.ToString() + "," + strAssembly.Trim() + "," + line0.Trim() + "," + strGu + "," + ".wixproj";
                                    System.Console.WriteLine(str2);
                                    allRootfilesItemList.Add(str2);
                                }
                            }
                            else
                            {
                                System.Console.WriteLine(line0);
                            }
                        }

                    }

                }
                allfileslist = null;
                //Assembly ass = Assembly.GetExecutingAssembly();
                //string path = System.IO.Path.GetDirectoryName(ass.Location);
                //File.WriteAllLines(@path + @"\InternalBinaryList.txt", allRootfilesItemList.ToList());
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\InternalBinaryList.txt", allRootfilesItemList.ToList());
            }
            else
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                string path = System.IO.Path.GetDirectoryName(ass.Location);
                //System.IO.File.Copy(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", @path + @"\InternalBinaryList.txt", true);
            }
            Console.ReadLine();
        }

        private static void GetCSprojlistIntoTextFile(string strRootpath, List<string> allRootfilesItemList)
        {
            string[] allfileslist;
            if (Directory.Exists(strRootpath))
            {
                int i = 0;
                allfileslist = Directory.GetFiles(strRootpath, "*.csproj", SearchOption.AllDirectories);
                foreach (string line1 in allfileslist)
                {

                    string[] lines = System.IO.File.ReadAllLines(line1);
                    string str1, str2;
                    // Display the file contents by using a foreach loop.


                    /*foreach (string line in lines)
                    {
                        // Use a tab to indent each line of the file.
                        if (line.Contains(@"<AssemblyName>") && line.Contains(@"</AssemblyName>"))
                        {
                            str1 = line.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "");
                            i++;
                            str2 = i.ToString() + "," + str1.Trim() + "," + line1.Trim();
                            System.Console.WriteLine(str2);
                            allRootfilesItemList.Add(str2);

                        }

                    }*/

                    string strAssembly = string.Empty, strGu = string.Empty;
                    string strAssemblyline = lines.Where(x => x.Contains(@"<AssemblyName>")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(strAssemblyline))
                    {
                        strAssembly = strAssemblyline.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "").Trim();

                        string strGuideline = lines.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
                        if (!string.IsNullOrEmpty(strGuideline))
                        {
                            strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
                        }
                        i++;
                        str2 = i.ToString() + "," + strAssembly.Trim() + "," + line1.Trim() + "," + strGu;

                        System.Console.WriteLine(str2);
                        allRootfilesItemList.Add(str2);

                    }
                }

                //File.WriteAllLines(@"\\sccxe-scratch\scratch\v-satvin\IES\InternalBinaryList.txt", allRootfilesItemList.ToList());
                File.WriteAllLines(@"C:\Users\v-gikala\source\repos\NugetCleanupTest\NugetCleanupTest\InternalBinaryList.txt", allRootfilesItemList.ToList());


            }
        }
        //private static void Findcorrespondingcsproj(string strPath, List<string> allCsprojOfNugetsList)
        //{
        //    string[] allfileslist;
        //    if (Directory.Exists(strPath))
        //    {
        //        int i = 0;
        //        allfileslist = Directory.GetFiles(strPath, "*.csproj", SearchOption.AllDirectories);
        //        foreach (string line1 in allfileslist)
        //        {

        //            string[] lines = System.IO.File.ReadAllLines(line1);
        //            string str1, str2;
        //            Display the file contents by using a foreach loop.


        //            /*foreach (string line in lines)
        //            {
        //                 Use a tab to indent each line of the file.
        //                if (line.Contains(@"<AssemblyName>") && line.Contains(@"</AssemblyName>"))
        //                {
        //                    str1 = line.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "");
        //                    i++;
        //                    str2 = i.ToString() + "," + str1.Trim() + "," + line1.Trim();
        //                    System.Console.WriteLine(str2);
        //                    allRootfilesItemList.Add(str2);

        //                }

        //            }*/

        //           string strAssembly = string.Empty, strGu = string.Empty;
        //            string strAssemblyline = lines.Where(x => x.Contains(@"<AssemblyName>")).FirstOrDefault();
        //            if (!string.IsNullOrEmpty(strAssemblyline))
        //            {
        //                strAssembly = strAssemblyline.Replace(@"<AssemblyName>", "").Replace(@"</AssemblyName>", "").Trim();

        //                string strGuideline = lines.Where(x => x.Contains(@"<ProjectGuid>")).FirstOrDefault();
        //                if (!string.IsNullOrEmpty(strGuideline))
        //                {
        //                    strGu = (strGuideline.Replace(@"<ProjectGuid>", "").Replace(@"</ProjectGuid>", "").Replace("{", "").Replace("}", "")).ToString().Trim();
        //                }
        //                i++;
        //                str2 = i.ToString() + "," + strAssembly.Trim() + "," + line1.Trim() + "," + strGu;

        //                System.Console.WriteLine(str2);
        //                allRootfilesItemList.Add(str2);

        //            }
        //        }
        //    }
        //}
    }
}
