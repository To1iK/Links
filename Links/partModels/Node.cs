using System.ComponentModel.DataAnnotations.Schema;

namespace Links.Models
{
    public partial class Node
    {
        [NotMapped]
        public virtual bool isSelected { get; set; }

        [NotMapped]
        public virtual bool isExpanded { get; set; }
    }
}
