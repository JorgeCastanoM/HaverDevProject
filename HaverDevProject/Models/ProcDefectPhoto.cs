using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("procDefectPhoto")]
public class ProcDefectPhoto
{
    [Key]
    [Column("procDefectPhotoId")]
    public int ProcDefectPhotoId { get; set; }

    [Required]
    [Column("procDefectPhotoContent")]
    public byte[] ProcDefectPhotoContent { get; set; }

    [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters.")]
    [Display(Name = "File Name")]
    public string FileName { get; set; }

    [Required]
    [Column("procDefectPhotoType")]
    [StringLength(45)]
    [Unicode(false)]
    public string ProcDefectPhotoMimeType { get; set; }

    [Column("procDefectPhotoDescription")]
    [StringLength(300, ErrorMessage = "Only 300 characters for photo description.")]
    [DataType(DataType.MultilineText)]
    [Unicode(false)]
    public string ProcDefectPhotoDescription { get; set; }

    [Column("ncrProcurementId")]
    public int NcrProcurementId { get; set; }

    [ForeignKey("NcrProcurementId")]
    [InverseProperty("ProcDefectPhotos")]
    public virtual NcrProcurement NcrProcurement { get; set; }

    public ProcFileContent ProcFileContent { get; set; } = new ProcFileContent();
    //public ICollection<EngFileContent> EngFileContent { get; set; } = new HashSet<EngFileContent>();
}


