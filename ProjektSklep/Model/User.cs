using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class User
    {
        [Key,
          DatabaseGenerated(DatabaseGeneratedOption.Identity),
          Display(Name = "Id użytkownika"),
          Range(0, 9999)]
        public int userId { get; set; }

        [Required(ErrorMessage ="Nazwa jest obowiązkowa.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Login jest obowiązkowy.")]
        public string login { get; set; }

        [Required(ErrorMessage = "Hasło jest obowiązkowe.")]
        public string password { get; set; }

        [Required(ErrorMessage = "E-mail jest obowiązkoway.")]
        public string email { get; set; }

        [Required(ErrorMessage ="Typ użytkownika jest obowiązkowy.")]
        public int type { get; set; } = 0;

        public User() { }

        public User(string name, string login, string password, string email, int type)
        {
            this.name = name;
            this.login = login;
            this.password = password;
            this.email = email;
            this.type = type;
        }
    }
}
