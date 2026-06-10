using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using ZiraiIlacERPWeb.Models;
using ZiraiIlacERPWeb.Services;

namespace ZiraiIlacERPWeb.Controllers
{
    public class ProjectManagementController : Controller
    {
        private readonly ProjectManagementService _service;
        private readonly bool _isEnabled;

        public ProjectManagementController(ProjectManagementService service, IConfiguration config)
        {
            _service = service;
            _isEnabled = config.GetValue<bool>("Features:EnableProjectManagement", true);
        }

        private bool CheckAccess()
        {
            return _isEnabled;
        }

        public IActionResult Index()
        {
            if (!CheckAccess()) return NotFound("Bu modül devre dışı bırakılmıştır.");

            var data = _service.GetProjectData();
            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateProject(string name, string description, double budget, double capital, string status)
        {
            if (!CheckAccess()) return NotFound();

            var data = _service.GetProjectData();
            data.Project.Name = name;
            data.Project.Description = description;
            data.Project.Budget = budget;
            data.Project.Capital = capital;
            data.Project.Status = status;

            _service.SaveProjectData(data);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddTask(string taskName, int? assigneeId, int durationDays, int progressPercent, string predecessors)
        {
            if (!CheckAccess()) return NotFound();

            var data = _service.GetProjectData();
            
            var task = new ProjectTask
            {
                Id = data.Tasks.Count > 0 ? data.Tasks.Max(t => t.Id) + 1 : 1,
                ProjectId = 1,
                TaskName = taskName,
                AssigneeId = assigneeId,
                DurationDays = durationDays,
                ProgressPercent = progressPercent
            };

            if (!string.IsNullOrWhiteSpace(predecessors))
            {
                foreach (var p in predecessors.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(p.Trim(), out int predId))
                    {
                        task.PredecessorTaskIds.Add(predId);
                    }
                }
            }

            data.Tasks.Add(task);
            _service.SaveProjectData(data);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteTask(int id)
        {
            if (!CheckAccess()) return NotFound();

            var data = _service.GetProjectData();
            var task = data.Tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                data.Tasks.Remove(task);
                
                // Clean predecessor references
                foreach (var t in data.Tasks)
                {
                    t.PredecessorTaskIds.Remove(id);
                }

                _service.SaveProjectData(data);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddTransaction(string type, double amount, string category, string description)
        {
            if (!CheckAccess()) return NotFound();

            var data = _service.GetProjectData();
            var tx = new CapitalTransaction
            {
                Id = data.Transactions.Count > 0 ? data.Transactions.Max(t => t.Id) + 1 : 1,
                ProjectId = 1,
                Type = type,
                Amount = amount,
                Category = category,
                Date = DateTime.Now,
                Description = description
            };

            data.Transactions.Add(tx);
            _service.SaveProjectData(data);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteTransaction(int id)
        {
            if (!CheckAccess()) return NotFound();

            var data = _service.GetProjectData();
            var tx = data.Transactions.FirstOrDefault(t => t.Id == id);
            if (tx != null)
            {
                data.Transactions.Remove(tx);
                _service.SaveProjectData(data);
            }

            return RedirectToAction("Index");
        }
    }
}
