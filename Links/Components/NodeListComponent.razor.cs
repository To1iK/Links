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
using System.Data;

namespace Links.Components
{
    public partial class NodeListComponent
    {

        [Parameter]
        public int nodeId { get; set; } = 0;

        //[Parameter]
        //public static int MainNode { get; set; } = 0;

       // public Node selectedNode { get; set; }

        private string curentUser;

        private List<NodeAccess2> Nodes;

        private Links.Components.Modal Modal1 { get; set; }

        private Links.Components.NodeEditComponent NEC { get; set; }

        void select(Node n)
        {     
            if(LinksContext.selectedNode is not null && LinksContext.selectedNode != n)
            {
                LinksContext.selectedNode.isSelected = false;           
            }
            LinksContext.selectedNode = n;
            Debug.WriteLine(n.NodeName);
        }

        void openModal(Node n)
        {
            StateHasChanged();
            Modal1.Open();
        }

        void ModalClose()
        {
            LinksContext.selectedNode.EditTime = DateTime.Now;
            LinksContext.SaveChanges();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            Nodes = GetCurentNodes(LinksContext.MainNode.Id)
                      .ToList();  
            
        }

        protected override async Task OnParametersSetAsync()
        {
            Nodes = GetCurentNodes(LinksContext.MainNode.Id)
                      .ToList();


        }

        private List<NodeAccess2> GetCurentNodes(int nodeId)
        {
            var userId = LinksContext.curentUser.Id;

            Node? userNode = LinksContext.Nodes.Find(LinksContext.curentUser.NodeId);
            Node? curentNode = LinksContext.Nodes.Find(1036);

            List<NodeAccess2> ln = new List<NodeAccess2>();
           
            NodeAccess2 na1 = new NodeAccess2();
            na1.AccessLevel = 100;
            NodeAccess2 na2 = new NodeAccess2();
            na2.AccessLevel = 100;

            na1.Node = userNode;
            na2.Node = curentNode;

            ln.Add(na2);
            ln.Add(na1);
        

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

           // return nal;

            return ln;
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