using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HaverDevProject.Models
{
    public class FileContent
    {
        [Key, ForeignKey("ItemDefectPhoto")]
        public int FileContentID { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        public ItemDefectPhoto ItemDefectPhoto { get; set; }
        
    }
}
