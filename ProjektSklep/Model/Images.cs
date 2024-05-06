using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class Images
{
        [Key,
            DatabaseGenerated(DatabaseGeneratedOption.Identity),
            Display(Name = "Id obrazu"),
            Range(0, 9999)]
        public int imageId { get; set; }
        public byte[] image { get; set; }
        public Images() { }
    }
}

