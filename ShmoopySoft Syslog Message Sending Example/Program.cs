/*
 * MIT License
 * 
 * Copyright(c) 2020 ShmoopySoft (Pty) Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

// REQUIRED: SyslogNet 
// NuGet Package Manager Command:
// Install-Package SyslogNet.Client

using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShmoopySoft_Syslog_Message_Sending_Example
{
    // Syslog extension class for SyslogRfc5424MessageSerializer and SyslogRfc3164MessageSerializer.
    // The class provides Serialize() extension methods that return string values.
    internal static class SyslogExtensions
    {
        public static string Serialize(this SyslogRfc5424MessageSerializer serializer, SyslogMessage message)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(message, stream);
                stream.Flush();
                stream.Position = 0;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadLine();
            }
        }

        public static string Serialize(this SyslogRfc3164MessageSerializer serializer, SyslogMessage message)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(message, stream);
                stream.Flush();
                stream.Position = 0;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadLine();
            }
        }

        public static string Serialize(this SyslogLocalMessageSerializer serializer, SyslogMessage message)
        {
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(message, stream);
                stream.Flush();
                stream.Position = 0;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadLine();
            }
        }
    }

    /// <summary>
    /// The Program class's responsibility is to provide an entry point for the application.
    /// </summary>
    class Program
    {
        private const string SyslogServer = "127.0.01";             // <<<!!! INSERT YOUR SYSLOG SERVER IP ADDRESS HERE !!!>>>
        private const int SyslogPort = 514;                         // <<<!!! INSERT YOUR SYSLOG SERVER PORT HERE !!!>>>
        private const string AppName = "My Event Log";              // <<<!!! INSERT YOUR SYSLOG APP-NAME HERE !!!>>>

        /// <summary>
        /// C# applications have an entry point called the Main Method. 
        /// It is the first method that gets invoked when an application starts.
        /// </summary>
        /// <param name="args">Command line arguments as string type parameters</param>
        static void Main(string[] args)
        {
            try
            {
                // Create a new SyslogUdpSender with the SyslogServer and SyslogPort provided.
                // The using block ensures the SyslogUdpSender is automatically closed.
                using (SyslogUdpSender syslogSender = new SyslogUdpSender(SyslogServer, SyslogPort))
                {
                    // Set the message to send.
                    string messageToSend = "Test message at " + DateTime.Now;

                    /////////////////////////////////////////////
                    // Send an RFC 3164 Syslog Message. See:
                    // https://tools.ietf.org/html/rfc3164
                    /////////////////////////////////////////////
                    // In RFC 3164, the message component (known as MSG) was specified as having these fields: TAG, 
                    // which should be the name of the program or process that generated the message, and CONTENT which 
                    // contains the details of the message.
                    // The content field should be encoded in a UTF-8 character set and octet values in the traditional 
                    // ASCII control character range should be avoided.
                    /////////////////////////////////////////////

                    // Display progress.
                    Console.WriteLine("Sending RFC3164 Syslog message: " + messageToSend);

                    // Create a new Syslog message (RFC3164).
                    var rfc3164SyslogMessage = new SyslogMessage(DateTimeOffset.Now,                    // TIMESTAMP
                                                                 Facility.UserLevelMessages,            // FACILITY
                                                                 Severity.Informational,                // SEVERITY
                                                                 Environment.MachineName,               // HOSTNAME
                                                                 AppName,                               // APP-NAME
                                                                 messageToSend);                        // MSG

                    // Create a new Syslog RFC3164 serializer.
                    var rfc3164SyslogSerializer = new SyslogRfc3164MessageSerializer();

                    // Display the serialised message to be sent.
                    Console.Write(Environment.NewLine);
                    Console.WriteLine(rfc3164SyslogSerializer.Serialize(rfc3164SyslogMessage));
                    Console.Write(Environment.NewLine);

                    // Send the Syslog message, using the serializer created.
                    syslogSender.Send(rfc3164SyslogMessage, rfc3164SyslogSerializer);

                    // Display a confirmation.
                    Console.WriteLine("RFC3164 message was successfully sent to Syslog Server :-)");

                    /////////////////////////////////////////////
                    // Send an RFC 5424 Syslog Message. See:
                    // https://tools.ietf.org/html/rfc5424
                    /////////////////////////////////////////////
                    // In RFC 3164, the message component (known as MSG) was specified as having these fields: TAG, 
                    // which should be the name of the program or process that generated the message, and CONTENT which 
                    // contains the details of the message.
                    // Described in RFC 5424,[9] "MSG is what was called CONTENT in RFC 3164. The TAG is now part of the 
                    // header, but not as a single field. The TAG has been split into APP-NAME, PROCID, and MSGID. 
                    // This does not totally resemble the usage of TAG, but provides the same functionality for most of 
                    // the cases." Popular syslog tools such as Rsyslog conform to this new standard.
                    // The content field should be encoded in a UTF-8 character set and octet values in the traditional 
                    // ASCII control character range should be avoided.
                    /////////////////////////////////////////////

                    // Display progress.
                    Console.Write(Environment.NewLine);
                    Console.WriteLine("Sending RFC5424 Syslog message: " + messageToSend);

                    // Create structured data items for RFC5424 compliance.
                    var sdi1 = "myexampleSDID@12345";
                    var key1 = "myeventId";
                    var value1 = "1234";

                    var sdi2 = "myexampleSDID@23456";
                    var key2 = "myeventSource";
                    var value2 = "My Application";

                    // Create a new StructuredDataElement list.
                    var structuredDataElements = new List<StructuredDataElement>
                    {
                        new StructuredDataElement(sdi1, new Dictionary<string, string> {{key1, value1}}),
                        new StructuredDataElement(sdi2, new Dictionary<string, string> {{key2, value2}}),
                    };

                    // Set the PROCID and MSGID.
                    string procId = "My ProcId";
                    string msgId = "My MsgId";

                    // Create a new Syslog message (RFC5424).
                    SyslogMessage rfc5424SyslogMessage = new SyslogMessage(DateTimeOffset.Now,          // TIMESTAMP
                                                                 Facility.UserLevelMessages,            // FACILITY
                                                                 Severity.Informational,                // SEVERITY
                                                                 Environment.MachineName,               // HOSTNAME
                                                                 AppName,                               // APP-NAME
                                                                 procId,                                // PROCID
                                                                 msgId,                                 // MSGID
                                                                 messageToSend,                         // MSG
                                                                 structuredDataElements.ToArray());     // STRUCTURED-DATA

                    // Create a new Syslog RFC5424 serializer.
                    var rfc5424SyslogSerializer = new SyslogRfc5424MessageSerializer();

                    // Display the serialised message to be sent.
                    Console.Write(Environment.NewLine);
                    Console.WriteLine(rfc5424SyslogSerializer.Serialize(rfc5424SyslogMessage));
                    Console.Write(Environment.NewLine);

                    // Send the Syslog message, using the serializer created.
                    syslogSender.Send(rfc5424SyslogMessage, rfc5424SyslogSerializer);

                    // Display a confirmation.
                    Console.WriteLine("RFC5424 message was successfully sent to Syslog Server :-)");

                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                // Display an error.
                Console.WriteLine("Failed to send the message to Syslog :-(");
                Console.Write(Environment.NewLine);
                Console.WriteLine(ex.ToString());

                Console.ReadKey();
            }
        }
    }
}
