using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("item")]
public class Item
{
    [Key]
    [Column("itemId")]
    public int ItemId { get; set; }
    public string Summary => $"{ItemNumber} - {ItemName}";

    [Column("itemNumber")]
    [Display(Name = "SAP No.")]
    [Required(ErrorMessage = "You must provide an item Number")]
    public int ItemNumber { get; set; }

    [Required(ErrorMessage = "You must provide the Item Name.")]
    [Display(Name = "Name")]
    [Column("itemName")]
    [StringLength(45, ErrorMessage = "The Item Name cannot be more than 45 characters long.")]
    [Unicode(false)]
    public string ItemName { get; set; }    

    [InverseProperty("Item")]
    public ICollection<NcrQa> NcrQas { get; set; } = new HashSet<NcrQa>();
}
