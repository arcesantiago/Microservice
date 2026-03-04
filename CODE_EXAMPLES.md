# Code Examples & Patterns

## Complete Working Examples

### Example 1: Simple CRUD Operations

```csharp
// CREATE
var createCmd = new CreateExampleCommand 
{ 
    Description = "AI Generated Content",
    Status = "Active"
};
var createResult = await mediator.Send(createCmd);

if (createResult.IsSuccess)
{
    var newId = createResult.Value;
    Console.WriteLine($"Created record with ID: {newId}");
}

// READ by ID
var getQuery = new GetExampleByIdQuery { Id = newId };
var getResult = await mediator.Send(getQuery);

if (getResult.IsSuccess)
{
    var dto = getResult.Value;
    Console.WriteLine($"Retrieved: {dto.Description}");
}

// UPDATE
var updateCmd = new UpdateExampleCommand
{
    Id = newId,
    Description = "Updated by AI",
    Status = "Processed"
};
var updateResult = await mediator.Send(updateCmd);

// DELETE
var deleteCmd = new DeleteExampleCommand { Id = newId };
var deleteResult = await mediator.Send(deleteCmd);
```

---

### Example 2: Batch Processing with Pagination

```csharp
public async Task ProcessLargeDataset(
    IMediator mediator,
    Func<IEnumerable<GetExamplesPaginatedDto>, Task> processor)
{
    const int pageSize = 500;
    int page = 1;
    bool hasMore = true;

    while (hasMore)
    {
        // Fetch one page
        var query = new GetExamplesPaginatedQuery 
        { 
            CurrentPage = page, 
            PageSize = pageSize 
        };
        var result = await mediator.Send(query);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"Error fetching page {page}: {result.ErrorMessage}");
            break;
        }

        var pagedResult = result.Value;
        
        // Process this page
        await processor(pagedResult.Results);

        // Check if more pages exist
        var totalPages = (pagedResult.RowsCount + pageSize - 1) / pageSize;
        hasMore = page < totalPages;
        page++;

        Console.WriteLine($"Processed page {page-1} of {totalPages}");
    }

    Console.WriteLine("Batch processing complete");
}
```

---

### Example 3: Selective Field Update

```csharp
public async Task UpdateOnlyModifiedFields(
    IMediator mediator,
    int recordId,
    Dictionary<string, object> changes)
{
    // Step 1: Fetch current record
    var getQuery = new GetExampleByIdQuery { Id = recordId };
    var getResult = await mediator.Send(getQuery);

    if (!getResult.IsSuccess)
        return;

    var current = getResult.Value;
    var fieldsToUpdate = new List<Expression<Func<Example, object>>>();

    // Step 2: Identify which fields actually changed
    foreach (var change in changes)
    {
        var currentValue = current.GetType()
            .GetProperty(change.Key)
            ?.GetValue(current);

        if (!currentValue?.Equals(change.Value) ?? true)
        {
            // Field changed, mark for update
            fieldsToUpdate.Add(CreatePropertySelector(change.Key));
        }
    }

    if (fieldsToUpdate.Count == 0)
    {
        Console.WriteLine("No changes detected");
        return;
    }

    // Step 3: Update only changed fields
    var updateCmd = new UpdateExampleFieldsCommand
    {
        Id = recordId,
        PropertiesToUpdate = fieldsToUpdate.ToArray()
    };

    var updateResult = await mediator.Send(updateCmd);
    Console.WriteLine($"Updated {fieldsToUpdate.Count} field(s)");
}

private static Expression<Func<Example, object>> CreatePropertySelector(string propertyName)
{
    var parameter = Expression.Parameter(typeof(Example));
    var property = Expression.Property(parameter, propertyName);
    var lambda = Expression.Lambda<Func<Example, object>>(
        Expression.Convert(property, typeof(object)),
        parameter);
    return lambda;
}
```

---

### Example 4: Bulk Operations

```csharp
public async Task ProcessAndCleanupDuplicates(
    IMediator mediator,
    List<int> duplicateIds)
{
    // Option 1: Delete duplicates
    var deleteCmd = new DeleteManyExamplesCommand 
    { 
        Ids = duplicateIds 
    };
    var deleteResult = await mediator.Send(deleteCmd);

    if (deleteResult.IsSuccess)
    {
        Console.WriteLine($"Deleted {deleteResult.Value} duplicate records");
    }

    // Option 2: Update multiple records
    var updateCmd = new UpdateManyExamplesCommand
    {
        Ids = new List<int> { 1, 2, 3, 4, 5 }
        // Additional update parameters as needed
    };
    var updateResult = await mediator.Send(updateCmd);

    if (updateResult.IsSuccess)
    {
        Console.WriteLine($"Updated {updateResult.Value} records");
    }
}
```

---

### Example 5: Complex Analysis with Raw SQL

```csharp
public async Task AnalyzeDataWithSQL(IMediator mediator, DateTime since)
{
    var analysisQuery = new ExecuteSqlWithResultQuery
    {
        Sql = $@"
            SELECT 
                e.Id,
                e.Status,
                e.CreatedDate,
                (SELECT COUNT(*) FROM Examples 
                 WHERE Status = e.Status) as StatusCount,
                (SELECT AVG(CAST(DATEDIFF(hour, CreatedDate, GETDATE()) AS FLOAT))
                 FROM Examples 
                 WHERE Status = e.Status) as AvgAgeDays
            FROM Examples e
            WHERE e.CreatedDate > {since}
            AND e.Status IN ('Active', 'Processing')
            ORDER BY e.CreatedDate DESC
        "
    };

    var result = await mediator.Send(analysisQuery);

    if (result.IsSuccess)
    {
        var analytics = result.Value;
        
        // Process analytical results
        var statusGroups = analytics
            .GroupBy(x => x.Status)
            .Select(g => new 
            { 
                Status = g.Key, 
                Count = g.Count(),
                AvgAge = g.Average(x => x.AvgAgeDays) 
            });

        foreach (var group in statusGroups)
        {
            Console.WriteLine(
                $"Status: {group.Status}, Count: {group.Count}, Avg Age: {group.AvgAge} hours"
            );
        }
    }
}
```

---

### Example 6: Atomic Transaction Workflow

```csharp
public async Task ExecuteAtomicWorkflow(
    IMediator mediator,
    List<Example> newExamples,
    List<int> idsToArchive)
{
    var transactionCmd = new ExecuteInTransactionCommand
    {
        // This would need custom implementation based on your needs
        // Example structure shown below
    };

    // Using raw transaction via SqlRepository
    var sql = $@"
        BEGIN TRANSACTION;
        
        -- Step 1: Archive old records
        DELETE FROM Examples WHERE Id IN ({string.Join(",", idsToArchive)});
        
        -- Step 2: Insert new records
        INSERT INTO Examples (Description, Status, CreatedDate) VALUES
        {string.Join(",", newExamples.Select(x => $"('{x.Description}', 'New', GETDATE())"))}
        
        -- Step 3: Update counts
        UPDATE ExampleStats SET TotalCount = (SELECT COUNT(*) FROM Examples);
        
        COMMIT;
    ";

    var result = await mediator.Send(new ExecuteSqlCommandHandler
    {
        // Configuration
    });

    if (result.IsSuccess)
    {
        Console.WriteLine("Transaction completed successfully");
    }
    else
    {
        Console.WriteLine("Transaction rolled back");
    }
}
```

---

### Example 7: Validation Before Creation

```csharp
public async Task ValidateBeforeCreating(
    IMediator mediator,
    CreateExampleCommand command)
{
    // Check 1: Does referenced entity exist?
    if (command.RelatedExampleId.HasValue)
    {
        var exists = await mediator.Send(
            new ExistsExampleQuery { Id = command.RelatedExampleId.Value }
        );
        
        if (!exists.Value)
        {
            Console.WriteLine($"Related example {command.RelatedExampleId} not found");
            return;
        }
    }

    // Check 2: Total count not exceeded?
    var countResult = await mediator.Send(new CountExamplesQuery());
    if (countResult.Value >= 10000)
    {
        Console.WriteLine("Record limit reached");
        return;
    }

    // Check 3: Duplicate prevention?
    var existingQuery = new GetExampleByPredicateQuery
    {
        // Build predicate to check for duplicates
    };
    var existingResult = await mediator.Send(existingQuery);
    
    if (existingResult.IsSuccess)
    {
        Console.WriteLine("Record already exists");
        return;
    }

    // All validations passed - create
    var result = await mediator.Send(command);
    Console.WriteLine($"Created successfully: {result.Value}");
}
```

---

### Example 8: Projection for Lightweight Responses

```csharp
public async Task FetchMinimalDataForAPI(IMediator mediator)
{
    // Scenario: Need only IDs and Names for dropdown, not all 20+ columns
    
    var query = new GetExamplesWithProjectionQuery();
    var result = await mediator.Send(query);

    if (result.IsSuccess)
    {
        var items = result.Value;
        
        // This only queried: SELECT Id, Name FROM Examples
        // Not: SELECT * FROM Examples (all columns)
        
        var dropdownItems = items
            .Select(x => new SelectListItem 
            { 
                Value = x.Id.ToString(), 
                Text = x.Name 
            })
            .ToList();

        return Ok(dropdownItems);
    }
}
```

---

### Example 9: AI Agent Data Processing Pipeline

```csharp
public class AIDataProcessingPipeline
{
    private readonly IMediator _mediator;
    private readonly IAIAgent _aiAgent;

    public AIDataProcessingPipeline(IMediator mediator, IAIAgent aiAgent)
    {
        _mediator = mediator;
        _aiAgent = aiAgent;
    }

    public async Task Execute()
    {
        // Phase 1: Fetch data in batches
        Console.WriteLine("Phase 1: Fetching data...");
        var unprocessedRecords = await FetchUnprocessedBatches();

        // Phase 2: AI analysis
        Console.WriteLine("Phase 2: Analyzing with AI...");
        var analysisResults = await _aiAgent.AnalyzeBatch(unprocessedRecords);

        // Phase 3: Selective updates
        Console.WriteLine("Phase 3: Updating results...");
        await UpdateWithAIResults(analysisResults);

        // Phase 4: Remove low-quality records
        Console.WriteLine("Phase 4: Cleanup...");
        var lowQualityIds = analysisResults
            .Where(x => x.Confidence < 0.5)
            .Select(x => x.Id)
            .ToList();
        
        await DeleteLowQuality(lowQualityIds);

        Console.WriteLine("Pipeline complete");
    }

    private async Task<List<Example>> FetchUnprocessedBatches()
    {
        var allData = new List<Example>();
        int page = 1;
        bool hasMore = true;

        while (hasMore && page <= 100) // Safety limit
        {
            var query = new GetExamplesPaginatedQuery 
            { 
                CurrentPage = page, 
                PageSize = 1000 
            };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess) break;

            var pageData = result.Value;
            allData.AddRange(pageData.Results);

            hasMore = page * 1000 < pageData.RowsCount;
            page++;
        }

        return allData;
    }

    private async Task UpdateWithAIResults(List<AIAnalysisResult> results)
    {
        // Update only Status and Confidence fields
        foreach (var result in results)
        {
            var cmd = new UpdateExampleFieldsCommand
            {
                Id = result.Id,
                PropertiesToUpdate = new[]
                {
                    (Expression<Func<Example, object>>)(x => x.Status),
                    (Expression<Func<Example, object>>)(x => x.Confidence)
                }
            };
            
            await _mediator.Send(cmd);
        }
    }

    private async Task DeleteLowQuality(List<int> ids)
    {
        if (ids.Count == 0) return;

        var cmd = new DeleteManyExamplesCommand { Ids = ids };
        var result = await _mediator.Send(cmd);
        
        Console.WriteLine($"Deleted {result.Value} low-quality records");
    }
}
```

---

### Example 10: Repository Direct Usage (Advanced)

```csharp
// When you need more control, use repository directly
public class AdvancedDataAccessService
{
    private readonly ILINQRepository<Example> _linqRepo;
    private readonly ISqlRepository<Example> _sqlRepo;

    public async Task DemoLinqRepository()
    {
        // Complex query with includes
        var examples = await _linqRepo.GetListAsync(
            predicate: x => x.Status == "Active" && x.CreatedDate > DateTime.Now.AddDays(-30),
            select: x => x, // Can transform entity here
            orderBy: q => q.OrderByDescending(x => x.CreatedDate),
            includeProperties: new[] { 
                (Expression<Func<Example, object>>)(x => x.RelatedExamples),
                (Expression<Func<Example, object>>)(x => x.Metadata)
            },
            disableTracking: true
        );
    }

    public async Task DemoSqlRepository()
    {
        // Raw SQL with transaction
        var result = await _sqlRepo.ExecuteInTransactionAsync(
            async (repo) =>
            {
                // Step 1
                var inserted = await repo.ExecuteSqlAsync(
                    $"INSERT INTO Examples (Description) VALUES ({description})"
                );

                // Step 2
                var updated = await repo.ExecuteSqlAsync(
                    $"UPDATE Examples SET ModifiedDate = GETDATE() WHERE Id > {someId}"
                );

                return inserted + updated;
            }
        );
    }

    public async Task DemoProjection()
    {
        // Get specific columns only - lightweight
        var lightData = await _linqRepo.GetListAsync(
            select: x => new { x.Id, x.Name, x.Status },
            predicate: x => x.Status == "Active",
            orderBy: q => q.OrderBy(x => x.Name)
        );
    }
}
```

---

## Common Patterns Summary

| Pattern | Use When | Handlers |
|---------|----------|----------|
| CRUD Basic | Simple create/read/update/delete | Create/Get/Update/Delete + Single handlers |
| Batch Processing | Large datasets | GetExamplesPaginatedQueryHandler |
| Selective Update | Only some fields change | UpdateExampleFieldsCommandHandler |
| Validation | Need to check preconditions | ExistsExampleQueryHandler |
| Bulk Operations | Affect many records | UpdateManyExamplesCommandHandler / DeleteManyExamplesCommandHandler |
| Complex Analysis | Need aggregations/joins | ExecuteSqlWithResultQueryHandler |
| Transactions | Need atomic operations | ExecuteInTransactionCommandHandler |
| Lightweight Responses | Save bandwidth | GetExamplesWithProjectionQueryHandler |
| Direct SQL | Need performance | GetExamplesFromSqlQueryHandler |

---

## Error Handling Patterns

```csharp
// Pattern 1: Simple check
var result = await mediator.Send(query);
if (result.IsSuccess)
{
    ProcessData(result.Value);
}

// Pattern 2: With logging
var result = await mediator.Send(query);
if (!result.IsSuccess)
{
    _logger.LogError($"Handler failed: {result.ErrorMessage}");
    return BadRequest(result.ErrorMessage);
}

// Pattern 3: Chaining operations
var result1 = await mediator.Send(query1);
if (!result1.IsSuccess) return;

var result2 = await mediator.Send(query2);
if (!result2.IsSuccess) return;

// Use both results...
```

---

Last Updated: 2025-03-12
