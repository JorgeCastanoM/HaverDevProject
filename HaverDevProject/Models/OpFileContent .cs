using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public class OpFileContent
    {
        [Key, ForeignKey("OpDefectPhoto")]
        public int FileContentID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        public OpDefectPhoto OpDefectPhoto { get; set; }

    }
}
