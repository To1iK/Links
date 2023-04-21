using Links.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Links
{
    public class BPO
    {
        public BPO(LinksContext lc)
        {
            _lc = lc;
            //_user = u;
        }

        LinksContext _lc { get; set; }

        public int _user { get; set; } = 0;

        void makeChoice(Transaction t)
        {
            var choices = _lc.Choices.Where(x => x.N == t.ChoiceN
                                         & x.StartStageId == t.StageId);
            foreach (var ch in choices)
            {
                switch (ch.OperationId)
                {
                    
                    case (int)Operations.update:
                        t.Status = 1;
                        break;
                    case (int)Operations.createNext:
                        t.Status = 0;
                        createEndTr(t, ch.EndStageId);
                        break;

                }
            }

            _lc.SaveChanges();

        }

        /// <summary>
        /// Створення першої транзакції процесу за вказаним ідентифікатором етапу
        /// </summary>
        /// <param name="stage">Ідентифікатор етапу в базі даних</param>    
        public Transaction createFirstTrn(int stage, JsonElement trData)
        {

            Transaction t = new Transaction();

            t.CreateDt = DateTime.Now;
            t.StageId = stage;
            t.Status = 1;
            t.UserId = _user;
            t.Data = trData.ToString();
            
            t.ChoiceN = int.Parse(trData.GetProperty("choice").GetString());

            _lc.Transactions.Add(t);          
            
            makeChoice(t);

            return t;

        }

        void createEndTr(Transaction startTr, int endStageId)
        {

            Transaction t = new Transaction();

            t.CreateDt = DateTime.Now;
            t.Status = 1;
            t.UserId = _user;

            t.StageId = endStageId;
            t.FlowId = startTr.Id;

            t.StartTransactions.Add(startTr);

            _lc.Transactions.Add(t);
            //_lc.SaveChanges();

        }

        public Transaction updateTr(int id, JsonElement trData)
        {
            var t = _lc.Transactions.Find(id);

            t.CloseDt = DateTime.Now;
            t.UserId = _user;
            t.Status = 1;
            t.Data = trData.ToString();            

            t.ChoiceN = int.Parse(trData.GetProperty("choice").GetString());           
            
            makeChoice(t);       

            return t;

        }

       
    }

    /// <summary>
    /// Операції
    /// </summary>
    public enum Operations
    {
        createNew = 1,
        update = 2,
        createNext = 3


    }


    /// <summary>
    /// Операції
    /// </summary>
    public enum AccessLevels
    {
        disabled = 0,
        canRead = 10,
        canChange = 20,
        canAdd = 30,
        isOwner = 100


    }
}
