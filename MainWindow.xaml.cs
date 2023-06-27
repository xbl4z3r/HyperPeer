using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using XamlAnimatedGif;
using WebSocketSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace HyperPeer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Utils.Account account;
        private WebSocket websocket;
        private ListViewItem guildTemplate;
        private ListViewItem createGuildItem;
        private ListViewItem joinGuildItem;
        private Grid popupTemplate;
        public MainWindow(object account)
        {
            this.account = (Utils.Account) account;
            InitializeComponent();
            this.popupTemplate = PopupTemplate;
            this.popupTemplate.Visibility = Visibility.Collapsed;
            ListView guildListView = guildList;
            this.guildTemplate = guildListView.Items[0] as ListViewItem;
            this.createGuildItem = guildListView.Items[1] as ListViewItem;
            this.joinGuildItem = guildListView.Items[2] as ListViewItem;
            
            guildListView.Items.Clear();

            this.guildTemplate.MouseLeftButtonDown += ClickGuild;
            this.account.guilds.ForEach(guild =>
            {
                int pos = guildListView.Items.Add(this.guildTemplate);
                if(pos != -1)
                {
                    ListViewItem guildItem = guildListView.Items.GetItemAt(pos) as ListViewItem;
                    guildItem.Name = guild.name;
                    guildItem.Uid = guild.id;
                    StackPanel guildStackPanel = guildItem.FindName("gStack") as StackPanel;
                    if (guildStackPanel != null)
                    {
                        Image guildIcon = guildStackPanel.FindName("guildAv") as Image;
                        if (guildIcon != null)
                        {
                            string imageUrl = guild.iconURL;
                            BitmapImage image = new BitmapImage(new Uri(imageUrl));
                            guildIcon.Source = image;
                        }
                        StackPanel guildDataStackPanel = guildItem.FindName("guildData") as StackPanel;
                        if (guildStackPanel != null)
                        {
                            TextBlock guildNameBlock = guildDataStackPanel.FindName("guildName") as TextBlock;
                            TextBlock memberCountBlock = guildDataStackPanel.FindName("memberCount") as TextBlock;
                            if(memberCountBlock != null && guildNameBlock != null)
                            {
                                guildNameBlock.Text = guild.name;
                                memberCountBlock.Text = guild.members.Count.ToString() + " Members";
                            }
                        }
                    }
                }
            });
            this.joinGuildItem.MouseLeftButtonDown += ClickJoinGuild;
            this.createGuildItem.MouseLeftButtonDown += ClickCreateGuild;
            guildListView.Items.Add(this.createGuildItem);
            guildListView.Items.Add(this.joinGuildItem);
            userTag.Text = this.account.username + "#" + this.account.discriminator;
            AnimationBehavior.SetRepeatBehavior(userAvatar, RepeatBehavior.Forever);
            try
            {
                AnimationBehavior.SetSourceUri(userAvatar, new Uri(this.account.avatarURL));
            } 
            catch(Exception ex)
            {
                AnimationBehavior.SetSourceUri(userAvatar, new Uri("https://vectorified.com/images/unknown-avatar-icon-7.jpg"));
            }

            //websocket = new WebSocket(url: "wss://YOUR_SOCKET_URL", onMessage: OnMessage, onError: OnError);
            //websocket.Connect();
        }

        private void ClickGuild(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("test2");
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {

            }
        }

        private void ClickCreateGuild(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("test");
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                this.popupTemplate.Visibility = Visibility.Visible;
            }
        }

        private void ClickJoinGuild(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("test1");
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                //Do your stuff
            }
        }

        private static Task OnError(ErrorEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static Task OnMessage(MessageEventArgs messageEventArgs)
        {
            string text = messageEventArgs.Text.ReadToEnd();
            Console.WriteLine(text);

            return Task.FromResult(0);

        }

        private void SendMessage(string message)
        {

        }

        private void SendMessageClicked(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsClicked(object sender, RoutedEventArgs e)
        {

        }

        private void MinimalizeClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (WindowState)
            {
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    button.Content = "◻";
                    break;
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    button.Content = "❏";
                    break;
            }
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PinClicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (Topmost)
            {
                Topmost = false;
                button.Style = (Style)FindResource("TopTabButton");
            }
            else
            {
                Topmost = true;
                button.Style = (Style)FindResource("TogglePressed");
            }
        }
        
        private void MouseTabDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}