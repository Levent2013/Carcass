using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;

namespace Carcass.Models.Configurations
{
    public class CarModelConfiguration : ModelMetadataConfiguration<CarModel>
    {
        public CarModelConfiguration()
        {
            Configure(model => model.Id).Hide();

            Configure(model => model.Brand)
                .Required()
                .MaximumLength(64);

            Configure(model => model.Model)
                .Required()
                .MaximumLength(64);

            Configure(model => model.ProductionYear)
                .Required()
                .Range(1900, 2100);

            Configure(model => model.Price)
                .FormatAsCurrency()
                .Required()
                .Range(100m, 1000000m);
        }
    }
}