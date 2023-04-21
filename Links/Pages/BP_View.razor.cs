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
using System.Text.Json;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using System.Diagnostics;
using Markdig;

namespace Links.Pages
{
    public partial class BP_View
    {
        int userId = 1;
        [Parameter]
        public int nodeId { get; set; }

        public Node node { get; set; }

        int _stageId;
        int stageId {
            get
            {
                return _stageId;
            }

            set
            {
                Debug.WriteLine(value);
                _stageId = value;
            }
        }

       
        string ms1 = "";
        private void addItem()
        {
 var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
               // .UseDiagrams()
                .Build();

            ms1 = Markdig.Markdown.ToHtml(@"
* [ ] Додати контролер для sql
    * [ ] Отримання параметрів в форматі json
    * [ ] Отримання параметрів в посиланні
* [ ] Перенести контролер для роботи з контентом
* [ ] Перенести механізм формування вузлів

```mermaid
graph TD;
    A-->B;
    A-->C;
    B-->D;
    C-->D;
```
.

```mermaid
  flowchart TB;        
        subgraph Технолог
        A[Опрацювання реквізитів] 
        C[Перевірка]  
        B[Доопрацювання]
        end
        subgraph Майстри
        O[Ознайомлення]
        end
        subgraph Аналітик
        D[Обробка]
        P
        E[Доопрацювання]  
        end
        subgraph Інженери
        T[Тестування]
        end
        D-->T
        T-->C
        C-->O
        T-->E
        E-->T
        C-->E
        A-->P{Все вірно?}
        P-- Ні -->B
        P-- Так -->D
        B-->P
```
"
, pipeline);
            

        }

        protected override async Task OnInitializedAsync()
        {
            node = _LinksContext.Nodes.Find(nodeId);
            // await ForecastService.GetForecastAsync(DateOnly.FromDateTime(DateTime.Now));
        }

    }
}