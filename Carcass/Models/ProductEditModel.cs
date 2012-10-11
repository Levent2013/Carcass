using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;

namespace Carcass.Models
{
    public class ProductEditModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Category { get; set; }

        public decimal Price { get; set; }
    }

    public class ProductEditModelConfiguration : ModelMetadataConfiguration<ProductEditModel>
    {
        public ProductEditModelConfiguration()
        {
            Configure(model => model.Id).Hide();

            Configure(model => model.Name)
                .Required()
                .MaximumLength(64);

            Configure(model => model.Category)
                .Required();

            Configure(model => model.Price)
                .FormatAsCurrency()
                .Required()
                .Range(10.00m, 1000.00m);
        }
    }
    
}