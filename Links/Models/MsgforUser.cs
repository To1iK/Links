﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Links.Models;

public partial class MsgforUser
{
    public int MsgId { get; set; }

    public int UserId { get; set; }

    public int? Status { get; set; }

    public virtual Msg Msg { get; set; }

    public virtual User User { get; set; }
}