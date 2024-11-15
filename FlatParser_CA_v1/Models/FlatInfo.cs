namespace FlatParser_CA_v1.Models
{
    public class FlatInfo
    {
        public string Link { get; set; }
        public string Address { get; set; }
        public string Price { get; set; }
        public int RegionId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FlatInfo other = (FlatInfo)obj;

            return Link == other.Link && Address == other.Address && Price == other.Price;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Address, Price);
        }
    }
}
