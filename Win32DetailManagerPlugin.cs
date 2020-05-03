
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using OpenBveApi.Interface;
using OpenBveApi.Runtime;

namespace DetailManager {
    /// <summary>Represents a legacy Win32 plugin.</summary>
    internal class Win32DetailManagerPlugin : IRuntime {

        public void DoorChange(DoorStates oldState, DoorStates newState) {
            if (newState == DoorStates.None) {
                Win32DetailManagerPInvoke.DoorClose();
            } else if (oldState== DoorStates.None) {
                Win32DetailManagerPInvoke.DoorOpen();
            }
        }

        public void Elapse(ElapseData data) {
            var win32Handles = Win32DetailManagerPInvoke.Elapse(new Win32DetailManagerPInvoke.ATS_VEHICLESTATE(data), 
                ref Win32DetailManagerPInvoke.panel, ref Win32DetailManagerPInvoke.sound);
            data.Handles.BrakeNotch = win32Handles.Brake;
            data.Handles.PowerNotch = win32Handles.Power;
            data.Handles.Reverser = win32Handles.Reverser;
            if (win32Handles.ConstantSpeed == Win32DetailManagerPInvoke.ATS_CONSTANTSPEED_ENABLE) {
                data.Handles.ConstSpeed = true;
            } else if (win32Handles.ConstantSpeed == Win32DetailManagerPInvoke.ATS_CONSTANTSPEED_DISABLE) {
                data.Handles.ConstSpeed = false;
            }
        }

        public void HornBlow(HornTypes type) {
            Win32DetailManagerPInvoke.HornBlow((int)type-1);
        }

        public void Initialize(InitializationModes mode) {
            Win32DetailManagerPInvoke.Initialize((int)mode+1);
        }

        public void KeyDown(VirtualKeys key) {
            Win32DetailManagerPInvoke.KeyDown((int)key);
        }

        public void KeyUp(VirtualKeys key) {
            Win32DetailManagerPInvoke.KeyUp((int)key);
        }

        public bool Load(LoadProperties properties) {
            Win32DetailManagerPInvoke.Load();
            return true;
        }

        public void PerformAI(AIData data) {
            
        }

        public void SetBeacon(BeaconData data) {
            Win32DetailManagerPInvoke.SetBeaconData(new Win32DetailManagerPInvoke.ATS_BEACONDATA(data));
        }

        public void SetBrake(int brakeNotch) {
            Win32DetailManagerPInvoke.SetBrake(brakeNotch);
        }

        public void SetPower(int powerNotch) {
            Win32DetailManagerPInvoke.SetPower(powerNotch);
        }

        public void SetReverser(int reverser) {
            Win32DetailManagerPInvoke.SetReverser(reverser);
        }

        public void SetSignal(SignalData[] data) {
            Win32DetailManagerPInvoke.SetSignal(data[0].Aspect);
        }

        public void SetVehicleSpecs(VehicleSpecs specs) {
            Win32DetailManagerPInvoke.SetVehicleSpec(new Win32DetailManagerPInvoke.ATS_VEHICLESPEC(specs));
        }

        public void Unload() {
            Win32DetailManagerPInvoke.Dispose();
        }
    }
}