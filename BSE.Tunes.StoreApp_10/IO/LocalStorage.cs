using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BSE.Tunes.StoreApp.IO
{
	public class LocalStorage
	{
		#region Constants
		private const string CacheFolderName = "cache";
		private const string ImageFolderName = "img";
		private const string TempFolderName = "tmp";
		#endregion

		#region MethodsPublic
		/// <summary>
		/// Gets the main storage folder.
		/// </summary>
		/// <returns>The main storage folder.</returns>
		public static async Task<StorageFolder> GetCacheFolderAsync()
		{
			return await ApplicationData.Current.LocalFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
		}
		/// <summary>
		/// Gets the images storage folder.
		/// </summary>
		/// <returns>The images storage folder.</returns>
		public static async Task<StorageFolder> GetImageFolderAsync()
		{
			var storageFolder = await GetCacheFolderAsync();
			return await storageFolder.CreateFolderAsync(ImageFolderName, CreationCollisionOption.OpenIfExists);
		}
		/// <summary>
		/// Gets the tmp storage folder.
		/// </summary>
		/// <returns>The tmp storage folder.</returns>
		public static async Task<StorageFolder> GetTempFolderAsync()
		{
			var storageFolder = await GetCacheFolderAsync();
			return await storageFolder.CreateFolderAsync(TempFolderName, CreationCollisionOption.OpenIfExists);
		}
		/// <summary>
		/// Deletes the tmp Folder with its files from the local store.
		/// </summary>
		/// <returns></returns>
		public static async Task ClearTempFolderAsync()
		{
			var storageFolder = await GetTempFolderAsync();
			await storageFolder.DeleteAsync();
		}
		#endregion
	}
}
