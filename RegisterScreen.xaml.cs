using System.Windows;
using System.Windows.Media;

namespace HyperPeer
{
    public partial class RegisterScreen : Window
    {
        private LoginScreen loginScreen;
        public RegisterScreen(LoginScreen loginScreen)
        {
            this.loginScreen = loginScreen;
            loginScreen.errorMessage.Text = "";
            InitializeComponent();
            errorMessage.Foreground = Brushes.Red;
            errorMessage.Text = "";
        }

        private void RegisterAccount(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string email = EmailBox.Text;
            string password = PasswordBox.Text;

            bool isEmail = Utils.isEmail(email);

            if (username.Length > 16)
            {
                errorMessage.Text = "The username is too long (max. 16)";
                return;
            }
            
            if (email == null || email.Length < 3 || email == "" || !isEmail)
            {
                errorMessage.Text = "Invalid Email";
                return;
            }

            if (password == null || password.Length < 8 || password == "")
            {
                errorMessage.Text = "Invalid Password";
                return;
            }
            if (Utils.sendRegisterRequest(username, email, password) == "400")
            {
                errorMessage.Text = "An Error Occurred";
                return;
            }
            SwitchBackToLoginWindow();
        }

        private void SwitchBackToLoginWindow()
        {
            loginScreen.errorMessage.Foreground = Brushes.Green;
            loginScreen.errorMessage.Text = "Account created successfully";
            this.Hide();
            loginScreen.WindowState = WindowState.Normal;
            loginScreen.maximizeButton.Content = "◻";
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
