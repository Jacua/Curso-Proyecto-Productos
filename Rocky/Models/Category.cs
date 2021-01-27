using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="El campo {0}, es requerido")]
        public string Name { get; set; }

        [Display(Name = "Display Order")]

        [Range(1,int.MaxValue,ErrorMessage ="El campo {0}, debe ser mayor o igual a : {1}")]
        public int DisplayOrder { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
