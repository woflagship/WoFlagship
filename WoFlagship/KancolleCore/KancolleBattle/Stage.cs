using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoFlagship.KancolleCore.KancolleBattle
{
    public class Stage
    {
        public StageTypes StageType { get; set; }
        public StageTypes SubType { get; set; }
        public List<Attack> Attacks { get; set; }

        public override string ToString()
        {
            string str = string.Format("Stage:{0}\tSubStage:{1}\n", StageType.ToString(), SubType.ToString());
            if (Attacks != null)
            {
                foreach (var attack in Attacks)
                {
                    str += attack.ToString() + "\n";
                }
            }

            return str;
        }
    }
}
