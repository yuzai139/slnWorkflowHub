﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjWorkflowHubAdmin.ContextModels;

public partial class TMessage
{
    public int FMessageId { get; set; }

    public int FArticleId { get; set; }

    public int FMemberId { get; set; }

    public string FMessageContent { get; set; }

    public DateTime FCreatedAt { get; set; }

    public DateTime? FUpdatedAt { get; set; }

    public virtual TArticle FArticle { get; set; }

    public virtual TMember FMember { get; set; }
}