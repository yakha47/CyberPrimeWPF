using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CyberPrimeWPF
{
    public partial class MainWindow : Window
    {
        private Chatbot chatbot;
        private AudioService audioService;
        private DispatcherTimer typingTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
            LoadAsciiArt();
            PlayStartupAudio();
            ShowWelcomeMessage();
        }

        private void InitializeChatbot()
        {
            chatbot = new Chatbot();
            chatbot.OnBotResponse += AddBotMessage;
            chatbot.OnUserMemoryUpdated += UpdateUserInfo;

            audioService = new AudioService();
        }

        private void LoadAsciiArt()
        {
            string[] logoLines = {
                @"   ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██████╗ ██████╗ ██╗███╗   ███╗███████╗",
                @"  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔══██╗██║████╗ ████║██╔════╝",
                @"  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██████╔╝██████╔╝██║██╔████╔██║█████╗  ",
                @"  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██╔═══╝ ██╔══██╗██║██║╚██╔╝██║██╔══╝  ",
                @"  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║██║     ██║  ██║██║██║ ╚═╝ ██║███████╗",
                @"   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝  ╚═╝╚═╝╚═╝     ╚═╝╚══════╝",
                @"",
                @""                    
                @"                     Cybersecurity Awareness Chatbot"
            };

            AsciiArt.Text = string.Join("\n", logoLines);
            AsciiArt.FontSize = 9;
        }

        private void PlayStartupAudio()
        {
            Task.Run(() => audioService.PlayGreeting());
        }

        private void ShowWelcomeMessage()
        {
            AddBotMessage("Welcome to CyberPrime Security System! 🔒");
            AddBotMessage("I'm your cybersecurity awareness chatbot.");
            AddBotMessage("What's your name?");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift))
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            // Add user message to chat
            AddUserMessage(userInput);
            InputTextBox.Clear();

            // Show typing indicator
            ShowTypingIndicator(true);

            // Simulate processing delay
            await Task.Delay(500);

            // Process response
            string response = chatbot.ProcessInput(userInput);

            // Hide typing indicator
            ShowTypingIndicator(false);

            // Add bot response
            AddBotMessage(response);

            // Scroll to bottom
            ScrollToBottom();
        }

        private async void VoiceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowTypingIndicator(true);
            await Task.Delay(500);
            AddBotMessage("🎤 Voice input feature is ready! (Please type your question for now)");
            ShowTypingIndicator(false);
        }

        private void AddUserMessage(string message)
        {
            var chatMessage = new ChatMessage
            {
                Sender = "You",
                Message = message,
                Style = (Style)FindResource("ChatBubbleUser"),
                Foreground = Brushes.White,
                Timestamp = DateTime.Now.ToString("HH:mm:ss"),
                Margin = new Thickness(100, 5, 20, 5)
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatListBox.Items.Add(chatMessage);
            });
        }

        private void AddBotMessage(string message)
        {
            var isWarning = message.ToLower().Contains("warning") ||
                           message.ToLower().Contains("alert") ||
                           message.ToLower().Contains("danger");

            var chatMessage = new ChatMessage
            {
                Sender = "CyberPrime",
                Message = message,
                Style = isWarning ? (Style)FindResource("WarningBubble") : (Style)FindResource("ChatBubbleBot"),
                Foreground = isWarning ? Brushes.Red : Brushes.LightGreen,
                Timestamp = DateTime.Now.ToString("HH:mm:ss"),
                Margin = new Thickness(20, 5, 100, 5)
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatListBox.Items.Add(chatMessage);
                ScrollToBottom();
            });
        }

        private void ShowTypingIndicator(bool show)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (show)
                {
                    StatusText.Text = "CyberPrime is typing...";
                    StatusText.Foreground = Brushes.Yellow;
                }
                else
                {
                    StatusText.Text = "Connected";
                    StatusText.Foreground = Brushes.Green;
                }
            });
        }

        private void UpdateUserInfo(string info)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UserInfo.Text = info;
            });
        }

        private void ScrollToBottom()
        {
            if (ChatListBox.Items.Count > 0)
            {
                ChatListBox.ScrollIntoView(ChatListBox.Items[ChatListBox.Items.Count - 1]);
            }
        }
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public Style Style { get; set; }
        public Brush Foreground { get; set; }
        public string Timestamp { get; set; }
        public Thickness Margin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}