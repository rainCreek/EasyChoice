using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

//namespace Todos.ViewModels
namespace EasyChoice.DataBase
{
    class ItemViewModel
    {
        private ObservableCollection<DataBase.Item> allItems;

        public ObservableCollection<DataBase.Item> AllItems
        {
            get { return this.allItems; }
        }

        private DataBase.Item selectedItem = default(DataBase.Item);
        public DataBase.Item SelectedItem
        {
            get { return selectedItem; }
            set { this.selectedItem = value; }
        }

        public ItemViewModel()
        {
            // 加入一个用来测试的item
            allItems = new ObservableCollection<DataBase.Item>();
            //从数据库获取数据
            allItems = DataBase.DbContext.getAllItem();

        }

        public void AddItem(string title, string answera, string answerb,
            string answerc, string answerd, string answer, int state)
        {
            // 向备忘录里面添加新的元素；
            DataBase.Item todo = new DataBase.Item(title, answera, answerb, answerc, answerd, answer, state);
            this.allItems.Add(todo);
            DataBase.DbContext.InsertData(todo.getID(), title, answera, answerb,
                answerc, answerd, answer, state);

        }

        public void RemoveItem(int id)
        {
            // DIY
            // set selectedItem to null after remove
            int index = getIndexOfItemById(id);
            if (index != -1)
            {
                this.allItems.Remove(selectedItem);
                DataBase.DbContext.DeleteData(id);
            }
           
            this.selectedItem = null;
        }

        public void UpdateItem(int id, string title, string answera, string answerb,
            string answerc, string answerd, string answer, int state)
        {
            int index = getIndexOfItemById(id);
            if (index != -1)
            {
               DataBase.DbContext.UpdateData(id, title, answera, answerb, answerc, answerd, answer, state);
                allItems[index].UpdateItem(title, answera, answerb, answerc, answerd, answer, state);
            }

                // set selectedItem to null after update
                this.selectedItem = null;
        }

        private int getIndexOfItemById(int id)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].IdEqual(id))
                    return i;
            }
            return -1;
        }  
    }
}
