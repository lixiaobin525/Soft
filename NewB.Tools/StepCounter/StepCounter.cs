using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewB.Code.StepCounter
{
    /// <summary>
    /// カスタマイズして使用できる標準のステップカウンタです
    /// </summary>
    public class StepCounter
    {
        private static string CATEGORY_PATTERN = "\\[\\[(.*?)\\]\\]";
        private static string IGNORE_PATTERN = "\\[\\[IGNORE\\]\\]";

        private List<string> mlineComments = new List<string>();
        private List<AreaComment> mAreaComments = new List<AreaComment>();
        private List<string> mSkipPatterns = new List<string>();
        private string _FileType = "";

        /// <summary>
        /// ファイルの種類を設定,取得します
        /// </summary>
        public string FileType
        {
            get { return _FileType; }
            set { _FileType = value; }
        }

        /// <summary>
        /// スキップするパターンを取得します
        /// </summary>
        public List<string> SkipPatterns
        {
            get { return mSkipPatterns; }
        }

        /// <summary>
        /// 単一行コメントの開始文字列を追加します
        /// </summary>
        /// <param name="pComment"></param>
        public void addLineComment(string pComment)
        {
            this.mlineComments.Add(pComment);
        }

        /// <summary>
        /// 複数行コメントを追加します
        /// </summary>
        /// <param name="pAreaComment"></param>
        public void addAreaComment(AreaComment pAreaComment)
        {
            this.mAreaComments.Add(pAreaComment);
        }

        /// <summary>
        /// スキップするパターン（正規表現）を追加します。
        /// </summary>
        /// <param name="pSkipPattern">スキップするパターン（正規表現）</param>
        public void addSkipPattern(string pSkipPattern)
        {
            this.mSkipPatterns.Add(pSkipPattern);
        }

        /// <summary>
        /// カウントします
        /// </summary>
        /// <param name="pFile"></param>
        /// <param name="pFileType"></param>
        /// <returns></returns>
        public CountResult mCount(FileInfo pFile, string pFileType)
        {
            string strCategory = "";
            string strCharSetName = pFileType;
            if (string.IsNullOrEmpty(strCharSetName))
            {
                strCharSetName = "";
            }

            long emptyCnt = 0;
            long commentCnt = 0;
            long stepCnt = 0;

            using (StreamReader reader = new StreamReader(pFile.FullName, System.Text.Encoding.Default))
            {
                string line = "";
                Boolean  areaFlg = false;
                AreaComment endAreaComment=null;

                while ((line = reader.ReadLine())!=null)
                {
                    if (strCategory.Length == 0)
                    {
                        Match matcher = Regex.Match(line, CATEGORY_PATTERN);
                        if (matcher.Success)
                        {
                            strCategory = matcher.Groups[1].Value ;
                        }
                        
                    }

                    if (Regex.Match(line, IGNORE_PATTERN).Success)
                    {
                        return null;
                    }

                    string trimLine = line.Trim();
                    // 複数行コメントチェック
                    if (areaFlg == false)
                    {
                        if (emptyLineCheck(trimLine))
                        {
                            emptyCnt++;
                        }
                        else if(lineCommentCheck(trimLine ))
                        {
                            commentCnt++;
                        }
                        else if(skipPatternCheck(trimLine ))
                        {
                            emptyCnt ++;
                        }
                        else if((endAreaComment =areaCommentStartCheck(trimLine ))!=null ){
                            // 複数行コメントあります時、開始
                            commentCnt ++;
                            areaFlg =true;
                        }
                        else {
                            stepCnt ++;
                        }
                    }
                    else
                    {
                        commentCnt ++;
                        // 複数行コメント終了
                        if(areaCommentEndCheck(trimLine ,endAreaComment )){
                            areaFlg =false ;
                        }
                    }
                }
            }

            return new CountResult()
            {
                Fileinfo = pFile,
                FileType = pFileType,
                Category =strCategory ,
                EmptyCnt = emptyCnt,
                CommentCnt = commentCnt,
                StepCnt = stepCnt
            };
        }
        

        /// <summary>
        /// スキップパターンにマッチするかチェック
        /// </summary>
        /// <param name="pline">行文字列</param>
        /// <returns></returns>
        private Boolean skipPatternCheck(string pline)
        {
            foreach (string skipPattern in mSkipPatterns)
            {
                if (Regex.Match(pline, skipPattern).Success)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 空行かどうかをチェック
        /// </summary>
        /// <param name="pline">行文字列</param>
        /// <returns></returns>
        private Boolean emptyLineCheck(string pline)
        {
            if (string.IsNullOrEmpty(pline))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 単一行コメントかどうかをチェック
        /// </summary>
        /// <param name="pline">行文字列</param>
        /// <returns></returns>
        private Boolean lineCommentCheck(string pline)
        {
            foreach (string comment in mlineComments)
            {
                if (pline.StartsWith(comment))
                {
                    return true;
                }
            }

            foreach (AreaComment area in mAreaComments)
            {
                int intStartIndex = pline.IndexOf(area.StartString);
                // 例：/*------*/
                if (intStartIndex == 0 && pline.IndexOf(area.EndString, intStartIndex) == pline.Length - area.EndString.Length)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 複数行コメントが開始しているかチェック
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private AreaComment  areaCommentStartCheck(string pline)
        {
            foreach (AreaComment area in mAreaComments)
            {
                int intStartIndex = pline.IndexOf (area.StartString);
                // コメント開始
                if (intStartIndex >= 0 && pline.IndexOf(area.EndString, intStartIndex) < 0)
                {
                    return area;
                }
            }
            return null;
        }

        /// <summary>
        /// 複数行コメントが終了しているかチェック
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Boolean areaCommentEndCheck(string pline,AreaComment pArea)
        {
            if (pline.IndexOf(pArea.EndString) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
