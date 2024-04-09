using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class Category
    {
        [Key,
            DatabaseGenerated(DatabaseGeneratedOption.Identity),
            Display(Name = "Id kategorii"),
            Range(0, 9999)]
        public int categoryId { get; set; }
        [Required(ErrorMessage ="Nazwa jest obowiązkowa"),
            MaxLength(20, ErrorMessage ="Nazwa nie może być dłuższa niż 20 znaków")]
        public string name { get; set; }
        
        //placeholder constructor
        public Category(int categoryId,  string name)
        {
            this.categoryId = categoryId;
            this.name = name;
        }
    }
}
