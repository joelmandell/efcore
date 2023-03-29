// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class SqlServerJsonFunctionsTranslator : IMethodCallTranslator
{
    private readonly Dictionary<MethodInfo, string> _methodInfoJsonFunctions
        = new()
        {
            {
                typeof(SqlServerDbFunctionsExtensions).GetRuntimeMethod(nameof(SqlServerDbFunctionsExtensions.JsonValue), new[] { typeof(DbFunctions), typeof(object), typeof(string) })!,
                "JSON_VALUE"
            },
            {
                typeof(SqlServerDbFunctionsExtensions).GetRuntimeMethod(nameof(SqlServerDbFunctionsExtensions.JsonQuery), new[] { typeof(DbFunctions), typeof(object), typeof(string) })!,
                "JSON_QUERY"
            },
        };

    private readonly ISqlExpressionFactory _sqlExpressionFactory;


    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SqlServerJsonFunctionsTranslator(
        ISqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (_methodInfoJsonFunctions.TryGetValue(method, out var function))
        {
            var expression = arguments[1];

            var path = arguments[2];

            //Do not allow null as path arg. Pass '$' as default path.
            if (((SqlConstantExpression)path).Value == null) path = _sqlExpressionFactory.Constant("$");

            var functionArguments = new List<SqlExpression> { expression, path };

            return _sqlExpressionFactory.Function(
                function,
                functionArguments,
                nullable: true,
                argumentsPropagateNullability: functionArguments.Select(_ => true).ToList(),
                typeof(string));
        }

        return null;
    }
}
