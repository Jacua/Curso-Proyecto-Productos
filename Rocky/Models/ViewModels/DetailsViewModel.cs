using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            product = new Product();
        }
        public Product product { get; set; }

        public bool ExistsInCart { get; set; }
    }
}
