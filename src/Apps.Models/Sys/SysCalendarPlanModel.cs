using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Models.Sys
{
    public class CalendarPlanJsonModel
    {
        public string id { get; set; }
        public string title { get; set; }//he text on an event's element
        public string content { get; set; }//content
        public string color { get; set; }//Sets an event's background and border color 
        public string textColor { get; set; }//Sets an event's text color
        public DateTime start { get; set; }//The date/time an event begins
        public DateTime end { get; set; }//The exclusive date/time an event ends
        public string url { get; set; }//A URL that will be visited when this event is clicked by the user
    }
}
