﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Globalization;

namespace UDPEchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Person.GetConnection();

            //Creates a UdpClient for reading incoming data.
            UdpClient udpServer = new UdpClient(1111);

            //Creates an IPEndPoint to record the IP Address and port number of the sender.  
            IPAddress ip = IPAddress.Parse("192.168.24.142");
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(ip, 1111);
           
            try
            {
                // Blocks until a message is received on this socket from a remote host (a client).
                Console.WriteLine("Ready to gather data");
                var list = new List<decimal>();
                var listTid = new List<DateTime>();
                while (true)
                {
                    //Køre den egentlig hele tiden altid? Yep, det en while True. Rgr så kan vi godt risikere at den flyder lidt imellem 0-1
                    Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint);
                    //Den stopper her efter de 10 stk, nogen mulighed for at breake hvis den ikke giver svar eller noget?
                    // jep. Let me show.
                    //Server is now activated");
                    //Du har bare sammensat de to metoder rigt? Så ren skriver jeg den lige nemlig
                    //well den kører nu :D, bare mere end 10 gange :D
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);
                    //Kan du finde dit python frem
                    if (receivedData.Contains("stop"))
                        break;

                    string[] data = receivedData.Split(' ');

                    decimal force = 0;
                    if (!string.IsNullOrEmpty(data[8]))
                        force = Convert.ToDecimal(data[8], CultureInfo.InvariantCulture);

                    DateTime tid = new DateTime();
                    if(!string.IsNullOrEmpty(data[3]))
                        tid = Convert.ToDateTime(data[3]);

                    string text = "Tid: " + tid.ToString() + " Force: " + force;
                    Console.WriteLine(text);

                    list.Add(force);
                    listTid.Add(tid);
                    string sendData = "Server: " + text.ToUpper();
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(sendData);

                    udpServer.Send(sendBytes, sendBytes.Length, RemoteIpEndPoint);

                   
                }

                decimal tryngdekraft = 9.82M;
                var avg = list.Average();
                var lastTime = listTid.Last();
                var firstTime = listTid.First();
                var time = lastTime - firstTime;
                decimal accel = avg * tryngdekraft;
                //Hvad er det du skal regne ud fra tiden, er det ms? 
                var hastighed = accel * Convert.ToInt16(time.Seconds);
                
                // thats it. Der sker ikke mere.
                Person.PersonDTO person = new Person.PersonDTO();
                person.Fornavn = "Frank";
                person.Efternavn = "Larsen";
                person.Email = "lol@lol.dk";
                person.Data.Acceleration = Convert.ToDecimal(accel);
                person.Data.Hastighed = Convert.ToDecimal(hastighed);
                person.Data.Tid = "test";
                

                var success = Person.createPerson(person);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
    }
}
