# Microservice Template - Architecture Documentation

## Overview

Este es un template de microservicio construido con arquitectura **CQRS (Command Query Responsibility Segregation)** y **Clean Architecture**, diseñado para ser una base sólida para nuevos microservicios que se desarrollarán con asistentes de IA como Cursor AI, Antigravity, Google Claude, etc.

**Stack Tecnológico:**
- .NET 10 / C# 14
- Entity Framework Core (LINQ & Raw SQL)
- MediatR (CQRS Bus)
- AutoMapper (DTO Mapping)
- Fluent Validation

---

## Architectural Patterns

### 1. **CQRS Pattern (Command Query Responsibility Segregation)**

El proyecto separa operaciones de lectura (Queries) de escritura (Commands):

#### **Commands** (Escritura)
- `CreateExampleCommandHandler` - Crear nuevas entidades
- `UpdateExampleCommandHandler` - Actualizar entidades completas
- `DeleteExampleCommandHandler` - Eliminar entidades
- `UpdateExampleFieldsCommandHandler` - Actualizar campos específicos (PATCH)
- `UpdateManyExamplesCommandHandler` - Actualizar múltiples registros en lote
- `DeleteManyExamplesCommandHandler` - Eliminar múltiples registros en lote
- `ExecuteSqlCommandHandler` - Ejecutar comandos SQL raw
- `ExecuteStoredProcedureCommandHandler` - Ejecutar stored procedures
- `ExecuteInTransactionCommandHandler` - Ejecutar operaciones en transacción

#### **Queries** (Lectura)
- `GetAllExamplesQueryHandler` - Obtener todos los registros
- `GetExampleByIdQueryHandler` - Obtener registro por ID
- `GetExampleByPredicateQueryHandler` - Obtener registro por predicado flexible
- `GetExamplesPaginatedQueryHandler` - Obtener registros con paginación
- `GetExamplesWithProjectionQueryHandler` - Obtener con proyección de campos
- `GetExampleWithProjectionQueryHandler` - Obtener uno con proyección
- `ExistsExampleQueryHandler` - Verificar existencia sin cargar datos
- `CountExamplesQueryHandler` - Contar registros totales
- `GetExamplesFromSqlQueryHandler` - Ejecutar SELECT raw SQL
- `ExecuteSqlWithResultQueryHandler` - Ejecutar SQL dinámico con resultados

### 2. **Repository Pattern**

Dos repositorios complementarios para diferentes necesidades:

#### **LINQRepository<T>** 
Operaciones basadas en ORM con LINQ

**Interfases Implementadas:**
- `IReadRepository<T>` - Lectura con LINQ
- `IWriteRepository<T>` - Escritura con Entity Framework
- `IQueryRepository<T>` - Proyecciones y DTOs

**Métodos Principales:**
- `FindAsync()` - Búsqueda por ID (rápida con índice)
- `GetEntityAsync()` - Búsqueda única con predicado complejo y includes
- `GetListAsync()` - Múltiples registros con filtros y ordenamiento
- `GetListPaginatedAsync()` - Resultados paginados (ideal para grandes volúmenes)
- `ExistsAsync()` - Verificación eficiente de existencia
- `AddAsync()` - Inserción de entidades
- `Update()` - Actualización completa
- `UpdateFields()` - Actualización selectiva de campos
- `UpdateManyAsync()` - Actualización en lote
- `Delete()` - Eliminación individual
- `DeleteManyAsync()` - Eliminación en lote
- `GetListAsync<TResult>()` - Proyección a DTO
- `GetEntityAsync<TResult>()` - Proyección individual

#### **SqlRepository<T>**
Operaciones SQL raw para casos especiales

**Interfases Implementadas:**
- `ISqlQueryRepository<T>` - Consultas SQL raw
- `ISqlCommandRepository<T>` - Comandos SQL (INSERT, UPDATE, DELETE)
- `ISqlRepository<T>` - Combinada

**Métodos Principales:**
- `FromSqlAsync()` - Ejecutar SELECT raw y obtener entidades
- `ExecuteSqlAsync()` - Ejecutar INSERT, UPDATE, DELETE
- `ExecuteStoredProcedureAsync()` - Ejecutar stored procedures
- `ExecuteSqlWithResultAsync()` - SELECT raw con mapeo a entidades
- `ExecuteInTransactionAsync<TResult>()` - Transacciones ACID

---

## Use Cases for AI Agents

### **Cursor AI, Antigravity, Google Claude Integration**

Los handlers y repositorios están diseñados para ser aprovechados por agentes de IA:

#### **Data Generation & Persistence**
```
CreateExampleCommandHandler
↓
AI genera contenido → Persistencia en BD
```

#### **Data Analysis & Processing**
```
GetExamplesPaginatedQueryHandler
↓
AI procesa datos en lotes → Análisis sin exhaustar memoria
```

#### **Selective Updates**
```
UpdateExampleFieldsCommandHandler
↓
AI identifica campos → Actualización solo de cambios
```

#### **Bulk Operations**
```
UpdateManyExamplesCommandHandler / DeleteManyExamplesCommandHandler
↓
AI procesa múltiples registros → Operación eficiente en lote
```

#### **Complex Analysis**
```
GetExamplesFromSqlQueryHandler / ExecuteSqlWithResultQueryHandler
↓
AI genera SQL complejo → Análisis con JOINs, CTEs, window functions
```

#### **Atomic Workflows**
```
ExecuteInTransactionCommandHandler
↓
AI ejecuta multi-step workflow → Garantía ACID (todo o nada)
```

---

## Performance Optimization Strategies

### **1. Projections (DTOs)**
Usar `GetListAsync<TResult>()` para obtener solo campos necesarios:
```csharp
// ❌ Ineficiente: obtiene 15 columnas
var allData = await repo.GetListAsync();

// ✅ Eficiente: obtiene solo 2 columnas
var minimal = await queryRepo.GetListAsync(
    x => new SimpleDto { Id = x.Id, Name = x.Name }
);
```

### **2. Pagination**
Para datasets grandes, usar `GetListPaginatedAsync()`:
```csharp
// Procesa 1M registros en lotes de 1000
var page = await repo.GetListPaginatedAsync(
    currentPage: 1,
    pageSize: 1000
);
```

### **3. Selective Updates**
Actualizar solo campos modificados con `UpdateFields()`:
```csharp
// ❌ SQL: UPDATE t SET col1=v1, col2=v2, col3=v3, ... (30 columnas)
repo.Update(entity);

// ✅ SQL: UPDATE t SET Status='Processed', Modified=now (2 columnas)
repo.UpdateFields(entity, x => x.Status, x => x.Modified);
```

### **4. Existence Checks**
Usar `ExistsAsync()` en lugar de `GetListAsync()` cuando solo importa presencia:
```csharp
// ❌ Carga entidad completa
var exists = await repo.GetListAsync(x => x.Id == id) != null;

// ✅ Verifica existencia sin cargar datos
var exists = await repo.ExistsAsync(x => x.Id == id);
```

### **5. Raw SQL for Complex Queries**
Usar `SqlRepository` para queries que LINQ no puede optimizar:
```csharp
// ❌ LINQ genera SQL suboptimal
var results = await repo.GetListAsync()
    .Where(x => /* complex logic */)
    .Select(/* projection */);

// ✅ SQL raw con optimizaciones específicas
var results = await sqlRepo.FromSqlAsync(
    $"SELECT * FROM Examples WHERE Score > {threshold} ORDER BY Score DESC LIMIT 1000"
);
```

---

## Unit of Work Pattern

Todas las operaciones de escritura deben ser committeadas con el patrón Unit of Work:

```csharp
// 1. Ejecutar operaciones en repositorio
await writeRepository.AddAsync(entity);
await writeRepository.Update(entity);

// 2. Commit todas las operaciones juntas
await unitOfWork.SaveChangesAsync();
```

Esto garantiza:
- Consistencia transaccional
- Una sola llamada a BD aunque se ejecuten múltiples operaciones
- Rollback automático si algo falla

---

## Error Handling Strategy

El patrón `Result<T>` proporciona un manejo de errores funcional:

```csharp
// En lugar de lanzar excepciones
public async Task<Result<GetExampleByIdDto>> Handle(...)
{
    var example = await readRepository.FindAsync(request.Id);
    
    // Retorna error sin excepción
    if (example == null)
        return Result<GetExampleByIdDto>.Failure("Ejemplo no encontrado");
    
    return Result<GetExampleByIdDto>.Success(mapper.Map<GetExampleByIdDto>(example));
}
```

Ventajas:
- Sin excepciones costosas
- Flujo funcional y predecible
- Compatible con resultados parciales
- Fácil logging de errores

---

## Handler Categories & Selection Guide

### **By Operation Type**

| Operación | Handler | Patrón |
|-----------|---------|--------|
| Crear 1 | CreateExampleCommandHandler | Command |
| Leer 1 por ID | GetExampleByIdQueryHandler | Query |
| Leer 1 flexible | GetExampleByPredicateQueryHandler | Query |
| Leer muchos | GetAllExamplesQueryHandler | Query |
| Leer paginado | GetExamplesPaginatedQueryHandler | Query |
| Leer proyectado | GetExamplesWithProjectionQueryHandler | Query |
| Contar | CountExamplesQueryHandler | Query |
| Verificar | ExistsExampleQueryHandler | Query |
| Actualizar 1 | UpdateExampleCommandHandler | Command |
| Actualizar 1 parcial | UpdateExampleFieldsCommandHandler | Command |
| Actualizar muchos | UpdateManyExamplesCommandHandler | Command |
| Eliminar 1 | DeleteExampleCommandHandler | Command |
| Eliminar muchos | DeleteManyExamplesCommandHandler | Command |

### **By Data Source**

| Fuente | Handler | Repositorio |
|--------|---------|-------------|
| LINQ simple | Get*/Update*/Delete* | LINQRepository |
| SQL raw | GetExamplesFromSql | SqlRepository |
| SQL dinámico | ExecuteSqlWithResult | SqlRepository |
| Stored Proc | ExecuteStoredProcedure | SqlRepository |
| Transaccional | ExecuteInTransaction | SqlRepository |

---

## Security Best Practices

### **SQL Injection Prevention**

✅ **Correcto - Usa FormattableString:**
```csharp
var sql = $"SELECT * FROM Examples WHERE Status = {status}";
await sqlRepo.FromSqlAsync(sql);
```

❌ **Incorrecto - Concatenación:**
```csharp
var sql = $"SELECT * FROM Examples WHERE Status = '{status}'"; // ¡VULNERABLE!
```

### **Entity Existence Validation**
Siempre validar que entidades existen antes de actualizar/eliminar:
```csharp
var example = await readRepository.FindAsync(id);
if (example == null)
    return Result<int>.Failure("Entidad no encontrada");
```

---

## Transaction Handling

Para operaciones que requieren atomicidad ACID:

```csharp
var result = await sqlRepository.ExecuteInTransactionAsync(
    async (repo) =>
    {
        // Operación 1
        await repo.ExecuteSqlAsync($"INSERT INTO ...");
        
        // Operación 2
        await repo.ExecuteSqlAsync($"UPDATE ...");
        
        // Si alguna falla, rollback automático de ambas
        return affectedRows;
    },
    cancellationToken
);
```

---

## Caching Considerations

Los handlers queries pueden implementar `ICacheableQuery` para caching:

```csharp
public class GetAllExamplesQuery : IRequest<Result<IEnumerable<GetAllExamplesDto>>>, ICacheableQuery
{
    public string CacheKey => "all-examples";
    public TimeSpan? Expiration => TimeSpan.FromHours(1);
}
```

Queries cachedadas:
- `GetAllExamplesQueryHandler` - Potencial candidata para cache
- `CountExamplesQueryHandler` - Buena candidata para cache
- `GetExamplesPaginatedQueryHandler` - Considerar cache por página

No cachear:
- `GetExampleByIdQueryHandler` - Datos más frescos son críticos
- Cualquier query que tenga parámetros dinámicos complejos

---

## Integration with .NET 10 & C# 14 Features

- **Primary Constructors:** Todos los handlers usan constructor primario
- **Records:** DTOs pueden ser records para mejor performance
- **Nullable Reference Types:** Soporte completo
- **FormattableString:** Parameterización segura de SQL
- **Async/Await:** Totalmente async-first

---

## Monitoring & Observability

Puntos clave para telemetría:

1. **Handler Execution Time** - Monitorear duración de cada handler
2. **Repository Operations** - Trackear operaciones de BD
3. **Error Tracking** - Logging de Result.Failure()
4. **Data Volume** - Monitorear tamaño de result sets
5. **SQL Performance** - Tiempo de queries raw SQL

---

## Next Steps for AI Agent Development

1. **Query Generation:** AI genera SQL safe usando FormattableString
2. **Bulk Processing:** AI procesa datos en lotes con GetListPaginatedAsync
3. **Selective Updates:** AI identifica cambios, usa UpdateFields()
4. **Validation:** AI valida existencia con ExistsAsync()
5. **Analysis:** AI ejecuta SQL complejo con GetExamplesFromSqlQueryHandler
6. **Atomicity:** AI ejecuta workflows con ExecuteInTransactionAsync()

---

**Última Actualización:** 2025-03-12
**Versión:** 1.0
**Compatibilidad:** .NET 10, C# 14
