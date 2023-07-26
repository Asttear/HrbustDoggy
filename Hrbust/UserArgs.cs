namespace Hrbust;

/// <summary>
/// 教务在线用户参数。
/// </summary>
/// <param name="StudentId">学生ID</param>
/// <param name="YearId">学年ID</param>
/// <param name="TermId">学期ID</param>
internal record struct UserArgs(string StudentId, string YearId, string TermId);
