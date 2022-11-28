using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Classes
{
    public class Column : ICloneable
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public bool Taken { get; set; }

        public Column(int id, double value)
        {
            Id = id;
            Value = value;
            Taken = false;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
