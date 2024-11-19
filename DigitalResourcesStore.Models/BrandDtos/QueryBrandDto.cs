using DigitalResourcesStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.BrandDtos
{
    public class QueryBrandDto : PagedRequest
    {
        public string? Keyword { get; set; }
    }
}
