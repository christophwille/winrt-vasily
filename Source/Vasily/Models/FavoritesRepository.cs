using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasily.ThirdParty;

namespace Vasily.Models
{
    public class FavoritesRepository
    {
        private const string StorageFileName = "favorites";

        public async Task<List<Favorite>> LoadAsync()
        {
            var objectStorageHelper = new StorageHelper<List<Favorite>>(StorageType.Local);
            List<Favorite> favorites = await objectStorageHelper.LoadAsync(StorageFileName);

            if (null == favorites)
                favorites = new List<Favorite>();

            return favorites;
        }

        public async Task<bool> SaveAsync(List<Favorite> favorites)
        {
            var objectStorageHelper = new StorageHelper<List<Favorite>>(StorageType.Local);
            bool result = await objectStorageHelper.SaveAsync(favorites, StorageFileName);
            return result;
        }
    }
}
