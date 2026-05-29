using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberPrimeWPF
{
    public class Chatbot
    {
        private Dictionary<string, List<string>> keywordResponses;
        private Dictionary<string, List<string>> sentimentResponses;
        private Dictionary<string, string> userMemory;
        private Random random;
        private string currentTopic;
        private string userName;
        private string userInterest;

        public event Action<string> OnBotResponse;
        public event Action<string> OnUserMemoryUpdated;

        public Chatbot()
        {
            InitializeResponses();
            userMemory = new Dictionary<string, string>();
            random = new Random();
            currentTopic = "";
        }

        private void InitializeResponses()
        {
            keywordResponses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "🔐 Use strong passwords with at least 12 characters, including uppercase, lowercase, numbers, and symbols!",
                    "⚠️ Never reuse passwords across different accounts. Each account needs its own unique password!",
                    "💡 Consider using a password manager to generate and store complex passwords securely.",
                    "🚫 Avoid using personal information like birthdays or pet names in your passwords!"
                },
                ["phishing"] = new List<string>
                {
                    "🎣 Phishing attacks trick you into revealing sensitive information. Always verify email senders!",
                    "⚠️ WARNING: Never click on suspicious links in emails or messages, even if they look legitimate!",
                    "📧 Legitimate companies never ask for passwords via email. When in doubt, contact them directly!",
                    "🔍 Check the URL carefully - scammers use addresses similar to real websites!"
                },
                ["scam"] = new List<string>
                {
                    "🚨 If something sounds too good to be true, it probably is! Scammers prey on urgency and fear.",
                    "⚠️ Never send money or personal information to someone you've only met online!",
                    "📞 Hang up on unsolicited calls asking for personal information or payment.",
                    "💼 Verify charity requests before donating - scammers create fake disaster relief campaigns!"
                },
                ["privacy"] = new List<string>
                {
                    "👤 Review your privacy settings on social media regularly. Limit what you share publicly!",
                    "🔒 Use two-factor authentication (2FA) on all accounts that offer it.",
                    "📱 Be careful what you post - once online, it's permanent even if you delete it!",
                    "🌐 Use a VPN on public Wi-Fi to protect your browsing privacy."
                },
                ["malware"] = new List<string>
                {
                    "🛡️ Keep your antivirus software updated and run regular scans!",
                    "⚠️ Don't download software from untrusted sources - stick to official websites!",
                    "💾 Always backup important data to protect against ransomware attacks.",
                    "🔧 Enable automatic updates for your operating system and applications!"
                },
                ["wifi"] = new List<string>
                {
                    "📶 Avoid accessing sensitive accounts on public Wi-Fi networks!",
                    "🔒 Use a VPN when connecting to public Wi-Fi to encrypt your data.",
                    "🏠 Secure your home Wi-Fi with WPA3 encryption and a strong password.",
                    "🚫 Turn off auto-connect to open Wi-Fi networks to prevent automatic connections!"
                },
                ["2fa"] = new List<string>
                {
                    "🔐 Enable 2FA everywhere! It adds an extra layer of security beyond just passwords.",
                    "📱 Use authenticator apps instead of SMS for better 2FA security.",
                    "💡 Backup your 2FA codes in a secure location in case you lose your phone."
                },
                ["vpn"] = new List<string>
                {
                    "🌐 A VPN encrypts your internet traffic, protecting your data from prying eyes!",
                    "🔒 Always use a reputable VPN service - free VPNs often sell your data.",
                    "📍 VPNs can also help bypass geographic restrictions on content."
                }
            };

            sentimentResponses = new Dictionary<string, List<string>>
            {
                ["worried"] = new List<string>
                {
                    "😟 I understand your concern. Cybersecurity can feel overwhelming, but let me help make it simple.",
                    "🤝 It's completely normal to feel worried about online threats. Let me share some practical steps you can take today.",
                    "💪 Don't worry - you're taking the right step by learning about cybersecurity. Most threats can be prevented with basic awareness!"
                },
                ["curious"] = new List<string>
                {
                    "🤔 Great question! Curiosity is the first step to staying safe online. Let me explain...",
                    "💡 I love your interest in cybersecurity! Here's what you should know...",
                    "📚 Excellent question! Cybersecurity is fascinating. Here are the key points..."
                },
                ["frustrated"] = new List<string>
                {
                    "😤 I know cybersecurity can be frustrating, but taking it step by step helps. Let's start simple.",
                    "🤗 Take a deep breath. We'll work through this together. Here's an easy tip to start with...",
                    "💪 You've got this! Let me break this down into smaller, manageable steps."
                }
            };
        }

        public string ProcessInput(string input)
        {
            string normalizedInput = input.ToLower().Trim();

            // Check for user name if not set
            if (string.IsNullOrEmpty(userName) && !normalizedInput.Contains("my name is"))
            {
                if (normalizedInput.Length < 30 && !normalizedInput.Contains("?") && !normalizedInput.Contains("help"))
                {
                    userName = CapitalizeName(normalizedInput);
                    userMemory["name"] = userName;
                    OnUserMemoryUpdated?.Invoke($"User: {userName}");
                    return $"Nice to meet you, {userName}! 👋\nI'll remember your name. What would you like to know about cybersecurity?";
                }
            }

            // Handle "my name is" pattern
            if (normalizedInput.Contains("my name is"))
            {
                var parts = normalizedInput.Split(new[] { "my name is" }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    userName = CapitalizeName(parts[1].Trim());
                    userMemory["name"] = userName;
                    OnUserMemoryUpdated?.Invoke($"User: {userName}");
                    return $"Great to meet you properly, {userName}! 🎉\nI've saved your name. Now, what cybersecurity topic interests you?";
                }
            }

            // Sentiment detection
            string sentiment = DetectSentiment(normalizedInput);
            if (!string.IsNullOrEmpty(sentiment) && sentimentResponses.ContainsKey(sentiment))
            {
                var sentimentResponse = GetRandomResponse(sentimentResponses[sentiment]);
                OnBotResponse?.Invoke(sentimentResponse);
                // Continue to provide actual cybersecurity tip
            }

            // Handle follow-up requests
            if (normalizedInput.Contains("another tip") || normalizedInput.Contains("tell me more") ||
                normalizedInput.Contains("explain more") || normalizedInput.Contains("more detail"))
            {
                if (!string.IsNullOrEmpty(currentTopic) && keywordResponses.ContainsKey(currentTopic))
                {
                    return GetRandomResponse(keywordResponses[currentTopic]);
                }
                return "What topic would you like another tip about? Try asking about passwords, phishing, scams, privacy, or malware!";
            }

            // Handle interest setting
            if (normalizedInput.Contains("interested in") || normalizedInput.Contains("i like") ||
                normalizedInput.Contains("my favorite") || normalizedInput.Contains("tell me about"))
            {
                foreach (var topic in keywordResponses.Keys)
                {
                    if (normalizedInput.Contains(topic))
                    {
                        userInterest = topic;
                        userMemory["interest"] = topic;
                        OnUserMemoryUpdated?.Invoke($"User: {userName} | Interest: {topic}");
                        return $"Great! I'll remember that you're interested in {topic}. 🌟\n" +
                               GetRandomResponse(keywordResponses[topic]);
                    }
                }
            }

            // Check for personalized greeting using memory
            if (!string.IsNullOrEmpty(userName) && (normalizedInput.Contains("hello") || normalizedInput.Contains("hi") ||
                normalizedInput.Contains("hey")))
            {
                if (!string.IsNullOrEmpty(userInterest))
                {
                    return $"Hello again, {userName}! 👋\nAs someone interested in {userInterest}, would you like another tip on that topic?";
                }
                return $"Hello, {userName}! 👋 How can I help you with cybersecurity today?";
            }

            // Keyword recognition
            foreach (var keyword in keywordResponses.Keys)
            {
                if (normalizedInput.Contains(keyword))
                {
                    currentTopic = keyword;
                    return GetRandomResponse(keywordResponses[keyword]);
                }
            }

            // Help command
            if (normalizedInput.Contains("help") || normalizedInput.Contains("what can you do") ||
                normalizedInput.Contains("commands"))
            {
                return GetHelpMessage();
            }

            // Default response for unknown input
            return GetDefaultResponse();
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious") ||
                input.Contains("nervous") || input.Contains("concerned") || input.Contains("afraid"))
                return "worried";

            if (input.Contains("curious") || input.Contains("interested") || input.Contains("wonder") ||
                input.Contains("tell me") || input.Contains("explain") || input.Contains("how"))
                return "curious";

            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("angry") ||
                input.Contains("tired") || input.Contains("ugh") || input.Contains("confused"))
                return "frustrated";

            return "";
        }

        private string GetRandomResponse(List<string> responses)
        {
            return responses[random.Next(responses.Count)];
        }

        private string GetHelpMessage()
        {
            return @"📚 **Available Cybersecurity Topics** 📚

🔐 **passwords** - Password security tips
🎣 **phishing** - Avoid phishing scams
⚠️ **scam** - Recognize and avoid scams
👤 **privacy** - Protect your privacy online
🛡️ **malware** - Malware protection
📶 **wifi** - Secure Wi-Fi usage
🔑 **2fa** - Two-factor authentication
🌐 **vpn** - VPN benefits

💬 **Commands:**
• 'another tip' - Get another tip on current topic
• 'tell me more' - Get detailed explanation
• 'interested in [topic]' - Save your interest
• 'hello/hi' - Personalized greeting
• 'my name is [name]' - Set your name

What would you like to learn about?";
        }

        private string GetDefaultResponse()
        {
            string[] defaultResponses = {
                "I'm not sure I understand. Could you rephrase that? Type 'help' to see available topics.",
                "Hmm, I didn't catch that. Try asking about passwords, phishing, scams, or type 'help' for options.",
                "I'm still learning! Could you ask about specific cybersecurity topics like passwords or privacy?",
                "Not quite sure what you mean. Would you like to learn about password safety, phishing, or online privacy?"
            };
            return defaultResponses[random.Next(defaultResponses.Length)];
        }

        private string CapitalizeName(string name)
        {
            var parts = name.Trim().Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                if (!string.IsNullOrEmpty(parts[i]) && parts[i].Length > 0)
                {
                    parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", parts);
        }
    }
}