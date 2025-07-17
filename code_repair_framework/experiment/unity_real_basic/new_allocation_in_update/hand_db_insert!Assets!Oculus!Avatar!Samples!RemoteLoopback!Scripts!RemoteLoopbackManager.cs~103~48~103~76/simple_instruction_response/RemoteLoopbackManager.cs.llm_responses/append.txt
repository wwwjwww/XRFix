            foreach (var packet in packetQueue)
            {
                packet.FakeLatency -= Time.deltaTime;

                if (packet.FakeLatency < 0f)
                {
                    ReceivePacketData(packet.PacketData);
                    deadList.Add(packet);
                }
            }

            foreach (var packet in deadList)
            {
                packetQueue.Remove(packet);
            }
        }
    }
    
    void SendPacketData(byte[] data)
    {
        PacketLatencyPair PacketPair = new PacketLatencyPair();
        PacketPair.PacketData = data;
        PacketPair.FakeLatency = LatencySettings.NextValue();

        packetQueue.AddLast(PacketPair);
    }

    void ReceivePacketData(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int sequence = reader.ReadInt32();

            OvrAvatarPacket avatarPacket;
            if (LoopbackAvatar.UseSDKPackets)
            {
                int size = reader.ReadInt32();
                byte[] sdkData = reader.ReadBytes(size);

                IntPtr packet = CAPI.ovrAvatarPacket_Read((UInt32)data.Length, sdkData);
                avatarPacket = new OvrAvatarPacket { ovrNativePacket = packet };
            }
            else
            {
                avatarPacket = OvrAvatarPacket.Read(inputStream);
            }

            LoopbackAvatar.GetComponent<OvrAvatarRemoteDriver>().QueuePacket(sequence, avatarPacket);
        }
    }
}
