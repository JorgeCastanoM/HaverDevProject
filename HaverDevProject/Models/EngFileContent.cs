using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public class EngFileContent
    {
        [Key, ForeignKey("EngDefectPhoto")]
        public int EngFileContentID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }
        
        public EngDefectPhoto EngDefectPhoto { get; set;}        
    }
}
