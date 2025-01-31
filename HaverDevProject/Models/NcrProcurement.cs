using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaverDevProject.Models
{
    [ModelMetadataType(typeof(NcrProcurementMetaData))]
    public class NcrProcurement : Auditable
    {
        public int NcrProcurementId { get; set; }
        public bool NcrProcSupplierReturnReq { get; set; }
        public DateTime? NcrProcExpectedDate { get; set; }
        public bool NcrProcDisposedAllowed { get; set; }
        public bool NcrProcSAPReturnCompleted { get; set; }
        public bool NcrProcCreditExpected { get; set; }
        public bool NcrProcSupplierBilled { get; set; }
        public decimal NcrProcRejectedValue { get; set; }
        public bool NcrProcFlagStatus { get; set; }
        public string NcrProcUserId { get; set; }
        public DateTime NcrProcCreated { get; set; }
        public DateTime NcrProcCompleteDate { get; set; }
        //public int SupplierReturnId { get; set; }
        //public SupplierReturn SupplierReturn { get; set; }
        public string SupplierReturnMANum { get; set; }
        public string SupplierReturnName { get; set; }
        public string SupplierReturnAccount { get; set; }
        public int NcrId { get; set; }
        public virtual Ncr Ncr { get; set; }
        public string NcrProcDefectVideo { get; set; }
        public ICollection<ProcDefectPhoto> ProcDefectPhotos { get; set; } = new HashSet<ProcDefectPhoto>();

    }
}
