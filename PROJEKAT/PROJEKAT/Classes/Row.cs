using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Classes
{
    public class Row
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public List<Column> Columns { get; set; }

        public Row(int id, double value, List<Column> columns)
        {
            Id = id;
            Value = value;
            Columns = columns;
        }
    }
}
