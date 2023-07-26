namespace Hrbust;

/// <summary>
/// 考试信息。
/// </summary>
/// <param name="CourseId">课程号</param>
/// <param name="CourseName">课程名称。</param>
/// <param name="Time">考试时间。</param>
/// <param name="Location">考试地点。</param>
/// <param name="Type">考试性质。</param>
public readonly record struct Exam(string CourseId, string CourseName, ExamTime Time, string Location, string Type);

