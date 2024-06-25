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
using Markdig;
using System.Security.Policy;

namespace Links.Components
{
    public partial class EditComponent
    {

        [Inject]
        public Links.Models.LinksContext LinksContext { get; set; }

        [Parameter]
        public Node node { get; set; }


        Content createContent()
        {
            try
            {
             Content c = new Content();

            node.Content = c;
                StateHasChanged();
           // LinksContext.SaveChanges();
            return c;
            }
            catch
            {
                return null;
            }
        }

        string[] noContentTypes = new string[]
        {
            "url",
            "frame",
            "bp"
        };

    }
}