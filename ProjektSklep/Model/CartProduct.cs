using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjektSklep.Model
{
    public class CartProduct : INotifyPropertyChanged
    {
        public int id {  get; set; }
        public string name { get; set; }
        private int Count;
        public int count {
            get { return Count; }
            set { Set(ref Count, value); } }
        public float singlePrice {  get; set; }

        public CartProduct(int id,string name, int count, float singlePrice) {
            this.id = id;
            this.name = name;
            this.count = count;
            this.singlePrice = singlePrice;
        }

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
