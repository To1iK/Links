using System.ComponentModel.DataAnnotations.Schema;

namespace Links.Models
{
    public partial class Node
    {
        [NotMapped]
        public virtual bool isSelected { get; set; }

        [NotMapped]
        public virtual bool isExpanded { get; set; }

    
        public Dictionary<int, Node> getParents()
        {
            Dictionary<int, Node> dn = new Dictionary<int, Node>();
            Node pn = ParentNode;
            while (pn != null)
            {
                dn.Add(pn.Id, pn);
                pn = pn.ParentNode;
            }
            return dn;
        }

        [NotMapped]
        public virtual string ParentNames {
            get {
                string parents = "";
                Node pn = ParentNode;
                while (pn != null)
                {
                    parents = pn.NodeName + "/" + parents;
                    pn = pn.ParentNode;
                }
                return parents;
            } 
        }

        public string getParentNames()
        {
            string parents = "";
            Node pn = ParentNode;
            while (pn != null)
            {
                parents = pn.NodeName + "/" + parents;
                pn = pn.ParentNode;
            }
            return parents;
        }
    }
}
