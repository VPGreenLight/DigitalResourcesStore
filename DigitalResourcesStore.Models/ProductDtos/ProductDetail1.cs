using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.ProductDtos
{
    public class ProductDetail1
    {
        public int Id { get; set; }

        public string? Serial { get; set; }

        public string? Code { get; set; }

        public int? ProductId { get; set; }

        public bool IsDelete { get; set; }
        
    }
}
