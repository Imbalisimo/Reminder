//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reminder
{
    using System;
    using System.Collections.Generic;
    
    public partial class Events
    {
        public int EventId { get; set; }
        public string Event_name { get; set; }
        public System.DateTime Event_date { get; set; }
        public bool Every_year { get; set; }

        public override string ToString()
        {
            return Event_date.ToShortDateString() + " " + Event_name;
        }
    }
}
