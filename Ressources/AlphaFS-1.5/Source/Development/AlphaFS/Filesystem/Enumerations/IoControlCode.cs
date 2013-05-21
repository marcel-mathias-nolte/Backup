/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */
using System.IO;

namespace Alphaleonis.Win32.Filesystem
{
   #region IoControl Types

   #region IoControlMethod

   internal enum IoControlMethod
   {
      Buffered = 0,
      //InDirect = 1,
      //OutDirect = 2,
      //Neither = 3
   }
   
   #endregion // IoControlMethod

   #region IoControlFileDevice

   internal enum IoControlFileDevice : uint
   {
      //Beep = 1,
      //CDRom = 2,
      //CDRomFileSytem = 3,
      //Controller = 4,
      //Datalink = 5,
      //Dfs = 6,
      //Disk = 7,
      //DiskFileSystem = 8,
      FileSystem = 9,
      //InPortPort = 10,
      //Keyboard = 11,
      //Mailslot = 12,
      //MidiIn = 13,
      //MidiOut = 14,
      //Mouse = 15,
      //MultiUncProvider = 16,
      //NamedPipe = 17,
      //Network = 18,
      //NetworkBrowser = 19,
      //NetworkFileSystem = 20,
      //Null = 21,
      //ParellelPort = 22,
      //PhysicalNetcard = 23,
      //Printer = 24,
      //Scanner = 25,
      //SerialMousePort = 26,
      //SerialPort = 27,
      //Screen = 28,
      //Sound = 29,
      //Streams = 30,
      //Tape = 31,
      //TapeFileSystem = 32,
      //Transport = 33,
      //Unknown = 34,
      //Video = 35,
      //VirtualDisk = 36,
      //WaveIn = 37,
      //WaveOut = 38,
      //Port8042 = 39,
      //NetworkRedirector = 40,
      //Battery = 41,
      //BusExtender = 42,
      //Modem = 43,
      //Vdm = 44,
      //MassStorage = 45,
      //Smb = 46,
      //Ks = 47,
      //Changer = 48,
      //Smartcard = 49,
      //Acpi = 50,
      //Dvd = 51,
      //FullscreenVideo = 52,
      //DfsFileSystem = 53,
      //DfsVolume = 54,
      //Serenum = 55,
      //Termsrv = 56,
      //Ksec = 57
   }

   #endregion // IoControlFileDevice

   #region IoControlCode

   //[Flags]
   internal enum IoControlCode : uint
   {
      //// STORAGE
      //StorageBase = IoControlFileDevice.MassStorage,
      //StorageCheckVerify = (StorageBase << 16) | (0x0200 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageCheckVerify2 = (StorageBase << 16) | (0x0200 << 2) | IoControlMethod.Buffered | (0 << 14), // FileAccess.Any
      //StorageMediaRemoval = (StorageBase << 16) | (0x0201 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageEjectMedia = (StorageBase << 16) | (0x0202 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageLoadMedia = (StorageBase << 16) | (0x0203 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageLoadMedia2 = (StorageBase << 16) | (0x0203 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageReserve = (StorageBase << 16) | (0x0204 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageRelease = (StorageBase << 16) | (0x0205 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageFindNewDevices = (StorageBase << 16) | (0x0206 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageEjectionControl = (StorageBase << 16) | (0x0250 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageMcnControl = (StorageBase << 16) | (0x0251 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageGetMediaTypes = (StorageBase << 16) | (0x0300 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageGetMediaTypesEx = (StorageBase << 16) | (0x0301 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageResetBus = (StorageBase << 16) | (0x0400 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageResetDevice = (StorageBase << 16) | (0x0401 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //StorageGetDeviceNumber = (StorageBase << 16) | (0x0420 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StoragePredictFailure = (StorageBase << 16) | (0x0440 << 2) | IoControlMethod.Buffered | (0 << 14),
      //StorageObsoleteResetBus = (StorageBase << 16) | (0x0400 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //StorageObsoleteResetDevice = (StorageBase << 16) | (0x0401 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      
      //// DISK
      //DiskBase = IoControlFileDevice.Disk,
      //DiskGetDriveGeometry = (DiskBase << 16) | (0x0000 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskGetPartitionInfo = (DiskBase << 16) | (0x0001 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetPartitionInfo = (DiskBase << 16) | (0x0002 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGetDriveLayout = (DiskBase << 16) | (0x0003 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetDriveLayout = (DiskBase << 16) | (0x0004 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskVerify = (DiskBase << 16) | (0x0005 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskFormatTracks = (DiskBase << 16) | (0x0006 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskReassignBlocks = (DiskBase << 16) | (0x0007 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskPerformance = (DiskBase << 16) | (0x0008 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskIsWritable = (DiskBase << 16) | (0x0009 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskLogging = (DiskBase << 16) | (0x000a << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskFormatTracksEx = (DiskBase << 16) | (0x000b << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskHistogramStructure = (DiskBase << 16) | (0x000c << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskHistogramData = (DiskBase << 16) | (0x000d << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskHistogramReset = (DiskBase << 16) | (0x000e << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskRequestStructure = (DiskBase << 16) | (0x000f << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskRequestData = (DiskBase << 16) | (0x0010 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskControllerNumber = (DiskBase << 16) | (0x0011 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskSmartGetVersion = (DiskBase << 16) | (0x0020 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSmartSendDriveCommand = (DiskBase << 16) | (0x0021 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskSmartRcvDriveData = (DiskBase << 16) | (0x0022 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskUpdateDriveSize = (DiskBase << 16) | (0x0032 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGrowPartition = (DiskBase << 16) | (0x0034 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskGetCacheInformation = (DiskBase << 16) | (0x0035 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskSetCacheInformation = (DiskBase << 16) | (0x0036 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskDeleteDriveLayout = (DiskBase << 16) | (0x0040 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskFormatDrive = (DiskBase << 16) | (0x00f3 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //DiskSenseDevice = (DiskBase << 16) | (0x00f8 << 2) | IoControlMethod.Buffered | (0 << 14),
      //DiskCheckVerify = (DiskBase << 16) | (0x0200 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskMediaRemoval = (DiskBase << 16) | (0x0201 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskEjectMedia = (DiskBase << 16) | (0x0202 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskLoadMedia = (DiskBase << 16) | (0x0203 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskReserve = (DiskBase << 16) | (0x0204 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskRelease = (DiskBase << 16) | (0x0205 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskFindNewDevices = (DiskBase << 16) | (0x0206 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //DiskGetMediaTypes = (DiskBase << 16) | (0x0300 << 2) | IoControlMethod.Buffered | (0 << 14),
      
      //// CHANGER
      //ChangerBase = IoControlFileDevice.Changer,
      //ChangerGetParameters = (ChangerBase << 16) | (0x0000 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerGetStatus = (ChangerBase << 16) | (0x0001 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerGetProductData = (ChangerBase << 16) | (0x0002 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerSetAccess = (ChangerBase << 16) | (0x0004 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //ChangerGetElementStatus = (ChangerBase << 16) | (0x0005 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //ChangerInitializeElementStatus = (ChangerBase << 16) | (0x0006 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerSetPosition = (ChangerBase << 16) | (0x0007 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerExchangeMedium = (ChangerBase << 16) | (0x0008 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerMoveMedium = (ChangerBase << 16) | (0x0009 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerReinitializeTarget = (ChangerBase << 16) | (0x000A << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      //ChangerQueryVolumeTags = (ChangerBase << 16) | (0x000B << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      
      //// FILESYSTEM
      //FsctlRequestOplockLevel1 = (IoControlFileDevice.FileSystem << 16) | (0 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlRequestOplockLevel2 = (IoControlFileDevice.FileSystem << 16) | (1 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlRequestBatchOplock = (IoControlFileDevice.FileSystem << 16) | (2 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlOplockBreakAcknowledge = (IoControlFileDevice.FileSystem << 16) | (3 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlOpBatchAckClosePending = (IoControlFileDevice.FileSystem << 16) | (4 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlOplockBreakNotify = (IoControlFileDevice.FileSystem << 16) | (5 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlLockVolume = (IoControlFileDevice.FileSystem << 16) | (6 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlUnlockVolume = (IoControlFileDevice.FileSystem << 16) | (7 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlDismountVolume = (IoControlFileDevice.FileSystem << 16) | (8 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlIsVolumeMounted = (IoControlFileDevice.FileSystem << 16) | (10 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlIsPathnameValid = (IoControlFileDevice.FileSystem << 16) | (11 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlMarkVolumeDirty = (IoControlFileDevice.FileSystem << 16) | (12 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlQueryRetrievalPointers = (IoControlFileDevice.FileSystem << 16) | (14 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlGetCompression = (IoControlFileDevice.FileSystem << 16) | (15 << 2) | IoControlMethod.Buffered | (0 << 14),
      FsctlSetCompression = (IoControlFileDevice.FileSystem << 16) | (16 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlMarkAsSystemHive = (IoControlFileDevice.FileSystem << 16) | (19 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlOplockBreakAckNo2 = (IoControlFileDevice.FileSystem << 16) | (20 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlInvalidateVolumes = (IoControlFileDevice.FileSystem << 16) | (21 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlQueryFatBpb = (IoControlFileDevice.FileSystem << 16) | (22 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlRequestFilterOplock = (IoControlFileDevice.FileSystem << 16) | (23 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlFileSystemGetStatistics = (IoControlFileDevice.FileSystem << 16) | (24 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlGetNtfsVolumeData = (IoControlFileDevice.FileSystem << 16) | (25 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlGetNtfsFileRecord = (IoControlFileDevice.FileSystem << 16) | (26 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlGetVolumeBitmap = (IoControlFileDevice.FileSystem << 16) | (27 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlGetRetrievalPointers = (IoControlFileDevice.FileSystem << 16) | (28 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlMoveFile = (IoControlFileDevice.FileSystem << 16) | (29 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlIsVolumeDirty = (IoControlFileDevice.FileSystem << 16) | (30 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlGetHfsInformation = (IoControlFileDevice.FileSystem << 16) | (31 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlAllowExtendedDasdIo = (IoControlFileDevice.FileSystem << 16) | (32 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlReadPropertyData = (IoControlFileDevice.FileSystem << 16) | (33 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlWritePropertyData = (IoControlFileDevice.FileSystem << 16) | (34 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlFindFilesBySid = (IoControlFileDevice.FileSystem << 16) | (35 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlDumpPropertyData = (IoControlFileDevice.FileSystem << 16) | (37 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlSetObjectId = (IoControlFileDevice.FileSystem << 16) | (38 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlGetObjectId = (IoControlFileDevice.FileSystem << 16) | (39 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteObjectId = (IoControlFileDevice.FileSystem << 16) | (40 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlSetReparsePoint = (IoControlFileDevice.FileSystem << 16) | (41 << 2) | IoControlMethod.Buffered | (0 << 14),
      FsctlGetReparsePoint = (IoControlFileDevice.FileSystem << 16) | (42 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteReparsePoint = (IoControlFileDevice.FileSystem << 16) | (43 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlEnumUsnData = (IoControlFileDevice.FileSystem << 16) | (44 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlSecurityIdCheck = (IoControlFileDevice.FileSystem << 16) | (45 << 2) | IoControlMethod.Neither | (FileAccess.Read << 14),
      //FsctlReadUsnJournal = (IoControlFileDevice.FileSystem << 16) | (46 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlSetObjectIdExtended = (IoControlFileDevice.FileSystem << 16) | (47 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlCreateOrGetObjectId = (IoControlFileDevice.FileSystem << 16) | (48 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlSetSparse = (IoControlFileDevice.FileSystem << 16) | (49 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlSetZeroData = (IoControlFileDevice.FileSystem << 16) | (50 << 2) | IoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlQueryAllocatedRanges = (IoControlFileDevice.FileSystem << 16) | (51 << 2) | IoControlMethod.Neither | (FileAccess.Read << 14),
      //FsctlEnableUpgrade = (IoControlFileDevice.FileSystem << 16) | (52 << 2) | IoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlSetEncryption = (IoControlFileDevice.FileSystem << 16) | (53 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlEncryptionFsctlIo = (IoControlFileDevice.FileSystem << 16) | (54 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlWriteRawEncrypted = (IoControlFileDevice.FileSystem << 16) | (55 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlReadRawEncrypted = (IoControlFileDevice.FileSystem << 16) | (56 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlCreateUsnJournal = (IoControlFileDevice.FileSystem << 16) | (57 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlReadFileUsnData = (IoControlFileDevice.FileSystem << 16) | (58 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlWriteUsnCloseRecord = (IoControlFileDevice.FileSystem << 16) | (59 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlExtendVolume = (IoControlFileDevice.FileSystem << 16) | (60 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlQueryUsnJournal = (IoControlFileDevice.FileSystem << 16) | (61 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlDeleteUsnJournal = (IoControlFileDevice.FileSystem << 16) | (62 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlMarkHandle = (IoControlFileDevice.FileSystem << 16) | (63 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlSisCopyFile = (IoControlFileDevice.FileSystem << 16) | (64 << 2) | IoControlMethod.Buffered | (0 << 14),
      //FsctlSisLinkFiles = (IoControlFileDevice.FileSystem << 16) | (65 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlHsmMsg = (IoControlFileDevice.FileSystem << 16) | (66 << 2) | IoControlMethod.Buffered | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlNssControl = (IoControlFileDevice.FileSystem << 16) | (67 << 2) | IoControlMethod.Buffered | (FileAccess.Write << 14),
      //FsctlHsmData = (IoControlFileDevice.FileSystem << 16) | (68 << 2) | IoControlMethod.Neither | ((FileAccess.Read | FileAccess.Write) << 14),
      //FsctlRecallFile = (IoControlFileDevice.FileSystem << 16) | (69 << 2) | IoControlMethod.Neither | (0 << 14),
      //FsctlNssRcontrol = (IoControlFileDevice.FileSystem << 16) | (70 << 2) | IoControlMethod.Buffered | (FileAccess.Read << 14),
      
      //// VIDEO
      //VideoQuerySupportedBrightness = (IoControlFileDevice.Video << 16) | (0x0125 << 2) | IoControlMethod.Buffered | (0 << 14),
      //VideoQueryDisplayBrightness = (IoControlFileDevice.Video << 16) | (0x0126 << 2) | IoControlMethod.Buffered | (0 << 14),
      //VideoSetDisplayBrightness = (IoControlFileDevice.Video << 16) | (0x0127 << 2) | IoControlMethod.Buffered | (0 << 14)
   }

   #endregion // IoControlCode

   #endregion // IoControl Types
}