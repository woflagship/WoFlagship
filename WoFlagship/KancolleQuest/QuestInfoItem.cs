using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.ViewModels;

namespace WoFlagship.KancolleQuest
{
    [Serializable]
    public class QuestInfoItem : ViewModelBase
    {
        private string _id;
        public string Id { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _detail;
        public string Detail { get { return _detail; } set { _detail = value; OnPropertyChanged(); } }

        private int _ran;
        public int Ran { get { return _ran; } set { _ran = value; OnPropertyChanged(); } }

        private int _dan;
        public int Dan { get { return _dan; } set { _dan = value; OnPropertyChanged(); } }

        private int _gang;
        public int Gang { get { return _gang; } set { _gang = value; OnPropertyChanged(); } }


        private int _lu;
        public int Lu { get { return _lu; } set { _lu = value; OnPropertyChanged(); } }


        private string _other;
        public string Other { get { return _other; } set { _other = value; OnPropertyChanged(); } }

        private string _note;
        public string Note { get { return _note; } set { _note = value; OnPropertyChanged(); } }

        private int _gameId;
        public int GameId { get { return _gameId; } set { _gameId = value; OnPropertyChanged(); } }

        private int[] _prerequisite;
        public int[] Prerequisite { get { return _prerequisite; } set { _prerequisite = value; OnPropertyChanged(); } }

        private string _category;
        public string Category { get { return _category; } set { _category = value; OnPropertyChanged(); } }

        public IQuestRequirement Requirements { get; set; }

    }
}
