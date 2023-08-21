using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace Hrbust;

public class HrbustClient
{
    private const string BaseUrl = "http://jwzx.hrbust.edu.cn/academic/";
    private const string CaptchaUrl = "getCaptcha.do";
    private const string CheckCaptchaUrl = "checkCaptcha.do";
    private const string ClassTableUrl = "manager/coursearrange/showTimetable.do";
    private const string CourseArrangementUrl = "student/currcourse/currcourse.jsdo";
    private const string ExamsUrl = "manager/examstu/studentQueryAllExam.do?sortColumnVLID=s.examRoom.exam.endTime&pagingPageVLID=1&sortDirectionVLID=-1&pagingNumberPerVLID=100";
    private const string IndexListLeftUrl = "listLeft.do";
    private const string LogoutCheckUrl = "logout_security_check";
    private const string SecurityCheckUrl = "j_acegi_security_check";
    private const string StudentInfoUrl = "student/studentinfo/studentInfoModifyIndex.do?frombase=0&wantTag=0";

    // TODO: 不使用硬编码时间
    private static readonly string[][] s_strTimes =
    {
        new[] { "08:00", "09:50", "13:30", "15:30", "18:10", "19:50" },
        new[] { "08:05", "10:10", "13:35", "15:45", "18:15", "19:55" },
        new[] { "08:10", "10:30", "13:40", "16:00", "18:20", "20:00" },
        new[] { "08:10", "10:30", "13:40", "16:00", "18:20", "20:00" },
    };

    /// <summary>
    /// 客户端构造函数。
    /// </summary>
    public HrbustClient()
    {
        // 启用 GBK 编码支持
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        HttpClientHandler httpClientHandler = new() { UseCookies = true };
        HttpClient = new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    /// <summary>
    /// 获取用于访问教务系统的 HttpClient 对象。
    /// </summary>
    public HttpClient HttpClient { get; private set; }

    /// <summary>
    /// 获取指示当前是否已登录的布尔值。
    /// </summary>
    public bool IsLoggedIn { get; private set; }

    /// <summary>
    /// 获取验证码。
    /// </summary>
    /// <returns>验证码图像流。</returns>
    /// <remarks>此方法不需要用户登录即可使用。</remarks>
    public async Task<Stream> GetCaptchaAsync()
    {
        var response = await HttpClient.GetAsync(CaptchaUrl);
        return response.Content.ReadAsStream();
    }

    /// <summary>
    /// 用户登录。
    /// </summary>
    /// <param name="username">用户名。</param>
    /// <param name="password">密码。</param>
    /// <param name="captchaCode">验证码。</param>
    /// <returns>登录结果。</returns>
    public async Task<LoginResult> LoginAsync(string username, string password, string captchaCode)
    {
        if (IsLoggedIn)
        {
            throw new InvalidOperationException("不能重复登录！");
        }
        HttpResponseMessage captchaResponse = await HttpClient.PostAsync($"{CheckCaptchaUrl}?captchaCode={captchaCode}", null);
        bool captchaOk = Convert.ToBoolean(await captchaResponse.Content.ReadAsStringAsync());
        if (!captchaOk)
        {
            return LoginResult.CaptchaError;
        }

        KeyValuePair<string, string>[] nameValueCollection =
        {
            new("j_username", username),
            new("j_password", password),
            new("j_captcha", captchaCode)
        };
        FormUrlEncodedContent content = new(nameValueCollection);
        HttpResponseMessage loginResponse = await HttpClient.PostAsync(SecurityCheckUrl, content);

        if (loginResponse.Content.Headers.ContentLength != 4000)
        {
            return LoginResult.CredentialError;
        }
        IsLoggedIn = true;
        return LoginResult.Success;
    }

    /// <summary>
    /// 用户注销。
    /// </summary>
    public async Task LogoutAsync()
    {
        if (!IsLoggedIn)
        {
            return;
        }
        try
        {
            await HttpClient.GetAsync(LogoutCheckUrl);
        }
        catch
        {
            // 啥也不干，当作注销了吧。
        }
        finally
        {
            IsLoggedIn = false;
        }
    }

    /// <summary>
    /// 获取课程表。
    /// </summary>
    /// <param name="style">课表样式。</param>
    /// <returns>包含有课程表数据的 Task 对象。</returns>
    /// <exception cref="InvalidOperationException">用户未登录。</exception>
    public async Task<ClassTable> GetClassTableAsync(ClassTableStyle style = ClassTableStyle.Combine)
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException("未登录！");
        }

        UserArgs userArgs = await GetUserArgsAsync();
        List<List<IEnumerable<Course>>> classes = await GetCourseListAsync(
            userArgs.YearId,
            userArgs.TermId,
            userArgs.StudentId,
            style == ClassTableStyle.Base ? "BASE" : "COMBINE");
        List<TimeOnly> times = await GetClassTimesAsync();
        int? week = await GetWeekAsync();

        ClassTable table = new(style, TimeSpan.FromMinutes(85), DateOnly.FromDateTime(DateTime.Now), week);
        for (int i = 0; i < classes.Count; i++)
        {
            for (int j = 0; j < classes[i].Count; j++)
            {
                table[j, i].AddRange(classes[i][j]);
            }
        }
        for (int i = 0; i < table.NumberOfClassesPerDay; i++)
        {
            table.StartTimes[i] = times[i];
        }
        return table;
    }

    /// <summary>
    /// 获取考试信息。
    /// </summary>
    /// <returns>包含考试信息的数组。</returns>
    /// <exception cref="InvalidOperationException">用户未登录。</exception>
    public async Task<Exam[]> GetExamsAsync()
    {
        if (!IsLoggedIn)
        {
            throw new InvalidOperationException("未登录。");
        }

        Stream response = await HttpClient.GetStreamAsync(ExamsUrl);
        HtmlDocument html = new();
        html.Load(response);
        IEnumerable<Exam> exams = html.DocumentNode.SelectNodes("/body/center/table[2]/tr/td/table/tr")
            .Skip(1)
            .Select(examNode =>
            {
                List<string> examStrs = examNode.SelectNodes("td")
                    .Select(examDataNode => WebUtility.HtmlDecode(examDataNode.InnerText).Trim())
                    .ToList();
                return new Exam(examStrs[0], examStrs[1], ExamTime.Parse(examStrs[2]), examStrs[3].Split(' ')[2], examStrs[4]);
            });
        return exams.ToArray();
    }

    /// <summary>
    /// 获取当前学周。
    /// </summary>
    /// <returns>包含有当前学周信息的 <see cref="Task"/> 对象，如果无法获取，则结果为 <see langword="null"/>。</returns>
    /// <remarks>此方法不需要用户登录即可使用。</remarks>
    public async Task<int?> GetWeekAsync()
    {
        Stream response = await HttpClient.GetStreamAsync(IndexListLeftUrl);
        HtmlDocument html = new();
        html.Load(response, Encoding.GetEncoding("GBK"));
        string[] dateInfo = html.DocumentNode.SelectSingleNode("//*[@id=\"date\"]/p").InnerText
            .Replace("&nbsp;", "")
            .Replace("\n", "")
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (dateInfo.Length != 4)
        {
            return null;
        }
        int week = int.Parse(dateInfo[3][1..^1]);
        return week;
    }

    /// <summary>
    /// 获取课程表课程。
    /// </summary>
    /// <param name="yearId">学年参数。</param>
    /// <param name="termId">学期参数。</param>
    /// <param name="studentId">学生ID参数。</param>
    /// <param name="sectionType">课表类型参数。</param>
    /// <returns>课程信息列表。索引顺序为“一节课/一天/一门课程”。</returns>
    private async Task<List<List<IEnumerable<Course>>>> GetCourseListAsync(string? yearId, string? termId, string studentId, string sectionType)
    {
        string tableUrl = $"{ClassTableUrl}?id={studentId}&yearid={yearId}&termid={termId}&timetableType=STUDENT&sectionType={sectionType}";
        Stream response = await HttpClient.GetStreamAsync(tableUrl);
        HtmlDocument html = new();
        html.Load(response, Encoding.GetEncoding("GBK"));

        // MAGIC!!!
        List<List<IEnumerable<Course>>> classes = html.DocumentNode.SelectNodes("//table[@id='timetable']/tr[@class='infolist_hr_common']")
            .Select(n => n.SelectNodes("./td")
                .Select(n =>
                {
                    string classStr = WebUtility.HtmlDecode(n.InnerHtml).Trim();
                    if (string.IsNullOrEmpty(classStr))
                    {
                        return Enumerable.Empty<Course>();
                    }
                    return classStr.Replace(">>", "").Split("<<", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Split("<br>", StringSplitOptions.RemoveEmptyEntries))
                        .Select(courseStrs =>
                        {
                            string[] titleAndNum = courseStrs[0].Split(';');
                            return new Course
                            (
                                Title: titleAndNum[0],
                                Num: int.Parse(titleAndNum[1]),
                                Location: courseStrs[1],
                                Teacher: courseStrs[2],
                                Schedule: Schedule.Parse(courseStrs[3]),
                                Description: courseStrs[4..]
                            );
                        });
                })
                .ToList())
            .ToList();
        return classes;
    }

    /// <summary>
    /// 获取课程时间表。
    /// </summary>
    /// <returns>课程时间表。</returns>
    /// <exception cref="InvalidOperationException">用户未登录。</exception>
    private async Task<List<TimeOnly>> GetClassTimesAsync()
    {
        Stream response = await HttpClient.GetStreamAsync(StudentInfoUrl);
        HtmlDocument html = new();
        html.Load(response, Encoding.GetEncoding("UTF-8"));
        int gradeYear = Convert.ToInt32(html.DocumentNode.SelectSingleNode("//*[@id='gradeChange']").InnerText[0..4]);
        DateTime now = DateTime.Now;
        int semester = now.Month < 8 ? 0 : 1;
        int grade = now.Year - gradeYear + semester;
        List<TimeOnly> times = s_strTimes[grade - 1].Select(TimeOnly.Parse).ToList();
        return times;
    }

    /// <summary>
    /// 获取当前教务系统用户参数。
    /// </summary>
    /// <returns>用户参数对象。</returns>
    /// <exception cref="InvalidOperationException">用户未登录。</exception>
    /// <exception cref="KeyNotFoundException">网站页面结构已更改，需重写爬虫逻辑。</exception>
    private async Task<UserArgs> GetUserArgsAsync()
    {
        Stream response = await HttpClient.GetStreamAsync(CourseArrangementUrl);
        HtmlDocument html = new();
        html.Load(response);
        HtmlAttributeCollection attributes =
            html.DocumentNode.Descendants().FirstOrDefault(n => n.Name.StartsWith("eduaffair:"))?.Attributes
            ?? throw new KeyNotFoundException("请求的关键字段缺失，可能需要更新获取逻辑。");
        UserArgs userArgs = new
        (
          attributes["studentId"].Value,
          attributes["year"].Value,
          attributes["term"].Value
        );
        return userArgs;
    }
}