using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("followUpType")]
public partial class FollowUpType
{
    public int FollowUpTypeId { get; set; }

    [Display(Name = "Follow Up Type")]
    [StringLength(45)]
    public string FollowUpTypeName { get; set; }
    public ICollection<NcrOperation> NcrOperations { get; set; } = new HashSet<NcrOperation>();
}
