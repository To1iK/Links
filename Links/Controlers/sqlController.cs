using Jint;
using Links.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sqlController : ControllerBase
    {      

        private readonly LinksContext? _context;

        public sqlController(LinksContext? context)
        {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<object>> CheckEngine()
        {
            Debug.WriteLine("gjhgj");
            Dictionary<string, object> mp;
          
            Type type = typeof(BPO);// Operations;
                        
            foreach(var m in type.GetMethods())
            {
                Debug.WriteLine(m.Name);
            }
            return type.ToString();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> sqlById(string id,[FromQuery]string q)
        {
            if (_context == null)
            {
                return Problem("'LinksContext'  is null.");
            }
            return q;
           // return Problem("'LinksContext'  is null.");

        }

        //[HttpPost("{id}")]
        //public async Task<ActionResult<object>> sqlFromJSON(int id, JsonElement requestContent)
        //{
        //    if (_context == null)
        //    {
        //        return Problem("'LinksContext'  is null.");
        //    }

        //    var _content = (await _context.Contents.FindAsync(id));

        //    if (_content is null)
        //    {
        //        return Problem($"За вказаним id - {id} не знайдено скрипта для виконання.");
        //    }

        //    if (requestContent.ValueKind == JsonValueKind.Object)
        //    {
        //        try
        //        {
        //         return runSqlFromJsonObj(_content.DataSource,_content.Data, requestContent);
        //        }
        //        catch (SqlException e)
        //        {
        //            Debug.WriteLine(e.Message);
        //            return new ContentResult
        //            {
        //                ContentType = "application/json",
        //                StatusCode = (int)System.Net.HttpStatusCode.OK,
        //                Content = e.ToString()
        //            };
        //        }
        //    }
        //    else if (requestContent.ValueKind == JsonValueKind.Array)
        //    {
        //        return Problem($"Обробка масиву ще не впроваджена - {requestContent.ValueKind.ToString()}");
        //    }
        //    else
        //    {
        //        return Problem($"Тип даних не підтримується - {requestContent.ValueKind.ToString()}");
        //    }                
        //}

        void log(object msg)
        {
            Debug.WriteLine(msg);
        }

        object runSqlFromJsonObj(string cs, string query, JsonElement je)
        {
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();

                String sql = query; //"select * from [Links].[dbo].[Nodes]";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    foreach (var v in je.EnumerateObject())
                    {
                        if (v.Value.ValueKind == JsonValueKind.String)
                        {
                            if (v.Value.TryGetDateTime(out var dt))
                            {
                                command.Parameters.Add(v.Name, System.Data.SqlDbType.DateTime);
                                command.Parameters[v.Name].Value = dt;
                                //Debug.WriteLine($"{v.Name} - {v.Value} -  {v.GetType} - {v.Value.ValueKind} - {v.Value.TryGetDateTime(out dt)} - {dt}");
                            }
                            else
                            {
                                command.Parameters.Add(v.Name, System.Data.SqlDbType.NVarChar);
                                command.Parameters[v.Name].Value = v.Value.ToString();
                            }
                        }
                        else if (v.Value.ValueKind == JsonValueKind.Number)
                        {
                            if (v.Value.TryGetDecimal(out var db))
                            {
                                command.Parameters.Add(v.Name, System.Data.SqlDbType.Decimal);
                                command.Parameters[v.Name].Value = db;
                            }
                            else
                            {
                                command.Parameters.Add(v.Name, System.Data.SqlDbType.Int);
                                command.Parameters[v.Name].Value = v.Value;
                            }
                            //Debug.WriteLine($"{v.Name} - {v.Value} -  {v.GetType} - {v.Value.ValueKind}");
                        }

                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<object> rows = new List<object>();

                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            rows.Add(row);
                        }
                        return rows;
                    }
                }
            }
        }

        //[HttpPost("test")]
        //public Object testing( object obj)
        //{
        //    Debug.WriteLine(obj);
        //    return obj;
    //}
}
}
