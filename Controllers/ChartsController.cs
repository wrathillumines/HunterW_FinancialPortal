using HunterW_FinancialPortal.ChartViewModels;
using HunterW_FinancialPortal.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HunterW_FinancialPortal.Controllers
{
    public class ChartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public JsonResult BudgetSpentData()
        {
            var userId = User.Identity.GetUserId();
            var household = db.Users.Find(userId).MyHouse;
            var dataSet = new List<ChartData>();

            foreach (var budget in household.Budget.ToList())
            {
                var data = new ChartData
                {
                    Label = budget.Name,
                    Budget = budget.Items.Select(i => i.Amount).Sum(),
                    Spent = budget.Transactions.Select(t => t.TransactionAmount).Sum()
                };
                dataSet.Add(data);
            }

            return Json(dataSet);
        }
    }
}