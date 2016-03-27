// ZipFile.Async.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa.  
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License. 
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs): 
// Time-stamp: <2009-September-03 18:34:19>
//
// ------------------------------------------------------------------
//
// This module defines the methods for asynchronous Save and Read
// operations on zip files: SaveAsync, ReadAsync, and supporting code.
//
// ------------------------------------------------------------------
//


using System;
using System.IO;
using System.Collections.Generic;

namespace Ionic.Zip
{

    public partial class ZipFile
    {
        /// <summary>
        /// An event handler invoked when a Save() completes. 
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     This event is used with the <see cref="SaveAsync()"/> method. It fires
        ///     after the asynchronous call completes.
        ///   </para>
        ///   <para>
        ///     There is a related event called <see cref="SaveProgress"/>.  That event
        ///     is useful for the case where you'd like to get finer-grained
        ///     notification of progress during the <c>Save()</c>, whether you call
        ///     <c>Save()</c> synchronously or asynchronously. <c>SaveCompleted</c> is
        ///     useful for the case where you want a single event at the end of an 
        ///     asynchronous <c>Save()</c>.
        ///   </para>
        ///
        ///   <para>
        ///     This event is not available in the Reduced version of the DotNetZip library.
        ///     It's also not avvailable in the Compact Framework version of the library.
        ///   </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="SaveProgress" />
        public event EventHandler<AsyncSaveCompletedEventArgs> SaveCompleted;

        private delegate void _Action();
        private delegate void _Action<T1,T2>(T1 arg1, T2 arg2);

        /// <summary>
        /// Save the zip file asynchronously. 
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     See the documentation for <see cref="Save()"/> for details on how saving
        ///     zip archives works. Prior to calling this method, you probably want to
        ///     add a handler for the <see cref="SaveCompleted" /> event.
        ///   </para>
        ///
        ///   <para>
        ///     This is not available in the Reduced version of the DotNetZip library.
        ///     It's also not avvailable in the Compact Framework version of the library.
        ///   </para>
        ///
        /// </remarks>
        /// 
        /// <seealso cref="SaveCompleted" />
        public void SaveAsync()
        {
            _Action a = this.Save;
            IAsyncResult result = a.BeginInvoke(AsyncSaveCompleted, null);
        }

        /// <summary>
        /// Save the zip file asynchronously to the specified file.
        /// </summary>
        ///
        public void SaveAsync(String fileName)
        {
            Action<String> a = this.Save;
            IAsyncResult result = a.BeginInvoke(fileName, AsyncSaveCompleted, null);
        }

        
        /// <summary>
        /// Save the zip file asynchronously to the specified Stream.
        /// </summary>
        ///
        public void SaveAsync(Stream outputStream)
        {
            Action<Stream> a = this.Save;
            IAsyncResult result = a.BeginInvoke(outputStream, AsyncSaveCompleted, null);
        }

        
        private void AsyncSaveCompleted(IAsyncResult ar)
        {
            _Action a = (ar as System.Runtime.Remoting.Messaging.AsyncResult).AsyncDelegate as _Action;
            AsyncSaveCompletedEventArgs args = null;
            try 
            {
                a.EndInvoke(ar);
                if (SaveCompleted != null)
                {
                    args = new AsyncSaveCompletedEventArgs(ArchiveNameForEvent);
                    if (_saveOperationCanceled)
                        args._wasCanceled = true;
                }
            }
            catch (Exception exc1)
            {
                if (SaveCompleted != null)
                    args = new AsyncSaveCompletedEventArgs(ArchiveNameForEvent, exc1);
            }

            if (args != null)
            {
                lock (LOCK)
                {
                    SaveCompleted(this, args);
                }
            }

        }

        
        /// <summary>
        /// Cancels any pending asynchronous <c>Save()</c> operation.
        /// </summary>
        ///
        ///
        /// <remarks>
        ///  <para>
        ///   This is not available in the Reduced version of the DotNetZip library.
        ///   It's also not avvailable in the Compact Framework version of the library.
        ///  </para>
        /// </remarks>
        ///
        public void SaveAsyncCancel()
        {
            _saveOperationCanceled= true;
        }



#if ASYNCREAD
        /// <summary>
        /// An event handler invoked when a Read() completes. 
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     This event is used with the ReadAsync() method. It fires
        ///     after the asynchronous read completes.
        ///   </para>
        ///   <para>
        ///     There is a related event called <see cref="ReadProgress"/>.  That event
        ///     is useful for the case where you'd like to get finer-grained
        ///     notification of progress during the <c>Read()</c>, whether you call
        ///     <c>Read()</c> synchronously or asynchronously. <c>ReadCompleted</c> is
        ///     useful for the case where you want a single event at the end of an 
        ///     asynchronous <c>Read()</c>.
        ///   </para>
        /// </remarks>
        ///
        /// <seealso cref="ReadProgress" />
        public event EventHandler<AsyncReadCompletedEventArgs> ReadCompleted;

        public void ReadAsync()
        {
            Action a = new Action(this.Save);
            IAsyncResult result = a.BeginInvoke(AsyncSaveCompleted, null);
        }

        private void AsyncReadCompleted(IAsyncResult ar)
        {
            Action a = (ar as System.Runtime.Remoting.Messaging.AsyncResult).AsyncDelegate as Action;
            AsyncReadCompletedEventArgs args = null;
            
            try 
            {
                a.EndInvoke(ar);
                if (ReadCompleted != null)
                    args = new AsyncReadCompletedEventArgs(ArchiveNameForEvent);
            }
            catch (Exception exc1)
            {
                if (ReadCompleted != null)
                    args = new AsyncReadCompletedEventArgs(ArchiveNameForEvent, exc1);
            }
                            
            if (args != null)
            {
                lock (LOCK)
                {
                    ReadCompleted(this, args);
                }
            }
        }

        
        /// <summary>
        /// Cancels any pending asynchronous <c>Read()</c> operation.
        /// </summary>
        ///
        public void ReadAsyncCancel()
        {
            _saveOperationCanceled= true;
        }
#endif



        /// <summary>
        /// An event handler invoked when a ExtractAll() completes. 
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     This event is used with the <see cref="ExtractAllAsync(String)"/> method. It fires
        ///     after the asynchronous call completes.
        ///   </para>
        ///
        ///   <para>
        ///     There is a related event called <see cref="ExtractProgress"/>.  That event
        ///     is useful for the case where you'd like to get finer-grained
        ///     notification of progress during the <c>ExtractAll()</c>, whether you call
        ///     <c>ExtractAll()</c> synchronously or asynchronously. <c>ExtractAllCompleted</c> is
        ///     useful for the case where you want a single event at the end of an 
        ///     asynchronous <c>ExtractAll()</c>.
        ///   </para>
        ///
        ///   <para>
        ///     This event is not available in the Reduced version of the DotNetZip library.
        ///     It's also not avvailable in the Compact Framework version of the library.
        ///   </para>
        ///
        /// </remarks>
        ///
        /// <seealso cref="ExtractProgress" />
        public event EventHandler<AsyncExtractAllCompletedEventArgs> ExtractAllCompleted;


        /// <summary>
        /// Extract all files in the zip file asynchronously. 
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///     See the documentation for <see cref="ExtractAll(String)"/> for details
        ///     on how extracting all files in a zip archive works. Prior to calling
        ///     this method, you probably want to add a handler for the <see
        ///     cref="ExtractAllCompleted" /> event.
        ///   </para>
        ///
        ///   <para>
        ///     This is not available in the Reduced version of the DotNetZip library.
        ///     It's also not avvailable in the Compact Framework version of the library.
        ///   </para>
        /// 
        /// </remarks>
        ///
        /// <seealso cref="ExtractAllCompleted" />
        /// <seealso cref="ExtractAll(String)" />
        /// <seealso cref="ExtractAllAsync(String,ExtractExistingFileAction)" />
        public void ExtractAllAsync(string path)
        {
            Action<String> a = this.ExtractAll;
            IAsyncResult result = a.BeginInvoke(path, AsyncExtractAllCompleted, null);
        }


        /// <summary>
        /// Extract all files in the zip file asynchronously. 
        /// </summary>
        ///
        /// <seealso cref="ExtractAllCompleted" />
        /// <seealso cref="ExtractAllAsync(String)" />
        /// <seealso cref="ExtractAll(String,ExtractExistingFileAction)" />
        public void ExtractAllAsync(string path, ExtractExistingFileAction extractExistingFile)
        {
            _Action<String, ExtractExistingFileAction> a = this.ExtractAll;
            IAsyncResult result = a.BeginInvoke(path, extractExistingFile, AsyncExtractAllCompleted, null);
        }

        
        private void AsyncExtractAllCompleted(IAsyncResult ar)
        {
            Action<String> a = (ar as System.Runtime.Remoting.Messaging.AsyncResult).AsyncDelegate as Action<String>;
            AsyncExtractAllCompletedEventArgs args = null;
            
            try 
            {
                a.EndInvoke(ar);
                if (ExtractAllCompleted != null)
                {
                    args = new AsyncExtractAllCompletedEventArgs(ArchiveNameForEvent);
                    if (_extractOperationCanceled)
                        args._wasCanceled = true;
                }
            }
            catch (Exception exc1)
            {
                if (ExtractAllCompleted != null)
                    args = new AsyncExtractAllCompletedEventArgs(ArchiveNameForEvent, exc1);
            }
                            
            if (args != null)
            {
                lock (LOCK)
                {
                    ExtractAllCompleted(this, args);
                }
            }
        }

        
        /// <summary>
        /// Cancels any pending asynchronous <c>ExtractAll()</c> operation.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///   This is not available in the Reduced version of the DotNetZip library.
        ///   It's also not avvailable in the Compact Framework version of the library.
        ///  </para>
        /// </remarks>
        public void ExtractAllAsyncCancel()
        {
            _extractOperationCanceled= true;
        }

        
        
        /// <summary>
        /// Returns true if there is a pending asynchronous operation.
        /// </summary>
        ///
        /// <remarks>
        ///  <para>
        ///   This property returns true if <see cref="SaveAsync()"/>
        ///   has been called and the save has not yet completed.
        ///  </para>
        ///  <para>
        ///   This is not available in the Reduced version of the DotNetZip library.
        ///   It's also not avvailable in the Compact Framework version of the library.
        ///  </para>
        /// </remarks>
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
        }

        partial void _SetBusy(bool value)
        {
            _isBusy = value;
        }

        private bool _isBusy;
    }


    /// <summary>
    ///   Provides information at the completion of an asynchronous <c>Save</c>. 
    /// </summary>
    ///
    /// <remarks>
    ///  <para>
    ///   This class is not available in the Reduced version of the DotNetZip library.
    ///   It's also not avvailable in the Compact Framework version of the library.
    ///  </para>
    /// </remarks>
    ///
    /// <seealso cref="ZipFile.SaveAsync()"/>
    public class AsyncSaveCompletedEventArgs : ZipProgressEventArgs
    {
        private System.Exception _exception;
        internal bool _wasCanceled;
        
        internal AsyncSaveCompletedEventArgs(string archiveName, Exception e) :
            base(archiveName, ZipProgressEventType.Saving_Completed)
        {
            _exception = e;
        }
            
        internal AsyncSaveCompletedEventArgs(string archiveName) :
            base(archiveName, ZipProgressEventType.Saving_Completed)
        {
        }
        
        /// <summary>
        /// The Exception generated during the save, if any.
        /// </summary>
        public Exception @Exception
        {
            get { return _exception; }
        }

        
        /// <summary>
        /// Indicates whether the asynchronous save operation was canceled.
        /// </summary>
        public bool Canceled
        {
            get { return _wasCanceled; }
        }
    }


    #if ASYNCREAD
    public class AsyncReadCompletedEventArgs : ZipProgressEventArgs
    {
        private System.Exception _exception;
        internal bool _wasCanceled;
        
        internal AsyncReadCompletedEventArgs(string archiveName, Exception e) :
            base(archiveName, ZipProgressEventType.Reading_Completed)
        {
            _exception = e;
        }
            
        internal AsyncReadCompletedEventArgs(string archiveName) :
            base(archiveName, ZipProgressEventType.Reading_Completed)
        {
        }
        
        /// <summary>
        /// The Exception generated during the read, if any.
        /// </summary>
        public Exception @Exception
        {
            get { return _exception; }
        }

        
        /// <summary>
        /// Indicates whether the asynchronous read operation was canceled.
        /// </summary>
        public bool Canceled
        {
            get { return _wasCanceled; }
        }
    }
#endif

    
    /// <summary>
    ///   Provides information at the completion of an asynchronous <c>ExtractAll</c>. 
    /// </summary>
    ///
    /// <remarks>
    ///  <para>
    ///   This class is not available in the Reduced version of the DotNetZip library.
    ///   It's also not avvailable in the Compact Framework version of the library.
    ///  </para>
    /// </remarks>
    ///
    /// <seealso cref="ZipFile.ExtractAllAsync(String)"/>
    public class AsyncExtractAllCompletedEventArgs : ZipProgressEventArgs
    {
        private System.Exception _exception;
        internal bool _wasCanceled;
        
        internal AsyncExtractAllCompletedEventArgs(string archiveName, Exception e) :
            base(archiveName, ZipProgressEventType.Extracting_AfterExtractAll)
        {
            _exception = e;
        }
            
        internal AsyncExtractAllCompletedEventArgs(string archiveName) :
            base(archiveName, ZipProgressEventType.Extracting_AfterExtractAll)
        {
        }
        
        /// <summary>
        /// The Exception generated during the <c>Extractall</c>, if any.
        /// </summary>
        public Exception @Exception
        {
            get { return _exception; }
        }

        
        /// <summary>
        /// Indicates whether the asynchronous <c>ExtractAll</c> operation was canceled.
        /// </summary>
        public bool Canceled
        {
            get { return _wasCanceled; }
        }
    }

}
