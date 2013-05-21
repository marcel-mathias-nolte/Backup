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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Alphaleonis.Win32.Net
{
   /// <summary>Provides static methods for the retrieving information from a computer local or remote host.</summary>
   public static class Host
   {
      #region AlphaFS

      #region EnumerateDisks

      /// <summary>Enumerate disks from the local host.</summary>
      /// <returns>Returns <see cref="IEnumerable{String}"/> disks from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDisks()
      {
         return EnumerateDiskResourcesInternal(null, false, 0).Cast<string>();
      }

      /// <summary>Enumerate disks from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <returns>Returns <see cref="IEnumerable{String}"/> disks from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<string> EnumerateDisks(string host)
      {
         return EnumerateDiskResourcesInternal(host, false, 0).Cast<string>();
      }

      #endregion // EnumerateDisks

      #region EnumerateShares

      /// <summary>Enumerate Server Message Block (SMB) shares from the local host.</summary>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares()
      {
         return EnumerateDiskResourcesInternal(null, true, 503).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the local host.</summary>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the local host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(int shareInfoLevel)
      {
         return EnumerateDiskResourcesInternal(null, true, shareInfoLevel).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(string host)
      {
         return EnumerateDiskResourcesInternal(host, true, 503).Cast<ShareInfo>();
      }

      /// <summary>Enumerate Server Message Block (SMB) shares from the specified host.</summary>
      /// <param name="host">A <see cref="String"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns <see cref="IEnumerable{ShareInfo}"/> shares from the specified host.</returns>
      [SecurityCritical]
      public static IEnumerable<ShareInfo> EnumerateShares(string host, int shareInfoLevel)
      {
         return EnumerateDiskResourcesInternal(host, true, shareInfoLevel).Cast<ShareInfo>();
      }

      #endregion // EnumerateShares

      #region GetHostShareFromPath

      /// <summary>Gets the host and Server Message Block (SMB) share name for a given unc path.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <returns>string[0] = host, string[1] = share;</returns>
      [SecurityCritical]
      public static string[] GetHostShareFromPath(string uncPath)
      {
         if (string.IsNullOrEmpty(uncPath))
            return null;

         // Get Host and Share.
         uncPath = uncPath.Replace(Path.LongPathUncPrefix, string.Empty);
         uncPath = uncPath.Replace(Path.UncPrefix, string.Empty);
         return uncPath.Split(Path.DirectorySeparatorChar);
      }

      #endregion // GetHostShareFromPath

      #region GetShareLocalPath

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the local host.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <returns>Returns the filesystem path for <paramref name="uncPath"/> or <see langref="null"/> on failure or when not available.</returns>
      /// <remarks>GetShareLocalPath() only works correctly for shares defined on the local host.</remarks>
      [SecurityCritical]
      public static string GetShareLocalPath(string uncPath)
      {
         return GetShareLocalPath(uncPath, 2);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the local host.</summary>
      /// <param name="uncPath">The share in the format: \\host\share</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns the filesystem path for <paramref name="uncPath"/> or <see langref="null"/> on failure or when not available.</returns>
      /// <remarks>GetShareLocalPath() only works correctly for shares defined on the local host.</remarks>
      [SecurityCritical]
      public static string GetShareLocalPath(string uncPath, int shareInfoLevel)
      {
         string[] hostShare = GetHostShareFromPath(uncPath);
         return hostShare == null ? null : GetShareLocalPath(hostShare[0], hostShare[1], shareInfoLevel);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <returns>Returns the filesystem path for <paramref name="host"/>\<paramref name="share"/> or <see langref="null"/> on failure or when not available.</returns>
      [SecurityCritical]
      public static string GetShareLocalPath(string host, string share)
      {
         return GetShareLocalPath(host, share, 2);
      }

      /// <summary>Gets the filesystem path for the Server Message Block (SMB) share as defined on the host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>Returns the filesystem path for <paramref name="host"/>\<paramref name="share"/> or <see langref="null"/> on failure or when not available.</returns>
      [SecurityCritical]
      public static string GetShareLocalPath(string host, string share, int shareInfoLevel)
      {
         object shareInfo = GetShareInfoInternal(host, share, shareInfoLevel);
         return shareInfo == null
                   ? null
                   : (shareInfoLevel == 2
                         ? ((ShareInfo.ShareInfo2) shareInfo).Path
                         : ((ShareInfo.ShareInfo503) shareInfo).Path);
      }

      #endregion // GetShareLocalPath


      #region Unified Internals

      #region EnumerateDiskResourcesInternal

      /// <summary>Enumerate Server Message Block (SMB) shares or disks from the specified host.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="getShares"><c>true</c> enumerates shares, <c>false</c> enumerates disks.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>If <paramref name="getShares"/> is <c>true</c>, returns <see cref="IEnumerable{ShareInfo}"/> shares, when <c>false</c> returns <see cref="IEnumerable{String}"/> disks.</returns>
      /// <remarks>If <see cref="ShareInfo.ShareInfo503"/> fails, fallback with <see cref="ShareInfo.ShareInfo2"/> is executed. If this also fails an exception is raised.</remarks>
      [SecurityCritical]
      internal static IEnumerable<object> EnumerateDiskResourcesInternal(string host, bool getShares, int shareInfoLevel)
      {
         if (host != null && host.Trim() == string.Empty)
            host = null;

         uint infoLevel = (uint) shareInfoLevel;
         Type objectType = getShares ? infoLevel == 2 ? typeof (ShareInfo.ShareInfo2) : typeof (ShareInfo.ShareInfo503) : typeof (IntPtr);
         SafeNetApiBuffer netApiBuffer = null;
         uint lastError;
         uint resume = 0;
         bool raiseException = false;
         bool isDone = false;

         do
         {
            uint entriesRead;
            uint totalEntries;

            // +2 is necessary for disks.
            int objectSize = Marshal.SizeOf(objectType) + (getShares ? 0 : 2);

            // Note that you must free the buffer even if the function fails with ERROR_MORE_DATA.
            if (netApiBuffer != null)
               netApiBuffer.Dispose();

            // MAX_PREFERRED_LENGTH = -1: If you specify MAX_PREFERRED_LENGTH, the function allocates the amount of memory required for the data.
            // Note: This parameter is currently ignored for function: NetServerDiskEnum().

            lastError = getShares
                           ? NativeMethods.NetShareEnum(host, infoLevel, out netApiBuffer, -1, out entriesRead, out totalEntries, ref resume)
                           : NativeMethods.NetServerDiskEnum(host, infoLevel, out netApiBuffer, -1, out entriesRead, out totalEntries, ref resume);

            switch (lastError)
            {
                  // Retrieved shares/disks.
               case Win32Errors.NO_ERROR:
                  if (entriesRead > 0)
                  {
                     for (long i = 0, itemOffset = netApiBuffer.DangerousGetHandle().ToInt64(); i < entriesRead; i++, itemOffset += objectSize)
                        yield return getShares
                                        ? (object)
                                          new ShareInfo(host, infoLevel, Marshal.PtrToStructure(new IntPtr(itemOffset), objectType))
                                        : Marshal.PtrToStringUni(new IntPtr(itemOffset));
                  }
                  isDone = true;
                  continue;

                  // Non-existent host.
               case Win32Errors.ERROR_BAD_NETPATH:
                  lastError = Win32Errors.ERROR_BAD_NET_NAME; // Throw more appropriate error.
                  raiseException = true;
                  isDone = true;
                  continue;

                  // All other errors.
               default:
                  if (getShares)
                  {
                     // Fallback to ShareInfo2 structure.
                     switch (infoLevel)
                     {
                        case 503:
                           infoLevel = 2;
                           objectType = typeof (ShareInfo.ShareInfo2);
                           break;

                        default:
                           raiseException = true;
                           isDone = true;
                           continue;
                     }
                  }
                  else
                  {
                     raiseException = true;
                     isDone = true;
                  }
                  break;
            }
         } while (!isDone);

         if (raiseException)
            NativeError.ThrowException(lastError, host);
      }

      #endregion // EnumerateDiskResourcesInternal

      #region GetRemoteNameInfoInternal

      /// <summary>This method uses <see cref="RemoteNameInfo"/> level to retieve full REMOTE_NAME_INFO structure.</summary>
      /// <param name="path">The local path with drive name.</param>
      /// <returns>A <see cref="RemoteNameInfo"/> structure.</returns>
      /// <exception cref="System.IO.PathTooLongException">When <paramref name="path"/> exceeds <see cref="Filesystem.NativeMethods.MaxPath"/></exception>
      /// <exception cref="NativeError.ThrowException()"/>
      /// <remarks>AlphaFS regards network drives created using SUBST.EXE as invalid: http://alphafs.codeplex.com/discussions/316583</remarks>
      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
      [SecurityCritical]
      internal static RemoteNameInfo GetRemoteNameInfoInternal(string path)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // If path already is a network share path, we fill the RemoteNameInfo structure ourselves.
         if (Path.IsUnc(path))
            return new RemoteNameInfo
            {
               UniversalName = Path.DirectorySeparatorAdd(path, false),
               ConnectionName = Path.DirectorySeparatorRemove(path, false),
               RemainingPath = Path.DirectorySeparatorChar.ToString(CultureInfo.CurrentCulture)
            };

         path = Path.GetRegularPath(path);

         if (path.Length > Filesystem.NativeMethods.MaxPath)
            throw new PathTooLongException();


         // Start with a large buffer to prevent multiple calls.
         uint bufferSize = Filesystem.NativeMethods.MaxPathUnicode;
         SafeGlobalMemoryBufferHandle safeBuffer = new SafeGlobalMemoryBufferHandle((int)bufferSize);

         try
         {
            do
            {
               // Structure: UNIVERSAL_NAME_INFO_LEVEL = 1 (not used in AlphaFS).
               // Structure: REMOTE_NAME_INFO_LEVEL    = 2
               uint lastError = NativeMethods.WNetGetUniversalName(path, 2, safeBuffer, out bufferSize);

               switch (lastError)
               {
                  case Win32Errors.NO_ERROR:
                     return (RemoteNameInfo)Marshal.PtrToStructure(safeBuffer.DangerousGetHandle(), typeof(RemoteNameInfo));

                  case Win32Errors.ERROR_MORE_DATA:
                     safeBuffer.Dispose();
                     safeBuffer = new SafeGlobalMemoryBufferHandle((int)bufferSize);
                     break;

                  // The device specified by the lpLocalPath parameter is not redirected.
                  case Win32Errors.ERROR_NOT_CONNECTED:

                  // -None of the network providers recognize the local name as having a connection.
                  //  However, the network is not available for at least one provider to whom the connection may belong.
                  // -A Softgrid "Q:" drive is encountered;
                  case Win32Errors.ERROR_NO_NET_OR_BAD_PATH:

                     // Return an empty structure (all fields set to null).
                     return new RemoteNameInfo();

                  default:
                     NativeError.ThrowException(lastError, path);
                     break;
               }
            } while (true);
         }
         finally
         {
            safeBuffer.Dispose();
         }
      }

      #endregion // GetRemoteNameInfoInternal

      #region GetShareInfoInternal

      /// <summary>Gets the ShareInfo structure of a Server Message Block (SMB) share.</summary>
      /// <param name="host">A <see cref="string"/> that specifies the DNS or NetBIOS name of the remote server.</param>
      /// <param name="share">A <see cref="string"/> that specifies the name of the Server Message Block (SMB) share.</param>
      /// <param name="shareInfoLevel">One of the <see cref="ShareInfo"/> structure numbers: 2 or 503</param>
      /// <returns>A <see cref="ShareInfo"/> structure, or <see langref="null"/> on failure or when not available.</returns>
      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
      [SecurityCritical]
      internal static object GetShareInfoInternal(string host, string share, int shareInfoLevel)
      {
         if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(share))
            return null;

         SafeNetApiBuffer netApiBuffer;

         if (NativeMethods.NetShareGetInfo(host, share, (uint)shareInfoLevel, out netApiBuffer) != Win32Errors.NO_ERROR)
            return null;

         Type objectType = null;

         switch (shareInfoLevel)
         {
            case 1005:
               objectType = typeof (ShareInfo.ShareInfo1005);
               break;

            case 503:
               objectType = typeof (ShareInfo.ShareInfo503);
               break;

            case 2:
               objectType = typeof (ShareInfo.ShareInfo2);
               break;
         }

         return objectType == null ? null : Marshal.PtrToStructure(netApiBuffer.DangerousGetHandle(), objectType);
      }

      #endregion // GetShareInfoInternal

      #endregion // Unified Internals

      #endregion // AlphaFS
   }
}