using System;
using AnquetteLTD.Models;
using Xunit;

namespace AnquetteLTD.Tests;
public class PollLogicTests
{
    [Theory]
    // IsEnabled,  start,                     end,                       now,                        expected
    [InlineData(true, null, null, "2025-05-10T12:00:00Z", true)]
    [InlineData(false, null, null, "2025-05-10T12:00:00Z", false)]
    [InlineData(false, "2025-05-10T00:00:00Z", "2025-05-11T00:00:00Z", "2025-05-10T12:00:00Z", true)]  // window overrides flag
    [InlineData(true, "2025-05-12T00:00:00Z", "2025-05-13T00:00:00Z", "2025-05-10T12:00:00Z", false)] // not yet started
    [InlineData(true, "2025-05-08T00:00:00Z", "2025-05-09T00:00:00Z", "2025-05-10T12:00:00Z", false)] // already ended
    public void IsPollOpen_ReturnsExpected(
        bool isEnabled,
        string? startUtc,
        string? endUtc,
        string testNowUtc,
        bool expected)
    {
        var poll = new Anquette
        {
            IsEnabled = isEnabled,
            StartsAt = startUtc != null ? DateTimeOffset.Parse(startUtc) : (DateTimeOffset?)null,
            EndsAt = endUtc != null ? DateTimeOffset.Parse(endUtc) : (DateTimeOffset?)null
        };

        bool actual = IsPollOpen(poll, DateTimeOffset.Parse(testNowUtc));

        Assert.Equal(expected, actual);
    }

    // helper replicates the private method in the controller
    private static bool IsPollOpen(Anquette q, DateTimeOffset now)
    {
        if (q.StartsAt is null && q.EndsAt is null)
            return q.IsEnabled;

        if (q.StartsAt is { } start && now < start) return false;
        if (q.EndsAt is { } end && now > end) return false;

        return true;
    }
}