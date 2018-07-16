using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseRent.Models
{
    public class Review
    {
        public int ID { get; set; }

        public int AdvertiseID { get; set; }

        public string Reviewer { get; set; }

        public int ReviewStar { get; set; }
    
    }
}
