namespace FNS.Main.Models
{
    public class Voting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public List<Answer> Answers { get; set; } = new List<Answer>();
        public List<VotedUsers> Votes { get; set; } = new List<VotedUsers>();
    }
}
