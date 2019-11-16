﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    public interface ISource<T>
    {
        Task PrepareAsync(IDictionary<string, string> parameters);
        Task<IEnumerable<T>> GetAsync(int batchSize);
        Task CleanupAsync();
    }
}
