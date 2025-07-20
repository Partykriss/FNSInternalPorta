namespace FNS.Main.Models
{
    public class HomeViewModel
    {
        public IEnumerable<News> News { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public Voting CurrentVoting { get; set; }
        public bool HasVoted { get; set; }
        public IEnumerable<Voting> RecentVotings { get; set; }
    }
}