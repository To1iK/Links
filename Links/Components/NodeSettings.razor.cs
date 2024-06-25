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
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Data.SqlClient.Server;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Links.Components
{
    public partial class NodeSettings
    {
        [Parameter]
        public Node node { get; set; }

        //IQueryable<NodeAccess> nau { get; set; }

        // IQueryable<NodeAccess> nag { get; set; }
        bool isAcsOpened = false;
        bool isNSOpened = false;

        protected override async Task OnInitializedAsync()
        {
          
            Debug.WriteLine("OnInitializedAsync");
           // nau = LinksContext.NodeAccesses
           //     .Where(x => x.NodeId == node.Id )
           //     .Include(x=>x.User)
           //      .Where(x => x.User != null)
           //     //.AsQueryable()
           //     ;

           // nag = LinksContext.NodeAccesses
           //.Where(x => x.NodeId == node.Id  )
           //.Include(x => x.Group)
           //.Where(x=>x.Group != null)
           //.AsQueryable()
           ;
        }
        protected override async Task OnParametersSetAsync()
        {
            Debug.WriteLine("OnParametersSetAsync");
           // nau = LinksContext.NodeAccesses
           //       .Where(x => x.NodeId == node.Id )
           //       .Include(x => x.User)
           //        .Where(x => x.User != null)
           //       // .AsQueryable()
           //        ;

           // nag = LinksContext.NodeAccesses
           //.Where(x => x.NodeId == node.Id )
           //.Include(x => x.Group)
           //.Where(x => x.Group != null)
           //.AsQueryable()
           ;

        }
    }
}