using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class Product
    {
        [Key,
           DatabaseGenerated(DatabaseGeneratedOption.Identity),
           Display(Name = "Id produktu"),
           Range(0, 9999)]
        public int productId { get; set; }

        [Required(ErrorMessage = "Nazwa jest obowiązkowa"),
                    MaxLength(30, ErrorMessage = "Nazwa nie może być dłuższa niż 30 znaków")]
        public string name { get; set; }
        
        public float price { get; set; }

        public string description { get; set; }

        public int categoryId{ get; set; }

        [ForeignKey("categoryId"),
            Display(Name = "Kategoria")]
        public virtual Category category { get; set; }


        //placeholder constructor
        public Product(string name, int categoryId)
        {
            this.name = name;
            this.categoryId = categoryId;
        }

        public Product() { }

        public virtual ICollection<ProductOrder>? ProductOrder { get; set; }
    }
}
