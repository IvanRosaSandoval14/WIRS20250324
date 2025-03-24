using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WIRS20250324.AppWebMVC.Models;

public partial class Brand
{
    public int Id { get; set; }

    [Display(Name = "Nombre Marca")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string BrandName { get; set; } = null!;

    [Display(Name = "Pais")]
    [Required(ErrorMessage = "El Pais es obligatorio.")]

    public string? Country { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
