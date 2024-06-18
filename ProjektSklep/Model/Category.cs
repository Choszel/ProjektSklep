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
    public class Category : INotifyPropertyChanged
    {
        [Key,
            DatabaseGenerated(DatabaseGeneratedOption.Identity),
            Display(Name = "Id kategorii"),
            Range(0, 9999)]
        public int categoryId { get; set; }

        [NotMapped]
        private string Name;

        [Required(ErrorMessage = "Nazwa jest obowiązkowa"),
            MaxLength(20, ErrorMessage = "Nazwa nie może być dłuższa niż 20 znaków")]
        public string name
        {
            get { return Name; }
            set { Set(ref Name, value); }
        }

        //placeholder constructor
        public Category(int categoryId,  string name)
        {
            this.categoryId = categoryId;
            this.name = name;
        }

        public Category() { }

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
