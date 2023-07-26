namespace Hrbust;

/// <summary>
/// 表示一节课，或一节课可能安排的课程。
/// </summary>
public class Class : List<Course>
{
    /// <summary>
    /// 构造一节空课。
    /// </summary>
    public Class() : base() { }

    /// <summary>
    /// 通过课程构造一节课。
    /// </summary>
    /// <param name="courses">课程。</param>
    public Class(IEnumerable<Course> courses) : base(courses) { }

    /// <summary>
    /// 获取该节课在指定学周安排的课程。
    /// </summary>
    /// <param name="week">学周。</param>
    /// <returns>一节安排的课程。如果该学周无课程安排，返回<see cref="null"/>。</returns>
    /// <remarks>如果有撞课（多节课程安排在同一时间），则仅返回集合中第一个对象。</remarks>
    public Course? FirstScheduledAt(int week)
    {
        foreach (var course in this)
        {
            if (course.CheckIfScheduled(week))
            {
                return course;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取该节课在指定学周安排的课程。
    /// </summary>
    /// <param name="week"></param>
    /// <returns>安排的课程。如果该学周无课程安排，返回<see cref="null"/>。</returns>
    /// <remarks>以列表形式返回撞课的课程。</remarks>
    public List<Course> ScheduledAt(int week)
    {
        return this.Where(c => c.CheckIfScheduled(week)).ToList();
    }
}
