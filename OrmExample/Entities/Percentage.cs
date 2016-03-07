using System;

namespace OrmExample.Entities
{
    public class Percentage
    {
        private readonly int value;

        public Percentage(int value)
        {
            if (value < 0)
                throw new ArgumentException("percenttage cannot be less than 0");
            if (value > 100)
                throw new ArgumentException("percentage cannot be more than 100");

            this.value = value;
        }

        protected bool Equals(Percentage other)
        {
            return value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Percentage)) return false;
            return Equals((Percentage) obj);
        }

        public override int GetHashCode()
        {
            return value;
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
        }
    }
}