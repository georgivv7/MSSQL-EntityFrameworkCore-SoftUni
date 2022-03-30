namespace P03_FootballBetting
{
    using P03_FootballBetting.Data;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new FootballBettingContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
