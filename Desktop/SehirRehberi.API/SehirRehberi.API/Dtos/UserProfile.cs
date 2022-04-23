namespace LetgoEcommerce.Dtos
{
    public class UserProfile
    {

        public int? id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int? city_id { get; set; }
        public string city { get; set; }
        public string image{ get; set; }

    }
}
