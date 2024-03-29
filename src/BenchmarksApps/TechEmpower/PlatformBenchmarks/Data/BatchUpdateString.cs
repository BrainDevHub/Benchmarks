// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace PlatformBenchmarks
{
    internal sealed class BatchUpdateString
    {
        private const int MaxBatch = 500;

        public static DatabaseServer DatabaseServer;

        internal static readonly string[] ParamNames = Enumerable.Range(0, MaxBatch * 2).Select(i => $"@p{i}").ToArray();

        private static string[] _queries = new string[MaxBatch + 1];

        public static string Query(int batchSize)
            => _queries[batchSize] is null
                ? CreateBatch(batchSize)
                : _queries[batchSize];

        private static string CreateBatch(int batchSize)
        {
            var sb = StringBuilderCache.Acquire();

            sb.Append("UPDATE world SET randomNumber = temp.randomNumber FROM (VALUES ");
            var c = 1;
            for (var i = 0; i < batchSize; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append($"(${c++}, ${c++})");
            }
            sb.Append(" ORDER BY 1) AS temp(id, randomNumber) WHERE temp.id = world.id");

            return _queries[batchSize] = StringBuilderCache.GetStringAndRelease(sb);
        }
    }
}
