using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WIRS20250324.AppWebMVC.Models;

public partial class Product
{
    public int Id { get; set; }

    [Display(Name = "Nombre Producto")]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string ProductName { get; set; } = null!;

    [Display(Name = "Descripcion Producto")]
    public string? Description { get; set; }

    [Display(Name = "Precio Producto")]
    [Required(ErrorMessage = "El precio es obligatorio.")]
    public decimal Price { get; set; }

    [Display(Name = "Precio Compra")]
    [Required(ErrorMessage = "El precio de Compra es obligatorio.")]
    public decimal PurchasePrice { get; set; }

    [Display(Name = "Bodega Producto")]
    [Required(ErrorMessage = "La Bodega es obligatoria.")]
    

    public int? WarehouseId { get; set; }

    [Display(Name = "ID Marca")]
    public int? BrandId { get; set; }

    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
