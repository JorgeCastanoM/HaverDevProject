using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HaverDevProject.Models;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.ViewModels;

public class NcrEngDTOMetaData
{

    [Key]
    [Column("ncrEngId")]
    public int NcrEngId { get; set; }

    [Display(Name = "NCR No.")]
    [Required(ErrorMessage = "You must provide the NCR Number.")]
    [Column("ncrNumber")]
    [StringLength(10, ErrorMessage = "The NCR Number cannot be more than 10 characters long.")]
    [Unicode(false)]
    public string NcrNumber { get; set; }

    [Display(Name = "Status")]
    [Column("ncrStatus")]
    public bool NcrStatus { get; set; }

    [Display(Name = "Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime NcrEngCompleteDate { get; set; }

    [Display(Name = "Date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime NcrEngCreationDate { get; set; }

    [Display(Name = "Customer Notification")]
    [Column("ncrEngCustomerNotification")]
    public bool NcrEngCustomerNotification { get; set; } = false;

    [Display(Name = "Disposition")]
    [Column("ncrEngDispositionDescription")]
    [StringLength(300, ErrorMessage = "Only 300 characters for disposition description.")]
    [DataType(DataType.MultilineText)]
    [Unicode(false)]
    [Required(ErrorMessage = "You must include a disposition description")]
    public string NcrEngDispositionDescription { get; set; }

    public bool NcrEngStatusFlag { get; set; }

    public NcrPhase NcrPhase { get; set; }

    [Display(Name = "Engineering")]
    [Column("ncrEngUserId")]
    public int NcrEngUserId { get; set; }

    [Display(Name = "Disposition Type")]
    [Column("engDispositionTypeId")]
    public int EngDispositionTypeId { get; set; }

    [Display(Name = "NCR")]
    [Column("ncrId")]
    public int NcrId { get; set; }

    [Key]
    [Column("drawingId")]
    public int DrawingId { get; set; }

    [Display(Name = "Check the box if drawing requires updating")]
    [Column("DrawingRequireUpdating")]
    public bool DrawingRequireUpdating { get; set; } = false;

    [Display(Name = "Original Rev. Number")]
    [Column("drawingOriginalRevNumber")]
    public int DrawingOriginalRevNumber { get; set; }

    [Display(Name = "Updated Rev. Number")]
    [Column("drawingUpdatedRevNumber")]
    public int DrawingUpdatedRevNumber { get; set; }


    [Display(Name = "Revision Date")]
    [Column("drawingRevDate", TypeName = "date")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DrawingRevDate { get; set; } = DateTime.Now;

    [Display(Name = "Drawing User ID")]
    [Column("drawingUserId")]
    public int DrawingUserId { get; set; }

    [Display(Name = "Video Link")]
    [StringLength(100, ErrorMessage = "Video link cannot be more than 100 characters.")]
    public string NcrEngDefectVideo { get; set; }

    [Display(Name = "Engineer Photos")]
    public ICollection<EngDefectPhoto> EngDefectPhotos { get; set; } = new HashSet<EngDefectPhoto>();
}
