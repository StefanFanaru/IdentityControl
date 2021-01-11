namespace IdentityControl.API.Data.Entities
{
    public class Address // ValueObject
    {
        public Address(string street, string city, string county, string country, string zipcode)
        {
            Street = street;
            City = city;
            County = county;
            Country = country;
            ZipCode = zipcode;
        }

        private Address()
        {
        }

        public string Street { get; }

        public string City { get; }

        public string County { get; }

        public string Country { get; }

        public string ZipCode { get; }
    }
}