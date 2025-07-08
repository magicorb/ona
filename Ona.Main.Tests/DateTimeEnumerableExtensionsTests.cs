using Ona.Model;

namespace Ona.Main.Tests;

public class DateTimeEnumerableExtensionsTests
{
    [Fact]
    public void GetCycles()
    {
        DateTime[] dates = 
        [
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 2),
            new DateTime(2024, 1, 5),
            new DateTime(2024, 1, 16),
            new DateTime(2024, 1, 19),
            new DateTime(2024, 1, 23),
            new DateTime(2024, 1, 24)
        ];

        DateTime[][] expectedCycles =
        [
            [
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 2),
                new DateTime(2024, 1, 5),
                new DateTime(2024, 1, 16),
                new DateTime(2024, 1, 19),
            ],
            [
                new DateTime(2024, 1, 23),
                new DateTime(2024, 1, 24)
            ]
        ];

        var cycles = dates.GetCycles();

        Assert.Equal(expectedCycles, cycles);
    }

    [Theory]
    [MemberData(nameof(DatesData))]
    public void GetPeriodLength(DateTime[] dates, int expectedLength)
    {
        Assert.Equal(dates.GetPeriodLength(), expectedLength);
    }

    public static IEnumerable<object[]> DatesData { get; } = 
    [
        [new DateTime[] { }, 5],
        [new DateTime[] { new DateTime(2024, 1, 1) }, 1],
        [new DateTime[] { new DateTime(2024, 1, 1), new DateTime(2024, 1, 2) }, 2],
        [new DateTime[] { new DateTime(2024, 1, 1), new DateTime(2024, 1, 4) }, 2],
        [new DateTime[] { new DateTime(2024, 1, 1), new DateTime(2024, 1, 5) }, 1],
        [new DateTime[] { new DateTime(2024, 1, 1), new DateTime(2024, 1, 2), new DateTime(2024, 1, 3), new DateTime(2024, 1, 4), new DateTime(2024, 1, 5) }, 5]
    ];
}