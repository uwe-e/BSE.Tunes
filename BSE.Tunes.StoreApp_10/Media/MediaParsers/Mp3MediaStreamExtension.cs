//-----------------------------------------------------------------------
// <copyright file="Mp3MediaStreamSource.cs" company="Larry Olson">
// (c) Copyright Larry Olson.
//
// Changes to support duration and streaming (i.e. non-seekable) content
// (c) Copyright 2010 Rdio.
//
// This source is subject to the Microsoft Public License (Ms-PL)
// See http://code.msdn.microsoft.com/ManagedMediaHelpers/Project/License.aspx
// All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//-----------------------------------------------------------------------
// adapted by Uwe Eichkorn
// original is located at https://github.com/loarabia/ManagedMediaHelpers/blob/master/Mp3MediaStreamSource.SL4/Mp3MediaStreamSource.cs
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.IO;

namespace MediaParsers
{
	public static class Mp3MediaStreamExtension
	{
		/// <summary>
		/// Buffer for decoding audio frames into. 4096 should be larger than we'll ever need, right? (144*448*1000/44100)
		/// </summary>
		private static byte[] buffer = new byte[4096];
		/// <summary>
		/// Read off the Id3Data from the stream and return the first MpegFrame of the audio stream.
		/// This assumes that the first bit of data is either an ID3 segment or an MPEG segment. Calls a separate thread
		/// to read past ID3v2 data.
		/// </summary>
		/// <param name="randomAccessStream"></param>
		/// <returns>The first MpegFrame.</returns>
		public static MpegFrame ReadPastId3V2Tags(this IRandomAccessStream randomAccessStream)
		{
			MpegFrame mpegFrame = null;
			var audioStream = randomAccessStream.AsStreamForRead();
			if (audioStream != null)
			{
				// Read and (throw out) any Id3 data if present.
				byte[] data = new byte[10];
				audioStream.Position = 0;
				if (audioStream.Read(data, 0, 3) != 3)
				{
					goto cleanup;
				}
				if (data[0] == 73 /* I */ &&
					data[1] == 68 /* D */ &&
					data[2] == 51 /* 3 */)
				{
					// Need to update to read the is footer present flag and account for its 10 bytes if needed.
					if (audioStream.Read(data, 3, 7) != 7)
					{
						goto cleanup;
					}

					int id3Size = BitTools.ConvertSyncSafeToInt32(data, 6);
					int bytesRead = 0;

					// Read through the ID3 Data tossing it out.)
					while (id3Size > 0)
					{
						bytesRead = (id3Size - buffer.Length > 0)
															? audioStream.Read(buffer, 0, buffer.Length)
															: audioStream.Read(buffer, 0, id3Size);
						id3Size -= bytesRead;
					}
					mpegFrame = new MpegFrame(audioStream);
				}
				else
				{
					// No ID3 tag present, presumably this is streaming and we are starting right at the Mp3 data.
					// Assume the stream isn't seekable.
					if (audioStream.Read(data, 3, 1) != 1)
					{
						goto cleanup;
					}
					mpegFrame = new MpegFrame(audioStream, data);
				}
			}
			return mpegFrame;
		// Cleanup and quit if you couldn't even read the initial data for some reason.
		cleanup:
			throw new Exception("Could not read intial audio stream data");
		}
	}
}
