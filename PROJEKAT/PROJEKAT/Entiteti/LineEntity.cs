using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Entities
{
    public class LineEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long FirstEnd { get; set; }
        public long SecondEnd { get; set; }

        public LineEntity()
        {

        }

        public LineEntity(long id, string name, long firstEnd, long secondEnd)
        {
            Id = id;
            Name = name;
            FirstEnd = firstEnd;
            SecondEnd = secondEnd;
        }

        public override string ToString()
        {
            return $"Line:\nID: {Id}\nName: {Name}";
        }
    }
}
