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
using Links.Shared;
using Links.Models;
using Markdig;

namespace Links.Components
{
    public partial class ViewComponent
    {

        //[Inject]
        //public LinksContext LinksContext { get; set; }
   

        [Parameter]
        public Node node { get; set; }
            

        MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                              .UseAdvancedExtensions()
                              .Build();
        async protected override void OnInitialized()
        {


        }

        protected override void OnParametersSet()
        {

        }



    }
}