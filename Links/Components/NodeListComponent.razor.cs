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
using Links.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Links.Components
{
    public partial class NodeListComponent
    {

        [Parameter]
        public int nodeId { get; set; } = 0;

        //[Parameter]
        //public static int MainNode { get; set; } = -1;

        public Node selectedNode { get; set; }

        private string curentUser;

        private List<NodeAccess2> Nodes;

        private Links.Components.Modal Modal1 { get; set; }

        private Links.Components.NodeEditComponent NEC { get; set; }

        void select(Node n)
        {     
            if(selectedNode is not null && selectedNode != n)
            {
            selectedNode.isSelected = false;           
            }
            selectedNode = n;
            Debug.WriteLine(n.NodeName);
        }

        void openModal(Node n)
        {
            StateHasChanged();
            Modal1.Open();
        }

        void ModalClose()
        {
            selectedNode.EditTime = DateTime.Now;
            LinksContext.SaveChanges();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            Nodes = GetCurentNodes(nodeId)
                      .ToList();  
            
        }

        protected override async Task OnParametersSetAsync()
        {
            Nodes = GetCurentNodes(nodeId)
                      .ToList();

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

            //var Nodes2 = LinksContext.Nodes
            //             .Where(x=>x.ParentNodeId==0 & x.IsActive == true)
            //             .Include(x=>x.NodeType)
            //    .ToList();

            //return Nodes2;

        }
    }
}

//var nal = LinksContext.NodeAccesses2.FromSqlRaw($@"
//  select max(na.id) as id
//  ,NodeId as NodeId
//  ,max(na.userId) as userId
//  ,max(na.groupId) as groupId
//  ,max(na.accessLevel) as accessLevel
//  from [NodeAccess] na
//  join [Nodes] n on na.NodeId = n.id
//  left join UserGroups ug on (ug.GroupId=na.GroupId and ug.UserId = {userId}) 
// where n.ParentNodeId = {nodeId} 
//and n.isActive = 1
//and (na.UserId = {userId} or ug.UserId = {userId})
// group by 
// NodeId
//").Include(x => x.Node)