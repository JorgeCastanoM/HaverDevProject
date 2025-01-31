using HaverDevProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HaverDevProject.ViewModels
{
    public class NcrOperationMetaData
    {
        [Key]
        [Column("ncrOpId")]
        public int NcrOpId { get; set; }

        [Column("ncrId")]
        [Display(Name = "NCR")]
        public int NcrId { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime NcrEngCompleteDate { get; set; }

        [Column("ncrOpCreationDate")]
        [Display(Name = "NCR Creation Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime NcrOpCreationDate { get; set; }

        [Column("opDispositionTypeId")]
        [Display(Name = "Purchasing's Preliminary Decision")]
        [Required(ErrorMessage = "You must provide the Disposition Type.")]
        public int OpDispositionTypeId { get; set; }

        [ForeignKey("OpDispositionTypeId")]
        [InverseProperty("NcrPurchasings")]
        [Display(Name = "Purchasing's Preliminary Decision")]
        public OpDispositionType OpDispositionType { get; set; }

        [Column("ncrPurchasingDescription")]
        [StringLength(300)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "You must include a description")]
        public string NcrPurchasingDescription { get; set; }

        [Column("ncrCar")]
        [Display(Name = "Was a CAR raised")]
        [Required(ErrorMessage = "You select an option if the CAR was raised.")]
        public bool Car { get; set; }

        [Column("ncrCarNumber")]
        [Display(Name = "If \"Yes\" indicate CAR #")]
        public string CarNumber { get; set; }

        [Display(Name = "Follow-up Required?")]
        public bool FollowUp { get; set; }

        [Column("ncrExpectedDate")]
        [Display(Name = "Expected Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpectedDate { get; set; }

        [Column("ncrFollowUpType")]
        [Display(Name = "If \"Yes\" indicate type & expected date")]
        public int? FollowUpTypeId { get; set; }

        [Display(Name = "If \"Yes\" indicate type & expected date" )]
        public FollowUpType FollowUpType { get; set; }

        [Column("ncrUpdateOp")]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime UpdateOp { get; set; }

        [Column("ncrOperationUserId")]
        [Display(Name = "Operation Manager")]
        public int NcrPurchasingUserId { get; set; }

        [Column("ncrOperationVideo")]
        [Display(Name = "Video Link")]
        public string NcrOperationVideo { get; set; }


        [Display(Name = "Operations Photos")]
        public ICollection<OpDefectPhoto> OpDefectPhotos { get; set; } = new HashSet<OpDefectPhoto>();
    }
}
