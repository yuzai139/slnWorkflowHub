﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace apiWorkflowHub.ContextModels;

public partial class TArticle
{
    public int FArticleId { get; set; }

    public string FArticleName { get; set; }

    public string FArticleContent { get; set; }

    public int FCategoryNumber { get; set; }

    public int FMemberId { get; set; }

    public DateTime FCreatedAt { get; set; }

    public DateTime FUpdatedAt { get; set; }

    public virtual TCategory FCategoryNumberNavigation { get; set; }

    public virtual TMember FMember { get; set; }

    public virtual ICollection<TMessage> TMessages { get; set; } = new List<TMessage>();
}