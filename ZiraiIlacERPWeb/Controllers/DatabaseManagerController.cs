using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using ZiraiIlacERPWeb.Data;

namespace ZiraiIlacERPWeb.Controllers
{
    public class DatabaseManagerController : Controller
    {
        private readonly ERPDbContext _context;

        public DatabaseManagerController(ERPDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSchema()
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                // 1. Get Row Counts safely
                var rowCounts = new Dictionary<string, long>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            t.name AS TableName, 
                            SUM(p.rows) AS [RowCount]
                        FROM sys.tables t
                        JOIN sys.indexes i ON t.object_id = i.object_id
                        JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
                        WHERE i.type <= 1
                        GROUP BY t.name;";
                    
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tableName = reader.GetString(0);
                            var count = Convert.ToInt64(reader.GetValue(1));
                            rowCounts[tableName] = count;
                        }
                    }
                }

                // 2. Get Columns and Metadata safely
                var schemaData = new Dictionary<string, object>();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT 
                            t.name AS TableName, 
                            c.name AS ColumnName, 
                            ty.name AS DataType,
                            c.is_nullable AS IsNullable,
                            ISNULL((SELECT TOP 1 1 FROM sys.index_columns ic 
                                    JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                                    WHERE ic.object_id = t.object_id AND ic.column_id = c.column_id AND i.is_primary_key = 1), 0) AS IsPrimaryKey,
                            ISNULL((SELECT TOP 1 rt.name + '.' + rc.name
                                    FROM sys.foreign_key_columns fkc
                                    JOIN sys.foreign_keys fk ON fkc.constraint_object_id = fk.object_id
                                    JOIN sys.tables rt ON fkc.referenced_object_id = rt.object_id
                                    JOIN sys.columns rc ON fkc.referenced_object_id = rc.object_id AND fkc.referenced_column_id = rc.column_id
                                    WHERE fkc.parent_object_id = t.object_id AND fkc.parent_column_id = c.column_id), '') AS ForeignKeyInfo
                        FROM sys.tables t
                        JOIN sys.columns c ON t.object_id = c.object_id
                        JOIN sys.types ty ON c.user_type_id = ty.user_type_id
                        ORDER BY TableName, c.column_id;";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        string currentTable = "";
                        List<object> currentCols = null;

                        while (await reader.ReadAsync())
                        {
                            var tableName = reader.GetString(0);
                            var columnName = reader.GetString(1);
                            var dataType = reader.GetString(2);
                            var isNullable = reader.GetBoolean(3);
                            var isPrimaryKey = Convert.ToInt32(reader.GetValue(4)) == 1;
                            var foreignKeyInfo = reader.GetString(5);

                            if (tableName != currentTable)
                            {
                                currentTable = tableName;
                                currentCols = new List<object>();

                                string label = tableName;
                                string icon = "🗄️";

                                switch (tableName)
                                {
                                    case "Products": label = "Ürünler"; icon = "⚙️"; break;
                                    case "Categories": label = "Kategoriler"; icon = "📂"; break;
                                    case "Customers": label = "Müşteriler"; icon = "👤"; break;
                                    case "Orders": label = "Siparişler"; icon = "📋"; break;
                                    case "OrderDetails": label = "Sipariş Detayları"; icon = "📦"; break;
                                }

                                rowCounts.TryGetValue(tableName, out long count);

                                schemaData[tableName] = new
                                {
                                    label = label,
                                    icon = icon,
                                    rows = count,
                                    cols = currentCols
                                };
                            }

                            currentCols.Add(new
                            {
                                n = columnName,
                                t = dataType.ToUpper(),
                                pk = isPrimaryKey,
                                fk = string.IsNullOrEmpty(foreignKeyInfo) ? null : foreignKeyInfo,
                                nn = !isNullable
                            });
                        }
                    }
                }

                return Json(schemaData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Sql))
            {
                return Json(new { success = false, message = "Sorgu boş olamaz." });
            }

            var sql = request.Sql.Trim();
            var timer = Stopwatch.StartNew();

            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;

                    // If it is a SELECT query (or similar that returns a result set)
                    if (IsSelectQuery(sql))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var columns = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                            }

                            var values = new List<List<object?>>();
                            while (await reader.ReadAsync())
                            {
                                var row = new List<object?>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    var val = reader.GetValue(i);
                                    if (val == DBNull.Value)
                                    {
                                        row.Add(null);
                                    }
                                    else
                                    {
                                        row.Add(val);
                                    }
                                }
                                values.Add(row);
                            }

                            timer.Stop();
                            return Json(new
                            {
                                success = true,
                                isSelect = true,
                                elapsedMs = timer.ElapsedMilliseconds,
                                columns = columns,
                                values = values
                            });
                        }
                    }
                    else
                    {
                        // DML Query (INSERT, UPDATE, DELETE)
                        var rowsAffected = await cmd.ExecuteNonQueryAsync();
                        timer.Stop();
                        return Json(new
                        {
                            success = true,
                            isSelect = false,
                            elapsedMs = timer.ElapsedMilliseconds,
                            rowsAffected = rowsAffected
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                timer.Stop();
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    elapsedMs = timer.ElapsedMilliseconds
                });
            }
        }

        private bool IsSelectQuery(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return false;

            // Remove comments and whitespace
            var lines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var cleanedSql = "";
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                // Skip single-line comments
                if (trimmedLine.StartsWith("--") || trimmedLine.StartsWith("//"))
                {
                    continue;
                }
                cleanedSql += " " + trimmedLine;
            }

            cleanedSql = cleanedSql.Trim();

            // Detect if first word matches select keywords
            return cleanedSql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                   cleanedSql.StartsWith("WITH", StringComparison.OrdinalIgnoreCase) ||
                   cleanedSql.StartsWith("SHOW", StringComparison.OrdinalIgnoreCase) ||
                   cleanedSql.StartsWith("EXPLAIN", StringComparison.OrdinalIgnoreCase) ||
                   cleanedSql.StartsWith("PRAGMA", StringComparison.OrdinalIgnoreCase);
        }
    }

    public class QueryRequest
    {
        public string Sql { get; set; }
    }
}
