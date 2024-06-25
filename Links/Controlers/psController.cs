using Jint;
using Links.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Management.Automation;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class psController : ControllerBase
    {

        private readonly LinksContext? _context;

        public psController(LinksContext? context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> CheckPs()
        {

            PowerShell ps = PowerShell.Create().AddCommand("Get-Process");

            IAsyncResult asyncpl = ps.BeginInvoke();

             string s = "";
            
            foreach (PSObject result in ps.EndInvoke(asyncpl))
            {
             s+= @$"{result.Members["ProcessName"].Value} - { result.Members["Id"].Value}" +  "\r\n";
            }

            return s;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> sqlById(string id, [FromQuery] string q)
        {
            if (_context == null)
            {
                return Problem("'LinksContext'  is null.");
            }
            return q;
            // return Problem("'LinksContext'  is null.");

        }

        [HttpPost("{id}")]
        public async Task<ActionResult<object>> psFromJSON(int id, JsonElement requestContent)
        {
            if (_context == null)
            {
                return Problem("'LinksContext'  is null.");
            }

            var _content = (await _context.Contents.FindAsync(id));

            if (_content is null)
            {
                return Problem($"За вказаним id - {id} не знайдено скрипта для виконання.");
            }

            if (requestContent.ValueKind == JsonValueKind.Object)
            {
                try
                {
                    return runPsFromJsonObj(_content.DataSource, _content.Data, requestContent);
                }
                catch (Exception e)
                {
                    // Debug.WriteLine(e.Message);
                    return new ContentResult
                    {
                        ContentType = "application/json",
                        StatusCode = (int)System.Net.HttpStatusCode.OK,
                        Content = e.ToString()
                    };
                }
            }
            else if (requestContent.ValueKind == JsonValueKind.Array)
            {
                return Problem($"Обробка масиву ще не впроваджена - {requestContent.ValueKind.ToString()}");
            }
            else
            {
                return Problem($"Тип даних не підтримується - {requestContent.ValueKind.ToString()}");
            }
        }

        object runPsFromJsonObj(string cs, string query, JsonElement je)
        {

            PowerShell ps = PowerShell.Create().AddCommand("Get-Process");
                    

            foreach (var v in je.EnumerateObject())
            {
                if (v.Value.ValueKind == JsonValueKind.String)
                {
                    if (v.Value.TryGetDateTime(out var dt))
                    {
                        ps.AddScript($"[datetime]{v.Name} = {dt}");                       

                    }
                    else
                    {
                        ps.AddScript($"[string]{v.Name} = {v.Value}");
                    }
                }
                else if (v.Value.ValueKind == JsonValueKind.Number)
                {
                    if (v.Value.TryGetDecimal(out var db))
                    {
                        ps.AddScript($"[decimal]{v.Name} = {db}");
                    }
                    else
                    {
                        ps.AddScript($"[int32]{v.Name} = {v.Value}");
                    }

                }

            }



            IAsyncResult asyncps = ps.BeginInvoke();

            string s = "";

            foreach (PSObject result in ps.EndInvoke(asyncps))
            {
                s += @$"{result.Members["ProcessName"].Value} - {result.Members["Id"].Value}" + "\r\n";
            }

            return s;


        }

        //[HttpPost("test")]

    }
}
