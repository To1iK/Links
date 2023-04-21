using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Links;
using Links.Shared;
using System.Configuration;
using Links.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Links.Components
{
    public partial class NodeComponent
    {

        [Parameter]
        public NodeAccess2 na { get; set; }

        [Parameter]
        public Node node { get; set; }

        [Parameter]
        public EventCallback<Node> onEdit { get; set; }

        [Parameter]
        public EventCallback<Node> onSelect { get; set; }

        private string curentUser;

        private List<NodeAccess2> nodes;
       

        public bool isExpanded
        {
            get
            {
                showChilds();
                return node.isExpanded;
            }
            set
            {
                showChilds();
                node.isExpanded = value;
            }
        }

        async Task edit()
        {
            await onEdit.InvokeAsync(node);
        }

        async Task select()
        {         
            node.isSelected = true;   
            await onSelect.InvokeAsync(node);
        }

        async Task edit2(Node n)
        {
            await onEdit.InvokeAsync(n);
        }

        async Task select2(Node n)
        {
            await onSelect.InvokeAsync(n);
        }

        void add_node()
        {
            Node newNode = new Node();
           // newNode.ParentNode = node;
            newNode.NodeName = "New node";
            newNode.Code = DateTime.Now.Ticks.ToString("x");
            newNode.NodeTypeId = 1;
            newNode.IsActive = true;
            newNode.EditTime = DateTime.Now;
            newNode.Order = node.InverseParentNode.Count;
           // newNode.Responsible = 1;
            node.InverseParentNode.Add(newNode);

            NodeAccess na = new NodeAccess();
            na.Node = newNode;
            na.UserId = 1;
            na.AccessLevel = 10;
            newNode.NodeAccesses.Add(na);

            LinksContext.SaveChanges();

        }

        void remove_node()
        {
            node.EditTime = DateTime.Now;
            node.IsActive = false;
            LinksContext.SaveChanges();

        }

        protected override async Task OnInitializedAsync()
        {
           if(na is not null)
            {
                node = na.Node;
            }           

            if (node.isExpanded)
            {               
                nodes = GetCurentNodes(node.Id)
                            .ToList();

            }
        }

        void showChilds()
        {
            if (node.isExpanded)
            {
                //Nodes = GetCurentNodes(node.Id)
                //            .ToList();
                nodes = GetCurentNodes(node.Id)
                            .ToList();
            }
        }

        private List<NodeAccess2> GetCurentNodes(int nodeId)
        {
            var userId = LinksContext.curentUser.Id;
            var nal = LinksContext.NodeAccesses2.FromSqlRaw($@"
  select 
  NodeId as NodeId  
  ,max(na.accessLevel) as accessLevel
  from [NodeAccess] na
  join [Nodes] n on na.NodeId = n.id
  left join UserGroups ug on (ug.GroupId=na.GroupId and ug.UserId = {userId}) 
 where n.ParentNodeId = {nodeId} 
and n.isActive = 1
and na.accessLevel>=1
and (na.UserId = {userId} or ug.UserId = {userId})
 group by 
 NodeId
").Include(x => x.Node)
       //.Select(x => x.Node)
       .ToList();

                        return nal;

            //return LinksContext.Nodes
            //  .Where(x => x.ParentNodeId == node.Id & x.IsActive == true)
            //  .Include(x => x.NodeType)
            //    .ToList();
        }

        private string GetHref
        {
            get
            {
                if (node.NodeType.Code == "link")
                {
                    return node.NodeContent;
                }
                else
                {
                    return $"Viewer/{node.Id}";
                }
            }
        }

        private string GetTarget
        {
            get
            {
                if (node.NodeType.Code == "link")
                {
                    return "_blank";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}