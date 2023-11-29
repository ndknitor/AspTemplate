using System;
using System.Collections.Generic;

namespace AspTemplate.Context;

public partial class Trip
{
    public int TripId { get; set; }

    public int RouteId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int BusId { get; set; }

    public virtual Route Route { get; set; }

    public virtual ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
}
