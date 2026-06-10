using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using ZiraiIlacERPWeb.Models;

namespace ZiraiIlacERPWeb.Services
{
    public class ProjectManagementService
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public ProjectManagementService(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.WebRootPath, "data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            _filePath = Path.Combine(dataDir, "project_management.json");
            InitializeMockData();
        }

        public ProjectDataWrapper GetProjectData()
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                {
                    return new ProjectDataWrapper();
                }

                try
                {
                    var json = File.ReadAllText(_filePath);
                    var data = JsonSerializer.Deserialize<ProjectDataWrapper>(json);
                    
                    if (data != null && data.Tasks != null)
                    {
                        ComputeCriticalPath(data.Tasks);
                        RecalculateProgress(data);
                    }
                    
                    return data ?? new ProjectDataWrapper();
                }
                catch
                {
                    return new ProjectDataWrapper();
                }
            }
        }

        public void SaveProjectData(ProjectDataWrapper data)
        {
            lock (_lock)
            {
                ComputeCriticalPath(data.Tasks);
                RecalculateProgress(data);
                
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(_filePath, json);
            }
        }

        private void RecalculateProgress(ProjectDataWrapper data)
        {
            if (data.Tasks != null && data.Tasks.Count > 0)
            {
                double totalDuration = data.Tasks.Sum(t => t.DurationDays);
                if (totalDuration > 0)
                {
                    double weightedProgress = data.Tasks.Sum(t => t.DurationDays * (t.ProgressPercent / 100.0));
                    data.Project.Progress = (int)Math.Round((weightedProgress / totalDuration) * 100);
                }
            }
        }

        public void ComputeCriticalPath(List<ProjectTask> tasks)
        {
            if (tasks == null || tasks.Count == 0) return;

            // 1. Reset values
            foreach (var t in tasks)
            {
                t.EarlyStart = 0;
                t.EarlyFinish = 0;
                t.LateStart = 0;
                t.LateFinish = 0;
                t.Slack = 0;
                t.IsCritical = false;
            }

            var taskDict = tasks.ToDictionary(t => t.Id);

            // 2. Forward Pass (ES & EF)
            bool changed = true;
            int maxIterations = tasks.Count * tasks.Count;
            int iteration = 0;
            while (changed && iteration < maxIterations)
            {
                changed = false;
                foreach (var task in tasks)
                {
                    int maxEF = 0;
                    foreach (var predId in task.PredecessorTaskIds)
                    {
                        if (taskDict.TryGetValue(predId, out var predTask))
                        {
                            if (predTask.EarlyFinish > maxEF)
                            {
                                maxEF = predTask.EarlyFinish;
                            }
                        }
                    }

                    int newES = maxEF;
                    int newEF = newES + task.DurationDays;

                    if (task.EarlyStart != newES || task.EarlyFinish != newEF)
                    {
                        task.EarlyStart = newES;
                        task.EarlyFinish = newEF;
                        changed = true;
                    }
                }
                iteration++;
            }

            // Project duration is max EF
            int projectFinishTime = tasks.Count > 0 ? tasks.Max(t => t.EarlyFinish) : 0;

            // 3. Backward Pass (LS & LF)
            foreach (var task in tasks)
            {
                task.LateFinish = projectFinishTime;
                task.LateStart = projectFinishTime - task.DurationDays;
            }

            // Map successors
            var successors = tasks.ToDictionary(t => t.Id, t => new List<ProjectTask>());
            foreach (var task in tasks)
            {
                foreach (var predId in task.PredecessorTaskIds)
                {
                    if (successors.ContainsKey(predId))
                    {
                        successors[predId].Add(task);
                    }
                }
            }

            changed = true;
            iteration = 0;
            while (changed && iteration < maxIterations)
            {
                changed = false;
                foreach (var task in tasks)
                {
                    int minLS = projectFinishTime;
                    var taskSuccessors = successors[task.Id];
                    if (taskSuccessors.Count > 0)
                    {
                        minLS = taskSuccessors.Min(s => s.LateStart);
                    }

                    int newLF = minLS;
                    int newLS = newLF - task.DurationDays;

                    if (task.LateFinish != newLF || task.LateStart != newLS)
                    {
                        task.LateFinish = newLF;
                        task.LateStart = newLS;
                        changed = true;
                    }
                }
                iteration++;
            }

            // 4. Slack and Critical Path
            foreach (var task in tasks)
            {
                task.Slack = task.LateStart - task.EarlyStart;
                task.IsCritical = (task.Slack == 0);
            }
        }

        private void InitializeMockData()
        {
            if (File.Exists(_filePath)) return;

            var mockData = new ProjectDataWrapper
            {
                Project = new Project
                {
                    Id = 1,
                    Name = "Zirai İlaç Üretim Kapasitesi Artırımı & Akıllı Depolama Entegrasyonu",
                    Description = "Akademik ve endüstriyel standartlarda zirai ilaç üretim kapasitesini 2 katına çıkaracak yeni reaktör montajı, saha genişletilmesi ve IoT tabanlı akıllı depolama sisteminin anahtar teslim entegrasyonu projesidir.",
                    Budget = 750000.00,
                    Capital = 1200000.00,
                    StartDate = DateTime.Now.AddDays(-25),
                    EndDate = DateTime.Now.AddDays(50),
                    Status = "Devam Ediyor",
                    Progress = 0 // Will be calculated
                },
                Team = new List<TeamMember>
                {
                    new TeamMember { Id = 1, Name = "Doç. Dr. Selim Aksoy", Role = "Proje Yöneticisi", Email = "s.aksoy@tarim.com", Phone = "0532 999 1111", Avatar = "👨‍💼", Specialty = "Proje Yönetimi & CPM" },
                    new TeamMember { Id = 2, Name = "Dr. Elif Yılmaz", Role = "Kimya Ar-Ge Lideri", Email = "e.yilmaz@tarim.com", Phone = "0533 888 2222", Avatar = "👩‍🔬", Specialty = "Kimyasal Formülasyon" },
                    new TeamMember { Id = 3, Name = "Murat Can", Role = "IoT & Otomasyon Müh.", Email = "m.can@tarim.com", Phone = "0535 777 3333", Avatar = "👨‍💻", Specialty = "Gömülü Sistemler & Yazılım" },
                    new TeamMember { Id = 4, Name = "Zeynep Demir", Role = "Tedarik & Satın Alma", Email = "z.demir@tarim.com", Phone = "0536 666 4444", Avatar = "👩‍💼", Specialty = "Sözleşme & Lojistik" },
                    new TeamMember { Id = 5, Name = "Hasan Kaya", Role = "Saha Şefi", Email = "h.kaya@tarim.com", Phone = "0537 555 5555", Avatar = "👷", Specialty = "İnşaat & Donanım Kurulumu" }
                },
                Tasks = new List<ProjectTask>
                {
                    new ProjectTask { Id = 1, ProjectId = 1, TaskName = "Fizibilite & Pazar Analizi Raporu", AssigneeId = 1, DurationDays = 8, ProgressPercent = 100, PredecessorTaskIds = new List<int>() },
                    new ProjectTask { Id = 2, ProjectId = 1, TaskName = "İlaç Formülasyonu & Hammadde Onayı", AssigneeId = 2, DurationDays = 12, ProgressPercent = 100, PredecessorTaskIds = new List<int> { 1 } },
                    new ProjectTask { Id = 3, ProjectId = 1, TaskName = "Reaktör ve Ekipman İthalat Siparişi", AssigneeId = 4, DurationDays = 15, ProgressPercent = 100, PredecessorTaskIds = new List<int> { 2 } },
                    new ProjectTask { Id = 4, ProjectId = 1, TaskName = "Üretim Alanı Zemin & Altyapı Hazırlığı", AssigneeId = 5, DurationDays = 20, ProgressPercent = 90, PredecessorTaskIds = new List<int> { 1 } },
                    new ProjectTask { Id = 5, ProjectId = 1, TaskName = "Yeni Reaktör Kurulumu ve Montajı", AssigneeId = 5, DurationDays = 14, ProgressPercent = 35, PredecessorTaskIds = new List<int> { 3, 4 } },
                    new ProjectTask { Id = 6, ProjectId = 1, TaskName = "Akıllı Depo RFID & Sensör Yazılımı", AssigneeId = 3, DurationDays = 22, ProgressPercent = 50, PredecessorTaskIds = new List<int> { 2 } },
                    new ProjectTask { Id = 7, ProjectId = 1, TaskName = "Depo Sensör Altyapısı Donanım Montajı", AssigneeId = 3, DurationDays = 10, ProgressPercent = 10, PredecessorTaskIds = new List<int> { 4, 6 } },
                    new ProjectTask { Id = 8, ProjectId = 1, TaskName = "Sistem Entegrasyonu & Pilot Denemeler", AssigneeId = 2, DurationDays = 7, ProgressPercent = 0, PredecessorTaskIds = new List<int> { 5, 7 } },
                    new ProjectTask { Id = 9, ProjectId = 1, TaskName = "Kabul Testleri & Kapanış Raporlaması", AssigneeId = 1, DurationDays = 4, ProgressPercent = 0, PredecessorTaskIds = new List<int> { 8 } }
                },
                Transactions = new List<CapitalTransaction>
                {
                    new CapitalTransaction { Id = 1, ProjectId = 1, Type = "Gelir", Amount = 800000.00, Category = "Sermaye", Date = DateTime.Now.AddDays(-24), Description = "Kurucu ortaklar nakdi sermaye aktarımı" },
                    new CapitalTransaction { Id = 2, ProjectId = 1, Type = "Gelir", Amount = 400000.00, Category = "Hibe Desteği", Date = DateTime.Now.AddDays(-15), Description = "KOSGEB / TÜBİTAK Proje 1. dönem Ar-Ge desteği" },
                    new CapitalTransaction { Id = 3, ProjectId = 1, Type = "Gider", Amount = 280000.00, Category = "Ekipman", Date = DateTime.Now.AddDays(-14), Description = "Cam astarlı reaktör ithalat bedeli" },
                    new CapitalTransaction { Id = 4, ProjectId = 1, Type = "Gider", Amount = 95000.00, Category = "Altyapı", Date = DateTime.Now.AddDays(-10), Description = "Tesis zemin epoksi kaplama ve havalandırma işleri" },
                    new CapitalTransaction { Id = 5, ProjectId = 1, Type = "Gider", Amount = 45000.00, Category = "Yazılım & Lisans", Date = DateTime.Now.AddDays(-8), Description = "RFID entegrasyonu ve bulut lisans bedeli" },
                    new CapitalTransaction { Id = 6, ProjectId = 1, Type = "Gider", Amount = 65000.00, Category = "Hammadde", Date = DateTime.Now.AddDays(-5), Description = "Deneme üretimi için ön hammadde tedariki" }
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(mockData, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
