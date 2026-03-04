# CreateExampleCommandValidator - Id Validation

## ✅ Validación Agregada

Se ha agregado validación para el campo `Id` en `CreateExampleCommandValidator`.

### **Regla de Validación**

```csharp
RuleFor(x => x.Id)
    .GreaterThan(0)
    .WithMessage("Id must be greater than 0")
    .WithErrorCode("IdInvalid")
    .WithSeverity(Severity.Error);
```

---

## 📋 Detalles de la Validación

| Aspecto | Valor |
|--------|-------|
| **Campo** | `Id` |
| **Condición** | Debe ser > 0 |
| **Mensaje** | "Id must be greater than 0" |
| **Código Error** | "IdInvalid" |
| **Severidad** | Error |

---

## 🎯 Casos de Validación

### **Caso 1: Id Válido** ✅

```csharp
var command = new CreateExampleCommand(Id: 1);
var result = await mediator.Send(command);

// Resultado: ✅ Pasa validación
result.IsSuccess == true
```

### **Caso 2: Id Cero** ❌

```csharp
var command = new CreateExampleCommand(Id: 0);
var result = await mediator.Send(command);

// Resultado: ✅ Retorna Result.Failure() (SIN EXCEPCIÓN)
result.IsSuccess == false
result.Error == "Id: Id must be greater than 0"
```

### **Caso 3: Id Negativo** ❌

```csharp
var command = new CreateExampleCommand(Id: -5);
var result = await mediator.Send(command);

// Resultado: ✅ Retorna Result.Failure()
result.IsSuccess == false
result.Error == "Id: Id must be greater than 0"
```

### **Caso 4: Id Mayor que 0** ✅

```csharp
var command = new CreateExampleCommand(Id: 999);
var result = await mediator.Send(command);

// Resultado: ✅ Pasa validación
result.IsSuccess == true
```

---

## 🤖 Integración con AI Agents

### **Pattern: AI Validation Awareness**

```csharp
public class AICommandExecutor
{
    public async Task<Result<int>> ExecuteCreateCommand(int id)
    {
        // AI intenta crear con ID
        var command = new CreateExampleCommand(Id: id);
        var result = await mediator.Send(command);

        // ✅ Sin try-catch - Manejo funcional
        if (!result.IsSuccess)
        {
            // Extrae error de validación
            if (result.Error.Contains("Id must be greater than 0"))
            {
                // AI sabe que Id es inválido
                Console.WriteLine("AI: Id debe ser mayor que 0");
                return await ExecuteCreateCommand(Math.Abs(id) + 1);
            }
        }

        // AI continuó exitosamente
        Console.WriteLine($"✓ Registro creado: {result.Value}");
        return result;
    }
}
```

---

## 🧪 Testing Examples

### **Test: Validación de Id > 0**

```csharp
[TestClass]
public class CreateExampleCommandValidatorTests
{
    private CreateExampleCommandValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        _validator = new CreateExampleCommandValidator();
    }

    [TestMethod]
    public async Task Validate_IdGreaterThanZero_IsValid()
    {
        // Arrange
        var command = new CreateExampleCommand(Id: 1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public async Task Validate_IdEqualsZero_IsInvalid()
    {
        // Arrange
        var command = new CreateExampleCommand(Id: 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.Contains("greater than 0"));
    }

    [TestMethod]
    public async Task Validate_IdNegative_IsInvalid()
    {
        // Arrange
        var command = new CreateExampleCommand(Id: -10);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
    }

    [TestMethod]
    public async Task Validate_IdLargeNumber_IsValid()
    {
        // Arrange
        var command = new CreateExampleCommand(Id: 999999);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.IsTrue(result.IsValid);
    }
}
```

---

## 📊 Validación en Pipeline

```
Request: CreateExampleCommand(Id: 0)
    ↓
ValidationBehaviour Intercepts
    ↓
Run CreateExampleCommandValidator
    ↓
RuleFor(x => x.Id).GreaterThan(0) → FAILS
    ↓
TryCreateResultFailure("Id: Id must be greater than 0")
    ↓
Return Result<int>.Failure()
    ↓
Handler NEVER CALLED ✅
    ↓
Response: Result.IsSuccess = false
```

---

## 💡 Mejores Prácticas

### **En Aplicaciones AI**

```csharp
// ✅ CORRECTO: Validación consciente
var command = new CreateExampleCommand(Id: Math.Abs(userInput));
var result = await mediator.Send(command);

if (!result.IsSuccess)
{
    aiAgent.LogValidationFailure(result.Error);
}

// ❌ EVITAR: No validar antes
var command = new CreateExampleCommand(Id: userInput); // Podría ser inválido
```

### **En Batch Processing**

```csharp
// ✅ CORRECTO: Recolectar errores sin excepciones
var results = new List<Result<int>>();

foreach (var id in idList)
{
    var command = new CreateExampleCommand(Id: id);
    var result = await mediator.Send(command);
    
    if (!result.IsSuccess)
        validationErrors.Add(id, result.Error);
    else
        results.Add(result);
}

// Procesar errores sin overhead de excepciones
```

---

## 🔄 Integración con Entity

La validación en el validator es consistente con la validación en la entity:

```csharp
// Validator (Application Layer)
RuleFor(x => x.Id).GreaterThan(0)

// Entity (Domain Layer)
public Example(int id)
{
    if (id <= 0)
        throw new ArgumentException("Id must be greater than 0");
    // ...
}
```

**Ventajas:**
- ✅ Validación temprana en pipeline
- ✅ Previene handler execution innecesaria
- ✅ Consistencia entre capas
- ✅ Mejor performance (fail fast)

---

## 📈 Flow de Validación Completo

```
User Input (Id = 0)
    ↓
CreateExampleCommand(Id: 0)
    ↓
ValidationBehaviour Pipeline
    ↓
CreateExampleCommandValidator.Validate()
    ↓
RuleFor(x => x.Id).GreaterThan(0)
    ├─ Condición: 0 > 0? NO
    └─ Resultado: FALLA
    ↓
TryCreateResultFailure()
    ├─ Detecta: TResponse = Result<int>
    └─ Retorna: Result<int>.Failure("Id: Id must be greater than 0")
    ↓
Response a Cliente
{
    "IsSuccess": false,
    "Error": "Id: Id must be greater than 0"
}
    ↓
Handler NUNCA EJECUTADO (Fail Fast ⚡)
```

---

## 🎯 Casos de Uso

### **Use Case 1: Validación Temprana**
```csharp
// Falla ANTES de handler execution
// Performance: Fasta (no database call)
var command = new CreateExampleCommand(Id: 0);
var result = await mediator.Send(command);
// Resultado: Validation error en 0.1ms
```

### **Use Case 2: Auto-Corrección por AI**
```csharp
if (!result.IsSuccess && result.Error.Contains("greater than 0"))
{
    // AI auto-corrige
    command = new CreateExampleCommand(Id: Math.Abs(id));
    result = await mediator.Send(command); // Retry
}
```

### **Use Case 3: Error Reporting**
```csharp
// Estructura clara del error
var error = result.Error; // "Id: Id must be greater than 0"
var field = error.Split(": ")[0]; // "Id"
var message = error.Split(": ")[1]; // "Id must be greater than 0"
```

---

## ✅ Verificación

```
Build Status: ✅ PASSING
Validation: ✅ IMPLEMENTED
Error Code: ✅ "IdInvalid"
Message: ✅ "Id must be greater than 0"
Severity: ✅ Error
Documentation: ✅ COMPLETE
```

---

## 📚 Relacionado

Para más información sobre validación y Result Pattern:
- Ver: `RESULT_PATTERN_IMPLEMENTATION.md`
- Ver: `RESULT_PATTERN_AI_EXAMPLES.md`
- Ver: `ValidationBehaviour.cs` comments

---

**Date:** 2025-03-12
**Status:** ✅ Complete
**Build:** ✅ Passing
