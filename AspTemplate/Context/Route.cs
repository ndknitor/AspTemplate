using System;
using System.Collections.Generic;

namespace NewTemplate.Context
{
    public partial class Route
    {
        public int RouteId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int BasePrice { get; set; }
        public bool Deleted { get; set; }
    }
}
