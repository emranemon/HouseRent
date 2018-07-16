using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseRent.Models
{
    public class Comment
    {
        public int ID { get; set; }

        public int AdvertiseID { get; set; }

        public string Commenter { get; set; }

        public DateTime CommentTime { get; set; }

        public bool Anonymous { get; set; }

        public string CommentText { get; set; }

    }
}
