using Jint;
using Links.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class jsEngineController : ControllerBase
    {

        Jint.Engine engine;

        private readonly LinksContext? _context;

        public jsEngineController(LinksContext? context)
        {
            _context = context;
            engine = new Jint.Engine(cfg => cfg.AllowClr())
             .SetValue("log", new Action<object>(log));

            engine.SetValue("myFetch", new Func<string, ExpandoObject, Task<ExpandoObject>>(myFetch));
            engine.Execute(
                @"
async function fetch(url,options=null){

    let q = await myFetch(url, options).Result

let response = { 
status: q.response.StatusCode,
statusText: q.response.ReasonPhrase,
OK: q.response.IsSuccessStatusCode,
body:q.response.Content,
url:url
}
response.json = function() {
        return q.body
}
    return response
}
");
            engine.Execute("var responseContent = {}");
        }

        [HttpGet]
        public Object CheckEngine()
        {
            using (engine)
            {
                var rez = engine.Evaluate("let j = 'jsEngine is working'; j;");
                return rez.ToString();
            }
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<object>> ProcessById(int id)
        {
            if (_context == null)
            {
                return Problem("'LinksContext'  is null.");
            }

            using (engine)
            {
                // engine.Execute("var responseContent = JSON.stringify({noData:'Дані не задано'});");              

                var _content = (await _context.Contents.FindAsync(id));

                if (_content is null)
                {                    
                    return Problem($"За вказаним id - {id} не знайдено скрипта для виконання.");
                }

                try
                {
                    engine.Execute(_content.Data);
                    object responseContent = engine.GetValue("responseContent").ToObject();

                    if (responseContent.GetType() == typeof(System.String))
                    {
                        return responseContent;
                    }
                    else
                    {
                        dynamic d = new ExpandoObject();
                        d.msg = "Скрипт виконано без надсилання даних. Для отримання результату перемінна responseContent має бути задана строкою JSON";
                        d.responseContentType = responseContent.GetType().ToString();
                        d.responseContent = responseContent;                      
                        return d;
                    }
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                    dynamic d = new ExpandoObject();
                    d.msg = "В процесі виконання скрипта виникла помилка";
                    d.error = ex.Message;
                    d.code = _content.Data;
                    return d;
                }
            }
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<object>> processJSON(int id, JsonElement requestContent)
        {
            if (_context == null)
            {
                return Problem("'LinksContext'  is null.");
            }
            using (engine)
            {
                engine.Execute($"var requestContent = JSON.stringify({requestContent.ToString()});");

                var _content = (await _context.Contents.FindAsync(id));

                if (_content is null)
                {
                    return Problem($"За вказаним id - {id} не знайдено скрипта для виконання.");
                }
                try
                {
                    engine.Execute(_content.Data);
                    object responseContent = engine.GetValue("responseContent").ToObject();
                    return responseContent;
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                    dynamic d = new ExpandoObject();
                    d.msg = "В процесі виконання скрипта виникла помилка";
                    d.error = ex.Message;
                    d.code = _content.Data;
                    return d;
                }
            }
        }

        void log(object msg)
        {
            Debug.WriteLine(msg);
        }

        async Task<ExpandoObject> myFetch(string url, ExpandoObject opts = null)
        {
            using (var client = new System.Net.Http.HttpClient())
            {

                dynamic eo = new ExpandoObject();


                if (opts != null)
                {
                    dynamic options = opts;

                    IDictionary<string, object?> dict = new Dictionary<string, object?>(opts);

                    if (dict.Keys.Contains("headers"))
                    {
                        Debug.WriteLine(dict["headers"]);
                    }

                    StringContent? reqContent = null;

                    if (dict.Keys.Contains("body"))
                    {
                        reqContent = new StringContent(dict["body"].ToString(),
                                                       System.Text.Encoding.UTF8,
                                                       "application/json");
                    }

                    if (dict.Keys.Contains("method"))
                    {
                        HttpResponseMessage? r = null;
                        switch (dict["method"])
                        {
                            case "GET":
                                r = client.GetAsync(url).Result;
                                break;
                            case "PUT":
                                if (reqContent != null)
                                    r = client.PutAsync(url, reqContent).Result;
                                else
                                    r = new HttpResponseMessage(System.Net.HttpStatusCode.SeeOther);
                                break;
                            case "POST":
                                if (reqContent != null)
                                    r = client.PostAsync(url, reqContent).Result;
                                else
                                    r = new HttpResponseMessage(System.Net.HttpStatusCode.SeeOther);
                                break;
                            case "DELETE":
                                r = client.DeleteAsync(url).Result;
                                break;
                            default:

                                break;
                        }
                        eo.response = r;
                    }

                }

                else
                {
                    eo.response = client.GetAsync(url).Result;
                }

                eo.body = eo.response.Content.ReadAsStringAsync().Result;

                return eo;

            }
        }

    }
}
