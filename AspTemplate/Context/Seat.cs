using System;
using System.Collections.Generic;

namespace AspTemplate.Context;

public partial class Seat
{
    public int SeatId { get; set; }

    public int BusId { get; set; }

    public int Price { get; set; }

    public bool Deleted { get; set; }

    public string Name { get; set; }

    public virtual Bus Bus { get; set; }
}
