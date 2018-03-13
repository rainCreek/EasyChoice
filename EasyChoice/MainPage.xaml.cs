using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.ApplicationModel.DataTransfer;
using SQLitePCL;
using System.Text;
using System.Threading.Tasks;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace EasyChoice
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string name_;
        private string password_;
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            name_ = "student";
            password_ = "123456";
            name.Text = "student";
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void startButtonClick(object sender, RoutedEventArgs e)
        {
            if (name.Text != name_ || password.Password != password_)
            {
                var i = new MessageDialog("请输入正确的用户名和密码！").ShowAsync();
            } else
            {
                tile_create();                      // 当成功登陆时创建磁贴
                this.Frame.Navigate(typeof(MyPage));
            }
            
        }

        private void ChangeButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (name.Text == "" || password.Password == "")
            {
                var i = new MessageDialog("请输入新的的用户名和密码！").ShowAsync();
            }
            else
            {

                name_ = name.Text;
                password_ = password.Password;
                
            }

        }

        private async void click3(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;  //设置文件的现实方式，这里选择的是图标

            //picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; //设置打开时的默认路径，这里选择的是图片库

            picker.FileTypeFilter.Add(".jpg");                 //添加可选择的文件类型，这个必须要设置

            picker.FileTypeFilter.Add(".jpeg");

            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();　　　　//只能选择一个文件
            if (file != null)

            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                var bitmap = new BitmapImage();
                //BitmapImage img = new BitmapImage();

                await bitmap.SetSourceAsync(stream);

                // 显示  
                image.ImageSource = bitmap;

            }
        }
        private void tile_create()
        {
            String tile_ = File.ReadAllText("XMLFile1.xml");     // 导入xml文件
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(tile_);
            XmlNodeList Texttitle = xml.GetElementsByTagName("text");             // 列表
            XmlNodeList IMAGE = xml.GetElementsByTagName("image");
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            
            Texttitle[0].InnerText = Texttitle[1].InnerText = Texttitle[2].InnerText = name.Text;   // 磁贴显示内容为用户名
           

            TileNotification new_tile = new TileNotification(xml);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(new_tile);
            
        }
    }
}
