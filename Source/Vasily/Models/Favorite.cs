using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vasily.Models
{
    public class Favorite
    {
        public Favorite()
        {
            UniqueId = Guid.NewGuid().ToString();
            Added = DateTime.Now;
        }

        public string UniqueId { get; set; }
        public string HostName { get; set; }
        public string PortNumber { get; set; }

        public DateTime Added { get; set; }
    }
}
