using Hrbust;
using HrbustDoggy.Cli;
using System.Diagnostics;

HrbustClient hrbust = new();

string? username, password;
do
{
    do
    {
        Console.Write("请输入用户名：");
        username = Console.ReadLine();
    } while (username == null);
    do
    {
        Console.Write("请输入密码：");
        password = Console.ReadLine();
    } while (password == null);
} while (!await LoginAsync(username, password));

while (true)
{
    Console.Clear();
    Console.WriteLine("""
        1) 课程表
        2) 考试信息
        0) 退出
        """);
    int input;
    do
    {
        Console.Write("请输入：");
    } while (!int.TryParse(Console.ReadLine(), out input));

    switch (input)
    {
        case 0:
            return 0;
        case 1:
            await PrintClassTableAsync();
            break;
        case 2:
            await PrintExamsAsync();
            break;
        default:
            break;
    }
}

async Task<bool> LoginAsync(string username, string password)
{
    while (true)
    {
        Bitmap captchaImage = new(await hrbust.GetCaptchaAsync());
        ImageForm? form = null;
        _ = Task.Run(() =>
        {
            form = new(captchaImage);
            form.Activate();
            Application.Run(form);
        });
        string? captchaCode;
        do
        {
            Console.Write("请输入验证码：");
            captchaCode = Console.ReadLine();
        } while (captchaCode == null);
        form?.Invoke(form.Close);
        var result = await hrbust.LoginAsync(username, password, captchaCode);
        switch (result)
        {
            case LoginResult.Success:
                Console.WriteLine("登录成功！");
                await Task.Delay(1000);
                return true;
            case LoginResult.CaptchaError:
                Console.WriteLine("验证码错误！");
                continue;
            case LoginResult.CredentialError:
                Console.WriteLine("用户名或密码错误！");
                return false;
            default:
                throw new UnreachableException();
        }
    }
}

async Task PrintClassTableAsync()
{
    ClassTable table = await hrbust.GetClassTableAsync();
    DateOnly date = table.DateWhenObtained;
    int week = table.WeekWhenObtained ?? -1;
    while (true)
    {
        Console.Clear();
        Console.WriteLine($"{date:yyyy年MM月dd日，ddd}，{(week > 0 ? $"第{week}周" : "未开学")}");
        int day = ((int)date.DayOfWeek + 6) % 7;
        int classesPerDay = (int)table.Style;
        for (int i = 0; i < classesPerDay; i++)
        {
            Console.WriteLine($"第 {i + 1} 大节 {table.StartTimes[i]:HH:mm}");
            if (table[day, i].Count == 0)
            {
                Console.WriteLine("无课程安排");
            }
            else
            {
                var courses = table[day, i].Where(c => c.CheckIfScheduled(week));
                if (courses.Any())
                {
                    IEnumerable<string> courseInfos = courses.Select(c =>
                        $"""
                         课程：{c.Title}
                         地点：{c.Location}
                         教师：{c.Teacher}
                         排课：{c.Schedule.RawText}
                         其他：{string.Join('\n', c.Description)}
                         """);
                    Console.WriteLine(string.Join("\n\n", courseInfos));
                }
                else
                {
                    Console.WriteLine("本周无课程安排");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("""
            1) 后一天
            2) 前一天
            3）今天
            0) 返回
            """);
        int input;
        do
        {
            Console.Write("请输入：");
        } while (!int.TryParse(Console.ReadLine(), out input));
        switch (input)
        {
            case 0:
                return;
            case 1:
                date = date.AddDays(1);
                if (date.DayOfWeek == DayOfWeek.Monday)
                {
                    week++;
                }
                break;
            case 2:
                date = date.AddDays(-1);
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    week--;
                }
                break;
            case 3:
                date = DateOnly.FromDateTime(DateTime.Now);
                week = table.GetWeek(date) ?? 0;
                break;
            default:
                break;
        }
    }
}

async Task PrintExamsAsync()
{
    Console.Clear();
    Exam[] exams = await hrbust.GetExamsAsync();
    foreach (Exam e in exams)
    {
        Console.WriteLine($"{e.CourseId}\t{e.CourseName}\t{e.Time}\t{e.Location}\t{e.Type}");
    }
    Console.WriteLine("按任意键继续……");
    Console.ReadKey();
}
