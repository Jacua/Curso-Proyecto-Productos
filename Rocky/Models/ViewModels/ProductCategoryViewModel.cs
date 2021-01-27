using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class ProductCategoryViewModel
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> categorySelectList { get; set; }
    }
}
