﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjMSIT158API8.Models;

public partial class TAppraisal
{
    public int RankId { get; set; }

    public string Rank { get; set; }

    public virtual ICollection<TReview> TReviews { get; set; } = new List<TReview>();
}