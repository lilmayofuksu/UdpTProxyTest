using System.Runtime.InteropServices;

namespace UdpTproxyTest {
    public unsafe static class Libc {
        [StructLayout(LayoutKind.Sequential)]
        public struct user_msghdr {
            public void* msg_name;           // Pointer to socket address structure
            public int msg_namelen;          // Size of socket address structure
            public void* msg_iov;            // Pointer to scatter/gather array
            public nuint msg_iovlen;         // Number of elements in msg_iov
            public void* msg_control;         // Pointer to ancillary data
            public nuint msg_controllen;      // Ancillary data buffer length
            public uint msg_flags;            // Flags on received message
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct cmsghdr {
            public nuint cmsg_len;
            public int cmsg_level;
            public int cmsg_type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct iovec {
            public void* iov_base;  /* BSD uses caddr_t (1003.1g requires void *) */
            public nuint iov_len; /* Must be size_t (1003.1g) */
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SockAddrIn {
            public short SinFamily; // Address family
            public ushort SinPort;   // Port number
            public uint SinAddr;     // IP address
            public ulong SinZero;   // Padding

            public SockAddrIn(short family, ushort port, uint addr) {
                SinFamily = family;
                SinPort = port;
                SinAddr = addr;
                SinZero = 0;
            }
        }

        [DllImport("libc")]
        public static extern nuint recvmsg(int sockfd, user_msghdr* msg, int flags);
    }
}
