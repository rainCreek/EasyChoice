using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace EasyChoice
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyPage : Page
    {

        // private String question;
        // private String choice;
        DataBase.ItemViewModel ViewModel { get; set; }
        // 记录正确答案,格式例子: "A",每次从DB获取数据都要更新
        private String correctAns;
        private int show;
        // 正确率统计
        int correctNum;  // 记录本次作对题目的数量
        int totalNum;  // 记录本次做过题目的总数
        bool finishThisQuestion;

        // 错题复习模式标记
        /// mmmmmmmmmmmm
        bool isOnlyWrongQues;
        int curWrong;

        private int count_all = 0;  // 所有题目
        private int count_wrong = 0;   // 错误题目
        private int count_collect = 0;   // 收藏题目
        private int count_reminder = 0;   // 剩余题目
        public MyPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            show = 1;
            this.InitializeComponent();
            isOnlyWrongQues = false;
            correctNum = 0;
            totalNum = 1;
            curWrong = 0;

            //初始状态的时候插入一条用于检测的
            DataBase.DbContext sql = new DataBase.DbContext();
            this.ViewModel = new DataBase.ItemViewModel();
            getQuestionFromDB();
            count();
            allquestion.Text = count_all.ToString();
            shengyu.Text = count_reminder.ToString();
            shoucang.Text = count_collect.ToString();
            wrong.Text = count_wrong.ToString();
        }

        

        // 从数据库获取题目 要更新题目内容,每个选项内容,和正确答案
        private void getQuestionFromDB()
        {
            // 从数据库获取题目内容
            finishThisQuestion = false;
            if (ViewModel.AllItems != null)
            {
                foreach (var item in this.ViewModel.AllItems)
                {
                    int state = item.getState();
                    bool isThisItem = false;
                    if (isOnlyWrongQues && (state == 3 || state == 2) &&item.getID() > curWrong)
                    {
                        curWrong = item.getID();
                        isThisItem = true;
                    }
                    else if (!isOnlyWrongQues && state == 0)
                        isThisItem = true;
                    if (isThisItem)
                    {
                        questionBlock.Text = item.Title;
                        A.Text = item.AnswerA;
                        B.Text = item.AnswerB;
                        C.Text = item.AnswerC;
                        D.Text = item.AnswerD;
                        correctAns = item.Answer;
                        show = item.getID();
                        return;
                    }
                }
                // 没有合适的题目
                questionBlock.Text = "Congratulations! You finish all questions";
                A.Text = B.Text = C.Text = D.Text = "";
            }
        }

        // 点击选项触发事件
        private void choiceButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            // var i = new MessageDialog(button.Name).ShowAsync();

            // 显示结果和答案 通过改变框的颜色
            //int id = show;
            string title = questionBlock.Text;
            string answera = (string)A.Text;
            string answerb = (string)B.Text;
            string answerc = (string)C.Text;
            string answerd = (string)D.Text;
            string answer = (string)correctAns;
            //int flag = 1;

            if (button.Name[0] != correctAns[0])  // 回答错误
            {
                button.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                aouMediaElement.Play();
                ViewModel.UpdateItem(show, title, answera, answerb, answerc, answerd, answer, 2);
            }
            else if (finishThisQuestion == false)  // 回答正确
            {
                //flag = 2;
                goodMediaElement.Play();
                ViewModel.UpdateItem(show, title, answera, answerb, answerc, answerd, answer, 1);
                correctNum++;
            }

            count();
            allquestion.Text = count_all.ToString();
            shengyu.Text = count_reminder.ToString();
            shoucang.Text = count_collect.ToString();
            wrong.Text = count_wrong.ToString();
            int num = (int)correctAns[0] - 'A';
            Button correctButtons = (Button)VisualTreeHelper.GetChild(choices, num);
            correctButtons.Background = new SolidColorBrush(Windows.UI.Colors.Green);
            correctRatioNum.Text = ((double)correctNum / (double)totalNum * 100).ToString() + "%";
            finishThisQuestion = true;

        }

        // 下一题按钮触发事件
        private void goNextButtonClick(object sender, RoutedEventArgs e)
        {
            if (finishThisQuestion == false) return;
            totalNum++;
            // 获取题目并且复原选项框颜色
            getQuestionFromDB();
            Button button;
            for (int i = 0; i < 4; i++)
            {
                button = (Button)VisualTreeHelper.GetChild(choices, i);
                button.Background = new SolidColorBrush(Windows.UI.Colors.White);
            }

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            // TODO
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;

        }



        // 分享按钮事件
        private void shareButtonClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            //var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/my.jpg"));
            dp.Properties.Title = "I have a question with this problem, please help me\n";
            dp.Properties.Description = questionBlock.Text + "\n" + A.Text + "\n" + B.Text + "\n" + C.Text + "\n" + D.Text + "\n";
            dp.SetText(dp.Properties.Description);
            //dp.SetStorageItems(new List<StorageFile> { photoFile });
            deferral.Complete();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        // 模式选择按钮触发 选择一般模式或者错题复习模式
        private void modeSelectButtonClick(object sender, RoutedEventArgs e)
        {
            if (modeSelectButton.Label == "Normal")
            {
                modeSelectButton.Label = "Review";
                textBlock.Text = "Normal Mode";
                isOnlyWrongQues = false;
                //ImageBrush image = new ImageBrush();
                bgi.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/NormalBackG.jpg", UriKind.Absolute));
                //this.BackgroundImage.Background = image;
            }
            else
            {
                modeSelectButton.Label = "Normal";
                textBlock.Text = "Review Mode";
                isOnlyWrongQues = true;
                curWrong = 0;
                //ImageBrush image = new ImageBrush();
                bgi.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/ReviewBackG.png", UriKind.Absolute));
                //this.BackgroundImage.Background = image;
            }
            getQuestionFromDB();
        }

        // 收藏按钮触发，可以将任何题改成错题，以便复习
        private void FavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            int id = show;
            string title = questionBlock.Text;
            string answera = (string)A.Text;
            string answerb = (string)B.Text;
            string answerc = (string)C.Text;
            string answerd = (string)D.Text;
            string answer = (string)correctAns;
            int flag = 3;
            ViewModel.UpdateItem(id, title, answera, answerb, answerc, answerd, answer, flag);
            count();
            allquestion.Text = count_all.ToString();
            shengyu.Text = count_reminder.ToString();
            shoucang.Text = count_collect.ToString();
            wrong.Text = count_wrong.ToString();
        }
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 800)
            {
                this.Frame.Navigate(typeof(NewPage));
            }

            
          

        }

       private void  count ()      // 每次做题或收藏时调用
        {
             count_all = 0;  // 所有题目
             count_wrong = 0;   // 错误题目
             count_collect = 0;   // 收藏题目
             count_reminder = 0;   // 剩余题目
            if (ViewModel.AllItems != null)
            {
                foreach (var item in this.ViewModel.AllItems)
                {
                    count_all++;
                    int state = item.getState();
                    bool isThisItem = false;
                    if (state == 0)
                        count_reminder++;
                    else if (state == 2)
                        count_wrong++;
                    else if (state == 3)
                        count_collect++;
                }
            }
        }
    }
}
