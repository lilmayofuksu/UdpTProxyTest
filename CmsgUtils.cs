using System.Runtime.CompilerServices;
using static UdpTproxyTest.Libc;

namespace UdpTproxyTest {
    public static class CmsgUtils {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static cmsghdr* CMSG_FIRSTHDR(user_msghdr msg) {
            return __CMSG_FIRSTHDR(msg.msg_control, msg.msg_controllen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static cmsghdr* __CMSG_FIRSTHDR(void* ctl, nuint len) {
            if (len >= (nuint)sizeof(cmsghdr)) {
                return (cmsghdr*)ctl;
            }

            return (cmsghdr*)nint.Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint CMSG_ALIGN(nuint len) {
            // Get the size of long in bytes
            nuint longSize = sizeof(long);
            return (len + longSize - 1) & ~(longSize - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static cmsghdr* CMSG_NEXTHDR(user_msghdr* __msg, cmsghdr* __cmsg) {
            return __cmsg_nxthdr(__msg->msg_control, __msg->msg_controllen, __cmsg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static cmsghdr* __cmsg_nxthdr(void* __ctl, nuint __size, cmsghdr* __cmsg) {
            cmsghdr* __ptr;
            __ptr = (cmsghdr*)(((char*)__cmsg) + CMSG_ALIGN(__cmsg->cmsg_len));
            if ((nuint)((char*)(__ptr + 1) - (char*)__ctl) > __size)
                return (cmsghdr*)0;

            return __ptr;
        }

        public static unsafe void* CMSG_DATA(cmsghdr* cmsg) {
            return (byte*)cmsg + sizeof(cmsghdr);
        }
    }
}
