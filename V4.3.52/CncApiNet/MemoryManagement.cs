using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace OosterhofDesign
{
    public class PropertyChangedEvent : INotifyPropertyChanged
    {
        private PropertyInfo[] AllProperties = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyChangedEvent()
        {
            AllProperties = this.GetType().GetProperties();
        }

        public virtual void OnPropertyChanged(string PROPERTY_NAME)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PROPERTY_NAME));
        }
        public virtual void SetPropertyIfChanged<T>(T VALUE, ref T DESTINATION, [System.Runtime.CompilerServices.CallerMemberName] string PROPERTY_NAME = "")
        {
            for (int i = 0; i < AllProperties.Length; i++)
            {
                if (AllProperties[i].Name == PROPERTY_NAME)
                {

                    object currentValue = AllProperties[i].GetValue(this);

                    if (currentValue == null && VALUE != null || currentValue.GetType().Equals(VALUE) == false)
                    {
                        if (currentValue == null && VALUE != null)
                        {
                            DESTINATION = VALUE;
                            OnPropertyChanged(PROPERTY_NAME);
                        }
                        else if (currentValue.GetType().IsArray == true)
                        {

                            Array arrayCurrentValue = ((Array)currentValue);
                            Array arrayNewValue = (Array)((object)VALUE);

                            if (arrayCurrentValue.Rank == arrayNewValue.Rank && arrayCurrentValue.Length == arrayNewValue.Length)
                            {
                                int ranks = arrayCurrentValue.Rank;
                                if (ranks == 1)
                                {
                                    Array[] R1_arrayCurrentValue = (Array[])arrayCurrentValue;
                                    Array[] R1_arrayNewValue = (Array[])arrayNewValue;
                                    for (int f = 0; f < R1_arrayCurrentValue.Length; f++)
                                    {
                                        if (R1_arrayCurrentValue[f].Equals(R1_arrayNewValue[i]) == false)
                                        {
                                            DESTINATION = VALUE;
                                            OnPropertyChanged(PROPERTY_NAME);
                                            break;
                                        }
                                    }
                                }
                                else
                                {//not yet implented
                                    DESTINATION = VALUE;
                                    OnPropertyChanged(PROPERTY_NAME);
                                    //arrayCurrentValue.GetValue(,);
                                }
                            }
                            else
                            {
                                DESTINATION = VALUE;
                                OnPropertyChanged(PROPERTY_NAME);
                            }

                            /*
                            object newValue = VALUE;
                            if (Enumerable.SequenceEqual((T[])currentValue, (T[])newValue) == false)
                            {

                            }*/
                        }
                        else if (AllProperties[i].GetValue(this).Equals(VALUE) == false)
                        {
                            DESTINATION = VALUE;
                            OnPropertyChanged(PROPERTY_NAME);
                        }
                    }
                    break;
                }
            }
        }


    }



    public interface IMemoryManagement
    {
        int GetPointerSize();
        void UpdatePointer(IntPtr POINTER);

        
    }
    public abstract unsafe class MemoryManagement : PropertyChangedEvent, IDisposable, IMemoryManagement
    {
        private IntPtr _pointer;
        private readonly object LockObject_Pointer = new object();
        private int _Id = 0;
        public int Id { get => _Id; set => SetPropertyIfChanged(value, ref _Id); }
        public int Size { get; }
        public bool NewPointer { get; }

        private bool _IsDisposed;
        public bool IsDisposed { get => _IsDisposed; private set => SetPropertyIfChanged(value, ref _IsDisposed); }
        public IntPtr Pointer
        {
            get
            {
                lock (LockObject_Pointer)
                {
                    if (IsDisposed == true)
                    {
                        throw new ObjectDisposedException("CncType is disposed");
                    }

                    return _pointer;
                }
            }
            set
            {
                lock (LockObject_Pointer)
                {
                    if (NewPointer == false)
                    {
                        SetPropertyIfChanged(value, ref _pointer);
                        _pointer = value;
                    }
                    else
                    {
                        throw new NotSupportedException("Error: only new pointers can be changed. check first the NewPointer property");
                    }
                }

            }
        }
        public MemoryManagement(int POINTER_SIZE)
        {
            Size = POINTER_SIZE;
            _pointer = Marshal.AllocHGlobal(POINTER_SIZE);
            NewPointer = true;
            ResetData();
        }
        public MemoryManagement(IntPtr POINTER, int POINTER_SIZE)
        {
            
            NewPointer = false;
            Size = POINTER_SIZE;
            _pointer = POINTER;
        }
        ~MemoryManagement()
        {
            Dispose();
        }
        public void Dispose()
        {
            lock (LockObject_Pointer)
            {
                if (IsDisposed == false && NewPointer == true)
                {
                    Marshal.FreeHGlobal(_pointer);
                    IsDisposed = true;
                }
            }
        }

        public void UpdatePointer(IntPtr POINTER)
        {
            lock (LockObject_Pointer)
            {
                _pointer = POINTER;
            }
        }
        public int GetPointerSize()
        {
            return Size;
        }

        public void ResetData()
        {
            lock (LockObject_Pointer)
            {
                for (int i = 0; i < Size; i++)
                {
                    *(byte*)(_pointer + i) = 0x00;
                }
            }
        }

        public bool WriteBytesToPointer(byte[] DATA)
        {
            return WriteBytesToPointer(DATA,0);
        }
        public bool WriteBytesToPointer(byte[] DATA, int OFFSET)
        {
            lock (LockObject_Pointer)
            {
                bool return_value = false;
                if (DATA.Length == Size)
                {
                    for (int i = 0; i < Size; i++)
                    {
                        *((byte*)_pointer + i) = DATA[i];
                    }
                    return_value = true;
                }
                return return_value;
            }
        }
        public byte[] CopyBytesToArray()
        {
            lock (LockObject_Pointer)
            {
                byte[] return_result = new byte[Size];

                for (int i = 0; i < Size; i++)
                {
                    return_result[i] = ((byte*)_pointer)[i];
                }
                return return_result;
            }
        }
        /// <summary>
        /// loop to update all cnc_types with an array that has one rank 
        /// </summary>
        /// <param name="CNC_REF_OBJECTS">the cnc type. This value is ony used when an array index is null</param>
        /// <param name="POINTER">the pointer with startoffset</param>
        /// <param name="TYPE">the array with all the cnc objects</param>
        internal void UpdateRefType(IMemoryManagement[] CNC_REF_OBJECTS, int OFFSET, Type TYPE)
        {
            for (int i = 0; i < CNC_REF_OBJECTS.Length; i++)
            {
                if (CNC_REF_OBJECTS[i] == null)
                {
                    CNC_REF_OBJECTS[i] = (IMemoryManagement)Activator.CreateInstance(TYPE, new object[] { ((Pointer + OFFSET) + (CNC_REF_OBJECTS[i].GetPointerSize() * i)) });
                }
                else
                {
                    CNC_REF_OBJECTS[i].UpdatePointer(((Pointer+OFFSET) + (CNC_REF_OBJECTS[i].GetPointerSize() * i)));
                }
            }
        }
        internal void UpdateRefType(IMemoryManagement[,] CNC_REF_OBJECTS, int OFFSET, Type TYPE)//? is mogelijk niet goed
        {
            int i_Length = CNC_REF_OBJECTS.GetLength(0);
            int f_Length = CNC_REF_OBJECTS.GetLength(1);

            for (int i = 0; i < i_Length; i++)
            {
                for (int f =0;f< f_Length;f++)
                {
                    int pointerSize = CNC_REF_OBJECTS[i, f].GetPointerSize();
                    if (CNC_REF_OBJECTS[i,f] == null)
                    {
                        CNC_REF_OBJECTS[i,f] = (IMemoryManagement)Activator.CreateInstance(TYPE, new object[] { (Pointer + OFFSET) + ((pointerSize * i *f_Length) + (f * pointerSize)) });
                    }
                    else
                    {
                        CNC_REF_OBJECTS[i,f].UpdatePointer((Pointer + OFFSET) + ((pointerSize * i *f_Length) + (f * pointerSize)));
                    }
                }
            }
        }

        internal void UpdateRefType(IMemoryManagement CNC_REF_OBJECT, int OFFSET, Type TYPE)
        {
            CNC_REF_OBJECT.UpdatePointer(((Pointer + OFFSET)));//+(CNC_REF_OBJECT.GetPointerSize())
        }

        internal void UpdateRefType(int[] CNC_VAL_OBJECTS, int OFFSET)
        {
            for (int i = 0; i < CNC_VAL_OBJECTS.Length; i++)
            {
                CNC_VAL_OBJECTS[i] = *((int*)(Pointer+OFFSET) + ((int)Offst_Int.TotalSize * i));
            }
        }
        internal void UpdateRefType(double[] CNC_VAL_OBJECTS, int OFFSET)
        {
            for (int i = 0; i < CNC_VAL_OBJECTS.Length; i++)
            {
                CNC_VAL_OBJECTS[i] = *((double*)(Pointer+OFFSET) + ((int)Offst_Double.TotalSize * i));
            }
        }
        internal void UpdateRefType(int[,] CNC_VAL_OBJECTS, int OFFSET)
        {
            int i_Length = CNC_VAL_OBJECTS.GetLength(0);
            int f_Length = CNC_VAL_OBJECTS.GetLength(1);
            for (int i = 0; i < i_Length; i++)
            {
                for (int f = 0; f < f_Length; f++)
                {
                    CNC_VAL_OBJECTS[i,f] = *((int*)(Pointer+OFFSET) + (((int)Offst_Int.TotalSize * i *f_Length) + ( f * (int)Offst_Int.TotalSize)));
                }
            }
        }

        internal void SetRefType(int[] CNC_VAL_OBJECTS,int[] VALUE, int OFFSET)
        {
            for(int i =0;i<CNC_VAL_OBJECTS.Length;i++)
            {
                if(VALUE.Length < i)
                {
                    *((int*)(Pointer + OFFSET) + ((int)Offst_Int.TotalSize * i)) = VALUE[i];
                    CNC_VAL_OBJECTS[i] = VALUE[i];
                }
            }
        }
        internal void SetRefType(double[] CNC_VAL_OBJECTS, double[] VALUE, int OFFSET)
        {
            for (int i = 0; i < CNC_VAL_OBJECTS.Length; i++)
            {
                if (VALUE.Length < i)
                {
                    *((double*)(Pointer + OFFSET) + ((int)Offst_Double.TotalSize * i)) = VALUE[i];
                    CNC_VAL_OBJECTS[i] = VALUE[i];
                }

            }
        }

        internal uint Get_unsigned_int_Value(int OFFSET)
        {
            return *(uint*)(Pointer + OFFSET);
        }
        internal void Set_unsigned_int_Value(int OFFSET, uint VALUE)
        {
            *(uint*)(Pointer + OFFSET) = VALUE;
        }

        internal byte Get_unsigned_char_Value(int OFFSET)
        {
            return *(byte*)(Pointer + OFFSET);
        }
        internal void Set_unsigned_char_Value(int OFFSET, byte VALUE)
        {
            
            *((byte*)(Pointer + OFFSET)) = VALUE;
        }

        internal long Get_time_t_Value(int OFFSET)
        {
            return *(long*)(Pointer + OFFSET);
        }
        internal void Set_time_t_Value(int OFFSET, long VALUE)
        {
            *(long*)(Pointer + OFFSET) = VALUE;
        }

        internal float Get_float_Value(int OFFSET)
        {
            return *(float*)(Pointer + OFFSET);
        }
        internal void Set_float_Value(int OFFSET, float VALUE)
        {
            *(float*)(Pointer + OFFSET) = VALUE;
        }

        internal bool Get_bool_Value(int OFFSET)
        {
            return *(bool*)(Pointer + OFFSET);
        }
        internal void Set_bool_Value(int OFFSET, bool VALUE)
        {
            *(bool*)(Pointer + OFFSET) = VALUE;
        }

        internal double Get_double_Value(int OFFSET)
        {
            return *(double*)(Pointer + OFFSET);
        }
        internal void Set_double_Value(int OFFSET, double VALUE)
        {
            *(double*)(Pointer + OFFSET) = VALUE;
        }

        internal long Get_long_long_Value(int OFFSET)
        {
            return *(long*)(Pointer + OFFSET);
        }
        internal void Set_long_long_Value(int OFFSET, long VALUE)
        {
            *(long*)(Pointer + OFFSET) = VALUE;
        }

        internal ushort Get_unsigned_short_Value(int OFFSET)
        {
            return *(ushort*)(Pointer + OFFSET);
        }
        internal void Set_unsigned_short_Value(int OFFSET, ushort VALUE)
        {
            *(ushort*)(Pointer + OFFSET) = VALUE;
        }

        internal int Get_int_Value(int OFFSET)
        {
            return *(int*)(Pointer + OFFSET);
        }
        internal void Set_int_Value(int OFFSET, int VALUE)
        {
            *(int*)(Pointer + OFFSET) = VALUE;
        }

        internal sbyte Get_char_Value(int OFFSET)
        {
            return *(sbyte*)(Pointer + OFFSET);
        }
        internal void Set_char_Value(int OFFSET, sbyte VALUE)
        {
            *(sbyte*)(Pointer + OFFSET) = VALUE;
        }

        internal char Get_wchar_t_Value(int OFFSET)
        {
            return *(char*)(Pointer + OFFSET);
        }
        internal void Set_wchar_t_Value(int OFFSET, char VALUE)
        {
            *(char*)(Pointer + OFFSET) = VALUE;
        }


        internal T Get_Enum_Value<T>(int OFFSET)
        {
            object temp = *(int*)(Pointer + OFFSET);
            //Convert.ChangeType(o, typeof(T));
            return (T)temp;
        }


        internal void Set_Enum_Value<T>(int OFFSET, T VALUE)
        {
            object temp = VALUE;
            *(int*)(Pointer + OFFSET) = (int)temp;
        }


        internal T[] Get_Enum_Value<T>(T[] CNC_VAL_OBJECTS, int OFFSET, int ITEM_SIZE)
        {
            
            for(int i =0; i< CNC_VAL_OBJECTS.Length;i++)
            {
                object temp = *(int*)((Pointer + OFFSET) + (ITEM_SIZE* i));
                CNC_VAL_OBJECTS[i] = (T)temp;
            }
            //Convert.ChangeType(o, typeof(T));
            return CNC_VAL_OBJECTS;
        }
        internal void Set_Enum_Value<T>(T[] CNC_VAL_OBJECTS,T[] VALUE, int OFFSET, int ITEM_SIZE)
        {
            if (CNC_VAL_OBJECTS.Length >= VALUE.Length)
            {
                for (int i = 0; i < CNC_VAL_OBJECTS.Length; i++)
                {
                    if (VALUE.Length < i)
                    {
                        object temp = VALUE[i];
                        *(int*)((Pointer + OFFSET) + (ITEM_SIZE * i)) = (int)temp;
                        
                    }
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Max index:"+CNC_VAL_OBJECTS.Length);
            }
            //Convert.ChangeType(o, typeof(T));
            
        }

    }
}
