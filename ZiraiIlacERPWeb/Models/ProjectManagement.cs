using System;
using System.Collections.Generic;

namespace ZiraiIlacERPWeb.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Budget { get; set; }
        public double Capital { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int Progress { get; set; }
    }

    public class TeamMember
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public string Specialty { get; set; }
    }

    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string TaskName { get; set; }
        public int? AssigneeId { get; set; }
        public int DurationDays { get; set; }
        public int ProgressPercent { get; set; }
        public List<int> PredecessorTaskIds { get; set; } = new List<int>();

        // Critical Path Method (CPM) parameters
        public int EarlyStart { get; set; }
        public int EarlyFinish { get; set; }
        public int LateStart { get; set; }
        public int LateFinish { get; set; }
        public int Slack { get; set; }
        public bool IsCritical { get; set; }
    }

    public class CapitalTransaction
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Type { get; set; } // "Gelir" or "Gider"
        public double Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public class ProjectDataWrapper
    {
        public Project Project { get; set; }
        public List<TeamMember> Team { get; set; } = new List<TeamMember>();
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public List<CapitalTransaction> Transactions { get; set; } = new List<CapitalTransaction>();
    }
}
