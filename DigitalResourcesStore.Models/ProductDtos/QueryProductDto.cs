﻿using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Models.ProductDtos
{
    public class QueryProductDto : PagedRequest
    {
        public string? Keyword { get; set; }
    }
}