using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("supplier")]
public partial class Supplier
{
    [Key]
    [Column("supplierId")]
    public int SupplierId { get; set; }

    [NotMapped]
    [Display(Name = "Supplier (incl Code)")]
    public string Summary => $"{SupplierCode} - {SupplierName}";

    [Display(Name = "Code")]
    [Required(ErrorMessage = "You must provide the Supplier Code.")]
    [Column("supplierCode")]
    [StringLength(45, ErrorMessage = "The Supplier Code cannot be more than 45 characters.")]
    [Unicode(false)]
    public string SupplierCode { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "You must provide the Supplier Name.")]
    [Column("supplierName")]
    [StringLength(45, ErrorMessage = "The Supplier Name cannot be more than 45 characters.")]
    [Unicode(false)]
    public string SupplierName { get; set; } 

    [Display(Name = "Contact Name")]
    [Column("supplierContactName")]
    [StringLength(90, ErrorMessage = "The Contact Name cannot be more than 90 characters.")]
    [Unicode(false)]
    public string SupplierContactName { get; set; }

    [Display(Name = "Email")]
    [Column("supplierEmail")]
    [StringLength(45, ErrorMessage = "The Supplier Email cannot be more than 45 characters.")]
    [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Please provide a valid email.")]
    [Unicode(false)]
    public string SupplierEmail { get; set; }

    [Display(Name = "Status")]
    [Column("supplierStatus")]
    public bool SupplierStatus { get; set; } = true; //Default value is true (active)
    
    [InverseProperty("Supplier")]
    public ICollection<NcrQa> NcrQas { get; set; } = new HashSet<NcrQa>();
}
