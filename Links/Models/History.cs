﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Links.Models;

public partial class History
{
    public int Id { get; set; }

    public int? NodeId { get; set; }

    public int? UserId { get; set; }

    public DateTime? Dt { get; set; }

    public virtual Node Node { get; set; }

    public virtual User User { get; set; }
}