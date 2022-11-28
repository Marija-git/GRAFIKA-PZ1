using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Entities
{
    public class SubstationEntity : PowerEntity
    {
        public SubstationEntity()
        {

        }

        public SubstationEntity(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return $"Substation:\nID: {Id}\nName: {Name}";
        }
    }
}
