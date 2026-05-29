# CyberPrimeWPF
On this second part I converted my console application into a GUI using a WPF, I then went on and updated my code, 
added a **Memory Recall** feature that will allow my chatbot to remember the previously sent messages by the user .
I have added a **Sentiment Detection** which will allow the chatbot to adjust it respnoses based on the user's sentiment to create an empathetic interaction.
There's also a **Keyword Recognition** feature that will enhance the chatbotsabiloty to recognise and respond to specific cybersecurity related topics and general enquiries.

**Rendom Response Handler** I have added a variation of predefined responses for common cybersecurity queries.
**Conversation Workflow** This feature enables the chatbot to maintain a conversational workflow that feels natural and responsive

All of these features were not implemented on the first part, and this also the change I have made. You can also say, it is the difference between part 1 and part 2

**Project Structure**

<img width="678" height="235" alt="image" src="https://github.com/user-attachments/assets/fffc7497-4443-497f-93ab-cb82336774d7" />
Key Components
1. MainWindow (View)
Manages UI elements and user interactions

Handles input validation and display updates

Coordinates between user and Chatbot

2. Chatbot (Controller/Model)
Processes user input using keyword matching

Manages conversation context and memory

Detects sentiment and provides empathetic responses

Generates random responses from predefined lists

3. AudioService
Plays audio files asynchronously

Gracefully handles missing files

**Overview**
CyberPrime is an intelligent, GUI-based cybersecurity awareness chatbot designed to educate users about online safety through natural, empathetic conversations. Built with WPF (.NET 6.0), it features sentiment detection, memory capabilities, and dynamic responses to create an engaging learning experience.

**Visual Features**
ASCII art logo display

Color-coded chat bubbles (user, bot, warnings)

Typing indicator animation

Responsive layout that adapts to window size





