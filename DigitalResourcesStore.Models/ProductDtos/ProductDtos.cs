using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.ProductDtos
{
    public class ProductDtos
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Image { get; set; }

        public DateTime? Expiry { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? Quantity { get; set; }
        public string? Category { get; set; }

        public string? Brand { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool? IsDelete { get; set; }

        public string? DeletedBy { get; set; }

    }
}
