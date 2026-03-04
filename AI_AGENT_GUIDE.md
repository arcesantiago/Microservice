# AI Agent Integration Guide

## Overview

Este microservice template ha sido diseñado específicamente para ser utilizado por agentes de IA como **Cursor AI**, **Antigravity**, **Google Claude**, y otros asistentes inteligentes.

---

## Quick Start for AI Agents

### **Step 1: Understand the Architecture**

```
API Layer
  ↓
Handler Layer (CQRS)
  ├─ Command Handlers (Escritura)
  └─ Query Handlers (Lectura)
  ↓
Repository Layer
  ├─ LINQRepository (ORM queries)
  └─ SqlRepository (Raw SQL)
  ↓
Database Layer (Entity Framework)
```

### **Step 2: Choose the Right Handler**

```
¿Qué necesitas hacer?

├─ Crear datos
│   └─ CreateExampleCommandHandler
│
├─ Leer datos
│   ├─ GetExampleByIdQueryHandler (ID específico)
│   ├─ GetAllExamplesQueryHandler (todos)
│   ├─ GetExamplesPaginatedQueryHandler (muchos con paginación)
│   ├─ GetExamplesWithProjectionQueryHandler (solo algunos campos)
│   └─ ExecuteSqlWithResultQueryHandler (SQL complejo)
│
├─ Actualizar datos
│   ├─ UpdateExampleCommandHandler (todas propiedades)
│   ├─ UpdateExampleFieldsCommandHandler (propiedades específicas)
│   └─ UpdateManyExamplesCommandHandler (múltiples registros)
│
├─ Eliminar datos
│   ├─ DeleteExampleCommandHandler (1 registro)
│   └─ DeleteManyExamplesCommandHandler (múltiples)
│
└─ Operaciones avanzadas
    ├─ ExecuteSqlCommandHandler (SQL raw)
    ├─ ExecuteStoredProcedureCommandHandler (Stored procs)
    └─ ExecuteInTransactionCommandHandler (Transacciones ACID)
```

---

## Common AI Agent Workflows

### **Pattern 1: Data Generation & Storage**

```csharp
// AI genera contenido → Persistencia
var aiGeneratedContent = aiAgent.GenerateContent();

var command = new CreateExampleCommand 
{ 
    Description = aiGeneratedContent.Text,
    Status = "Generated",
    Confidence = aiGeneratedContent.Score
};

var result = await mediator.Send(command);

if (result.IsSuccess)
{
    Console.WriteLine($"Contenido generado guardado con ID: {result.Value}");
}
```

**Handlers Utilizados:**
- `CreateExampleCommandHandler` - Persistencia

---

### **Pattern 2: Batch Analysis & Processing**

```csharp
// AI analiza datos en lotes sin exhaustar memoria
int pageSize = 1000;
bool hasMore = true;
int page = 1;

while (hasMore)
{
    var query = new GetExamplesPaginatedQuery { CurrentPage = page, PageSize = pageSize };
    var result = await mediator.Send(query);
    
    if (result.IsSuccess)
    {
        var examples = result.Value;
        
        // AI procesa este lote
        await aiAgent.AnalyzeBatch(examples.Results);
        
        // Decide si hay más
        hasMore = page * pageSize < examples.RowsCount;
        page++;
    }
}
```

**Handlers Utilizados:**
- `GetExamplesPaginatedQueryHandler` - Lectura paginada
- `CountExamplesQueryHandler` - Contar registros totales

---

### **Pattern 3: Selective Field Updates**

```csharp
// AI identifica qué campos cambiar, actualiza solo esos
var example = await mediator.Send(new GetExampleByIdQuery { Id = recordId });

// AI analiza y determina cambios
var updatedExample = example.Value;
updatedExample.Status = "Processed"; // AI cambió esto
updatedExample.Confidence = 0.95f;    // AI cambió esto
// Otros campos NO cambian

// Actualizar solo campos modificados (más eficiente)
var updateCommand = new UpdateExampleFieldsCommand 
{ 
    Id = recordId,
    PropertiesToUpdate = new[] { "Status", "Confidence" }
};

await mediator.Send(updateCommand);
```

**Handlers Utilizados:**
- `GetExampleByIdQueryHandler` - Leer registro
- `UpdateExampleFieldsCommandHandler` - Actualizar solo cambios

---

### **Pattern 4: Bulk Operations**

```csharp
// AI identifica registros duplicados → Eliminación en lote
var duplicateIds = await aiAgent.FindDuplicates();

var deleteCommand = new DeleteManyExamplesCommand 
{ 
    Ids = duplicateIds.ToList()
};

var result = await mediator.Send(deleteCommand);

Console.WriteLine($"Eliminados {result.Value} registros duplicados");
```

**Handlers Utilizados:**
- `DeleteManyExamplesCommandHandler` - Eliminación en lote
- `UpdateManyExamplesCommandHandler` - Actualización en lote

---

### **Pattern 5: Complex Analysis with Raw SQL**

```csharp
// AI necesita análisis complejo que LINQ no puede expresar
var analysisQuery = new ExecuteSqlWithResultQuery 
{ 
    Sql = $@"
        SELECT 
            Status,
            AVG(Confidence) as AvgConfidence,
            COUNT(*) as Count,
            MAX(CreatedDate) as LastProcessed
        FROM Examples
        WHERE CreatedDate > {threshold}
        GROUP BY Status
        HAVING COUNT(*) > {minCount}
        ORDER BY AvgConfidence DESC
    "
};

var result = await mediator.Send(analysisQuery);

if (result.IsSuccess)
{
    // AI procesa resultados agregados
    await aiAgent.ProcessAnalytics(result.Value);
}
```

**Handlers Utilizados:**
- `ExecuteSqlWithResultQueryHandler` - SQL dinámico
- `GetExamplesFromSqlQueryHandler` - SQL predefinido

---

### **Pattern 6: Atomic Multi-Step Workflows**

```csharp
// AI ejecuta workflow que requiere atomicidad (todo o nada)
var workflow = new ExecuteInTransactionCommand 
{ 
    Operations = new List<DbOperation>
    {
        new DbOperation { Type = "INSERT", Sql = $"INSERT INTO Examples ..." },
        new DbOperation { Type = "UPDATE", Sql = $"UPDATE Examples SET ..." },
        new DbOperation { Type = "DELETE", Sql = $"DELETE FROM Examples ..." }
    }
};

var result = await mediator.Send(workflow);

// Si cualquier step falla, TODOS se revierten automáticamente
// Si todos exitosos, TODOS se confirman juntos
```

**Handlers Utilizados:**
- `ExecuteInTransactionCommandHandler` - Transacciones ACID
- `ExecuteSqlCommandHandler` - Ejecución SQL

---

### **Pattern 7: Validation Before Operations**

```csharp
// AI verifica que referencia existe antes de crear relacionada
var parentId = 123;

var existsQuery = new ExistsExampleQuery { Id = parentId };
var exists = await mediator.Send(existsQuery);

if (exists.Value) // Verificación eficiente sin cargar datos completos
{
    var createCommand = new CreateExampleCommand { ParentId = parentId };
    await mediator.Send(createCommand);
}
else
{
    Console.WriteLine("Parent no existe, operación cancelada");
}
```

**Handlers Utilizados:**
- `ExistsExampleQueryHandler` - Verificación eficiente

---

## Handler Reference for AI Agents

### **Query Handlers (Read Operations)**

| Handler | When to Use | Performance | Returns |
|---------|------------|-------------|---------|
| `GetExampleByIdQueryHandler` | Fetch one record by ID | ⚡⚡⚡ Fast (index lookup) | Single DTO |
| `GetAllExamplesQueryHandler` | Fetch all records | ⚡ Medium (full scan) | IEnumerable<DTO> |
| `GetExampleByPredicateQueryHandler` | Fetch one by flexible criteria | ⚡⚡ Good (depends on predicate) | Single DTO |
| `GetExamplesPaginatedQueryHandler` | Batch processing without memory exhaustion | ⚡⚡⚡ Optimized for large datasets | PagedResult<DTO> |
| `GetExamplesWithProjectionQueryHandler` | Fetch only specific columns | ⚡⚡⚡ Best (minimal data) | IEnumerable<DTO> (projected) |
| `GetExampleWithProjectionQueryHandler` | Fetch one with specific columns | ⚡⚡⚡ Best (minimal data) | Single projected DTO |
| `ExistsExampleQueryHandler` | Check if record exists | ⚡⚡⚡ Very fast (just EXISTS) | Boolean |
| `CountExamplesQueryHandler` | Get total count | ⚡⚡⚡ Very fast (COUNT aggregation) | Integer |
| `GetExamplesFromSqlQueryHandler` | Execute predefined complex SQL | ⚡⚡ Varies | IEnumerable<DTO> |
| `ExecuteSqlWithResultQueryHandler` | Execute dynamic complex SQL | ⚡⚡ Varies | IReadOnlyList<DTO> |

### **Command Handlers (Write Operations)**

| Handler | When to Use | Performance | Returns |
|---------|------------|-------------|---------|
| `CreateExampleCommandHandler` | Create new record | ⚡⚡ Good | ID of created record |
| `UpdateExampleCommandHandler` | Update entire record | ⚡⚡ Good | Updated record ID |
| `UpdateExampleFieldsCommandHandler` | Update specific fields only | ⚡⚡⚡ Best (minimal columns) | Updated record ID |
| `UpdateManyExamplesCommandHandler` | Batch update multiple records | ⚡⚡ Good for batches | Count updated |
| `DeleteExampleCommandHandler` | Delete single record | ⚡⚡ Good | Deleted record ID |
| `DeleteManyExamplesCommandHandler` | Batch delete multiple records | ⚡⚡⚡ Best for bulk ops | Count deleted |
| `ExecuteSqlCommandHandler` | Execute raw SQL INSERT/UPDATE/DELETE | ⚡⚡ Varies | Count affected rows |
| `ExecuteStoredProcedureCommandHandler` | Execute database stored procedure | ⚡⚡ Varies | Result from procedure |
| `ExecuteInTransactionCommandHandler` | Execute atomic multi-step operations | ⚡⚡ Good + ACID | Transaction result |

---

## Performance Tips for AI Agents

### **Tip 1: Use Pagination for Large Datasets**

```csharp
❌ AVOID - Loads entire dataset into memory
var allData = await mediator.Send(new GetAllExamplesQuery());
var processed = allData.Value.Where(x => ShouldProcess(x)).ToList();

✅ CORRECT - Process in manageable chunks
var pageSize = 1000;
var hasMore = true;
var page = 1;

while (hasMore)
{
    var result = await mediator.Send(
        new GetExamplesPaginatedQuery { CurrentPage = page, PageSize = pageSize }
    );
    await ProcessBatch(result.Value.Results);
    
    hasMore = page * pageSize < result.Value.RowsCount;
    page++;
}
```

### **Tip 2: Use Projections for Reduced Bandwidth**

```csharp
❌ AVOID - Transfers entire entity
var examples = await mediator.Send(new GetAllExamplesQuery());

✅ CORRECT - Transfer only needed fields
var summaries = await mediator.Send(new GetExamplesWithProjectionQuery());
```

### **Tip 3: Use Selective Updates**

```csharp
❌ AVOID - Updates all columns even if unchanged
example.Status = "Updated"; // Only this changed
await mediator.Send(new UpdateExampleCommand { Entity = example });

✅ CORRECT - Update only changed fields
await mediator.Send(new UpdateExampleFieldsCommand 
{ 
    Id = example.Id,
    PropertiesToUpdate = new[] { "Status" }
});
```

### **Tip 4: Verify Before Operations**

```csharp
❌ AVOID - Try operation, handle exception
try 
{
    await mediator.Send(new CreateRelatedCommand { ParentId = parentId });
}
catch (Exception ex) 
{ 
    // Handle error 
}

✅ CORRECT - Verify first, fail gracefully
var exists = (await mediator.Send(new ExistsExampleQuery { Id = parentId })).Value;
if (exists)
{
    await mediator.Send(new CreateRelatedCommand { ParentId = parentId });
}
```

### **Tip 5: Use Raw SQL for Complex Queries**

```csharp
❌ AVOID - LINQ can generate suboptimal SQL
var results = (await mediator.Send(new GetAllExamplesQuery()))
    .Value
    .Where(x => /* complex predicate */)
    .GroupBy(x => x.Status)
    .Select(g => new { Status = g.Key, Count = g.Count() });

✅ CORRECT - Write optimized SQL
var results = await mediator.Send(new ExecuteSqlWithResultQuery 
{
    Sql = $@"
        SELECT Status, COUNT(*) as Count 
        FROM Examples 
        WHERE {complex_filter}
        GROUP BY Status
    "
});
```

---

## Error Handling Pattern

```csharp
// All handlers return Result<T> for functional error handling
var result = await mediator.Send(query);

if (result.IsSuccess)
{
    // Process result.Value
    Console.WriteLine($"Success: {result.Value}");
}
else
{
    // Handle error gracefully
    Console.WriteLine($"Error: {result.ErrorMessage}");
    // No exception thrown - controlled error flow
}
```

---

## Testing Pattern for AI Agents

```csharp
[Test]
public async Task AIAgent_ShouldCreateAndUpdateRecord()
{
    // Arrange
    var createCommand = new CreateExampleCommand { Description = "AI Generated" };
    
    // Act - Create
    var createResult = await mediator.Send(createCommand);
    Assert.IsTrue(createResult.IsSuccess);
    
    var recordId = createResult.Value;
    
    // Act - Read
    var readResult = await mediator.Send(new GetExampleByIdQuery { Id = recordId });
    Assert.IsTrue(readResult.IsSuccess);
    
    // Act - Update
    var updateCommand = new UpdateExampleFieldsCommand 
    { 
        Id = recordId,
        PropertiesToUpdate = new[] { "Status" }
    };
    var updateResult = await mediator.Send(updateCommand);
    Assert.IsTrue(updateResult.IsSuccess);
    
    // Assert
    var finalResult = await mediator.Send(new GetExampleByIdQuery { Id = recordId });
    Assert.AreEqual("Updated", finalResult.Value.Status);
}
```

---

## Recommended Handler Combinations for Common AI Tasks

### **Task: Content Generation & Storage**
```
1. CreateExampleCommandHandler ← Store AI-generated content
2. GetExampleByIdQueryHandler ← Verify stored
```

### **Task: Data Validation & Cleanup**
```
1. GetExamplesPaginatedQueryHandler ← Load batch
2. ExistsExampleQueryHandler ← Validate references
3. DeleteManyExamplesCommandHandler ← Remove invalid
```

### **Task: Analytics & Reporting**
```
1. ExecuteSqlWithResultQueryHandler ← Complex SQL
2. GetExamplesFromSqlQueryHandler ← Pre-defined queries
```

### **Task: Bulk Corrections**
```
1. ExecuteInTransactionCommandHandler ← Atomic updates
2. UpdateManyExamplesCommandHandler ← Batch updates
```

### **Task: Smart Filtering**
```
1. GetExamplesWithProjectionQueryHandler ← Minimal fields
2. GetExampleByPredicateQueryHandler ← Custom predicates
```

---

## Security Checklist for AI Agents

- ✅ Always use `FormattableString` ($"") for SQL parameters
- ✅ Never concatenate user/AI input directly into SQL
- ✅ Validate entity existence with `ExistsAsync()` before operations
- ✅ Use `ExecuteInTransactionAsync()` for atomic multi-step workflows
- ✅ Implement audit logging for all Create/Update/Delete operations
- ✅ Validate result.IsSuccess before processing results
- ✅ Use parameterized queries consistently

---

## Monitoring for AI Agents

Track these metrics for optimal AI agent performance:

1. **Handler Execution Time** - Slow handlers need optimization
2. **Database Query Duration** - Watch for N+1 queries
3. **Result Set Sizes** - Large sets need pagination
4. **Error Frequency** - Track handler failures
5. **Transaction Rollback Count** - Indicates conflict scenarios
6. **Cache Hit Ratio** - For cacheable queries

---

## Next Steps

1. **Read DOCUMENTATION.md** - Full architectural details
2. **Examine Handler Code** - Review comments in each handler
3. **Test Locally** - Try the handlers with sample data
4. **Implement Monitoring** - Add telemetry for production use
5. **Extend as Needed** - Add new handlers for your domain

---

**Last Updated:** 2025-03-12
**Version:** 1.0
**For:** AI Agents (Cursor AI, Antigravity, Claude, etc.)
