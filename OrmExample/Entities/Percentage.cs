using System;

namespace OrmExample.Entities
{
    public class Percentage
    {
        private int value;

        public Percentage(int value)
        {
            this.value = value;
        }

        public decimal Subtract(decimal price)
        {
            decimal fraction = (decimal)Value / 100;
            decimal pricePercentage = price * fraction;
            return price - pricePercentage;
        }

        public int Value
        {
            get { return value; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("percenttage cannot be less than 0");
                if (value > 100)
                    throw new ArgumentException("percentage cannot be more than 100");

                this.value = value;
            }
        }
    }
}