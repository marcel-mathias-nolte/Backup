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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Alphaleonis.Win32.Net
{
   internal static class NativeMethods
   {
      #region Structures

      #region NetResource

      /// <summary>The NETRESOURCE structure contains information about a network resource.</summary>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      internal struct NetResource
      {
         /// <summary>The scope of the enumeration. This member can be one of the following <see cref="ResourceScope"/> values.</summary>
         [MarshalAs(UnmanagedType.U4)] public ResourceScope Scope;

         /// <summary>The type of resource. This member can be one of the following <see cref="ResourceTypes"/> values.</summary>
         [MarshalAs(UnmanagedType.U4)] public ResourceTypes Type;

         /// <summary>The display options for the network object in a network browsing user interface. This member can be one of the following <see cref="ResourceDisplayType"/> values.</summary>
         [MarshalAs(UnmanagedType.U4)] public ResourceDisplayType DisplayType;

         /// <summary>A set of bit flags describing how the resource can be used. This member can be one of the following <see cref="ResourceUsage"/> values.</summary>
         [MarshalAs(UnmanagedType.U4)] public ResourceUsage Usage;

         /// <summary>If the <see cref="Scope"/> member is equal to <see cref="ResourceScope.Connected"/> or <see cref="ResourceScope.Remembered"/>.
         /// This member is a pointer to a null-terminated character string that specifies the name of a local device. 
         /// This member is NULL if the connection does not use a device.
         /// </summary>
         [MarshalAs(UnmanagedType.LPWStr)] public string LocalName;

         /// <summary>If the entry is a network resource, this member is a <see langref="string"/> that specifies the remote network name.
         /// If the entry is a current or persistent connection, <see cref="RemoteName"/> member points to the network name associated with the name pointed to by the <see cref="LocalName"/> member.
         /// The string can be <see cref="Filesystem.NativeMethods.MaxPath"/> characters in length, and it must follow the network provider's naming conventions.
         /// </summary>
         [MarshalAs(UnmanagedType.LPWStr)] public string RemoteName;

         /// <summary>A <see langref="string"/> that contains a comment supplied by the network provider.</summary>
         [MarshalAs(UnmanagedType.LPWStr)] public string Comment;

         /// <summary>A <see langref="string"/> that contains the name of the provider that owns the resource. 
         /// This member can be <see langref="null"/> if the provider name is unknown. To retrieve the provider name, you can call the WNetGetProviderName function.
         /// </summary>
         [MarshalAs(UnmanagedType.LPWStr)] public string Provider;
      }

      #endregion // NetResource

      #endregion // Structures

      #region mpr.dll

      #region WNetCloseEnum

      /// <summary>The WNetCloseEnum function ends a network resource enumeration started by a call to the WNetOpenEnum function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is <see cref="Win32Errors.NO_ERROR"/>
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "WNetCloseEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint WNetCloseEnum(IntPtr hEnum);

      #endregion // WNetCloseEnum

      #region WNetEnumResource

      /// <summary>The WNetEnumResource function continues an enumeration of network resources that was started by a call to the WNetOpenEnum function.
      /// This method does not enumerate hidden shares or users connected to a share.
      /// </summary>
      /// <returns>
      /// If the function succeeds, the return value is <see cref="Win32Errors.NO_ERROR"/>
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "WNetEnumResourceW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint WNetEnumResource(SafeNetEnumHandle hEnum, [MarshalAs(UnmanagedType.I4)] ref int lpcCount, SafeGlobalMemoryBufferHandle lpBuffer, [MarshalAs(UnmanagedType.I4)] ref int lpBufferSize);

      #endregion // WNetEnumResource

      #region WNetGetUniversalName

      /// <summary>The WNetGetUniversalName function takes a drive-based path for a network resource and returns an information structure that contains a more universal form of the name.</summary>
      /// <returns>
      /// If the function succeeds, the return value is <see cref="Win32Errors.NO_ERROR"/>
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "WNetGetUniversalNameW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint WNetGetUniversalName([MarshalAs(UnmanagedType.LPWStr)] string lpLocalPath, [MarshalAs(UnmanagedType.U4)] uint dwInfoLevel, SafeGlobalMemoryBufferHandle lpBuffer, [MarshalAs(UnmanagedType.U4)] out uint lpBufferSize);

      #endregion // WNetGetUniversalName

      #region WNetOpenEnum

      /// <summary>The WNetOpenEnum function starts an enumeration of network resources or existing connections. You can continue the enumeration by calling the WNetEnumResource function.</summary>
      /// <returns>
      /// If the function succeeds, the return value is <see cref="Win32Errors.NO_ERROR"/>
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("mpr.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "WNetOpenEnumW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint WNetOpenEnum(ResourceScope dwScope, ResourceTypes dwType, ResourceUsage dwUsage, [MarshalAs(UnmanagedType.AsAny)] object lpNetResource, out SafeNetEnumHandle lphEnum);

      #endregion // WNetOpenEnum

      #endregion // mpr.dll

      #region netapi32.dll

      #region NetApiBufferFree

      /// <summary>The NetApiBufferFree function frees the memory that the NetApiBufferAllocate function allocates.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetApiBufferFree")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetApiBufferFree(IntPtr buffer);

      #endregion // NetApiBufferFree

      #region NetServerDiskEnum

      /// <summary>The NetServerDiskEnum function retrieves a list of disk drives on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a <see cref="Win32Errors"/> code.
      /// </returns>
      /// <remarks>Only members of the Administrators or Server Operators local group can successfully execute the NetServerDiskEnum function on a remote computer.</remarks>
      /// <remarks>Minimum supported client: Windows 2000 Professional</remarks>
      /// <remarks>Minimum supported server: Windows 2000 Server</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetServerDiskEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetServerDiskEnum([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.U4)] uint level, out SafeNetApiBuffer bufPtr, [MarshalAs(UnmanagedType.I4)] int prefMaxLen, [MarshalAs(UnmanagedType.U4)] out uint entriesRead, [MarshalAs(UnmanagedType.U4)] out uint totalEntries, [MarshalAs(UnmanagedType.U4)] ref uint resumeHandle);

      #endregion // NetServerDiskEnum

      #region NetShareEnum

      /// <summary>Retrieves information about each (hidden) Server Message Block (SMB) resource/share on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>
      /// For interactive users (users who are logged on locally to the machine), no special group membership is required to execute the NetShareEnum function.
      /// For non-interactive users, Administrator, Power User, Print Operator, or Server Operator group membership is required to successfully execute
      /// the NetShareEnum function at levels 2, 502, and 503. No special group membership is required for level 0 or level 1 calls.
      /// </remarks>
      /// <remarks>
      /// Windows Server 2003 and Windows XP: For all users, Administrator, Power User, Print Operator,
      /// or Server Operator group membership is required to successfully execute the NetShareEnum function at levels 2 and 502.
      /// </remarks>
      /// <remarks>
      /// You can also use the WNetEnumResource function to retrieve resource information.
      /// However, WNetEnumResource does not enumerate hidden shares or users connected to a share.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetShareEnum")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetShareEnum([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.U4)] uint level, out SafeNetApiBuffer bufPtr, [MarshalAs(UnmanagedType.I4)] int prefMaxLen, [MarshalAs(UnmanagedType.U4)] out uint entriesRead, [MarshalAs(UnmanagedType.U4)] out uint totalEntries, [MarshalAs(UnmanagedType.U4)] ref uint resumeHandle);

      #endregion // NetShareEnum

      #region NetShareGetInfo

      /// <summary>Retrieves information about a particular Server Message Block (SMB) shared resource on a server.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NERR_Success.
      /// If the function fails, the return value is a system error code.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "NetShareGetInfo")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint NetShareGetInfo([MarshalAs(UnmanagedType.LPWStr)] string serverName, [MarshalAs(UnmanagedType.LPWStr)] string netName, [MarshalAs(UnmanagedType.U4)] uint level, out SafeNetApiBuffer lpBuffer);

      #endregion // NetShareGetInfo

      #endregion // netapi32.dll
   }
}