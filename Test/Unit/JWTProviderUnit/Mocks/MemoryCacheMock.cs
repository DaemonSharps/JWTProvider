using Microsoft.Extensions.Caching.Memory;

namespace Mocks;

internal class MemoryCacheMock
{
    public readonly Mock<IMemoryCache> _memoryCache = new();
    private readonly Mock<ICacheEntry> _cacheEntry = new();
    private object _keyPayload;
    private object _valuePayload;
    private TimeSpan? _expirationPayload;

    public MemoryCacheMock()
    {
        _memoryCache
            .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
            .Callback((object k) => _keyPayload = k)
            .Returns(_cacheEntry.Object); // this should address your null reference exception

        _cacheEntry
            .SetupSet(mce => mce.Value = It.IsAny<object>())
            .Callback<object>(v => _valuePayload = v);

        _cacheEntry
            .SetupSet(mce => mce.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>())
            .Callback<TimeSpan?>(dto => _expirationPayload = dto);
    }

    public IMemoryCache Object => _memoryCache.Object;

    public TKey Verify<TKey, TValue>(TValue expectedValue, TimeSpan? expectedExpiration)
        where TValue : class
        where TKey : struct
    {
        Assert.True(_keyPayload is TKey);
        Assert.Equal(expectedValue, _valuePayload as TValue);
        Assert.Equal(expectedExpiration, _expirationPayload);

        return (TKey)_keyPayload;
    }
}
