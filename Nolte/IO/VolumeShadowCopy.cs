using Alphaleonis.Win32.Vss;

namespace Nolte.IO
{
    using System;
    using System.Diagnostics;

    public class VolumeShadowCopy : IDisposable
    {
        public VolumeShadowCopy(string VolumeName)
        {
            IVssImplementation vss = VssUtils.LoadImplementation();
            BackupComponents = vss.CreateVssBackupComponents();
            BackupComponents.InitializeForBackup(null);
            BackupComponents.GatherWriterMetadata();
            BackupComponents.FreeWriterMetadata();
            SetId = BackupComponents.StartSnapshotSet();
            AddVolume(Path.GetPathRoot(VolumeName));
            BackupComponents.SetBackupState(false, true, VssBackupType.Full, false);
            BackupComponents.PrepareForBackup();
        }

        public void Dispose()
        {
            try
            {
                BackupComponents.BackupComplete();
            }
            catch { }

            try
            {
                BackupComponents.DeleteSnapshotSet(SetId, false); 
            }
            catch { }

            try
            {
                BackupComponents.Dispose();
            }
            catch { }
        }

        public void DoSnapshot()
        {
            BackupComponents.DoSnapshotSet();
        }

        public string GetSnapshotPath(string localPath)
        {
            Trace.WriteLine("New volume: " + Root);

            // This bit replaces the file's normal root information with root
            // info from our new shadow copy.
            if (Path.IsPathRooted(localPath))
            {
                string root = Path.GetPathRoot(localPath);
                localPath = localPath.Replace(root, String.Empty);
            }
            string slash = Path.DirectorySeparatorChar.ToString();
            if (!Root.EndsWith(slash) && !localPath.StartsWith(slash))
                localPath = localPath.Insert(0, slash);
            localPath = localPath.Insert(0, Root);

            Trace.WriteLine("Converted path: " + localPath);

            return localPath;
        }

        /// <summary>
        /// Gets the string that identifies the root of this snapshot.
        /// </summary>
        public string Root
        {
            get
            {
                if (SnapshotProperties == null)
                    SnapshotProperties = BackupComponents.GetSnapshotProperties(SnapId);
                return SnapshotProperties.SnapshotDeviceObject;
            }
        }

        public System.IO.Stream GetStream(string localPath)
        {
            // GetSnapshotPath() returns a very funky-looking path.  The
            // System.IO methods can't handle these sorts of paths, so instead
            // we're using AlphaFS, another excellent library by Alpha Leonis.
            // Note that we have no 'using System.IO' at the top of the file.
            // (The Stream it returns, however, is just a System.IO stream.)
            return File.OpenRead(GetSnapshotPath(localPath));
        }


        protected IVssBackupComponents BackupComponents { get; private set; }

        protected VssSnapshotProperties SnapshotProperties { get; private set; }

        protected Guid SetId { get; private set; }

        protected Guid SnapId { get; private set; }

        /// <summary>
        /// Adds a volume to the current snapshot.
        /// </summary>
        /// <param name="VolumeName">Name of the volume to add (eg. "C:\").</param>
        /// <remarks>
        /// Note the IsVolumeSupported check prior to adding each volume.
        /// </remarks>
        protected void AddVolume(string VolumeName)
        {
            if (BackupComponents.IsVolumeSupported(VolumeName))
            {
                SnapId = BackupComponents.AddToSnapshotSet(VolumeName);
            }
            else
            {
                throw new VssVolumeNotSupportedException(VolumeName);
            }
        }
    }
}
