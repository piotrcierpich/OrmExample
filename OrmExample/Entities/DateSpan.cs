using System;

namespace OrmExample.Entities
{
    public class DateSpan
    {
        public bool Encloses(DateTime dateTime)
        {
            return StartDate >= dateTime && EndDate <= dateTime;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}