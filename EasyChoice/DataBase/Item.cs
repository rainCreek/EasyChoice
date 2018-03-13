using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyChoice.DataBase
{
    class Item
    {
        private int ID;
        public string Title { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string Answer { get; set; }
        public int State { get; set; }

        public Item(string title, string answera, string answerb, 
            string  answerc, string answerd, string answer, int state)
        {
            //this.ID = Guid.NewGuid().ToString(); //生成id
            this.Title = title;
            this.AnswerA = answera;
            this.AnswerB = answerb;
            this.AnswerC = answerc;
            this.AnswerD = answerd;
            this.Answer = answer;
            this.State = state;
        }

        //更新item但是id保持不变
        public void UpdateItem(string title, string answera, string answerb,
            string answerc, string answerd, string answer, int state)
        {
            this.Title = title;
            this.AnswerA = answera;
            this.AnswerB = answerb;
            this.AnswerC = answerc;
            this.AnswerD = answerd;
            this.Answer = answer;
            this.State = state;
        }

        public bool IdEqual(int id)
        {
            return id == getID();
        }
        public int getID()
        {
            return ID;
        }
        public int getState()
        {
            return State;
        }
        //从数据库获得item或者将数据转换之后保存到数据库
        public Item(int id, string title, string answera, string answerb,
           string answerc, string answerd, string answer, int state)
        {
            this.ID = id; //生成id
            this.Title = title;
            this.AnswerA = answera;
            this.AnswerB = answerb;
            this.AnswerC = answerc;
            this.AnswerD = answerd;
            this.Answer = answer;
            //将图string类型的数转换为int类型的数
            this.State = state;
        }
    }
}
