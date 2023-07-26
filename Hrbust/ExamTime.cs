using System.Diagnostics.CodeAnalysis;

namespace Hrbust;

/// <summary>
/// 考试时间。
/// </summary>
/// <param name="Start">考试开始时间。</param>
/// <param name="Span">考试持续时间。</param>
public readonly record struct ExamTime(DateTime Start, TimeSpan Span) : IParsable<ExamTime>
{
    public DateTime End => Start + Span;

    public override string ToString() => $"{Start:yyyy/MM/dd HH:mm}-{End:t}";

    public static ExamTime Parse(string s, IFormatProvider? provider = null)
    {
        string[] d = s.Split("--");
        if (d.Length != 2)
        {
            throw new FormatException();
        }
        DateTime start = DateTime.Parse(d[0]);
        TimeOnly end = TimeOnly.Parse(d[1]);
        return new ExamTime(start, end - TimeOnly.FromDateTime(start));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ExamTime result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }
        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
