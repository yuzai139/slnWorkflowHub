﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace apiWorkflowHub.ContextModels;

public partial class TLecture
{
    public int FLectureId { get; set; }

    public string FLecName { get; set; }

    public int FPublisherId { get; set; }

    public decimal? FLecPrice { get; set; }

    public decimal? FLecPoints { get; set; }

    public string FLecDescription { get; set; }

    public bool FOnline { get; set; }

    public string FLecDate { get; set; }

    public string FLecLocation { get; set; }

    public int? FLecLimit { get; set; }

    public string FLink { get; set; }

    public byte[] FLecImage { get; set; }

    public string FLecImagePath { get; set; }
}