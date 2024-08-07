namespace test_staff
{
    public class Staff
    {
        public int? id { get; set; }
        public string? PIB { get; set; }
        public int? position { get; set; }
        public float? salary { get; set; }
        public DateTime? birthDate { get; set; }
        public bool? isFired { get; set; }
        public Staff(string? PIB, int? position, float? salary, DateTime? birthDate, int? id = null, bool? isFired = null)
        {
            this.id = id;
            this.PIB = PIB;
            this.position = position;
            this.salary = salary;
            this.birthDate = birthDate;
            this.isFired = isFired ?? false;
        }
    }
}
