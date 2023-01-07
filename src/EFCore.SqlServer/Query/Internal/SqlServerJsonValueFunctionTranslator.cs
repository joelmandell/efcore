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
public class SqlServerJsonValueFunctionTranslator : IMethodCallTranslator
{

    private static readonly MethodInfo MethodInfo
          = typeof(SqlServerDbFunctionsExtensions).GetRuntimeMethod(
              nameof(SqlServerDbFunctionsExtensions.JsonValue), new[] { typeof(DbFunctions), typeof(object), typeof(string) })!;

    private readonly ISqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public SqlServerJsonValueFunctionTranslator(
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
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var propertyReference = arguments[1];

        var typeMapping = propertyReference.TypeMapping;
        var path = propertyReference.Type == arguments[2].Type
            ? _sqlExpressionFactory.ApplyTypeMapping(arguments[2], typeMapping)
            : _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]);

        var functionArguments = new List<SqlExpression> { propertyReference, path };

        return _sqlExpressionFactory.Function(
            "JSON_VALUE",
            functionArguments,
            nullable: true,
            // TODO: don't propagate for now
            argumentsPropagateNullability: functionArguments.Select(_ => false).ToList(),
            typeof(string));
    }
}
