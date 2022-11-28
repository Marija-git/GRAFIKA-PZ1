using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJEKAT.Entities
{
    public class SwitchEntity : PowerEntity
    {
        public string Status { get; set; }

        public SwitchEntity()
        {

        }

        public SwitchEntity(long id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }

        public override string ToString()
        {
            return $"Switch:\nID: {Id}\nName: {Name}\nStatus: {Status}";
        }
    }
}
