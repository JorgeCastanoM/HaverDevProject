using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public class ProcFileContent
    {
        [Key, ForeignKey("ProcDefectPhoto")]
        public int ProcFileContentID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }


        public ProcDefectPhoto ProcDefectPhoto { get; set; }

        //public ICollection<ProcDefectPhoto> ProcDefectPhoto { get; set; } = new HashSet<ProcDefectPhoto>();
    }
}

