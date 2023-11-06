using System;
using System.Collections.Generic;

namespace NewTemplate.Context
{
    public partial class Bus
    {
        public Bus()
        {
            Seat = new HashSet<Seat>();
        }

        public int BusId { get; set; }
        public string Name { get; set; }
        public string LicensePlate { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Seat> Seat { get; set; }
    }
}
