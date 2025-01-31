using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("opDispositionType")]
public partial class OpDispositionType
{
    public int OpDispositionTypeId { get; set; }

    [Display(Name = "Purchasing's Preliminary Decision")]
    [Required(ErrorMessage = "You must provide the Disposition Type.")]
    [StringLength(45, ErrorMessage = "The Disposition Type cannot be more than 45 characters.")]
    public string OpDispositionTypeName { get; set; }

    public virtual ICollection<NcrOperation> NcrOperations { get; set; } = new List<NcrOperation>();
}
