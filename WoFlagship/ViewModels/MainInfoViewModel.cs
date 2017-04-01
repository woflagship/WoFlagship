using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoFlagship.KancolleCore.Navigation;

namespace WoFlagship.ViewModels
{
    class MainInfoViewModel : ViewModelBase
    {
        private ObservableCollection<TaskViewModel> _taskList = new ObservableCollection<TaskViewModel>();
        public ObservableCollection<TaskViewModel> TaskList { get { return _taskList; } set { _taskList = value; OnPropertyChanged(); } }
    }

    public class TaskViewModel : ViewModelBase
    {
        private KancolleTask _task;
        public KancolleTask Task { get { return _task; }
            set
            {
                _task = value;
                if(_task != null)
                {
                    Type = _task.TypeName;
                    Detail = _task.ToString();
                    Priority = _task.Priority.ToString();
                    OnPropertyChanged("Type");
                    OnPropertyChanged("Detail");
                }
            }
        }

        public string Type { get; private set; }

        public string Detail { get; private set; }

        public string Priority { get; private set; }
    }
}
