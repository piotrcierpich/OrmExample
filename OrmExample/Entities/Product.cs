using OrmExample.Mapping;

namespace OrmExample.Entities
{
    public class Product : EntityInUow
    {
        private string name;
        private decimal price;

        public override bool Equals(object obj)
        {
            Product other = obj as Product;
            if (other == null) return false;
            return other.Id == Id;
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                MarkDirty();
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                MarkDirty();
            }
        }
    }
}