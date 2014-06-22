using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.IO
{
	internal class DownloaderEventArgs : EventArgs
	{
		private long a;
		private long b;
		private double c;
		private bool d;
		private Exception e;
		private Uri m_requestUri;
		private Stream g;
		public Uri RequestUri
		{
			get
			{
				return this.m_requestUri;
			}
		}
		public Stream OutputStream
		{
			get
			{
				return this.g;
			}
		}
		public long BytesReceived
		{
			get
			{
				return this.a;
			}
		}
		public long TotalBytesToReceive
		{
			get
			{
				return this.b;
			}
		}
		public double ProgressRatio
		{
			get
			{
				return this.c;
			}
		}
		public Exception Error
		{
			get
			{
				return this.e;
			}
		}
		public bool IsCanceled
		{
			get
			{
				return this.d;
			}
		}
		public bool HasSucceeded
		{
			get
			{
				return !this.IsCanceled && this.Error == null;
			}
		}
		public DownloaderEventArgs(Uri requestUri, Stream stream, bool isCancelled, Exception exception)
		{
			this.m_requestUri = requestUri;
			this.g = stream;
			this.d = isCancelled;
			this.e = exception;
		}
		public DownloaderEventArgs(Uri responseUri, Stream stream, long bytesReceived, long totalBytesToReceive)
		{
			this.m_requestUri = responseUri;
			this.g = stream;
			this.a = bytesReceived;
			this.b = totalBytesToReceive;
			if (this.b == 0L)
			{
				this.c = 0.0;
				return;
			}
			this.c = (double)this.a / (double)this.b;
		}
	}
}
