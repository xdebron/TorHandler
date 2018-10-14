# TorHandler
Tor Handler for C#

```csharp
TorHandler th = new TorHandler(true, "Tor/tor.exe", 8080);
th.start_httpserver();

WebProxy proxy = new WebProxy("127.0.0.1", 8080);
proxy.UseDefaultCredentials = true;

WebClient webClient = new WebClient();
webClient.Proxy = proxy;

Console.Write(webClient.DownloadString("https://www.youtube.com/"));
``` 

### Dependencies

* NET Framework 4.0+

### Usage

Following code runs given tor executable and starts tor relay while blocking main thread then wraps it with https proxy listening 8080 port.

```csharp
TorHandler th = new TorHandler(true, "Tor/tor.exe", 8080);
th.start_httpserver();
```