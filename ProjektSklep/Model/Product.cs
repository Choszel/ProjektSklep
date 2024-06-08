using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjektSklep.Model
{
    public class Product : INotifyPropertyChanged
    {
        [Key,
           DatabaseGenerated(DatabaseGeneratedOption.Identity),
           Display(Name = "Id produktu"),
           Range(0, 9999)]
        public int productId { get; set; }
        [NotMapped]
        private string Name;

        [Required(ErrorMessage = "Nazwa jest obowiązkowa"),
                    MaxLength(30, ErrorMessage = "Nazwa nie może być dłuższa niż 30 znaków")]
        public string name
        {
            get { return Name; }
            set { Set(ref Name, value); }
        }
        [NotMapped]
        private float Price;
        public float price
        {
            get { return Price; }
            set { Set(ref Price, value); }
        }

        [NotMapped]
        private string Description;
        public string description
        {
            get { return Description; }
            set { Set(ref Description, value); }
        }

        public int categoryId{ get; set; }

        [ForeignKey("categoryId"),
            Display(Name = "Kategoria")]

        [NotMapped]
        private Category Category;
        public virtual Category category
        {
            get { return Category; }
            set { Set(ref Category, value); }
        }

        public int imageId {  get; set; }

        [ForeignKey("imageId"),
        Display(Name = "Obrazek")]
        public virtual Images image { get; set; }

        [NotMapped]
        public BitmapImage? BitmapImage;

        [NotMapped]
        public virtual BitmapImage? bitmapImage
        {
            get { return BitmapImage; }
            set { Set(ref BitmapImage, value); }
        }

        //placeholder constructor
        public Product(string name, int categoryId)
        {
            this.name = name;
            this.categoryId = categoryId;
        }

        public Product() { }

        public virtual ICollection<ProductOrder>? ProductOrder { get; set; }

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
