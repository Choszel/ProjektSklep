using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjektSklep.Model
{
    public class Warehouse : INotifyPropertyChanged
    {
        [Key,
         DatabaseGenerated(DatabaseGeneratedOption.Identity),
         Display(Name = "Id magazynu"),
         Range(0, 9999)]
        public int warehouseProductId { get; set; }

        [Required(ErrorMessage = "Produkt jest wymagany")]
        public int productId {  get; set; }

        [NotMapped]
        private int ActualState;

        [Required(ErrorMessage = "Stan faktyczny jest wymagany")]
        public int actualState
        {
            get { return ActualState; }
            set { Set(ref ActualState, value); }
        }
        [NotMapped]
        private int StockLevel;

        [Required(ErrorMessage = "Stan magazynowy jest wymagany")]
        public int stockLevel
        {
            get { return StockLevel; }
            set { Set(ref StockLevel, value); }
        }

        [ForeignKey("productId")]
        public virtual Product product { get; set; }

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
