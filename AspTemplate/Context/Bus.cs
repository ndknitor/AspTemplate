using System;
using System.Collections.Generic;

namespace AspTemplate.Context;

public partial class Bus
{
    public int BusId { get; set; }

    public string Name { get; set; }

    public string LicensePlate { get; set; }

    public bool Deleted { get; set; }

    public virtual ICollection<Seat> Seat { get; set; } = new List<Seat>();
}
