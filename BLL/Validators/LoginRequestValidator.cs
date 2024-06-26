﻿using FluentValidation;
using HM.BLL.Models.Users;

namespace HM.BLL.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(lr => lr.Email)
            .NotEmpty();
        RuleFor(lr => lr.Password)
            .NotEmpty();
    }
}
