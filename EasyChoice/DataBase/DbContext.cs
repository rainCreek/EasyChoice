using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyChoice.DataBase
{
    class DbContext
    {
        private static string DatabaseName = "Database.db";
        public static class SQL
        {
            public static string createsql = @"CREATE TABLE IF NOT EXISTS
                                                database(ID          INTEGER PRIMARY KEY NOT NULL,
                                                        Title        TEXT,
                                                        AnswerA      TEXT,
                                                        AnswerB      TEXT,
                                                        AnswerC      TEXT,
                                                        AnswerD      TEXT,
                                                        Answer       TEXT,
                                                        State        INTEGER);";
            public static string selectAllItems = @"SELECT ID, Title, AnswerA, AnswerB, AnswerC, AnswerD, Answer, State FROM database";
            //这里的状态0代表什么操作都没有，就是刚刚插入数据库的状态
            //1代表这道题做对了；2代表这道题做错了,3代表这道题处于收藏状态；
        }
        //创建数据库
        public DbContext()
        {
            var conn = GetsqliteConnection();
            using (var statement = conn.Prepare(SQL.createsql))
            {
                statement.Step();
            }
        }
        private static SQLiteConnection GetsqliteConnection()
        {
            return new SQLiteConnection(DatabaseName);
        }

        public static ObservableCollection<DataBase.Item> getAllItem()
        {
            ObservableCollection<DataBase.Item> todoItemList = new ObservableCollection<DataBase.Item>();
            var con = GetsqliteConnection();
            string sql = "SELECT * FROM database";
            var statement = con.Prepare(sql);

            while (statement.Step() == SQLiteResult.ROW)
            {
                todoItemList.Add(new DataBase.Item(Convert.ToInt32(statement[0]), (string)statement[1], (string)statement[2],
                                            (string)statement[3], (string)statement[4], (string)statement[5],
                                            (string)statement[6], Convert.ToInt32(statement[7])));
            }
            return todoItemList;
        }


        //向数据库插入内容
        public static bool InsertData(int id, string title, string answera, string answerb,
            string answerc, string answerd, string answer, int state)
        {
            var conn = GetsqliteConnection();
            try
            {
                using (var todo = conn.Prepare("INSERT INTO  database(ID, Title, AnswerA, AnswerB, AnswerC, AnswerD, Answer, State) VALUES(?, ?, ?, ?, ?, ?, ?, ?)"))
                {
                    todo.Bind(1, id);
                    todo.Bind(2, title);
                    todo.Bind(3, answera);
                    todo.Bind(4, answerb);
                    todo.Bind(5, answerc);
                    todo.Bind(6, answerd);
                    todo.Bind(7, answer);
                    todo.Bind(8, state);
                    todo.Step();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        // 实现数据对数据库内容的更新
        public static void UpdateData(int id, string title, string answera, string answerb,
            string answerc, string answerd, string answer, int state)
        {
            var connection = GetsqliteConnection();
            using (var todo = connection.Prepare("UPDATE database set Title=?, AnswerA=?, AnswerB=?, AnswerC=?, AnswerD=?, Answer=?, State=? WHERE ID=?"))
            {
                todo.Bind(1, title);
                todo.Bind(2, answera);
                todo.Bind(3, answerb);
                todo.Bind(4, answerc);
                todo.Bind(5, answerd);
                todo.Bind(6, answer);
                todo.Bind(7, state);
                todo.Bind(8, id);
                todo.Step();
            }
        }

        //删除数据库里的内容
        public static void DeleteData(int id)
        {
            var connection = GetsqliteConnection();
            using (var todo = connection.Prepare("DELETE FROM database WHERE ID=?"))
            {
                todo.Bind(1, id);
                todo.Step();
            }
        }

        
    }
}
