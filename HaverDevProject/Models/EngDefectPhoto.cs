using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models;

[Table("engDefectPhoto")]
public class EngDefectPhoto
{
    [Key]
    [Column("engDefectPhotoId")]
    public int EngDefectPhotoId { get; set; }

    [Required]
    [Column("engDefectPhotoContent")]
    public byte[] EngDefectPhotoContent { get; set; }

    [StringLength(255, ErrorMessage = "The name of the file cannot be more than 255 characters.")]
    [Display(Name = "File Name")]
    public string FileName { get; set; }

    [Required]
    [Column("engDefectPhotoMimeType")]
    [StringLength(45)]
    [Unicode(false)]
    public string EngDefectPhotoMimeType { get; set; }

    [Column("engDefectPhotoDescription")]
    [StringLength(300, ErrorMessage = "Only 300 characters for photo description.")]
    [DataType(DataType.MultilineText)]
    [Unicode(false)]
    public string EngDefectPhotoDescription { get; set; }

    [Column("ncrEngId")]
    public int NcrEngId { get; set; }

    [ForeignKey("NcrEngId")]
    [InverseProperty("EngDefectPhotos")]
    public virtual NcrEng NcrEng { get; set; }

    public EngFileContent EngFileContent { get; set; } = new EngFileContent();
    //public ICollection<EngFileContent> EngFileContent { get; set; } = new HashSet<EngFileContent>();
}
