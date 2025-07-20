using System.ComponentModel;

namespace FNS.Admin.Model
{
    public class VotingModel : INotifyPropertyChanged
    {
        private bool _isActive;
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>();
        public List<VoteModel> Votes { get; set; } = new List<VoteModel>();
    }

    public class AnswerModel
    {
        public int Id { get; set; }
        public int VotingId { get; set; }
        public string Title { get; set; }
        public int VotesCount { get; set; }
    }

    public class VoteModel
    {
        public int Id { get; set; }
        public int VotingId { get; set; }
        public int AnswerId { get; set; }
        public string UserName { get; set; }
    }
}
