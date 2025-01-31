using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("ncrReInspectPhoto")]
public partial class NcrReInspectPhoto
{
    [Key]
    [Column("ncrReInspectPhotoId")]
    public int NcrReInspectPhotoId { get; set; }

    [Required]
    [Column("ncrReInspectPhotoContent")]
    public byte[] NcrReInspectPhotoContent { get; set; }

    [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters.")]
    [Display(Name = "File Name")]
    public string FileName { get; set; }

    [Required]
    [Column("ncrReInspectPhotoMimeType")]
    [StringLength(45)]
    [Unicode(false)]
    public string NcrReInspectPhotoMimeType { get; set; }

    [Column("ncrReInspectPhotoDescription")]
    [StringLength(300, ErrorMessage = "Only 300 characters for photo description.")]
    [DataType(DataType.MultilineText)]
    [Unicode(false)]
    public string NcrReInspectPhotoDescription { get; set; }

    [Column("ncrReInspectId")]
    public int NcrReInspectId { get; set; }

    [ForeignKey("NcrReInspectId")]
    [InverseProperty("NcrReInspectPhotos")]
    public NcrReInspect NcrReInspect { get; set; }

    public ReInspectFileContent ReInspectFileContent { get; set; } = new ReInspectFileContent();

}
