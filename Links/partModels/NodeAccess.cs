﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Links.Models;

//[NotMapped]
[Keyless]
//[Table("NodeAccess")]
public partial class NodeAccess2
{
    //[NotMapped]
    //public int Id { get; set; }
   
    public int? NodeId { get; set; }
    //[NotMapped]
    //public int? UserId { get; set; }
    //[NotMapped]
    //public int? GroupId { get; set; }
    //[NotMapped]
    public int? AccessLevel { get; set; }
    //[NotMapped]
    //public virtual Group Group { get; set; }

    public virtual Node Node { get; set; }
    //[NotMapped]
    //public virtual User User { get; set; }
}