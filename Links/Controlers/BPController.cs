using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Links.Models;
using System.Diagnostics;
using System.Text.Json;
using Esprima.Ast;
using System.Xml.Linq;

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BPController : ControllerBase
    {
        private readonly LinksContext _context;
        BPO bpo;

        public BPController(LinksContext context)
        {
            _context = context;
            bpo = new BPO(_context);
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
          if (_context.Transactions == null)
          {
              return NotFound();
          }
            return await _context.Transactions.ToListAsync();
        }

        [HttpGet("{startTid}/{endSid}")]
        public async Task<ActionResult<object>> CreateTransaction(int startTid, int endSid)
        {
            if (_context.Transactions == null)
            {
                return NotFound();
            }

            var startT = await _context.Transactions.FindAsync(startTid);               
            if (startT == null)
            {
                return NotFound($"Транзакції {startTid} немає в базі даних");
            }

            var endS1 = await _context.Stages.FindAsync(endSid);
            if (endS1 == null)
            {
                return NotFound($"Етапу {endS1} немає в базі даних");
            }
            //_context.Entry(startT).Reference(c => c.Stage)
            //    .Load();
            //_context.Entry(startT.Stage).Collection(c => c.EndStages)
            //   .Load();

            var endS = _context.Stages.FromSqlRaw($@"
SELECT s.*
FROM [Transactions] t 
left join StageToStage ss on ss.StartStageId = t.StageId
join Stages s on s.id = ss.EndStageId
where t.id={startTid}
", startTid).Where(x=>x.Id==endSid).FirstOrDefault(x => x.Id == endSid);

           // var endS = startT.Stage.EndStages.ToList().FirstOrDefault;
            if (endS is null)
            {
                return Problem($"Запуск етапу {endSid} не передбачено з транзакції, що належить етапу {startT.StageId}");
            }

           await _context.Transactions.FindAsync(startTid);
          
            Transaction endT = startT.EndTransactions
                .Where(x=>x.StageId==endSid & x.Status == 0)
                .FirstOrDefault();
            if (endT is not null)
            {
                return Problem($"Вже є відкрита транзакція {endT.Id} для етапу {endSid}. Повторне створення заборонено.");
            }

            endT = new Transaction();
            endT.FlowId = startT.FlowId;
            endT.StageId = endSid;
            endT.CreateDt = DateTime.Now;
            endT.StartTransactions.Add(startT);


            return $"запуск з транзакції {startTid} етапу {endSid}";
        }


        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTransactionData(int id)
        {
          if (_context.Transactions == null)
          {
              return NotFound();
          }
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction.Data;
        }

        // PUT: api/Transactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, object trData)
        {

            Debug.WriteLine(trData.ToString());
            Debug.WriteLine(trData.GetType());

            var t = bpo.updateTr(id, (JsonElement)trData);

            return Ok();
          

          //  return NoContent();
        }

        // POST: api/Transactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}")]
        public async Task<ActionResult<object>> CreateNewTransaction(int id, object trData)
        {

            Debug.WriteLine(trData.ToString());
            Debug.WriteLine(trData.GetType());
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'LinksContext.Transactions'  is null.");
            }
         

            Transaction t;
             t = bpo.createFirstTrn(id, (JsonElement)trData);
                    //return CreatedAtAction("GetTransaction", t.Id );

            return Ok();
        }

       


        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            if (_context.Transactions == null)
            {
                return NotFound();
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool TransactionExists(int id)
        {
            return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
             
    

    }
}
