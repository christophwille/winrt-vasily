using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

//
// https://metrostoragehelper.codeplex.com/
//
namespace Vasily.ThirdParty
{
    public enum StorageType
    {
        Roaming, Local, Temporary
    }

    public class StorageHelper<T>
    {
        private ApplicationData appData = Windows.Storage.ApplicationData.Current;
        private XmlSerializer serializer;
        private StorageType storageType;
        public StorageHelper(StorageType StorageType)
        {
            serializer = new XmlSerializer(typeof(T));
            storageType = StorageType;
        }

        public async void DeleteAsync(string FileName)
        {
            FileName = FileName + ".xml";
            try
            {
                StorageFolder folder = GetFolder(storageType);

                var file = await GetFileIfExistsAsync(folder, FileName);
                if (file != null)
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveAsync(T Obj, string FileName)
        {
            FileName = FileName + ".xml";
            try
            {
                if (Obj != null)
                {
                    StorageFile file = null;
                    StorageFolder folder = GetFolder(storageType);
                    file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                    using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (Stream outStream = Task.Run(() => writeStream.AsStreamForWrite()).Result)
                        {
                            serializer.Serialize(outStream, Obj);
                            outStream.Dispose();
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
        public async Task<T> LoadAsync(string FileName)
        {
            FileName = FileName + ".xml";
            try
            {
                StorageFile file = null;
                StorageFolder folder = GetFolder(storageType);
                file = await folder.GetFileAsync(FileName);
                using (IRandomAccessStream readStream = await file.OpenReadAsync())
                {
                    using (Stream inStream = Task.Run(() => readStream.AsStreamForRead()).Result)
                    {
                        if (inStream.Length == 0) return default(T);

                        T deserialized = (T)serializer.Deserialize(inStream);
                        inStream.Dispose();

                        return deserialized;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                //file not existing is perfectly valid so simply return the default 
                return default(T);
                //throw;
            }
            catch (Exception)
            {
                //Unable to load contents of file
                throw;
            }
        }

        private StorageFolder GetFolder(StorageType storageType)
        {
            StorageFolder folder;
            switch (storageType)
            {
                case StorageType.Roaming:
                    folder = appData.RoamingFolder;
                    break;
                case StorageType.Local:
                    folder = appData.LocalFolder;
                    break;
                case StorageType.Temporary:
                    folder = appData.TemporaryFolder;
                    break;
                default:
                    throw new Exception(String.Format("Unknown StorageType: {0}", storageType));
            }
            return folder;
        }

        private async Task<StorageFile> GetFileIfExistsAsync(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFileAsync(fileName);

            }
            catch
            {
                return null;
            }
        }

    }
}
