using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class Order
    {
        [Key,
          DatabaseGenerated(DatabaseGeneratedOption.Identity),
          Display(Name = "Id zamówienia"),
          Range(0, 9999)]
        public int orderId { get; set; }

        public int userId { get; set; }

        [Required(ErrorMessage = "Kraj jest wymagany")]
        public string country { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane")]
        public string city { get; set; }

        [Required(ErrorMessage = "Ulica, numer domu i mieszkania są wymagane")]
        public string street { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        public int zipCode {  get; set; }
        
        [Required(ErrorMessage = "Cena detaliczna jest wymagana")]
        public float totalPrice { get; set; }

        public int? discount { get; set; }

        public string state {  get; set; } = "W trakcie realizacji";

        [ForeignKey("userId")]
        public virtual User user { get; set; }

        public virtual ICollection<ProductOrder>? ProductOrder { get; set; }

        public Order() { }
    }
}
