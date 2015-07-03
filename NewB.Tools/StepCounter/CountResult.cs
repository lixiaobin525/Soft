using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewB.Code.StepCounter
{
    public class CountResult
    {
        private FileInfo _Fileinfo;

        public FileInfo Fileinfo
        {
            get { return _Fileinfo; }
            set { _Fileinfo = value; }
        }

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        private string _FileType;

        public string FileType
        {
            get { return _FileType; }
            set { _FileType = value; }
        }

        private string _Category;

        public string Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        private long _EmptyCnt = 0;

        public long EmptyCnt
        {
            get { return _EmptyCnt; }
            set { _EmptyCnt = value; }
        }
        private long _CommentCnt = 0;

        public long CommentCnt
        {
            get { return _CommentCnt; }
            set { _CommentCnt = value; }
        }
        private long _StepCnt = 0;

        public long StepCnt
        {
            get { return _StepCnt; }
            set { _StepCnt = value; }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Fileinfo.Name).Append ("  ");
            sb.Append("実行：").Append(StepCnt.ToString()).Append("  ");
            sb.Append("空行：").Append(EmptyCnt . ToString()).Append("  ");
            sb.Append("ｺﾒﾝﾄ：").Append(CommentCnt  .ToString());

            return sb.ToString();
        }
    }
}
