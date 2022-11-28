using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Classes
{
    public class RowColumn
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public RowColumn Parent { get; set; }

        public RowColumn(int row, int column, RowColumn parent = null)
        {
            Row = row;
            Column = column;
            Parent = parent;
        }

        public RowColumn()
        {

        }
    }
}
