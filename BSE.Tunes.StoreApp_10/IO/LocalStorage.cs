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
			return await ApplicationData.Current.TemporaryFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
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
        public static async Task ReleaseCacheAsync(ulong maxFileSize = 50000000)
        {
            IStorageFolder storageFolder = await LocalStorage.GetTempFolderAsync();
            System.Collections.Generic.IReadOnlyList<StorageFile> fileList = await storageFolder.GetFilesAsync();
            ulong totalFileSize = 0;
            //order the files
            List<StorageFile> files = fileList?.OrderBy(itm => itm.DateCreated).ToList();
            //get the total size of all files
            foreach (StorageFile file in files)
            {
                totalFileSize += (await file.GetBasicPropertiesAsync()).Size;
            }

            if (totalFileSize > maxFileSize)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (totalFileSize < maxFileSize)
                    {
                        break;
                    }
                    var storageFile = files[i];
                    totalFileSize -= (await storageFile.GetBasicPropertiesAsync()).Size;
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
        }
        #endregion
    }
}
