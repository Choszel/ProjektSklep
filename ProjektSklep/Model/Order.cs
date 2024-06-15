using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjektSklep.Model
{
    public class Order : INotifyPropertyChanged
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

        [Required(ErrorMessage = "Kod pocztowy jest wymagany"),
            MaxLength(6, ErrorMessage = "Kod pocztowy jest za długi"),
            RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Kod pocztowy powinien mieć format XX-XXX")]
        public string zipCode { get; set; }

        [Required(ErrorMessage = "Cena detaliczna jest wymagana")]
        public float totalPrice { get; set; }

        public string? discount { get; set; }

        private string State = "W trakcie realizacji";
        public string state
        {
            get { return State; }
            set { Set(ref State, value); }
        }
        public DateTime orderDate { get; set; }

        [ForeignKey("userId")]
        public virtual User user { get; set; }

        public virtual ICollection<ProductOrder>? ProductOrder { get; set; }

        [NotMapped]
        public string products { get; set; }

        public Order() { }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void Set<TValue>(ref TValue field, TValue newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<TValue>.Default.Equals(field, default(TValue)) || !field.Equals(newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
