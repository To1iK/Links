﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Links.Models;

public partial class Detail
{
    public int Id { get; set; }

    public int? NodeId { get; set; }

    public int? TtansactionId { get; set; }

    public string Name { get; set; }

    public virtual Node Node { get; set; }

    public virtual ICollection<ObjectDetail> ObjectDetails { get; set; } = new List<ObjectDetail>();

    public virtual Transaction Ttansaction { get; set; }
}