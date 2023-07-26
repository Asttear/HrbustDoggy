using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Hrbust;

/// <summary>
/// 表示课程安排。
/// </summary>
/// <param name="RawText">原始文本。</param>
/// <param name="Start">开始周。</param>
/// <param name="End">结束周</param>
/// <param name="Type">单双周</param>
public readonly partial record struct Schedule(string RawText, int Start, int End, ScheduleType Type) : IParsable<Schedule>
{
    /// <summary>
    /// 原始文本。
    /// </summary>
    /// <returns>原始文本字符串。</returns>
    public override string ToString() => RawText;

    [GeneratedRegex(@"(\d+)-(\d+)周")]
    private static partial Regex s_regex1();

    [GeneratedRegex(@"第(\d+)周")]
    private static partial Regex s_regex2();

    [GeneratedRegex(@"(\d+)-(\d+)([单双])周")]
    private static partial Regex s_regex3();

    public static Schedule Parse(string s, IFormatProvider? provider = null)
    {
        Regex[] regices = { s_regex1(), s_regex2(), s_regex3() };
        Match? match = null;
        int m;
        for (m = 0; m < regices.Length; m++)
        {
            match = regices[m].Match(s);
            if (!match.Success)
            {
                continue;
            }
            break;
        }
        if (match is null) { throw new Exception("Oops!"); }
        Schedule? result = null;
        switch (m)
        {
            case 0:
                {
                    int start = int.Parse(match.Groups[1].Value);
                    int end = int.Parse(match.Groups[2].Value);
                    result = new(s, start, end, ScheduleType.EveryWeek);
                }
                break;

            case 1:
                {
                    int start = int.Parse(match.Groups[1].Value);
                    int end = start;
                    result = new(s, start, end, ScheduleType.EveryWeek);
                }
                break;

            case 2:
                {
                    int start = int.Parse(match.Groups[1].Value);
                    int end = int.Parse(match.Groups[2].Value);
                    ScheduleType type = match.Groups[3].Value switch
                    {
                        "单" => ScheduleType.OddNumberedWeeks,
                        "双" => ScheduleType.EvenNumberedWeeks,
                        _ => throw new NotImplementedException()
                    };
                    result = new(s, start, end, type);
                }
                break;
        }
        if (!result.HasValue) { throw new FormatException(); }
        return result.Value;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Schedule result)
    {
        if (string.IsNullOrWhiteSpace(s))
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
