using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Links.Models;
using System.Configuration;
using NuGet.Protocol;
//using Newtonsoft.Json;

using System.Text.Json;
using System.Diagnostics;
using Newtonsoft.Json;
using Jint;
using Esprima.Ast;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Jint.Runtime.Interop;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Markdig;

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly LinksContext _context;

        public ContentsController(LinksContext context)
        {
            _context = context;
        }

        // GET: api/Contents
        [HttpGet]
        public async Task<Object> GetContents()
        {
          if (_context.Contents == null)
          {
              return NotFound();
          }
            // return await _context.Contents.ToListAsync();

         //   string jsonString = JsonSerializer.Serialize(_context.Contents.ToList());

            return new ContentResult
            {                
                ContentType = "text/json",
                StatusCode = (int)System.Net.HttpStatusCode.OK,
                Content = _context.Contents.ToList().ToJson()
            };
        }

      
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<Object> GetContent(int id, [FromQuery] string vars = "", [FromQuery] char separator = ';')
        {

            var сontent = await _context.
                Contents
                .FindAsync(id);


            if (сontent == null)
            {
                return NotFound();

            }

            _context.Entry(сontent)
               .Reference(c => c.IdNavigation)
               .Load();

            _context.Entry(сontent.IdNavigation)
              .Reference(c => c.NodeType)
              .Load();

           if (сontent.IdNavigation.NodeType.Code == "js")
            {
                return new ContentResult
                {
                    ContentType = "application/javascript",
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    Content = сontent.Data
                };
            }
            else if (сontent.IdNavigation.NodeType.Code == "json")
            {
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    Content = сontent.Data
                };
            }

            else if (сontent.IdNavigation.NodeType.Code == "css")
            {
                return new ContentResult
                {
                    ContentType = "text/css",
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    Content = сontent.Data
                };
            }
            else if (сontent.IdNavigation.NodeType.Code == "md")
            {               
                return MD(сontent.Data);
            }

            else if (сontent.IdNavigation.NodeType.Code == "sql")
            {

                List<Object> strings = new List<Object>();
                try
                {
                    Debug.WriteLine(DateTime.Now);
                    using (SqlConnection connection = new SqlConnection(сontent.IdNavigation.NodeContent))
                    {

                        connection.Open();

                        String sql = сontent.Data; //"select * from [Links].[dbo].[Nodes]";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            if (vars != null && vars.Contains(":"))
                            {

                                Dictionary<string, string> dic = vars.Split(";").ToDictionary(x => x.Split(":")[0]);
                                foreach (var v in dic)
                                {
                                    command.Parameters.Add(v.Key, System.Data.SqlDbType.NVarChar);
                                    command.Parameters[v.Key].Value = v.Value.Split(":")[1];
                                }
                            }

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                var columns = Enumerable.Range(0, reader.FieldCount)
                                            .Select(reader.GetName).ToList();

                                strings.Add(String.Join(separator, columns));

                                while (reader.Read())
                                {
                                    Object[] values = new Object[reader.FieldCount];
                                    int fieldCount = reader.GetValues(values);
                                    strings.Add(String.Join(separator, values).Replace(" 0:00:00", ""));

                                }
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    return new ContentResult
                    {
                        ContentType = "application/json",
                        StatusCode = (int)System.Net.HttpStatusCode.OK,
                        Content = e.ToString()
                    };
                }


                return String.Join(Environment.NewLine, strings);

            }
            else
            {
                return CreateContentResult(сontent.Data, "text/html");
                //return new ContentResult
                //{
                //    ContentType = "text/html",
                //    StatusCode = (int)System.Net.HttpStatusCode.OK,
                //    Content = сontent.Data
                //};
            }



        }

        object CreateContentResult(string data, string contentType)
        {
            return new ContentResult
            {
                ContentType = contentType,
                StatusCode = (int)System.Net.HttpStatusCode.OK,
                Content = data
            };
        }

        [HttpGet("Source/{id}")]
        [AllowAnonymous]
        public async Task<Object> GetContentSource(int id)
        {
            var сontent = await _context.
              Contents
              .FindAsync(id);


            if (сontent == null)
            {
                return NotFound();
            }

            return сontent.Data;

        }
        [HttpPut("Source/{id}")]
        public async Task<Object> PutContentSource(int id, object data)
        {
            Debug.WriteLine(data);
            var сontent = await _context.
             Contents
             .FindAsync(id);

            if (сontent == null)
            {
                return NotFound();
            }
          
           // _context.Entry(сontent).State = EntityState.Modified;

            try
            {
                сontent.Data = data.ToString();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {                             
                    throw;                
            }
            return Ok();
        }

     object MD(string data)
        {

            var pipeline = new MarkdownPipelineBuilder();

                pipeline
            //      .UseAbbreviations()
            //.UseAutoIdentifiers()
            //.UseCitations()
            //.UseCustomContainers()
            //.UseDefinitionLists()
            //.UseEmphasisExtras()
            //.UseFigures()
            //.UseFooters()
            //.UseFootnotes()
            //.UseGridTables()
            //.UseMathematics()
            //.UseMediaLinks()
            //.UsePipeTables()
            //.UseListExtras()
            //.UseTaskLists()
            //.UseDiagrams()
            //.UseAutoLinks()
            //.UseGenericAttributes()
                               .UseAdvancedExtensions()                               
                               .Build();

            try
            {
                return new ContentResult
                {
                    ContentType = "text/html; charset=utf-8",
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    // Content = Markdig.Markdown.ToHtml(data,pipeline)
                    Content = ""
                };
            }
           catch(Exception ex)
            {
                return Problem(ex.Message);
            }          

         }




    }
}
