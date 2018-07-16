using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseRent.Models
{
    public class Image
    {
        public int ID { get; set; }

        public int AdvertiseID { get; set; }

        public byte[] FlatImage { get; set; }
    }
}
