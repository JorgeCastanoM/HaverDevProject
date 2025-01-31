using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("engDispositionType")]
public partial class EngDispositionType
{
    [Column("engDispositionTypeId")]
    public int EngDispositionTypeId { get; set; }

    [Display(Name = "Review by HBC Engineering")]
    [Required(ErrorMessage = "You must provide the Disposition Type.")]
    [Column("engDispositionTypeName")]
    [StringLength(45)]
    [Unicode(false)]
    public string EngDispositionTypeName { get; set; }
    public virtual ICollection<NcrEng> NcrEngs { get; set; } = new List<NcrEng>();
}
