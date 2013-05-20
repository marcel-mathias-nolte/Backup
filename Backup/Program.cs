namespace Backup
{
    using System;
    using System.Collections.Generic;

    using Alphaleonis.Win32.Filesystem;
    using Nolte.IO;

    class Program
    {
        static bool abort = false;
        static void Main(string[] args)
        {
            using (Nolte.IO.VolumeShadowCopy vss = new VolumeShadowCopy("Z:\\"))
            {
                vss.DoSnapshot();
                try
                {
                    string path = vss.GetSnapshotPath("Z:\\Downloads\\201212444.pdf");
                    string path2 = vss.GetSnapshotPath(@"Z:\Downloads\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\_very_long_folder_name_\test.txt");
                    Console.WriteLine(path);
                    Console.WriteLine(path2);
                    File.Copy(path, "C:\\201212444.pdf");
                    File.Copy(path2, "C:\\test.txt");
                }
                catch (Exception)
                {
                }
            }

            return;
















            string SourceDir = "I";
            string TargetDir = "D";
            System.IO.FileStream LogFile = new System.IO.FileStream("C:\\" + SourceDir + "_BACKUP.log", System.IO.File.Exists("C:\\" + SourceDir + "_BACKUP.log") ? System.IO.FileMode.Append : System.IO.FileMode.OpenOrCreate);
            System.IO.StreamWriter Log = new System.IO.StreamWriter(LogFile);
            int fdeleted = 0, fcopied = 0, ddeleted = 0, dcopied = 0;
            Stack<string> ToDo = new Stack<string>();
            ToDo.Push(SourceDir + @":\");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            string current, abs;
             
            while(ToDo.Count > 0)
            {
                current = ToDo.Pop();
                try
                {
                    foreach (string dir in System.IO.Directory.GetDirectories(current))
                    {
                        if (abort)
                            return;
                        abs = TargetDir + dir.Substring(1);
                        if (true || abs != "Q:\\XXX\\DOWNLOAD\\tumblr")
                        {
                        try
                        {
                            if (!Junction.Exists(dir))
                            {
                                ToDo.Push(dir);
                            }
                            else
                            {
                                if (Junction.Exists(abs))
                                {
                                    if (Junction.GetTarget(abs) != Junction.GetTarget(dir))
                                    {
                                        try
                                        {
                                            Junction.Create(abs, Junction.GetTarget(dir), true);
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Junction " + abs + " could not be created.");
                                            Log.WriteLine("Junction " + abs + " could not be created.");
                                        }
                                    }
                                }
                                else if (System.IO.Directory.Exists(abs))
                                {
                                    try
                                    {
                                        ddeleted++;
                                        System.IO.Directory.Delete(dir, true);
                                        Console.WriteLine("Directory " + abs + " deleted.");
                                        Log.WriteLine("Directory " + abs + " deleted.");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Directory " + abs + " could not be deleted.");
                                        Log.WriteLine("Directory " + abs + " could not be deleted.");
                                    }
                                    try
                                    {
                                        Junction.Create(abs, Junction.GetTarget(dir), true);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Junction " + abs + " could not be created.");
                                        Log.WriteLine("Junction " + abs + " could not be created.");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        Junction.Create(abs, Junction.GetTarget(dir), true);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Junction " + abs + " could not be created.");
                                        Log.WriteLine("Junction " + abs + " could not be created.");
                                    }
                                }
                            }
                        }
                        catch
                        { }
                        try
                        {
                            if (!System.IO.Directory.Exists(abs))
                            {
                                dcopied++;
                                System.IO.Directory.CreateDirectory(abs);
                                Console.WriteLine("Directory " + abs + " created.");
                                Log.WriteLine("Directory " + abs + " created.");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Directory " + abs + " could not be created.");
                            Log.WriteLine("Directory " + abs + " could not be created.");
                        }
                        }
                    }
                }
                catch
                { 
                    Console.WriteLine("Directory " + current + " could not be read.");
                    Log.WriteLine("Directory " + current + " could not be read.");
                }
                try
                {
                    foreach (string dir in System.IO.Directory.GetDirectories(TargetDir + current.Substring(1)))
                    {
                        if (abort)
                            return;
                        abs = SourceDir + dir.Substring(1);
                        
                        try
                        {
                            if (!System.IO.Directory.Exists(abs))
                            {
                                if (Junction.Exists(dir))
                                {
                                    Junction.Delete(dir);
                                    Console.WriteLine("Junction " + dir + " deleted.");
                                    Log.WriteLine("Junction " + dir + " deleted.");
                                }
                                else
                                {
                                    ddeleted++;
                                    System.IO.Directory.Delete(dir, true);
                                    Console.WriteLine("Directory " + dir + " deleted.");
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Directory " + dir + " could not be deleted.");
                            Log.WriteLine("Directory " + dir + " could not be deleted.");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Directory " + current + " could not be read.");
                    Log.WriteLine("Directory " + current + " could not be read.");
                } 
                try
                {
                    foreach (string file in System.IO.Directory.GetFiles(current))
                    {
                        if (abort)
                            return;
                        abs = TargetDir + file.Substring(1);
                        try
                        {
                            if (!System.IO.File.Exists(abs) || System.IO.File.GetLastWriteTime(file) != System.IO.File.GetLastWriteTime(abs))
                            {
                                fcopied++;
                                System.IO.File.Copy(file, abs, true);
                                Log.WriteLine("File " + file + " copied to " + abs);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine("File " + file + " could not be copied: " + ex.Message);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Directory " + current + " could not be read.");
                    Log.WriteLine("Directory " + current + " could not be read.");
                }
                try
                {
                    foreach (string file in System.IO.Directory.GetFiles(TargetDir + current.Substring(1)))
                    {
                        if (abort)
                            return;
                        abs = SourceDir + file.Substring(1);
                        try
                        {
                            if (!System.IO.File.Exists(abs))
                            {
                                fdeleted++;
                                System.IO.File.Delete(file);
                                Console.WriteLine("File " + file + " deleted.");
                                Log.WriteLine("File " + file + " deleted.");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("File " + file + " could not be deleted.");
                            Log.WriteLine("File " + file + " could not be deleted.");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Directory " + current + " could not be read.");
                    Log.WriteLine("Directory " + current + " could not be read.");
                }
            }
            Console.WriteLine("Backup completed successfully.");
            Console.WriteLine(dcopied.ToString() + " directories created, " + ddeleted.ToString() + " directories deleted.");
            Console.WriteLine(fcopied.ToString() + " files copied, " + fdeleted.ToString() + " files deleted.");
            Log.WriteLine("Backup completed successfully.");
            Log.WriteLine(dcopied.ToString() + " directories created, " + ddeleted.ToString() + " directories deleted.");
            Log.WriteLine(fcopied.ToString() + " files copied, " + fdeleted.ToString() + " files deleted.");
            Log.Flush();
            LogFile.Flush();
            Console.ReadKey();
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            abort = true;
        }
    }
}
