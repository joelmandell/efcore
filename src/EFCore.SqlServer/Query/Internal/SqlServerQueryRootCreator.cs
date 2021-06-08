// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore.SqlServer.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class SqlServerQueryRootCreator : QueryRootCreator
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override QueryRootExpression CreateQueryRoot(IEntityType entityType, QueryRootExpression? source)
        {
            if (source is TemporalQueryRootExpression tqre)
            {
                if (!entityType.GetRootType().IsTemporal())
                {
                    throw new InvalidOperationException($"Temporal query is trying to use navigation to an entity '{entityType.DisplayName()}' which itself doesn't map to temporal table. Either map the entity to temporal table or use join manually to access it.");
                }

                if (tqre.TemporalOperationType != TemporalOperationType.AsOf)
                {
                    throw new InvalidOperationException($"Navigation expansion is only supported for '{nameof(TemporalOperationType.AsOf)}' temporal operation. For other operations use join manually.");
                }

                return source.QueryProvider != null
                    ? new TemporalQueryRootExpression(source.QueryProvider, entityType, tqre.PointInTime, tqre.From, tqre.To, tqre.TemporalOperationType)
                    : new TemporalQueryRootExpression(entityType, tqre.PointInTime, tqre.From, tqre.To, tqre.TemporalOperationType);
            }

            if (entityType.GetRootType().IsTemporal()
                && source is null)
            {
                throw new InvalidOperationException($"Couldn't create a temporal query root for the entity type: '{entityType.DisplayName()}'.");
            }

            return base.CreateQueryRoot(entityType, source);
        }
    }
}
