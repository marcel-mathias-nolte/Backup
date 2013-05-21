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
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Alphaleonis.Win32.Security
{
   internal static class NativeMethods
   {
      #region Types

      [StructLayout(LayoutKind.Sequential)]
      internal struct TokenPrivilegesHdr
      {
         internal uint PrivilegeCount;
      }

      [StructLayout(LayoutKind.Sequential)]
      internal struct TokenPrivileges
      {
         internal uint PrivilegeCount;
         internal Luid Luid;
         internal uint Attributes;
      }

      [StructLayout(LayoutKind.Sequential)]
      internal struct LuidAndAttributes
      {
         internal Luid Luid;
         internal uint Attributes;
      }

      [StructLayout(LayoutKind.Sequential)]
      internal struct Luid
      {
         internal uint LowPart;
         internal uint HighPart;
      }

      /// <summary>The SecurityInformation (SECURITY_INFORMATION) data type identifies the object-related security information being set or queried.
      /// This security information includes:
      ///   The owner of an object;
      ///   The primary group of an object;
      ///   The discretionary access control list (DACL) of an object;
      ///   The system access control list (SACL) of an object;
      /// </summary>
      [Flags]
      internal enum SecurityInformation : uint
      {
         /// <summary>No SecurityInformation used.</summary>
         None = 0,

         /// <summary>Include the owner.</summary>
         Owner = 1,

         /// <summary>Include the primary group.</summary>
         Group = 2,

         /// <summary>Include the discretionary access control list (DACL).</summary>
         Dacl = 4,

         /// <summary>Include the system access control list (SACL).</summary>
         Sacl = 8,

         /// <summary>Include the mandatory integrity label access control entry (ACE).</summary>
         Label = 16,

         /// <summary>Include the attribute information of the SACL.</summary>
         Attribute = 32,

         /// <summary>Include the central access policy (CAP) identifier of the SACL.</summary>
         Scope = 64,

         /// <summary>The SACL inherits ACEs from the parent object.</summary>
         UnprotectedSacl = 268435456,

         /// <summary>The DACL inherits ACEs from the parent object.</summary>
         UnprotectedDacl = 536870912,

         /// <summary>The SACL cannot inherit ACEs.</summary>
         ProtectedSacl = 1073741824,

         /// <summary>The DACL cannot inherit access control entries (ACEs).</summary>
         ProtectedDacl = 2147483648
      }

      /// <summary>The ResourceType (SE_OBJECT_TYPE) enumeration contains values that correspond to the types of Windows objects that support security.
      /// The functions, such as GetSecurityInfo and SetSecurityInfo, that set and retrieve the security information of an object, use these values to indicate the type of object.
      /// </summary>
      internal enum ResourceType
      {
         /// <summary>Unknown object type.</summary>
         UnknownObjectType = 0,

         /// <summary>Indicates a file or directory. The name string that identifies a file or directory object can be in one of the following formats:
         ///   A relative path, such as FileName.dat or ..\FileName
         ///   An absolute path, such as FileName.dat, C:\DirectoryName\FileName.dat, or G:\RemoteDirectoryName\FileName.dat.
         ///   A UNC name, such as \\ComputerName\ShareName\FileName.dat.
         /// </summary>
         FileObject,

         /// <summary>Indicates a Windows service. A service object can be a local service, such as ServiceName, or a remote service, such as \\ComputerName\ServiceName.</summary>
         Service,

         /// <summary>Indicates a printer. A printer object can be a local printer, such as PrinterName, or a remote printer, such as \\ComputerName\PrinterName.</summary>
         Printer,

         /// <summary>Indicates a registry key. A registry key object can be in the local registry, such as CLASSES_ROOT\SomePath or in a remote registry, such as \\ComputerName\CLASSES_ROOT\SomePath.
         /// The names of registry keys must use the following literal strings to identify the predefined registry keys: "CLASSES_ROOT", "CURRENT_USER", "MACHINE", and "USERS".
         /// </summary>
         RegistryKey,

         /// <summary>Indicates a network share. A share object can be local, such as ShareName, or remote, such as \\ComputerName\ShareName.</summary>
         LmShare,

         /// <summary>Indicates a local kernel object. The GetSecurityInfo and SetSecurityInfo functions support all types of kernel objects.
         /// The GetNamedSecurityInfo and SetNamedSecurityInfo functions work only with the following kernel objects: semaphore, event, mutex, waitable timer, and file mapping.</summary>
         KernelObject,

         /// <summary>Indicates a window station or desktop object on the local computer. You cannot use GetNamedSecurityInfo and SetNamedSecurityInfo with these objects because the names of window stations or desktops are not unique.</summary>
         WindowObject,

         /// <summary>Indicates a directory service object or a property set or property of a directory service object.
         /// The name string for a directory service object must be in X.500 form, for example: CN=SomeObject,OU=ou2,OU=ou1,DC=DomainName,DC=CompanyName,DC=com,O=internet</summary>
         DsObject,

         /// <summary>Indicates a directory service object and all of its property sets and properties.</summary>
         DsObjectAll,

         /// <summary>Indicates a provider-defined object.</summary>
         ProviderDefinedObject,

         /// <summary>Indicates a WMI object.</summary>
         WmiGuidObject,

         /// <summary>Indicates an object for a registry entry under WOW64.</summary>
         RegistryWow6432Key
      }

      [Flags]
      internal enum SecurityDescriptorControl
      {
         OwnerDefaulted = 1,
         GroupDefaulted = 2,
         DaclPresent = 4,
         DaclDefaulted = 8,
         SaclPresent = 16,
         SaclDefaulted = 32,
         DaclAutoInheritReq = 256,
         SaclAutoInheritReq = 512,
         DaclAutoInherited = 1024,
         SaclAutoInherited = 2048,
         DaclProtected = 4096,
         SaclProtected = 8192,
         RmControlValid = 16384,
         SelfRelative = 32768
      }

      #endregion // Types

      #region Constants

      internal const uint SePrivilegeEnabledByDefault = 1;
      internal const uint SePrivilegeEnabled = 2;
      internal const uint SePrivilegeRemoved = 4;
      internal const uint SePrivilegeUsedForAccess = 2147483648;
      internal const string SeSecurityName = "SeSecurityPrivilege";

      #endregion // Constants

      #region DllImport

      #region Privilege

      /// <summary>The AdjustTokenPrivileges function enables or disables privileges in the specified access token. Enabling or disabling privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// To determine whether the function adjusted all of the specified privileges, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref TokenPrivileges newState, uint bufferLength, out TokenPrivileges previousState, out uint returnLength);

      /// <summary>The LookupPrivilegeDisplayName function retrieves the display name that represents a specified privilege.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeDisplayNameW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool LookupPrivilegeDisplayName([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, ref StringBuilder lpDisplayName, ref uint cchDisplayName, out uint lpLanguageId);

      /// <summary>The LookupPrivilegeValue function retrieves the locally unique identifier (LUID) used on a specified system to locally represent the specified privilege name.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool LookupPrivilegeValue([MarshalAs(UnmanagedType.LPWStr)] string lpSystemName, [MarshalAs(UnmanagedType.LPWStr)] string lpName, out Luid lpLuid);

      #endregion // Privilege

      #region Get/Set Security

      /// <summary>The GetFileSecurity function obtains specified information about the security of a file or directory.
      /// The information obtained is constrained by the caller's access rights and privileges.</summary>
      /// <returns>
      /// If the function succeeds, the return value is nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "GetFileSecurityW")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetFileSecurity([MarshalAs(UnmanagedType.LPWStr)] string lpFileName, SecurityInformation securityInfo, SafeHandle pSecurityDescriptor, uint nLength, out uint lpnLengthNeeded);

      /// <summary>The GetSecurityInfo function retrieves a copy of the security descriptor for an object specified by a handle.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetSecurityInfo(SafeHandle handle, ResourceType objectType, SecurityInformation securityInfo, out IntPtr pSidOwner, out IntPtr pSidGroup, out IntPtr pDacl, out IntPtr pSacl, out SafeGlobalMemoryBufferHandle pSecurityDescriptor);

      /// <summary>The SetSecurityInfo function sets specified security information in the security descriptor of a specified object. 
      /// The caller identifies the object by a handle.</summary>
      /// <returns>
      /// If the function succeeds, the function returns ERROR_SUCCESS.
      /// If the function fails, it returns a nonzero error code defined in WinError.h.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint SetSecurityInfo(SafeHandle handle, ResourceType objectType, SecurityInformation securityInfo, IntPtr psidOwner, IntPtr psidGroup, IntPtr pDacl, IntPtr pSacl);
      
      /// <summary>The SetNamedSecurityInfo function sets specified security information in the security descriptor of a specified object. The caller identifies the object by name.</summary>
      /// <returns>
      /// If the function succeeds, the function returns ERROR_SUCCESS.
      /// If the function fails, it returns a nonzero error code defined in WinError.h.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetNamedSecurityInfoW")]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint SetNamedSecurityInfo([MarshalAs(UnmanagedType.LPWStr)] string pObjectName, ResourceType objectType, SecurityInformation securityInfo, IntPtr pSidOwner, IntPtr pSidGroup, IntPtr pDacl, IntPtr pSacl);

      #endregion // Get/Set Security

      #region GetSecurityDescriptorXxx()

      /// <summary>The GetSecurityDescriptorDacl function retrieves a pointer to the discretionary access control list (DACL) in a specified security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorDacl(SafeHandle pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool lpbDaclPresent, out IntPtr pDacl, [MarshalAs(UnmanagedType.Bool)] out bool lpbDaclDefaulted);

      /// <summary>The GetSecurityDescriptorSacl function retrieves a pointer to the system access control list (SACL) in a specified security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorSacl(SafeHandle pSecurityDescriptor, [MarshalAs(UnmanagedType.Bool)] out bool lpbSaclPresent, out IntPtr pSacl, [MarshalAs(UnmanagedType.Bool)] out bool lpbSaclDefaulted);

      /// <summary>The GetSecurityDescriptorGroup function retrieves the primary group information from a security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorGroup(SafeHandle pSecurityDescriptor, out IntPtr pGroup, [MarshalAs(UnmanagedType.Bool)] out bool lpbGroupDefaulted);

      /// <summary>The GetSecurityDescriptorControl function retrieves a security descriptor control and revision information.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorControl(SafeHandle pSecurityDescriptor, out SecurityDescriptorControl pControl, out uint lpdwRevision);

      /// <summary>The GetSecurityDescriptorOwner function retrieves the owner information from a security descriptor.</summary>
      /// <returns>
      /// If the function succeeds, the function returns nonzero.
      /// If the function fails, it returns zero. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetSecurityDescriptorOwner(SafeHandle pSecurityDescriptor, out IntPtr pOwner, [MarshalAs(UnmanagedType.Bool)] out bool lpbOwnerDefaulted);

      /// <summary>The GetSecurityDescriptorLength function returns the length, in bytes, of a structurally valid security descriptor. The length includes the length of all associated structures.</summary>
      /// <returns>
      /// If the function succeeds, the function returns the length, in bytes, of the SECURITY_DESCRIPTOR structure.
      /// If the SECURITY_DESCRIPTOR structure is not valid, the return value is undefined.
      /// </returns>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule"), DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.U4)]
      internal static extern uint GetSecurityDescriptorLength(SafeHandle pSecurityDescriptor);

      #endregion // GetSecurityDescriptorXxx()

      /// <summary>Frees the specified local memory object and invalidates its handle.</summary>
      /// <returns>
      /// If the function succeeds, the return value is NULL.
      /// If the function fails, the return value is equal to a handle to the local memory object. To get extended error information, call GetLastError.
      /// </returns>
      /// <remarks>
      /// Note  The local functions have greater overhead and provide fewer features than other memory management functions.
      /// New applications should use the heap functions unless documentation states that a local function should be used.
      /// For more information, see Global and Local Functions.
      /// </remarks>
      /// <remarks>Minimum supported client: Windows XP</remarks>
      /// <remarks>Minimum supported server: Windows Server 2003</remarks>
      [SuppressMessage("Microsoft.Security", "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule")]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      internal static extern IntPtr LocalFree(IntPtr hMem);

      #endregion // DllImport

      #region GetAccessControlInternal

      /// <summary>Unified method GetAccessControlInternal() to get/set an <see cref="ObjectSecurity"/> for a particular directory or file.</summary>
      /// <param name="isFolder"><c>true</c> indicates a folder object, <c>false</c> indicates a file object.</param>
      /// <param name="path">The path to a directory containing a <see cref="DirectorySecurity"/> object that describes the directory's or file's access control list (ACL) information.</param>
      /// <param name="includeSections">One (or more) of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to receive.</param>
      /// <returns>An <see cref="ObjectSecurity"/> object that encapsulates the access control rules for the directory or file described by the <paramref name="path"/> parameter. </returns>
      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      [SecurityCritical]
      internal static ObjectSecurity GetAccessControlInternal(bool isFolder, string path, AccessControlSections includeSections)
      {
         if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("path");

         // 2012-10-19: Yomodo; GetFileSecurity() seems to perform better than GetNamedSecurityInfo() and doesn't require Administrator rights.

         // In the ANSI version of this function, the name is limited to MAX_PATH characters.
         // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
         // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
         string pathLp = Path.PrefixLongPath(path);


         SecurityInformation securityInfo = 0;
         PrivilegeEnabler privilegeEnabler = null;
         SafeGlobalMemoryBufferHandle buffer = null;
         ObjectSecurity objectSecurity;

         try
         {
            if ((includeSections & AccessControlSections.Access) != 0)
               securityInfo |= SecurityInformation.Dacl;

            if ((includeSections & AccessControlSections.Audit) != 0)
            {
               // We need the SE_SECURITY_NAME privilege enabled to be able to get the
               // SACL descriptor. So we enable it here for the remainder of this function.
               privilegeEnabler = new PrivilegeEnabler(Privilege.Security);
               securityInfo |= SecurityInformation.Sacl;
            }

            if ((includeSections & AccessControlSections.Group) != 0)
               securityInfo |= SecurityInformation.Group;

            if ((includeSections & AccessControlSections.Owner) != 0)
               securityInfo |= SecurityInformation.Owner;


            uint sizeRequired;
            buffer = new SafeGlobalMemoryBufferHandle(512);

            bool gotSecurityOk = GetFileSecurity(pathLp, securityInfo, buffer, (uint) buffer.Capacity, out sizeRequired);
            int lastError = Marshal.GetLastWin32Error();

            if (!gotSecurityOk)
            {
               // A larger buffer is required to store the descriptor; increase size and try again.
               if (sizeRequired > buffer.Capacity)
               {
                  buffer.Dispose();
                  using (buffer = new SafeGlobalMemoryBufferHandle((int)sizeRequired))
                  {
                     gotSecurityOk = GetFileSecurity(pathLp, securityInfo, buffer, (uint)buffer.Capacity, out sizeRequired);
                     lastError = Marshal.GetLastWin32Error();
                  }
               }
            }

            if (!gotSecurityOk)
               NativeError.ThrowException(lastError, pathLp);

            objectSecurity = (isFolder) ? (ObjectSecurity)new DirectorySecurity() : new FileSecurity();
            objectSecurity.SetSecurityDescriptorBinaryForm(buffer.ToByteArray(0, buffer.Capacity));
         }
         finally
         {
            if (buffer != null)
               buffer.Dispose();

            if (privilegeEnabler != null)
               privilegeEnabler.Dispose();
         }

         return objectSecurity;
      }

      #endregion // GetAccessControlInternal

      #region SetAccessControlInternal

      /// <summary>Unified method SetAccessControlInternal() applies access control list (ACL) entries described by a <see cref="FileSecurity"/> FileSecurity object to the specified file.</summary>
      /// <param name="path">A file to add or remove access control list (ACL) entries from. This parameter may be <see langword="null"/>.</param>
      /// <param name="handle">A handle to add or remove access control list (ACL) entries from. This parameter may be <see langword="null"/>.</param>
      /// <param name="objectSecurity">A <see cref="DirectorySecurity"/> or <see cref="FileSecurity"/> object that describes an ACL entry to apply to the file described by the <paramref name="path"/> parameter.</param>
      /// <param name="includeSections">One or more of the <see cref="AccessControlSections"/> values that specifies the type of access control list (ACL) information to set.</param>
      /// <remarks>Supply either a path or handle, not both.</remarks>
      [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
      [SecurityCritical]
      internal static bool SetAccessControlInternal(string path, SafeHandle handle, ObjectSecurity objectSecurity, AccessControlSections includeSections)
      {
         if (string.IsNullOrEmpty(path))
         {
            if (handle == null)
               throw new ArgumentNullException("path");
         }

         if (!Filesystem.NativeMethods.IsValidHandle(handle, false))
         {
            if (string.IsNullOrEmpty(path))
               throw new ArgumentException(Resources.HandleInvalid);
         }

         if (objectSecurity == null)
            throw new ArgumentNullException("objectSecurity");


         byte[] managedDescriptor = objectSecurity.GetSecurityDescriptorBinaryForm();
         using (SafeGlobalMemoryBufferHandle hDescriptor = new SafeGlobalMemoryBufferHandle(managedDescriptor.Length))
         {
            hDescriptor.CopyFrom(managedDescriptor, 0, managedDescriptor.Length);

            SecurityDescriptorControl control;
            uint revision;
            if (!GetSecurityDescriptorControl(hDescriptor, out control, out revision))
               NativeError.ThrowException();
            
            PrivilegeEnabler privilegeEnabler = null;
            try
            {
               SecurityInformation securityInfo = SecurityInformation.None;

               if ((includeSections & AccessControlSections.Access) != 0)
                  securityInfo |= SecurityInformation.Dacl;

               if ((includeSections & AccessControlSections.Audit) != 0)
               {
                  // We need the SE_SECURITY_NAME privilege enabled to be able to get the
                  // SACL descriptor. So we enable it here for the remainder of this function.
                  privilegeEnabler = new PrivilegeEnabler(Privilege.Security);
                  securityInfo |= SecurityInformation.Sacl;
               }

               if ((includeSections & AccessControlSections.Group) != 0)
                  securityInfo |= SecurityInformation.Group;

               if ((includeSections & AccessControlSections.Owner) != 0)
                  securityInfo |= SecurityInformation.Owner;


               IntPtr pDacl = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Access) != 0)
               {
                  securityInfo |= SecurityInformation.Dacl;
                  securityInfo |= (control & SecurityDescriptorControl.DaclProtected) != 0
                                     ? SecurityInformation.ProtectedDacl
                                     : SecurityInformation.UnprotectedDacl;

                  bool daclDefaulted, daclPresent;
                  if (!GetSecurityDescriptorDacl(hDescriptor, out daclPresent, out pDacl, out daclDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pSacl = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Audit) != 0)
               {
                  securityInfo |= SecurityInformation.Sacl;
                  securityInfo |= (control & SecurityDescriptorControl.SaclProtected) != 0
                                     ? SecurityInformation.ProtectedSacl
                                     : SecurityInformation.UnprotectedSacl;

                  privilegeEnabler = new PrivilegeEnabler(Privilege.Security);

                  bool saclDefaulted, saclPresent;
                  if (!GetSecurityDescriptorSacl(hDescriptor, out saclPresent, out pSacl, out saclDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pOwner = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Owner) != 0)
               {
                  securityInfo |= SecurityInformation.Owner;
                  bool ownerDefaulted;
                  if (!GetSecurityDescriptorOwner(hDescriptor, out pOwner, out ownerDefaulted))
                     NativeError.ThrowException();
               }

               IntPtr pGroup = IntPtr.Zero;
               if ((includeSections & AccessControlSections.Group) != 0)
               {
                  securityInfo |= SecurityInformation.Group;
                  bool groupDefaulted;
                  if (!GetSecurityDescriptorGroup(hDescriptor, out pGroup, out groupDefaulted))
                     NativeError.ThrowException();
               }

            
               if (!string.IsNullOrEmpty(path))
               {
                  // In the ANSI version of this function, the name is limited to MAX_PATH characters.
                  // To extend this limit to 32,767 wide characters, call the Unicode version of the function and prepend "\\?\" to the path.
                  // 2013-01-13: MSDN doesn't confirm LongPath usage but a Unicode version of this function exists.
                  string pathLp = Path.PrefixLongPath(path);

                  if (SetNamedSecurityInfo(pathLp, ResourceType.FileObject, securityInfo, pOwner, pGroup, pDacl, pSacl) != Win32Errors.ERROR_SUCCESS)
                     NativeError.ThrowException(pathLp);
               }

               if (Filesystem.NativeMethods.IsValidHandle(handle, false))
                  if (SetSecurityInfo(handle, ResourceType.FileObject, securityInfo, pOwner, pGroup, pDacl, pSacl) != Win32Errors.ERROR_SUCCESS)
                     NativeError.ThrowException();
            }
            finally
            {
               if (privilegeEnabler != null)
                  privilegeEnabler.Dispose();
            }
         }

         return true;
      }

      #endregion // SetAccessControlInternal
   }
}