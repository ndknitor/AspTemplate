using System;
using System.Collections.Generic;

namespace NewTemplate.Context
{
    public partial class Trip
    {
        public int TripId { get; set; }
        public int RouteId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BusId { get; set; }
    }
}
