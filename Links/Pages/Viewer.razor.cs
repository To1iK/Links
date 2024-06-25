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
using static System.Net.Mime.MediaTypeNames;
using Markdig;
using System.Security.Claims;
using System.Drawing.Drawing2D;

namespace Links.Pages
{
    public partial class Viewer
    {
        [Inject]
        public Links.Models.LinksContext LinksContext { get; set; }

        [Parameter]
        public int nodeId { get; set; }


        //[Parameter]
        //public int mainNodeId {
        //    get {
        //        return LinksContext.MainNode.Id;
        //    }
        //    set
        //    {
        //        var mn = LinksContext.Nodes.Find(mainNodeId);
        //        if (mn == null)
        //        {
        //            mn = new Node();
        //            mn.NodeName = "Кореневий вузол Links";
        //            mn.Id = 0;
        //        }
        //        else
        //        {

        //        }
        //        LinksContext.MainNode = mn;               
        //    }
        //} 

        Node node { get; set; }


        [Parameter]
        public int mode { get; set; } = 1;

        public bool isInfo { get; set; }

        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                              .UseAdvancedExtensions()
                              .Build();
  

        async protected override void OnInitialized()
        {         
         
        
        }

        protected override void OnParametersSet( )
        {
            //if (main!=-1) {Components.NodesMenu.MainNode = main; }    

             node = LinksContext.Nodes.Find(nodeId);
            if(node is not null)
            {
             LinksContext.Entry(node)
                    .Reference(c => c.NodeType)            
            .Load();
                LinksContext.Entry(node)
                         .Reference(c => c.Content)
                 .Load();
            }
                    
           
           // LinksContext.ActiveNode = node;

            StateHasChanged();
        }   


        void ChangeEditStatus(int m )
        {
          
            if (mode ==2 & m!=2)
            {
                if (node.Content.Data != null &&
                    node.Content.Data != "")
                {
                    // LinksContext.ExtContents.Remove(node.IdNavigation);
                    LinksContext.SaveChanges();
                }  
                
            }

           mode = m; 

        }
     


        //private string? authMessage;
        //private string? surname;
        //private IEnumerable<Claim> claims = Enumerable.Empty<Claim>();

        //private async Task GetClaimsPrincipalData()
        //{
        //    var authState = await AuthenticationStateProvider
        //        .GetAuthenticationStateAsync();
        //    var user = authState.User;

        //    if (user.Identity is not null && user.Identity.IsAuthenticated)
        //    {
        //        authMessage = $"{user.Identity.Name} is authenticated.";
        //        claims = user.Claims;
        //        surname = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value;
        //    }
        //    else
        //    {
        //        authMessage = "The user is NOT authenticated.";
        //    }
        //}
    }
}