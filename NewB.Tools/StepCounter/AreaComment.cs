using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewB.Code.StepCounter
{
    public class AreaComment
    {
        private string _StartString = "";

        public string StartString
        {
            get { return _StartString; }
            set { _StartString = value; }
        }


        private string _EndString = "";

        public string EndString
        {
            get { return _EndString; }
            set { _EndString = value; }
        }
    }
}
