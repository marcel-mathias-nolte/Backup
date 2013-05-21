using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nolte.IO.Backup
{
    using System.Globalization;
    using System.IO;
    using System.Threading;

    public class BackupJob
    {
        private JobType type;

        private string sourceDirectory;

        private string targetDirectory;

        private volatile bool paused = false;

        private volatile bool abort = false;

        private System.Threading.Thread thread;

        public BackupJob()
        {
            this.State = JobState.New;
            this.thread = new Thread(this.Worker);
        }

        public JobType Type
        {
            get
            {
                return this.type;
            }
            
            set
            {
                if (this.State == JobState.Aborted || this.State == JobState.Finished || this.State == JobState.New)
                {
                    this.type = value;
                }
            }
        }

        public JobState State { get; private set; }

        public string SourceDirectory
        {
            get
            {
                return this.sourceDirectory;
            }

            set
            {
                if (this.State == JobState.Aborted || this.State == JobState.Finished || this.State == JobState.New)
                {
                    this.sourceDirectory = value;
                }
                else
                {
                    throw new NotSupportedException("Property SourceDirectory cannot be changed while a job is running or paused.");
                }
            }
        }

        public string TargetDirectory
        {
            get
            {
                return this.targetDirectory;
            }

            set
            {
                if (this.State == JobState.Aborted || this.State == JobState.Finished || this.State == JobState.New)
                {
                    this.targetDirectory = value;
                }
                else
                {
                    throw new NotSupportedException("Property SourceDirectory cannot be changed while a job is running or paused.");
                }
            }
        }

        public string CompareDirectory { get; private set; }

        public string RealTargetDirectory { get; private set; }

        public string Logfile { get; set; }

        public int FilesDeleted { get; set; }

        public int DirectoriesDeleted { get; set; }

        public int FilesCopied { get; set; }

        public int DirectoriesCopied { get; set; }

        public void Start()
        {
            if (this.State == JobState.Paused || this.State == JobState.Running)
            {
                throw new NotSupportedException("You cannot start a job, which is already running.");
            }

            if (!System.IO.Directory.Exists(this.SourceDirectory))
            {
                throw new DirectoryNotFoundException("The source directory '" + this.SourceDirectory + "' does not exist!");
            }

            if (!System.IO.Directory.Exists(this.TargetDirectory))
            {
                throw new DirectoryNotFoundException("The target directory '" + this.TargetDirectory + "' does not exist!");
            }

            switch (Type)
            {
                case JobType.Protocol:
                    this.CompareDirectory = string.Empty;
                    this.RealTargetDirectory = this.TargetDirectory;
                    break;

                case JobType.Snapshot:
                    DirectoryInfo di = new DirectoryInfo(this.TargetDirectory);
                    DirectoryInfo[] dirs = di.GetDirectories("*", SearchOption.TopDirectoryOnly);
                    this.RealTargetDirectory = this.TargetDirectory.TrimEnd(new char[] { '\\' }) + "\\" + string.Format("{yyyy-MM-dd HH-mm-ss}", DateTime.Now);
                    if (dirs.Length == 0)
                    {
                        this.CompareDirectory = string.Empty;
                    }
                    else
                    {
                        Array.Sort<DirectoryInfo>(dirs, new Comparison<DirectoryInfo>((a, b) => a.CreationTime.CompareTo(b.CreationTime)));
                        this.CompareDirectory = dirs.Last().FullName;
                    }

                    break;

                case JobType.Sync:
                    this.CompareDirectory = string.Empty;
                    this.RealTargetDirectory = this.TargetDirectory;
                    break;
            }

            this.thread.Start();
            this.State = JobState.Running;
        }

        public void Abort()
        {
            if (this.State == JobState.Aborted || this.State == JobState.Finished || this.State == JobState.New)
            {
                throw new NotSupportedException("You cannot abort a job, which is not running.");
            }

            abort = true;
        }

        public void SuspendOrResume()
        {
            if (this.State == JobState.Aborted || this.State == JobState.Finished || this.State == JobState.New)
            {
                throw new NotSupportedException("You cannot suspend or resume a job, which is not running.");
            }

            this.paused = !this.paused;
            this.State = this.paused ? JobState.Paused : JobState.Running;
        }

        public void Worker()
        {
            var logFile = new System.IO.FileStream(this.Logfile, System.IO.File.Exists(this.Logfile) ? System.IO.FileMode.Append : System.IO.FileMode.OpenOrCreate);
            var log = new StreamWriter(logFile);

            this.SourceDirectory = this.SourceDirectory.TrimEnd(new char[] { '\\' }) + "\\";
            this.RealTargetDirectory = this.RealTargetDirectory.TrimEnd(new char[] { '\\' }) + "\\";
            this.CompareDirectory = this.CompareDirectory.TrimEnd(new char[] { '\\' }) + "\\";

            var todo = new Stack<string>();
            todo.Push(this.SourceDirectory);

            string currentDirectory, absolutePath;

            while (todo.Count > 0)
            {
                if (this.abort)
                {
                    this.State = JobState.Aborted;
                    goto finish;
                }

                while (this.paused)
                {
                    System.Threading.Thread.Sleep(50);
                }


            }

            this.State = JobState.Finished;

        finish:

            log.WriteLine("Backup completed successfully.");
            log.WriteLine(this.DirectoriesCopied.ToString(CultureInfo.InvariantCulture) + " directories created, " + this.DirectoriesDeleted.ToString(CultureInfo.InvariantCulture) + " directories deleted.");
            log.WriteLine(this.FilesCopied.ToString(CultureInfo.InvariantCulture) + " files copied, " + this.FilesDeleted.ToString(CultureInfo.InvariantCulture) + " files deleted.");

            log.Flush();
            logFile.Flush();
        }
    }

    public enum JobType
    {
        Protocol,
        Sync,
        Snapshot
    }

    public enum JobState
    {
        New,
        Running,
        Paused,
        Aborted,
        Finished
    }
}
