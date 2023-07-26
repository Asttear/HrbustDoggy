namespace Hrbust;

/// <summary>
/// 课程信息。
/// </summary>
/// <param name="Title">课程名。</param>
/// <param name="Num">课程序号。</param>
/// <param name="Location">上课地点。</param>
/// <param name="Teacher">任课老师。</param>
/// <param name="Schedule">课程安排。</param>
/// <param name="Description">其他描述。</param>
public readonly record struct Course(string Title, int Num, string Location, string Teacher, Schedule Schedule, string[] Description)
{
    /// <summary>
    /// 检查指定学周是否在该课程安排内。
    /// </summary>
    /// <param name="week">要检查的学周。</param>
    /// <returns>若为<see cref="true "/>，表示该学周在此课程安排内，否则返回<see cref="false"/>。</returns>
    public bool CheckIfScheduled(int week)
    {
        if (week < Schedule.Start || week > Schedule.End)
        {
            return false;
        }
        return Schedule.Type switch
        {
            ScheduleType.EveryWeek => true,
            ScheduleType.OddNumberedWeeks => week % 2 != 0,
            ScheduleType.EvenNumberedWeeks => week % 2 == 0,
            _ => throw new ArgumentOutOfRangeException(null, "Oops!")
        };
    }

    /// <summary>
    /// 将该课程信息转化为字符串形式。
    /// </summary>
    /// <returns>字符串形式的课程信息。</returns>
    public override string ToString() => $"{Title}\n@{Location}";
}
