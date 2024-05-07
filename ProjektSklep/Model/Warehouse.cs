using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class Warehouse
    {
        [Key,
         DatabaseGenerated(DatabaseGeneratedOption.Identity),
         Display(Name = "Id magazynu"),
         Range(0, 9999)]
        public int warehouseProductId { get; set; }

        [Required(ErrorMessage = "Produkt jest wymagany")]
        public int productId {  get; set; }

        [Required(ErrorMessage = "Stan faktyczny jest wymagany")]
        public int actualState { get; set; }

        [Required(ErrorMessage = "Stan magazynowy jest wymagany")]
        public int stockLevel {  get; set; }

        [ForeignKey("productId")]
        public virtual Product product { get; set; }
    }
}
