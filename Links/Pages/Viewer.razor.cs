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

namespace Links.Pages
{
    public partial class Viewer
    {
        [Inject]
        public Links.Models.LinksContext LinksContext { get; set; }

        [Parameter]
        public int nodeId { get; set; }


        [Parameter]
        public int main { get; set; } = -1;

        Node node { get; set; }

        Content post { get; set; }

        [Parameter]
        public bool isEdit { get; set; }

        public bool isInfo { get; set; }

        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                              .UseAdvancedExtensions()
                              .Build();

        private void ChangeHeader()
        {
            //OnHeaderChange.Invoke(node.NodeName);
            // MainLayout.ChangeHeader(node.NodeName);
            //MainLayout.header = node.NodeName;
            //StateHasChanged();

        }

        async protected override void OnInitialized()
        {         
            isEdit = false;
            isInfo = false;
        
        }

        protected override void OnParametersSet()
        {
            //if (main!=-1) {Components.NodesMenu.MainNode = main; }    

             node = LinksContext.Nodes.Find(nodeId);
            if(node is not null)
            {
             LinksContext.Entry(node)
            .Reference(c => c.NodeType)
            .Load();

            post = null;
            isEdit = false;
            isInfo = false;
            ShowPost();
            }
           
           
           
           // LinksContext.ActiveNode = node;

            StateHasChanged();
        }

        async Task ShowPost()
        {
            post = null;
            try
            {
                post = LinksContext.Contents.Find(nodeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        void ChangeEditStatus()
        {
            if (isEdit)
            {
                if (node.Content.Data != null &&
                    node.Content.Data != " ")
                {
                    // LinksContext.ExtContents.Remove(node.IdNavigation);
                    LinksContext.SaveChanges();
                }
                isEdit = false;
            }
            else
            {
                if (node.Content is null)
                {
                    //node.IdNavigation = new ExtContent();
                    Content content = new Content();
                    content.Id = node.Id;
                    content.Data = " ";
                    content.IdNavigation = node;
                    LinksContext.Contents.Add(content);
                    //LinksContext.SaveChanges();
                }
                isEdit = true;
            }

        }
        void ChangeInfoStatus()
        {

            if (isInfo)
            {
                isInfo = false;
            }
            else isInfo = true;

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