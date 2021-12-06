namespace Convience.Model.Models.Account
{
    public record CaptchaResultModel(string CaptchaKey, string CaptchaData);

    public record ChangePwdViewModel(string OldPassword, string NewPassword);

    public record LoginResultModel(
        string Name,
        string Avatar,
        string Token,
        string Identification,
        string Routes,
        string CostNo,
        int[] Werks,
        string VendorId);

    public record LoginViewModel(
        string UserName,
        string Password,
        string CaptchaKey,
        string CaptchaValue);

    public record ValidateCredentialsResultModel(
        string Token,
        string Name,
        string Avatar,
        string RoleIds,
        string CostNo,
        int[] Werks,
        string VendorId);
}
