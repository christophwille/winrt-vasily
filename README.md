Vasily
======================

Check connectivity to services on remote servers. This is like ping, but more specialized as it doesn't only test whether the server is reachable, but checks whether a specific service is listening.

As WinRT does not support ICMP in Windows.Networking.Sockets, a direct StreamSocket connection is used to "ping" the remote server and check whether it is available.

Screenshot

![In Action](https://raw.github.com/christophwille/winrt-vasily/master/screenshot.png)
