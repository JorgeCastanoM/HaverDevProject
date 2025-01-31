using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("opDefectPhoto")]
public class OpDefectPhoto
{
    [Key]
    [Column("opDefectPhotoId")]
    public int OpDefectPhotoId { get; set; }

    [Required]
    [Column("opDefectPhotoContent")]
    public byte[] OpDefectPhotoContent { get; set; }

    [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters.")]
    [Display(Name = "File Name")]
    public string FileName { get; set; }

    [Required]
    [Column("opDefectPhotoMimeType")]
    [StringLength(45)]
    [Unicode(false)]
    public string OpDefectPhotoMimeType { get; set; }

    [Column("opDefectPhotoDescription")]
    [StringLength(300, ErrorMessage = "Only 300 characters for photo description.")]
    [DataType(DataType.MultilineText)]
    [Unicode(false)]
    public string OpDefectPhotoDescription { get; set; }

    [Column("ncrOpId")]
    public int NcrOpId { get; set; }

    [ForeignKey("NcrOpId")]
    [InverseProperty("OpDefectPhotos")]
    public virtual NcrOperation NcrOperation { get; set; }

    public OpFileContent OpFileContent { get; set; } = new OpFileContent();

}