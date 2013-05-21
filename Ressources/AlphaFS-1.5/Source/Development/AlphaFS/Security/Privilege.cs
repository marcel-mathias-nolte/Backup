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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Alphaleonis.Win32.Security
{
   /// <summary>Represents a privilege for an access token. The privileges available on the local machine are available as 
   /// static instances from this class. To create a <see cref="Privilege"/> representing a privilege on another system,
   /// use the constructor specifying a system name together with one of these static instances.
   /// </summary>
   /// <seealso cref="PrivilegeEnabler"/>
   [ImmutableObject(true)]
   public class Privilege : IEquatable<Privilege>
   {
      #region System Privileges

      /// <summary>Required to assign the primary token of a process. User Right: Replace a process-level token.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege AssignPrimaryToken = new Privilege("SeAssignPrimaryTokenPrivilege");

      /// <summary>Required to generate audit-log entries. Give this privilege to secure servers. User Right: Generate security audits.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Audit = new Privilege("SeAuditPrivilege");

      /// <summary>Required to perform backup operations. This privilege causes the system to grant all read access control to any file, regardless of the access control list (ACL) specified for the file. Any access request other than read is still evaluated with the ACL. User Right: Back up files and directories.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Backup = new Privilege("SeBackupPrivilege");

      /// <summary>Required to receive notifications of changes to files or directories. This privilege also causes the system to skip all traversal access checks. It is enabled by default for all users. User Right: Bypass traverse checking.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege ChangeNotify = new Privilege("SeChangeNotifyPrivilege");

      /// <summary>Required to create named file mapping objects in the global namespace during Terminal Services sessions. This privilege is enabled by default for administrators, services, and the local system account. User Right: Create global objects.</summary>
      /// <remarks>Windows XP/2000:  This privilege is not supported. Note that this value is supported starting with Windows Server 2003, Windows XP SP2, and Windows 2000 SP4.</remarks>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege CreateGlobal = new Privilege("SeCreateGlobalPrivilege");

      /// <summary>Required to create a paging file. User Right: Create a pagefile.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pagefile")] [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege CreatePagefile = new Privilege("SeCreatePagefilePrivilege");

      /// <summary>Required to create a permanent object. User Right: Create permanent shared objects.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege CreatePermanent = new Privilege("SeCreatePermanentPrivilege");

      /// <summary>Required to create a symbolic link. User Right: Create symbolic links.</summary>           
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege CreateSymbolicLink = new Privilege("SeCreateSymbolicLinkPrivilege");

      /// <summary>Required to create a primary token. User Right: Create a token object.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege CreateToken = new Privilege("SeCreateTokenPrivilege");

      /// <summary>Required to debug and adjust the memory of a process owned by another account. User Right: Debug programs.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Debug = new Privilege("SeDebugPrivilege");

      /// <summary>Required to mark user and computer accounts as trusted for delegation. User Right: Enable computer and user accounts to be trusted for delegation.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege EnableDelegation = new Privilege("SeEnableDelegationPrivilege");

      /// <summary>Required to impersonate. User Right: Impersonate a client after authentication.</summary>
      /// <remarks>Windows XP/2000:  This privilege is not supported. Note that this value is supported starting with Windows Server 2003, Windows XP SP2, and Windows 2000 SP4.</remarks>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Impersonate = new Privilege("SeImpersonatePrivilege");

      /// <summary>Required to increase the base priority of a process. User Right: Increase scheduling priority.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege IncreaseBasePriority = new Privilege("SeIncreaseBasePriorityPrivilege");

      /// <summary>Required to increase the quota assigned to a process. User Right: Adjust memory quotas for a process.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege IncreaseQuota = new Privilege("SeIncreaseQuotaPrivilege");

      /// <summary>Required to allocate more memory for applications that run in the context of users. User Right: Increase a process working set.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege IncreaseWorkingSet = new Privilege("SeIncreaseWorkingSetPrivilege");

      /// <summary>Required to load or unload a device driver. User Right: Load and unload device drivers.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege LoadDriver = new Privilege("SeLoadDriverPrivilege");

      /// <summary>Required to lock physical pages in memory. User Right: Lock pages in memory.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege LockMemory = new Privilege("SeLockMemoryPrivilege");

      /// <summary>Required to create a computer account. User Right: Add workstations to domain.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege MachineAccount = new Privilege("SeMachineAccountPrivilege");

      /// <summary>Required to enable volume management privileges. User Right: Manage the files on a volume.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege ManageVolume = new Privilege("SeManageVolumePrivilege");

      /// <summary>Required to gather profiling information for a single process. User Right: Profile single process.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege ProfileSingleProcess = new Privilege("SeProfileSingleProcessPrivilege");

      /// <summary>Required to modify the mandatory integrity level of an object. User Right: Modify an object label.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Relabel")] public static readonly Privilege Relabel = new Privilege("SeRelabelPrivilege");

      /// <summary>Required to shut down a system using a network request. User Right: Force shutdown from a remote system.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege RemoteShutdown = new Privilege("SeRemoteShutdownPrivilege");

      /// <summary>Required to perform restore operations. This privilege causes the system to grant all write access control to any file, regardless of the ACL specified for the file. Any access request other than write is still evaluated with the ACL. Additionally, this privilege enables you to set any valid user or group SID as the owner of a file. User Right: Restore files and directories.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Restore = new Privilege("SeRestorePrivilege");

      /// <summary>Required to perform a number of security-related functions, such as controlling and viewing audit messages. This privilege identifies its holder as a security operator. User Right: Manage auditing and security log.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Security = new Privilege("SeSecurityPrivilege");

      /// <summary>Required to shut down a local system. User Right: Shut down the system.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Shutdown = new Privilege("SeShutdownPrivilege");

      /// <summary>Required for a domain controller to use the LDAP directory synchronization services. This privilege enables the holder to read all objects and properties in the directory, regardless of the protection on the objects and properties. By default, it is assigned to the Administrator and LocalSystem accounts on domain controllers. User Right: Synchronize directory service data.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege SyncAgent = new Privilege("SeSyncAgentPrivilege");

      /// <summary>Required to modify the nonvolatile RAM of systems that use this type of memory to store configuration information. User Right: Modify firmware environment values.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege SystemEnvironment = new Privilege("SeSystemEnvironmentPrivilege");

      /// <summary>Required to gather profiling information for the entire system. User Right: Profile system performance.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege SystemProfile = new Privilege("SeSystemProfilePrivilege");

      /// <summary>Required to modify the system time. User Right: Change the system time.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege SystemTime = new Privilege("SeSystemtimePrivilege");

      /// <summary>Required to take ownership of an object without being granted discretionary access. This privilege allows the owner value to be set only to those values that the holder may legitimately assign as the owner of an object. User Right: Take ownership of files or other objects.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege TakeOwnership = new Privilege("SeTakeOwnershipPrivilege");

      /// <summary>This privilege identifies its holder as part of the trusted computer base. Some trusted protected subsystems are granted this privilege. User Right: Act as part of the operating system.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tcb")] public static readonly Privilege Tcb = new Privilege("SeTcbPrivilege");

      /// <summary>Required to adjust the time zone associated with the computer's internal clock. User Right: Change the time zone.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege TimeZone = new Privilege("SeTimeZonePrivilege");

      /// <summary>Required to access Credential Manager as a trusted caller. User Right: Access Credential Manager as a trusted caller.</summary>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cred")] [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege TrustedCredManAccess = new Privilege("SeTrustedCredManAccessPrivilege");

      /// <summary>Required to undock a laptop. User Right: Remove computer from docking station.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege Undock = new Privilege("SeUndockPrivilege");

      /// <summary>Required to read unsolicited input from a terminal device. User Right: Not applicable.</summary>
      [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")] public static readonly
         Privilege UnsolicitedInput = new Privilege("SeUnsolicitedInputPrivilege");

      #endregion // System Privileges

      /// <summary>Create a new <see cref="Privilege"/> representing the specified privilege on the specified system.</summary>
      /// <param name="systemName">Name of the system.</param>
      /// <param name="privilege">The privilege to copy the privilege name from.</param>
      public Privilege(string systemName, Privilege privilege)
      {
         _mSystemName = systemName;
         if (privilege != null) _mName = privilege._mName;
      }

      /// <summary>Gets the system name identifying this privilege.</summary>
      /// <value>The system name identifying this privilege.</value>
      public string Name
      {
         get { return _mName; }
      }

      /// <summary>Retrieves the display name that represents this privilege.</summary>
      /// <returns>The display name that represents this privilege.</returns>
      [SecurityCritical]
      public string LookupDisplayName()
      {
         const UInt32 initialCapacity = 10;
         UInt32 displayNameCapacity = initialCapacity;
         StringBuilder displayName = new StringBuilder((int) displayNameCapacity);
         UInt32 languageId;

         if (
            !NativeMethods.LookupPrivilegeDisplayName(_mSystemName, _mName, ref displayName, ref displayNameCapacity,
                                                      out languageId))
         {
            int lastError = Marshal.GetLastWin32Error();
            if (lastError == Win32Errors.ERROR_INSUFFICIENT_BUFFER)
            {
               displayName = new StringBuilder((int) displayNameCapacity + 1);
               if (
                  !NativeMethods.LookupPrivilegeDisplayName(_mSystemName, _mName, ref displayName,
                                                            ref displayNameCapacity, out languageId))
                  NativeError.ThrowException();
            }
            else
               NativeError.ThrowException(lastError);
         }
         return displayName.ToString();
      }

      /// <summary>Retrieves the locally unique identifier (LUID) used on to represent this privilege (on the system from which it originates).</summary>
      /// <returns>the locally unique identifier (LUID) used on to represent this privilege (on the system from which it originates).</returns>
      [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Luid")]
      [SecurityCritical]
      public UInt64 LookupLuid()
      {
         NativeMethods.Luid luid;
         if (!NativeMethods.LookupPrivilegeValue(_mSystemName, _mName, out luid))
            NativeError.ThrowException();
         return Filesystem.NativeMethods.LuidToLong(luid);
      }

      /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
      /// <param name="other">An object to compare with this object.</param>
      /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
      public bool Equals(Privilege other)
      {
         if (other == null)
            return false;

         return _mName.Equals(other._mName, StringComparison.OrdinalIgnoreCase) &&
                ((_mSystemName == null && other._mSystemName == null) ||
                 (_mSystemName != null && _mSystemName.Equals(other._mSystemName, StringComparison.OrdinalIgnoreCase)));
      }

      /// <summary>Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.</summary>
      /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
      /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, false.</returns>
      /// <exception cref="System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
      public override bool Equals(object obj)
      {
         Privilege other = obj as Privilege;
         if (other == null)
            return false;

         return _mName.Equals(other._mName, StringComparison.OrdinalIgnoreCase) &&
                ((_mSystemName == null && other._mSystemName == null) ||
                 (_mSystemName != null && _mSystemName.Equals(other._mSystemName, StringComparison.OrdinalIgnoreCase)));
      }

      /// <summary>Serves as a hash function for a particular type.</summary>
      /// <returns>A hash code for the current <see cref="System.Object"/>.</returns>
      public override int GetHashCode()
      {
         return _mName.GetHashCode() ^ (_mSystemName == null ? 0 : _mSystemName.GetHashCode());
      }

      /// <summary>Returns the system name for this privilege.</summary>
      /// <remarks>This is equivalent to <see cref="Privilege.Name"/>.</remarks>
      /// <returns>A <see cref="System.String"/> that represents the current <see cref="System.Object"/>.</returns>
      public override string ToString()
      {
         return _mName;
      }

      /// <summary>Initializes a new instance of the <see cref="Privilege"/> class, representing a privilege with the specified name on the local system.</summary>
      /// <param name="name">The name.</param>
      private Privilege(String name)
      {
         if (name == null)
            throw new ArgumentNullException("name");

         if (name.Length == 0)
            throw new ArgumentException(Resources.Privilege_Privilege_name_cannot_be_empty, "name");

         _mName = name;
      }

      private readonly string _mName;
      private readonly string _mSystemName;
   };
}