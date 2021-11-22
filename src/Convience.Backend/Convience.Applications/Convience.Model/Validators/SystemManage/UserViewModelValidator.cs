using Convience.Model.Models.SystemManage;

using FluentValidation;

namespace Convience.Model.Validators.SystemManage
{
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(viewmodel => viewmodel.UserName).MaximumLength(15).WithMessage("用戶名過長！");
            RuleFor(viewmodel => viewmodel.UserName).NotEmpty().NotNull()
                .WithMessage("用戶名不能為空！");

            RuleFor(viewmodel => viewmodel.Name).MaximumLength(30).WithMessage("人名過長！");
            RuleFor(viewmodel => viewmodel.Name).NotEmpty().NotNull()
                .WithMessage("人名不能為空！");

            RuleFor(viewmodel => viewmodel.PhoneNumber).MaximumLength(11).WithMessage("電話號碼過長！");
            RuleFor(viewmodel => viewmodel.Avatar).MaximumLength(5).WithMessage("頭像内容過長！");
        }
    }
}
