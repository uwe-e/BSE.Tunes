using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BSE.Tunes.WebApi.Providers
{
    public class FileProvider : IDisposable
    {
        #region FieldsPrivate
        private FileStream m_fileStream = null;
        // Track whether Dispose has been called.
        private bool m_bDisposed;
        #endregion

        #region MethodsPublic

        public bool Exists(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo.Exists;
        }

        public FileStream Open(string fileName)
        {
            this.m_fileStream = null;
            if (this.Exists(fileName))
            {
                this.m_fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            }
            return this.m_fileStream;
;
        }
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        #endregion

        #region MethodsProtected
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.m_bDisposed == false)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    //component.Dispose();
                    if (this.m_fileStream != null)
                    {
                        this.m_fileStream.Dispose();
                    }
                }
                this.m_bDisposed = true;
            }
        }
        #endregion
    }
}