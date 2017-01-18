﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;
using com.yppiti.FileCommandProcessor.Processes.Interpreter;
using com.yppiti.FileCommandProcessor.Processes.Interpreter.DTOs;

namespace com.yppiti.FileCommandProcessor.Functions
{
    class Ping
    {
        public static string Error;
        public static void Command(string[] Destination)
        {
            int TimesToPing;
            if (Destination.Count() > 3 || Destination.Count() < 3 || int.TryParse(Destination[2], out TimesToPing))
            {
                Error = "The syntax of the command is incorrect. Expecting destination in hostname or IP address and integrer from 1 to 10 desginating ping attempts.";
                ValidationResponse.ResponseFromCommandClass = Error;
                ValidationResponse.CommandReturnedWasSuccessful = false;
            }

            else
            {
                string LocationToPing = Destination[1];
                Scriptlet_Ping(LocationToPing, TimesToPing);
            }
        }

        protected static void Scriptlet_Ping (string Destination, int Count)
        {
            if (Count > 10 || Count < 0)
            {
                Error = "Ping value must be a value 1 to 10.";
                ValidationResponse.ResponseFromCommandClass = Error;
                ValidationResponse.CommandReturnedWasSuccessful = false;
            }

            try
            {
                var ping = new System.Net.NetworkInformation.Ping();
                var result = ping.Send(Destination);

                if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
                {
                    Error = "Cannot reach destination host '"+ Destination +"'";
                    ValidationResponse.ResponseFromCommandClass = Error;
                    ValidationResponse.CommandReturnedWasSuccessful = false;
                }

                if (result.Status != System.Net.NetworkInformation.IPStatus.TimedOut)
                {
                    Error = "Timed out reaching '" + Destination + "'";
                    ValidationResponse.ResponseFromCommandClass = Error;
                    ValidationResponse.CommandReturnedWasSuccessful = false;
                }

                if (result.Status != System.Net.NetworkInformation.IPStatus.Unknown)
                {
                    Error = "Unknown response received from '" + Destination + "'";
                    ValidationResponse.ResponseFromCommandClass = Error;
                    ValidationResponse.CommandReturnedWasSuccessful = false;
                }

                if (result.Status != System.Net.NetworkInformation.IPStatus.TimeExceeded)
                {
                    Error = "The destination server for '" + Destination + "' discarded the packet sent to verify its status";
                    ValidationResponse.ResponseFromCommandClass = Error;
                    ValidationResponse.CommandReturnedWasSuccessful = false;
                }

                if (result.Status != System.Net.NetworkInformation.IPStatus.NoResources)
                {
                    Error = "Insufficient network resources to process this command.";
                    ValidationResponse.ResponseFromCommandClass = Error;
                    ValidationResponse.CommandReturnedWasSuccessful = false;
                }

                if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    IPHostEntry hostEntry;
                    hostEntry = Dns.GetHostEntry(Destination);
                    string IPAddress = hostEntry.AddressList[0].ToString();
                    string MessageResult = "Successfully received reply from '" + Destination + "' ("+ IPAddress +")";
                    CommandInterpreter.WriteToScreenWithNoInterrupt(AppOnlyScope.Status.CommandInformationMessage(MessageResult));
                    ValidationResponse.CommandReturnedWasSuccessful = true;
                }
            }

            catch (Exception PingLocationException)
            {
                ValidationResponse.ResponseFromCommandClass = PingLocationException.Message;
                ValidationResponse.CommandReturnedWasSuccessful = false;
            }
        }
    }
}
