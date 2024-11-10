using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.CategoryDtos
{
    public class QueryCategoryDto : PagedRequest
    {
        public string? Keyword { get; set; }
    }
}
