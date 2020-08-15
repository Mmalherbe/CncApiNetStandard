using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace OosterhofDesign
{
    public static class StringConversie
    {//StringToMaxCharArray
        public unsafe static string CharArrayToString(IntPtr POINTER, int OFFSET, int RANKL_1)
        {//char = bytesize
            string returnString = "";
            IntPtr withoffst = POINTER + OFFSET;
            char* temp = stackalloc char[RANKL_1];
            
            int result =ASCIIEncoding.ASCII.GetChars((byte*)withoffst, RANKL_1, temp, RANKL_1);
            
            for(int i =0;i<RANKL_1;i++)
            {
                if (temp[i] != '\0')
                {
                    returnString = returnString + temp[i];
                }
                else
                {
                    break;
                }
            }
     
            return returnString;//Marshal.ptr((POINTER + OFFSET), RANKL_1);
        }
        public static string[] CharArrayToString(IntPtr POINTER, int OFFSET, int RANKL_1, int RANKL_2)
        {//char = byte size
            string[] return_result = new string[RANKL_1];
            IntPtr pointerWithOffset = POINTER + OFFSET;

            for(int i =0; i<return_result.Length;i++)
            {
                return_result[i] = Marshal.PtrToStringAnsi((pointerWithOffset+(i*1*RANKL_1)), RANKL_2);
            }

            return return_result;
        }
        public static string UCharArrayToString(IntPtr POINTER, int OFFSET, int RANKL_1)
        {
            return CharArrayToString(POINTER, OFFSET,RANKL_1);
        }
        public static void StringToMaxUCharArray(string VALUE, IntPtr POINTER, int OFFSET, int RANKL_1)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(VALUE);

            unsafe
            {
                for (int i = 0; i < RANKL_1; i++)
                {
                    IntPtr newPointer = POINTER + OFFSET + i;
                    if (i < stringBytes.Length)
                    {
                        *(byte*)newPointer = stringBytes[i];
                    }
                    else
                    {
                        *(byte*)newPointer = 0x00;
                    }
                }
            }
        }


        public unsafe static void StringToMaxCharArray(string VALUE, IntPtr POINTER, int OFFSET, int RANKL_1)
        {
            byte[] temp_Value = Encoding.ASCII.GetBytes(VALUE);
            for (int i = 0; i < RANKL_1; i++)
            {
                IntPtr newpointer = POINTER + OFFSET + i;
                if (i<temp_Value.Length && temp_Value.Length > 0)
                {
                    *(sbyte*)newpointer = (sbyte)temp_Value[i];
                    
                }
                else
                {
                    *(sbyte*)newpointer = 0x00;
                }
            }
        }


        /// <summary>
        /// Convert a string array to a two dimensional char(byte) array
        /// </summary>
        /// <param name="VALUE"></param>
        /// <param name="POINTER">The base pointer without offsets</param>
        /// <param name="OFFSET">Startposition from the charArray</param>
        /// <param name="RANKL_1">The number of strings</param>
        /// <param name="RANKL_2">The mmaximum string length</param>
        public static void StringToMaxCharArray(string[] VALUE, IntPtr POINTER, int OFFSET, int RANKL_1, int RANKL_2)
        {
            IntPtr pointerWithOffset =POINTER + OFFSET;
            for (int i =0;i<RANKL_1;i++)
            {
                if (i< VALUE.Length)
                {
                    StringToMaxCharArray(VALUE[i],pointerWithOffset, i * 1 * RANKL_2, RANKL_2);//i *typezise*max chars in one string
                }
                else
                {
                    StringToMaxCharArray("", pointerWithOffset, i * 1 * RANKL_2, RANKL_2);
                }
            }
        }
        /// <summary>
        /// Converts a string to char array
        /// </summary>
        /// <param name="VALUE"></param>
        /// <param name="POINTER">The base pointer without offsets</param>
        /// <param name="OFFSET">Startposition from the charArray</param>
        /// <param name="RANKL_1">The mmaximum string length</param>
        public static void StringToMaxWCharArray(string VALUE, IntPtr POINTER, int OFFSET, int RANKL_1)
        {//wchar = char size
            char[] chars = VALUE.ToCharArray();
            unsafe
            {
                for (int i = 0; i < RANKL_1; i++)
                {
                    if (i < chars.Length)
                    {
                        ((char*)POINTER)[i] = chars[i];
                    }
                    else
                    {
                        ((char*)POINTER)[i] = '\0';
                    }
                }
            }
        }


        /// <summary>
        /// Converts a char array to string
        /// </summary>
        /// <param name="VALUE"></param>
        /// <param name="POINTER">The base pointer without offsets</param>
        /// <param name="OFFSET">Startposition from the charArray</param>
        /// <param name="RANKL_1">The mmaximum string length</param>
        public static string WCharArrayToString(IntPtr POINTER, int OFFSET, int RANKL_1)
        {//wchar = char size
            string return_result = "";
            unsafe
            {
                IntPtr pointerWithOffset = POINTER + OFFSET;
                for (int i = 0; i < RANKL_1; i++)
                {
                    return_result = return_result+Convert.ToString( *(char*)(pointerWithOffset + (i* Convert.ToInt32(Offst_WcharT.TotalSize))));
                }
            }

            return return_result.TrimEnd();
        }
    }
}
