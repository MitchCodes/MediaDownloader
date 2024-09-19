# Media Downloader

A C# console application built with .NET 8 that downloads media from various platforms (YouTube, Instagram, etc.) using `yt-dlp`, extracts audio using `ffmpeg`, generates captions using OpenAI's Whisper API, and summarizes the content using ChatGPT.

## **Features**

- **Download Media:** Download videos from various platforms using `yt-dlp`.
- **Time Range Selection:** Option to download a specific time range of the media.
- **Custom Download Folder:** Specify a custom folder or use the default downloads directory.
- **Caption Generation:** Convert video to audio and generate captions using OpenAI's Whisper API.
- **Content Summarization:** Summarize the video content using OpenAI's ChatGPT with a default or custom prompt.

## **Prerequisites**

1. **.NET 8 SDK**

   - Download and install from the [.NET website](https://dotnet.microsoft.com/download/dotnet/8.0).

2. **yt-dlp**

   - Install `yt-dlp` and ensure it's added to your system's PATH.
   - **Installation Instructions:**
     - **Windows:** Download the executable from the [yt-dlp releases page](https://github.com/yt-dlp/yt-dlp/releases/latest) and place it in a folder that's in your PATH.
     - **macOS/Linux:** Install via `pip`:

       ```bash
       pip install yt-dlp
       ```

3. **ffmpeg**

   - Install `ffmpeg` and ensure it's added to your system's PATH.
   - **Installation Instructions:**
     - **Windows:** Download from the [ffmpeg website](https://ffmpeg.org/download.html#build-windows).
     - **macOS:** Install via Homebrew:

       ```bash
       brew install ffmpeg
       ```

     - **Linux:** Install via your distribution's package manager:

       ```bash
       sudo apt-get install ffmpeg
       ```

4. **OpenAI API Key**

   - Sign up for an account at [OpenAI](https://platform.openai.com/).
   - Generate an API key from the [API keys page](https://platform.openai.com/account/api-keys).
   - **Enter the API key when running the app for the first time (it will prompt you)**

     - It saves a file `openaikey.txt` in a directory named `.mitchcodesmediadownloader` in your user home directory.

## **Installation**

1. **Clone the Repository**

   ```bash
   git clone https://github.com/MitchCodes/MediaDownloader.git
   cd MediaDownloader
