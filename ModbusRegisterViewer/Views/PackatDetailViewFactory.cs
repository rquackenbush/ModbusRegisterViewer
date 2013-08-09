﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Modbus.Utility;
using ModbusRegisterViewer.Model;
using ModbusRegisterViewer.ViewModel;
using ModbusRegisterViewer.Views.GenericPacketViewers;
using Unme.Common;

namespace ModbusRegisterViewer.Views
{
    public static class PackatDetailViewFactory
    {
        public static UIElement CreateView(PacketViewModel packet)
        {
            if (packet == null)
                return null;

            switch (packet.Function)
            {
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:

                    if (packet.Direction == MessageDirection.Response)
                    {
                        ushort startingAddress = 0;

                        //To display the correct register numbers, we need to look at the request packet
                        // to see which register was asked for.
                        if (packet.PreviousPacket != null && packet.PreviousPacket.Message.Length > 3)
                        {
                            // Get the starting address bytes
                            startingAddress = MessageUtilities.NetworkBytesToUInt16(packet.Message, 2);

                            //Increase this by one (modbus standard)
                            startingAddress++;
                        }

                        //Get the length of the payload
                        byte length = packet.Message[2];

                        //Get the payload
                        var registerBytes = packet.Message.Skip(3).Take(length).ToArray();

                        //And now get the uint16 values for the registers
                        var registers = ModbusUtility.NetworkBytesToHostUInt16(registerBytes);

                        return new RegistersView(packet, startingAddress, registers);
                    }

                    break;

                case FunctionCodes.WriteMultipleRegisters:

                    if (packet.Direction == MessageDirection.Request)
                    {
                        if (packet.Message.Length > 6)
                        {
                            //Get the starting address bytes
                            var startingAddress = MessageUtilities.NetworkBytesToUInt16(packet.Message, 2);

                            // Get the number of registers
                            var numberOfRegisters = MessageUtilities.NetworkBytesToUInt16(packet.Message, 4);

                            //Get the number of bytes to expect
                            var byteCount = packet.Message[6];

                            //Increase this by one (modbus standard)
                            startingAddress++;

                            //Get the payload
                            var registerBytes = packet.Message.Slice(7, packet.Message.Length - 9).ToArray();

                            //Convert it to uint16
                            var registers = ModbusUtility.NetworkBytesToHostUInt16(registerBytes);

                            return new RegistersView(packet, startingAddress, registers);
                        }
                    }

                    break;
            }

            return null;
        }
    }
}
