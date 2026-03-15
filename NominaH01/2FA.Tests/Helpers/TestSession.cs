using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _2FA.Tests.Helpers
{
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();
        public string Id { get; } = Guid.NewGuid().ToString();
        public bool IsAvailable { get; } = true;
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);

        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
    }
}
