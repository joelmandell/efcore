// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Sql Server database specific extension methods for LINQ queries.
    /// </summary>
    public static class SqlServerQueryableExtensions
    {
        private static readonly MethodInfo AsNoTrackingMethodInfo
            = typeof(EntityFrameworkQueryableExtensions)
                .GetRequiredDeclaredMethod(nameof(EntityFrameworkQueryableExtensions.AsNoTracking));

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public static IQueryable<TEntity> TemporalAsOf<TEntity>(
            this IQueryable<TEntity> source,
            DateTime pointInTime)
        {
            var queryableSource = (IQueryable)source;

            return queryableSource.Provider.CreateQuery<TEntity>(
                GenerateTemporalAsOfQueryRoot<TEntity>(
                    queryableSource,
                    pointInTime));
        }

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public static IQueryable<TEntity> TemporalFromTo<TEntity>(
            this IQueryable<TEntity> source,
            DateTime from,
            DateTime to)
        {
            var queryableSource = (IQueryable)source;

            return queryableSource.Provider.CreateQuery<TEntity>(
                GenerateRangeTemporalQueryRoot<TEntity>(
                    queryableSource,
                    from,
                    to,
                    TemporalOperationType.FromTo));
        }

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public static IQueryable<TEntity> TemporalBetween<TEntity>(
            this IQueryable<TEntity> source,
            DateTime from,
            DateTime to)
        {
            var queryableSource = (IQueryable)source;

            return queryableSource.Provider.CreateQuery<TEntity>(
                GenerateRangeTemporalQueryRoot<TEntity>(
                    queryableSource,
                    from,
                    to,
                    TemporalOperationType.Between));
        }

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public static IQueryable<TEntity> TemporalContainedIn<TEntity>(
            this IQueryable<TEntity> source,
            DateTime from,
            DateTime to)
        {
            var queryableSource = (IQueryable)source;

            return queryableSource.Provider.CreateQuery<TEntity>(
                GenerateRangeTemporalQueryRoot<TEntity>(
                    queryableSource,
                    from,
                    to,
                    TemporalOperationType.ContainedIn));
        }

        /// <summary>
        ///     TODO: add comments
        /// </summary>
        public static IQueryable<TEntity> TemporalAll<TEntity>(
            this IQueryable<TEntity> source)
        {
            var queryableSource = (IQueryable)source;
            var queryRootExpression = (QueryRootExpression)source.Expression;
            var entityType = queryRootExpression.EntityType;

            var temporalQueryRootExpression = queryRootExpression.QueryProvider != null
                ? new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider,
                    entityType,
                    pointInTime: null,
                    from: null,
                    to: null,
                    temporalOperationType: TemporalOperationType.All)
                : new TemporalQueryRootExpression(
                    entityType,
                    pointInTime: null,
                    from: null,
                    to: null,
                    temporalOperationType: TemporalOperationType.All);

            return queryableSource.Provider.CreateQuery<TEntity>(
                Expression.Call(
                    instance: null,
                    method: AsNoTrackingMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    arguments: temporalQueryRootExpression));
        }

        private static Expression GenerateTemporalAsOfQueryRoot<TEntity>(
            IQueryable source,
            DateTime pointInTime)
        {
            var queryRootExpression = (QueryRootExpression)source.Expression;
            var entityType = queryRootExpression.EntityType;

            var temporalQueryRootExpression = queryRootExpression.QueryProvider != null
                ? new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider,
                    entityType,
                    pointInTime: pointInTime,
                    from: null,
                    to: null,
                    temporalOperationType: TemporalOperationType.AsOf)
                : new TemporalQueryRootExpression(
                    entityType,
                    pointInTime: pointInTime,
                    from: null,
                    to: null,
                    temporalOperationType: TemporalOperationType.AsOf);

            return Expression.Call(
                instance: null,
                method: AsNoTrackingMethodInfo.MakeGenericMethod(typeof(TEntity)),
                arguments: temporalQueryRootExpression);

            //return queryRootExpression.QueryProvider != null
            //    ? new TemporalQueryRootExpression(
            //        queryRootExpression.QueryProvider,
            //        entityType,
            //        pointInTime: pointInTime,
            //        from: null,
            //        to: null,
            //        temporalOperationType: TemporalOperationType.AsOf)
            //    : new TemporalQueryRootExpression(
            //        entityType,
            //        pointInTime: pointInTime,
            //        from: null,
            //        to: null,
            //        temporalOperationType: TemporalOperationType.AsOf);

            //return new TemporalQueryRootExpression(
            //    queryRootExpression.QueryProvider!,
            //    entityType,
            //    pointInTime);
        }

        private static Expression GenerateRangeTemporalQueryRoot<TEntity>(
            IQueryable source,
            DateTime from,
            DateTime to,
            TemporalOperationType temporalOperationType)
        {
            var queryRootExpression = (QueryRootExpression)source.Expression;
            var entityType = queryRootExpression.EntityType;

            var temporalQueryRootExpression = queryRootExpression.QueryProvider != null
                ? new TemporalQueryRootExpression(
                    queryRootExpression.QueryProvider,
                    entityType,
                    pointInTime: null,
                    from: from,
                    to: to,
                    temporalOperationType: temporalOperationType)
                : new TemporalQueryRootExpression(
                    entityType,
                    pointInTime: null,
                    from: from,
                    to: to,
                    temporalOperationType: temporalOperationType);

            // TODO: is there more elegant way to set tracking on provider level?
            return Expression.Call(
                instance: null,
                method: AsNoTrackingMethodInfo.MakeGenericMethod(typeof(TEntity)),
                arguments: temporalQueryRootExpression);

            //return queryRootExpression.QueryProvider != null
            //    ? new TemporalQueryRootExpression(
            //        queryRootExpression.QueryProvider,
            //        entityType,
            //        pointInTime: null,
            //        from: from,
            //        to: to,
            //        temporalOperationType: temporalOperationType)
            //    : new TemporalQueryRootExpression(
            //        entityType,
            //        pointInTime: null,
            //        from: from,
            //        to: to,
            //        temporalOperationType: temporalOperationType);

        }
    }
}
