using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
	public class BaseApiController : ApiController
	{
		#region FieldsPrivate
		private ITunesService m_tunesService;
		// Track whether Dispose has been called.
		private bool m_isDisposed;
		#endregion

		#region Properties
		public ITunesService TunesService
		{
			get
			{
				return this.m_tunesService;
			}
		}
		#endregion

		#region MethodsPublic
		public BaseApiController()
		{
			this.m_tunesService = new BSE.Tunes.Entities.TunesBusinessObject();
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
		protected override void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (this.m_isDisposed == false)
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					if (this.m_tunesService != null)
					{
						this.m_tunesService = null;
					}
				}
				this.m_isDisposed = true;
			}
			base.Dispose(disposing);
		}
		#endregion

	}
}