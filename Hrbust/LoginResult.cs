namespace Hrbust;

/// <summary>
/// 表示登录结果。
/// </summary>
public enum LoginResult
{
    /// <summary>
    /// 登录成功。
    /// </summary>
    Success,
    /// <summary>
    /// 验证码错误。
    /// </summary>
    CaptchaError,
    /// <summary>
    /// 身份验证错误。
    /// </summary>
    CredentialError
}