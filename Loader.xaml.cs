using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HyperPeer;

namespace HyperPeer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Loader : Window
    {
        public Loader()
        {
            InitializeComponent();
            RunApiChecks();
            RunChecks();
            
        }

        private async void RunChecks()
        {
            LoginScreen loginScreen = new LoginScreen();
            if (!Directory.Exists(Utils.SettingsDir))
            {
                Directory.CreateDirectory(Utils.SettingsDir);
            }
            if (!File.Exists(System.IO.Path.Combine(Utils.SettingsDir, "config.json")))
            {
                var f = File.Create(System.IO.Path.Combine(Utils.SettingsDir, "config.json"));
                f.Close();
            }
            string configJsonString = File.ReadAllText(System.IO.Path.Combine(Utils.SettingsDir, "config.json"));
            if (configJsonString == null || configJsonString.Length < 1 || configJsonString == "")
            {
                Console.WriteLine("[LOADER] Empty config file. Prompting to log in.");
                this.Hide();
                loginScreen.Show();
            }
            else
            {
                Utils.Account acc = Utils.parseAccountFromConfigString(configJsonString);
                if (acc != null)
                {
                    object ApiResponse = Utils.sendProfileRequest(acc.token);
                    if (ApiResponse != "400")
                    {
                        Utils.PublicProfileResponseData responseData = Utils.parsePublicProfileData(ApiResponse.ToString());
                        List<Utils.Guild> guilds = new List<Utils.Guild>();
                        responseData.data.guilds.ForEach(guildId =>
                        {
                            var guildReponse = Utils.sendPublicGuildDataRequest(guildId, acc.token);
                            if (guildReponse == "400")
                            {
                                Console.WriteLine("[LOADER] Failed to fetch guild of id " + guildId);
                                this.Hide();
                                loginScreen.Show();
                            }
                            else
                            {
                                Utils.GuildPublicReponseData guildReponseData = Utils.parsePublicGuildData(guildReponse.ToString());
                                List<Utils.Channel> channels = new List<Utils.Channel>();
                                guildReponseData.data.channels.ForEach(channelId =>
                                {
                                    var channelResponse = Utils.sendChannelDataRequest(channelId, guildId, acc.token);
                                    if (channelResponse == "400")
                                    {
                                        Console.WriteLine("[LOADER] Failed to parse channel data of id " + channelId);
                                        this.Hide();
                                        loginScreen.Show();
                                    }
                                    else
                                    {
                                        Utils.ChannelReponseData channelResponseData = Utils.parseChannelData(channelResponse.ToString());
                                        List<Utils.Message> messages = new List<Utils.Message>();
                                        channelResponseData.data.messages.ForEach(messageId =>
                                        {
                                            var messageResponse = Utils.sendMessageDataRequest(messageId, channelId, guildId, acc.token);
                                            if (messageResponse == "400")
                                            {
                                                Console.WriteLine("[LOADER] Failed to parse message data of id " + messageId);
                                                this.Hide();
                                                loginScreen.Show();
                                            }
                                            else
                                            {
                                                Utils.MessageResponseData messageResponseData = Utils.parseMessageData(messageResponse.ToString());
                                                messages.Add(new Utils.Message()
                                                {
                                                    id = messageResponseData.data._id,
                                                    content = messageResponseData.data.content,
                                                    authorId = messageResponseData.data.authorId,
                                                    channelId = messageResponseData.data.channelId,
                                                    createdAt = messageResponseData.data.createdAt
                                                });
                                            }
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
                                    }
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
                            }
                        });
                        Utils.Account account = new Utils.Account()
                        {
                            username = responseData.data.username,
                            discriminator = responseData.data.discriminator,
                            email = acc.email,
                            token = acc.token,
                            accountId = responseData.data._id,
                            avatarURL = responseData.data.avatarURL,
                            timestamps = responseData.data.timestamps,
                            guilds = guilds

                        };
                        Utils.saveConfig(account);
                        MainWindow win = new MainWindow(account);
                        this.Hide();
                        win.Show();
                    }
                    else
                    {
                        Console.WriteLine("[LOADER] Failed to fetch profile data. Prompting to log in.");
                        this.Hide();
                        loginScreen.Show();
                    }
                }
                else
                {
                    Console.WriteLine("[LOADER] Could not parse account data from config.");
                    this.Hide();
                    loginScreen.Show();
                }
            }
        }

        private async void RunApiChecks()
        {

        }

        private void Drag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }


    }
}
