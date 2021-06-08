// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
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
    public class TemporalQueryRootExpression : QueryRootExpression
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TemporalQueryRootExpression(
            IEntityType entityType,
            DateTime? pointInTime,
            DateTime? from,
            DateTime? to,
            TemporalOperationType temporalOperationType)
            : base(entityType)
        {
            PointInTime = pointInTime;
            From = from;
            To = to;
            TemporalOperationType = temporalOperationType;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public TemporalQueryRootExpression(
            IAsyncQueryProvider queryProvider,
            IEntityType entityType,
            DateTime? pointInTime,
            DateTime? from,
            DateTime? to,
            TemporalOperationType temporalOperationType)
            : base(queryProvider, entityType)
        {
            PointInTime = pointInTime;
            From = from;
            To = to;
            TemporalOperationType = temporalOperationType;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DateTime? PointInTime { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DateTime? From { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DateTime? To { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TemporalOperationType TemporalOperationType { get; }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override Expression DetachQueryProvider()
            => new TemporalQueryRootExpression(EntityType, PointInTime, From, To, TemporalOperationType);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
            => this;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected override void Print(ExpressionPrinter expressionPrinter)
        {
            base.Print(expressionPrinter);
            switch (TemporalOperationType)
            {
                case TemporalOperationType.AsOf:
                    expressionPrinter.Append($".TemporalAsOf({PointInTime})");
                    break;

                case TemporalOperationType.FromTo:
                    expressionPrinter.Append($".TemporalFromTo({From}, {To})");
                    break;

                case TemporalOperationType.Between:
                    expressionPrinter.Append($".TemporalBetween({From}, {To})");
                    break;

                case TemporalOperationType.ContainedIn:
                    expressionPrinter.Append($".TemporalContainedIn({From}, {To})");
                    break;

                default:
                    expressionPrinter.Append($".TemporalAll()");
                    break;
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override bool Equals(object? obj)
            => obj != null
                && (ReferenceEquals(this, obj)
                    || obj is TemporalQueryRootExpression queryRootExpression
                    && Equals(queryRootExpression));

        private bool Equals(TemporalQueryRootExpression queryRootExpression)
            => base.Equals(queryRootExpression)
                && Equals(PointInTime, queryRootExpression.PointInTime);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(base.GetHashCode());
            hashCode.Add(PointInTime);

            return hashCode.ToHashCode();
        }
    }
}
