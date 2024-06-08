using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class ProductOrder
    {
        [Key,
           DatabaseGenerated(DatabaseGeneratedOption.Identity),
           Display(Name = "Id zamówionego produktu"),
           Range(0, 9999999999)]
        public int productOrderId { get; set; }

        public int orderId { get; set; }
        public int productId { get; set; }
        public int count { get; set; }

        [ForeignKey("orderId")]
        public virtual Order order { get; set; }

        [ForeignKey("productId")]
        public virtual Product product { get; set; }

        public ProductOrder() { }
    }
}
