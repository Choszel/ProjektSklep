using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    internal class UserType
    {
        private static UserType _instance;
        public static UserType Instance => _instance ??= new UserType();

        public int numericType { get; set; } = -1;
    }
}
