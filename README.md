# ShmoopySoft Syslog Message Sending Example

A Visual Studio 2019 solution written in C# to demonstrate sending messages to a Syslog Server using the SyslogNet open source library for .NET. Supports both RFC 3164 and RFC 5424 Syslog standards.

## Getting Started

In order to send messages to Syslog, you must have a Syslog Server setup and configured. You will need its IP address and UDP Port number to send messages.

### Running

1. Download the solution from our GitHub repository
2. Open the solution in Visual Studio 2019
3. Install the SyslogNet NuGet Package: Install-Package SyslogNet.Client
4. Edit the 'SyslogServer' string constant with your Syslog Server IP address
5. Edit the 'SyslogPort' string constant with your Syslog Server UDP port number (usually 514)
6. Edit the 'AppName' string constant with your Syslog application name
7. Click the Start button, or press F5

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* https://github.com/emertechie/SyslogNet
