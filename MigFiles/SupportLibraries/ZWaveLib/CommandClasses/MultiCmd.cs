﻿/*
    This file is part of HomeGenie Project source code.

    HomeGenie is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    HomeGenie is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with HomeGenie.  If not, see <http://www.gnu.org/licenses/>.  
*/

/*
*     Author: https://github.com/mdave
*     Project Homepage: http://homegenie.it
*/
using System;
using ZWaveLib.Values;

namespace ZWaveLib.CommandClasses
{
    public class MultiCmd : ICommandClass
    {
        public CommandClass GetClassId()
        {
            return CommandClass.MultiCmd;
        }

        public ZWaveEvent GetEvent(ZWaveNode node, byte[] message)
        {
            byte i, offset = 3;

            ZWaveEvent parent = null, child = null;

            Utility.DebugLog(DebugMessageType.Information, String.Format("MultiCmd encapsulated message: {0}", Utility.ByteArrayToString(message)));

            if (message[1] != (byte)1)
            {
                return parent;
            }

            // Loop over each message and process it in turn.
            for (i = 0; i < message[2]; i++)
            {
                // Length and command classes of sub-command.
                byte length   = message[offset];
                byte cmdClass = message[offset+1];

                // Copy message into new array.
                var instanceMessage = new byte[length];
                Array.Copy(message, offset+1, instanceMessage, 0, length);
                Utility.DebugLog(DebugMessageType.Information, String.Format("Processing message chunk: {0}", Utility.ByteArrayToString(instanceMessage)));

                // Grab command class from the factory. If we don't have one, print out a warning and continue.
                var cc = CommandClassFactory.GetCommandClass(cmdClass);

                if (cc == null)
                {
                    Utility.DebugLog(DebugMessageType.Information, String.Format("Can't find CommandClass handler for command class {0}", cmdClass));
                    continue;
                }

                // Chain this event onto previously seen events.
                ZWaveEvent tmp = cc.GetEvent(node, instanceMessage);
                offset += (byte)(length+1);

                if (tmp == null)
                {
                    continue;
                }

                if (parent == null)
                {
                    parent = child = tmp;
                }
                else
                {
                    child.NestedEvent = tmp;
                    child = tmp;
                }
            }

            return parent;
        }
    }
}
