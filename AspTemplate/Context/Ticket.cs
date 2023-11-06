using System;
using System.Collections.Generic;

namespace NewTemplate.Context
{
    public partial class Ticket
    {
        public int TicketId { get; set; }
        public int Status { get; set; }
        public int Price { get; set; }
        public int TripId { get; set; }
        public int SeatId { get; set; }
        public DateTime BookedDate { get; set; }
        public int UserId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
