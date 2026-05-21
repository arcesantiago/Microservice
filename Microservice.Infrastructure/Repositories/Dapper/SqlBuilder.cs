// Microservice.Infrastructure/Persistence/Dapper/SqlBuilder.cs

using Dapper;
using Microservice.Domain.Common;

public static class SqlBuilder
{
    /// <summary>
    /// Construye cláusula WHERE desde los filtros del QueryParameters.
    /// NUNCA interpola strings directamente — todo va por parámetros Dapper.
    /// </summary>
    public static (string WhereClause, DynamicParameters Params) BuildWhere<T>(
        QueryParameters<T> parameters) where T : BaseDomainModel
    {
        if (parameters.Filters.Count == 0)
            return ("", new DynamicParameters());

        var conditions = new List<string>();
        var dbParams = new DynamicParameters();

        foreach (var (column, value) in parameters.Filters)
        {
            // Validación: solo se permiten nombres de columna alfanuméricos
            if (!IsValidColumnName(column))
                throw new ArgumentException($"Invalid column name: {column}");

            if (value is null)
            {
                conditions.Add($"{column} IS NULL");
            }
            else
            {
                conditions.Add($"{column} = @{column}");
                dbParams.Add(column, value);
            }
        }

        return ($"WHERE {string.Join(" AND ", conditions)}", dbParams);
    }

    public static string BuildOrderBy<T>(QueryParameters<T> parameters)
        where T : BaseDomainModel
    {
        if (string.IsNullOrWhiteSpace(parameters.OrderByColumn)) return "";
        if (!IsValidColumnName(parameters.OrderByColumn))
            throw new ArgumentException($"Invalid column name: {parameters.OrderByColumn}");

        var direction = parameters.OrderDescending ? "DESC" : "ASC";
        return $"ORDER BY {parameters.OrderByColumn} {direction}";
    }

    public static string BuildPagination(int page, int pageSize)
    {
        var offset = (page - 1) * pageSize;
        return $"LIMIT {pageSize} OFFSET {offset}";
    }

    public static string BuildColumnList(IReadOnlyList<string>? columns)
    {
        if (columns is null || columns.Count == 0) return "*";
        foreach (var col in columns)
            if (!IsValidColumnName(col))
                throw new ArgumentException($"Invalid column name: {col}");
        return string.Join(", ", columns);
    }

    // Previene SQL injection en nombres de columna
    public static bool IsValidColumnName(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        name.All(c => char.IsLetterOrDigit(c) || c == '_');
}