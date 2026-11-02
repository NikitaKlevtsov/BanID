namespace BanID.Models
{
    public class PersonalData
    {
        public string FullName { get; set; }
        public string Passport { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public string ToDataString() => $"{FullName}|{Passport}|{Address}|{Phone}";
        public static PersonalData FromDataString(string data)
        {
            var parts = data.Split('|');
            if (parts.Length != 4) return new PersonalData();
            return new PersonalData
            {
                FullName = parts[0],
                Passport = parts[1],
                Address = parts[2],
                Phone = parts[3]
            };
        }
    }
}