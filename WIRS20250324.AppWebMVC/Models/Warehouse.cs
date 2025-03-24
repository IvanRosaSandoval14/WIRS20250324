using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WIRS20250324.AppWebMVC.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    [Display(Name = "Nombre Bodega")]
    [Required(ErrorMessage = "El nombre de la Bodega es obligatorio.")]
    public string WarehouseName { get; set; } = null!;


    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
