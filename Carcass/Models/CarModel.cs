using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;

namespace Carcass.Models
{
    public class CarModel
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int ProductionYear { get; set; }

        public decimal Price { get; set; }
    }
}