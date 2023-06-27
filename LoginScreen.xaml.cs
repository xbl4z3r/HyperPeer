using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HyperPeer
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
            errorMessage.Foreground = Brushes.Red;
            errorMessage.Text = "";
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            RegisterScreen registerScreen = new RegisterScreen(this);
            registerScreen.Show();
            MinimalizeClicked(null, null);
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Text;

            bool isEmail = Utils.isEmail(email);
            if (email == null || email.Length < 3 || email == "" || !isEmail)
            {
                errorMessage.Foreground = Brushes.Red;
                errorMessage.Text = "Invalid Email";
                return;
            }

            if (password == null || password.Length < 8 && password == "")
            {
                errorMessage.Foreground = Brushes.Red;
                errorMessage.Text = "Invalid Password";
                return;
            }

            var loginResponse = Utils.sendLoginRequest(email, password);
            if (loginResponse == "400")
            {
                errorMessage.Foreground = Brushes.Red;
                errorMessage.Text = "Incorrect Email or Password";
                return;
            }
            else
            {
                Utils.LoginResponseData responseData = Utils.parseLoginResponseData(loginResponse.ToString());
                List<Utils.Guild> guilds = new List<Utils.Guild>();
                responseData.data.guilds.ForEach(guildId =>
                {
                    var guildReponse = Utils.sendPublicGuildDataRequest(guildId, responseData.data.token);
                    if (guildReponse == "400")
                    {
                        errorMessage.Foreground = Brushes.Red;
                        errorMessage.Text = "An Error Occurred";
                        return;
                    }
                    Utils.GuildPublicReponseData guildReponseData = Utils.parsePublicGuildData(guildReponse.ToString());
                    List<Utils.Channel> channels = new List<Utils.Channel>();
                    guildReponseData.data.channels.ForEach(channelId =>
                    {
                        var channelResponse = Utils.sendChannelDataRequest(channelId, guildId, responseData.data.token);
                        if (channelResponse == "400")
                        {
                            errorMessage.Foreground = Brushes.Red;
                            errorMessage.Text = "An Error Occurred";
                            return;
                        }
                        Utils.ChannelReponseData channelResponseData = Utils.parseChannelData(channelResponse.ToString());
                        List<Utils.Message> messages = new List<Utils.Message>();
                        channelResponseData.data.messages.ForEach(messageId =>
                        {
                            var messageResponse = Utils.sendMessageDataRequest(messageId, channelId, guildId, responseData.data.token);
                            if (messageResponse == "400")
                            {
                                errorMessage.Foreground = Brushes.Red;
                                errorMessage.Text = "An Error Occurred";
                                return;
                            }
                            Utils.MessageResponseData messageResponseData = Utils.parseMessageData(messageResponse.ToString());
                            messages.Add(new Utils.Message()
                            {
                                id = messageResponseData.data._id,
                                content = messageResponseData.data.content,
                                authorId = messageResponseData.data.authorId,
                                channelId = messageResponseData.data.channelId,
                                createdAt = messageResponseData.data.createdAt
                            });

                        });
                        channels.Add(new Utils.Channel()
                        {
                            id = channelResponseData.data._id,
                            name = channelResponseData.data.name,
                            type = channelResponseData.data.type,
                            messages = messages,
                            connectedUsers = channelResponseData.data.connectedUsers,
                            timestamps = channelResponseData.data.timestamps,

                        });

                    });
                    guilds.Add(new Utils.Guild()
                    {
                        id = guildReponseData.data._id,
                        name = guildReponseData.data.name,
                        iconURL = guildReponseData.data._id,
                        channels = channels,
                        members = guildReponseData.data.members,
                        ownerId = guildReponseData.data.ownerId,
                        timestamps = guildReponseData.data.timestamps,
                    });
                });
                Utils.Account account = new Utils.Account()
                {
                    username = responseData.data.username,
                    discriminator = responseData.data.discriminator,
                    email = responseData.data.email,
                    token = responseData.data.token,
                    accountId = responseData.data._id,
                    avatarURL = responseData.data.avatarURL,
                    timestamps = responseData.data.timestamps,
                    guilds = guilds
                };
                Utils.saveConfig(account);
                MainWindow mainWin = new MainWindow(account);
                this.Hide();
                mainWin.Show();
            }
        }

        private void MinimalizeClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MouseTabDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
