using HaverDevProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.ViewModels
{
    [ModelMetadataType(typeof(NcrOperationMetaData))]
    public class NcrOperationDTO : Auditable
    {
        public string NcrNumber { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime NcrOpCompleteDate { get; set; }
        public DateTime NcrOpCreationDate { get; set; }
        public bool NcrStatus { get; set; } = true;
        public bool NcrOpStatusFlag { get; set; }
        public int NcrOpId { get; set; }
        public int NcrId { get; set; }
        public Ncr Ncr { get; set; }
        public int OpDispositionTypeId { get; set; }
        public OpDispositionType OpDispositionType { get; set; }
        public string NcrPurchasingDescription { get; set; }
        public bool Car { get; set; }
        public string CarNumber { get; set; }
        public bool FollowUp { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public int? FollowUpTypeId { get; set; }
        public FollowUpType FollowUpType { get; set; }
        public DateTime UpdateOp { get; set; }
        public string NcrOperationUserId { get; set; }
        public NcrEng NcrEng { get; set; }
        public string NcrOperationVideo { get; set; }
        public ICollection<OpDefectPhoto> OpDefectPhotos { get; set; } = new HashSet<OpDefectPhoto>();
    }
}
