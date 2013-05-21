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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Alphaleonis.Win32.Filesystem;

namespace Alphaleonis.Win32
{
   internal static class NativeError
   {
      private static string FormatError(string message, string path)
      {
         return string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", message ?? string.Empty,
                              (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(path)) ? ":" : string.Empty,
                              path ?? string.Empty);
      }

      [SecurityCritical]
      public static void ThrowException()
      {
         ThrowException((uint)Marshal.GetLastWin32Error(), null, null);
      }

      #region int

      [SecurityCritical]
      public static void ThrowException(int errorCode)
      {
         ThrowException((uint)errorCode, null, null);
      }

      [SecurityCritical]
      public static void ThrowException(int errorCode, string readPath)
      {
         ThrowException((uint)errorCode, readPath, null);
      }

      [SecurityCritical]
      public static void ThrowException(int errorCode, string readPath, string writePath)
      {
         ThrowException((uint)errorCode, readPath, writePath);
      }

      #endregion // int

      #region uint

      [SecurityCritical]
      public static void ThrowException(uint errorCode)
      {
         ThrowException(errorCode, null, null);
      }

      [SecurityCritical]
      public static void ThrowException(uint errorCode, string readPath)
      {
         ThrowException(errorCode, readPath, null);
      }

      [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
      [SecurityCritical]
      public static void ThrowException(uint errorCode, string readPath, string writePath)
      {
         // Generate an exception representing the error from Marshal. We embed
         // this as an inner exception if we decide to generate our own exception.
         Exception e = Marshal.GetExceptionForHR(Win32Errors.GetHRFromWin32Error(errorCode));

         switch (errorCode)
         {
            case Win32Errors.ERROR_FILE_NOT_FOUND:
               throw new FileNotFoundException(e.Message, readPath ?? (writePath ?? string.Empty), e);

            case Win32Errors.ERROR_INVALID_TRANSACTION:
               throw new InvalidTransactionException(Resources.InvalidTransaction, e);

            case Win32Errors.ERROR_ALREADY_EXISTS:
            case Win32Errors.ERROR_FILE_EXISTS:
               throw new AlreadyExistsException(Resources.PathAlreadyExists, writePath ?? string.Empty, e);

            case Win32Errors.ERROR_PATH_NOT_FOUND:
               throw new DirectoryNotFoundException(FormatError(Resources.DirectoryNotFound, readPath ?? writePath), e);

            case Win32Errors.ERROR_DIRECTORY:
               throw new NotSupportedException(FormatError(Resources.InvalidDirectoryName, readPath ?? writePath), e);

            case Win32Errors.ERROR_DIR_NOT_EMPTY:
               throw new DirectoryNotEmptyException(FormatError(Resources.DirectoryNotEmpty, writePath), e);

            case Win32Errors.ERROR_TRANSACTION_ALREADY_COMMITTED:
               throw new TransactionAlreadyCommittedException(Resources.TransactionAlreadyCommitted, e);

            case Win32Errors.ERROR_TRANSACTION_ALREADY_ABORTED:
               throw new TransactionAlreadyAbortedException(Resources.TransactionAlreadyAborted, e);

            case Win32Errors.ERROR_TRANSACTIONAL_CONFLICT:
               throw new TransactionalConflictException(Resources.TransactionalConflict, e);

            case Win32Errors.ERROR_TRANSACTION_NOT_ACTIVE:
               throw new TransactionException(Resources.TransactionNotActive, e);

            case Win32Errors.ERROR_TRANSACTION_NOT_REQUESTED:
               throw new TransactionException(Resources.TransactionNotRequested, e);

            case Win32Errors.ERROR_TRANSACTION_REQUEST_NOT_VALID:
               throw new TransactionException(Resources.InvalidTransactionRequest, e);

            case Win32Errors.ERROR_TRANSACTIONS_UNSUPPORTED_REMOTE:
               throw new UnsupportedRemoteTransactionException(Resources.InvalidTransactionRequest, e);

            case Win32Errors.ERROR_NOT_A_REPARSE_POINT:
               throw new NotAReparsePointException(Resources.NotAReparsePoint, e);

            case Win32Errors.ERROR_SUCCESS:
            case Win32Errors.ERROR_SUCCESS_REBOOT_INITIATED:
            case Win32Errors.ERROR_SUCCESS_REBOOT_REQUIRED:
            case Win32Errors.ERROR_SUCCESS_RESTART_REQUIRED:
               // We should really never get here, throwing an exception for a successful operation.
               throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, Resources.AlphaFSInternalError + ": " + Resources.AttemptingToGenerateExceptionFromSuccessfulOperation + Resources.ErrorCodeWas0 + ": " + errorCode));

            case Win32Errors.ERROR_ACCESS_DENIED:
               throw new UnauthorizedAccessException(e.Message, e);

            default:
               // We don't have a specific exception to generate for this error.
               throw e;
         }
      }

      #endregion // uint

      #region string

      [SecurityCritical]
      public static void ThrowException(string readPath)
      {
         ThrowException((uint)Marshal.GetLastWin32Error(), readPath, null);
      }

      [SecurityCritical]
      public static void ThrowException(string readPath, string writePath)
      {
         ThrowException((uint)Marshal.GetLastWin32Error(), readPath, writePath);
      }

      #endregion // string
   }
}