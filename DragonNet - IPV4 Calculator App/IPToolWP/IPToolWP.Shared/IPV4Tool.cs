using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IPToolWP
{
    public class IPV4Tool
    {
        // first (0x00) only for fill - so array starts with bit 1
        static byte[] Bit = { 0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, (byte)0x80 };

        protected string IP = null;
        protected byte[] IPBytes = null;
        protected int NetworkPrefix; 
        protected bool Bit1 = false;

        //default constructor
        public IPV4Tool(string IP)
        {
            this.SetIP(IP);
        }

        public bool Allow1STBit()
        {
            return Bit1;
        }

        public void SetAllow1STBit(bool b)
        {
            Bit1 = b;
        }
        public String GetIP()
        {
            return this.IP;
        }
        public void SetIP(String IP)
        {
            bool Valid = false;
            Valid = CheckIPValid(IP);
            if (Valid)
            {
                this.IP = IP;
                this.IPBytes = GetBytes();
                this.SetPrefix2NetworkClassPrefix();
            }
            else this.IP = null;
        }

        public bool CheckIPValid(String IP)
        {
            // Split string by ".", check that array length is 3
            string[] Octets = IP.Split('.');
            if (Octets.Length != 4)
            {
                return false;
            }
            // Check each substring checking that the int value is less than 255 
            // and that is char[] length is !>     2
            Int16 Maxvalue = 255;
            Int32 Temp; // Parse returns Int32
            foreach (String StrOctet in Octets)
            {
                //Check the length
                if (StrOctet.Length > 3)
                {
                    return false;
                }
                //check to see if null
                if (StrOctet == "")
                {
                    return false;
                }
                //parse if first cases are passed
                Temp = int.Parse(StrOctet);
                if (Temp > Maxvalue)
                {
                    return false;
                }
            }
            return true;
        }
        //	return array with all bytes of IP
        public byte[] GetBytes()
        {
            string[] Octets = new string[4];
            Octets = IP.Split('.');
            byte[] Array = new byte[4];
            int Counter = 0;
            while (Counter < 4)
            {
                Array[Counter] = Convert.ToByte(Octets[Counter]);
                Counter++;
            }
            return Array;
        }
        // return first byte of IP
        public byte Get1Byte()
        {
            return this.IPBytes[0];
        }

        //	return second byte of IP
        public byte Get2Byte()
        {
            return this.IPBytes[1];
        }

        //	return third byte of IP
        public byte Get3Byte()
        {
            return this.IPBytes[2];
        }

        //	return fourth byte of IP
        public byte Get4Byte()
        {
            return this.IPBytes[3];
        }

        public void SetPrefix2NetworkClassPrefix()
        {
            this.setNetworkPrefix(this.GetNetworkClassPrefix());
        }

        public void setNetworkPrefix(int Prefix)
        {
            this.NetworkPrefix = Prefix;
        }
        public String GetNetworkMaskByPrefix(int Prefix)
        {
            switch (Prefix)
            {
                case 8: return "255.0.0.0";
                case 9: return "255.128.0.0";
                case 10: return "255.192.0.0";
                case 11: return "255.224.0.0";
                case 12: return "255.240.0.0";
                case 13: return "255.248.0.0";
                case 14: return "255.252.0.0";
                case 15: return "255.254.0.0";
                case 16: return "255.255.0.0";
                case 17: return "255.255.128.0";
                case 18: return "255.255.192.0";
                case 19: return "255.255.224.0";
                case 20: return "255.255.240.0";
                case 21: return "255.255.248.0";
                case 22: return "255.255.252.0";
                case 23: return "255.255.254.0";
                case 24: return "255.255.255.0";
                case 25: return "255.255.255.128";
                case 26: return "255.255.255.192";
                case 27: return "255.255.255.224";
                case 28: return "255.255.255.240";
                case 29: return "255.255.255.248";
                case 30: return "255.255.255.252";
                default: return "";
            }
        }

        public int GetNetworkClassPrefix()
        {
            // set network-prefix with Class-Type
            int NetClass = GetNetworkClass();
            switch (NetClass)
            {
                case 0: return 8;	// Class A
                case 1: return 16;	// Class B									
                case 2: return 24;	// Class C									
            }
            return 30;//return 30 for class D and E, UI wont show anyways
        }
        public String GetNetworkClassName()
        {
            switch (this.GetNetworkClass())
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                case 4: return "E";
                default: return "Error";
            }
        }

        public int GetNetworkClass()
        {
            //Class A
            if (!IPV4Tool.IsBitSet(this.Get1Byte(), 8)) return 0;
            //Class B
            else if (!IPV4Tool.IsBitSet(this.Get1Byte(), 7)) return 1;
            //Class C
            else if (!IPV4Tool.IsBitSet(this.Get1Byte(), 6)) return 2;
            //Class D
            else if (!IPV4Tool.IsBitSet(this.Get1Byte(), 5)) return 3;
            //Class E
            else return 4;
        }
       
        public int GetMaxNetworkHosts()
        {
            return ((int)Math.Pow(2, (32 - this.NetworkPrefix))) - 2;
        }

        public int GetMaxSubNets()
        {
            int count = (int)Math.Pow(2, this.NetworkPrefix - this.GetNetworkClassPrefix());

            // 1 bit for routing
            if (!this.Allow1STBit() || this.GetNetworkClassPrefix() == this.NetworkPrefix)
                count -= 2;
            if (count < 0) count = 1;
            return count;
        }
        public long GetNetworkLong()
        {
            long mask = (long)Math.Pow(2, this.NetworkPrefix) - 1;
            mask = mask << (32 - this.NetworkPrefix);

            return (mask & IP2Long());
        }

        public String GetNetwork()
        {
            return Long2String(GetNetworkLong());
        }

        public long GetBroadCastLong()
        {
            long netMask = (long)Math.Pow(2, this.NetworkPrefix) - 1;
            netMask = netMask << (32 - this.NetworkPrefix);
            long hostMask = (long)Math.Pow(2, 32 - this.NetworkPrefix) - 1;

            return (netMask = (IP2Long() & netMask) | hostMask);
        }

        public String GetBroadCast()
        {
            return Long2String(GetBroadCastLong());
        }


        public String[] GetNetworkIPRange()
        {
            String[] result = new String[2];
            result[0] = Long2String(GetNetworkLong() + 1);
            result[1] = Long2String(GetBroadCastLong() - 1);
            return result;
        }

        public static bool IsBitSet(byte b, int Bit)
        {
            bool r = false;
            if (Bit >= 1 && Bit <= 8)
                r = ((b & IPV4Tool.Bit[Bit]) != 0);

            return r;
        }
        public static int Byte2Int(byte b)
        {
            int i = b;
            if (b < 0) i = b & 0x7f + 128;

            return i;
        }
        public long IP2Long()
        {
            return ((long)(IPV4Tool.Byte2Int(this.Get1Byte()) * 256 +
                IPV4Tool.Byte2Int(this.Get2Byte())) * 256 +
                IPV4Tool.Byte2Int(this.Get3Byte())) * 256 +
                IPV4Tool.Byte2Int(this.Get4Byte());
        }

        public String Long2String(long IP)
        {
            long One = (long)(IP & 0xff000000) >> 24;
            long Two = (long)(IP & 0x00ff0000) >> 16;
            long Three = (long)(IP & 0x0000ff00) >> 8;
            long Four = (long)(IP & 0xff);

            return One + "." + Two + "." + Three + "." + Four;
        }
    }
}
