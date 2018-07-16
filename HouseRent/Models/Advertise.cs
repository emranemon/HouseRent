using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace HouseRent.Models
{
    public class Advertise
    {
        public int ID { get; set; }

        public string Heading { get; set; }

        [Display(Name = "Contact Mail")]
        public string UserMail { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Post Time")]
        public DateTime PostTime { get; set; }

        [Display(Name = "From")]
        [DataType(DataType.Date)]
        public DateTime RentDate { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Youtube Link")]
        public string YoutubeLink { get; set; }

        [Required]
        [Display(Name = "House Size")]
        public Decimal FlatSize { get; set; }

        [Required]
        [Display(Name = "House Type")]
        public string FlatType { get; set; } //apartment room duplex

        [Required]
        [Display(Name = "Available for")]
        public string Category { get; set; } //family or bachelor

        [Required]
        public Decimal Rent { get; set; }

        [Required]
        [Display(Name = "House Details")]
        public string FlatDetails { get; set; }

        [Required]
        [Display(Name = "Utilities Bill")]
        public Decimal UtilitiesBill { get; set; }

        [Required]
        [Display(Name = "Others Bill")]
        public Decimal OtherBill { get; set; }


        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }


    }
}
