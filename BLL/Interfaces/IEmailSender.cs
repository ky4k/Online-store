﻿using HM.BLL.Models;

namespace HM.BLL.Interfaces;

public interface IEmailSender
{
    Task<OperationResult> SendEmailAsync(IUserMailInfo recipient, string subject,
        string htmlText, CancellationToken cancellationToken);
}