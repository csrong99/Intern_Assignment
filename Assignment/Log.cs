//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Assignment
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public int Id { get; set; }
        public System.DateTime Attempt_Time { get; set; }
        public Nullable<int> Employee_ID { get; set; }
        public string Ipv4 { get; set; }
        public bool successful { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
