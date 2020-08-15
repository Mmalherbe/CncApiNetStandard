using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace OosterhofDesign
{

    public unsafe class CncKeepUiAliveFunction:IDisposable
    {
        public delegate int UiAliveFunction(ref int VALUE);
        private bool IsDisposed = false;
        private GCHandle FunctionGcHandle;
        internal IntPtr Functionpfunc;
        internal IntPtr _Functionparameter;
        public int Functionparameter
        {
            get
            {
                return *(int*)_Functionparameter;
            }
            set
            {
                *(int*)_Functionparameter = value;
            }
        }
        private void NewIntPtr()
        {
            _Functionparameter = Marshal.AllocHGlobal((int)Offst_Int.TotalSize);
           
        }
        
        public CncKeepUiAliveFunction(UiAliveFunction DelegatefunctionCall, int parameter)
        {
            NewIntPtr();
            FunctionGcHandle = GCHandle.Alloc(DelegatefunctionCall);
            Functionpfunc = Marshal.GetFunctionPointerForDelegate(DelegatefunctionCall);
            Functionparameter = parameter;
            
        }
        ~CncKeepUiAliveFunction() { Dispose(); }
        public void Dispose()
        {
            if(IsDisposed == false)
            {
                Marshal.FreeHGlobal(_Functionparameter);
                IsDisposed = true;
                FunctionGcHandle.Free();
            }
        }
    }
}
