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

namespace Links.Controlers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly LinksContext _context;

        public TransactionsController(LinksContext context)
        {
            _context = context;
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
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
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

            return transaction;
        }

        // PUT: api/Transactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
          //  return NoContent();
        }

        // POST: api/Transactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
          if (_context.Transactions == null)
          {
              return Problem("Entity set 'LinksContext.Transactions'  is null.");
          }
            _context.Transactions.Add(transaction);
          
            await _context.SaveChangesAsync();

            //if (transaction.Status == 1 & transaction.NextStage != null)
            //{
            //    Transaction transaction2 = new Transaction();
            //    transaction2.StageId = transaction.NextStage;
            //    transaction2.PrevTransaction = transaction.Id;
            //    transaction2.Status = 0;
            //    _context.Transactions.Add(transaction2);
            //}

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);

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

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("Data/{id}")]
        public async Task<ActionResult<JsonElement>> getContent(int id)
        {

            Debug.WriteLine(id);
            if (_context.Contents == null)
            {
                return NotFound();
            }
            var content = await _context.Transactions.FindAsync(id);

            if (content == null)
            {
                return NotFound();
            }

            return new ContentResult
            {
                ContentType = "text/json",
                StatusCode = (int)System.Net.HttpStatusCode.OK,
                Content = content.Data.ToString()
            };

        }

    

    }
}
