using OpenBveApi.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DetailManager {
    internal class Win32DetailManagerPInvoke {

        public const string MODULE_NAME = "RockOnDetailManager.dll";

        public static int[] panel = new int[256];
        public static int[] sound = new int[256];

        public const int ATS_VERSION = 0x00020000;
        public const int ATS_KEY_S = 0; // S Key
        public const int ATS_KEY_A1 = 1; // A1 Key
        public const int ATS_KEY_A2 = 2; // A2 Key
        public const int ATS_KEY_B1 = 3; // B1 Key
        public const int ATS_KEY_B2 = 4; // B2 Key
        public const int ATS_KEY_C1 = 5; // C1 Key
        public const int ATS_KEY_C2 = 6; // C2 Key
        public const int ATS_KEY_D = 7; // D Key
        public const int ATS_KEY_E = 8; // R Key
        public const int ATS_KEY_F = 9; // F Key
        public const int ATS_KEY_G = 10; // G Key
        public const int ATS_KEY_H = 11; // H Key
        public const int ATS_KEY_I = 12; // I Key
        public const int ATS_KEY_J = 13; // J Key
        public const int ATS_KEY_K = 14; // K Key
        public const int ATS_KEY_L = 15; // L Key
        public const int ATS_INIT_REMOVED = 2; // Handle Removed
        public const int ATS_INIT_EMG = 1; // Emergency Brake
        public const int ATS_INIT_SVC = 0; // Service Brake
        public const int ATS_SOUND_STOP = -10000; // Stop
        public const int ATS_SOUND_PLAY = 1; // Play Once
        public const int ATS_SOUND_PLAYLOOPING = 0; // Play Repeatedly
        public const int ATS_SOUND_CONTINUE = 2; // Continue
        public const int ATS_HORN_PRIMARY = 0; // Horn 1
        public const int ATS_HORN_SECONDARY = 1; // Horn 2
        public const int ATS_HORN_MUSIC = 2; // Music Horn
        public const int ATS_CONSTANTSPEED_CONTINUE = 0; // Continue
        public const int ATS_CONSTANTSPEED_ENABLE = 1; // Enable
        public const int ATS_CONSTANTSPEED_DISABLE = 2; // Disable

        public struct ATS_VEHICLESPEC {
            public int BrakeNotches; // Number of Brake Notches
            public int PowerNotches; // Number of Power Notches
            public int AtsNotch; // ATS Cancel Notch
            public int B67Notch; // 70% of Brake Force
            public int Cars; // Number of Cars

            public ATS_VEHICLESPEC(VehicleSpecs specs) : this(specs.BrakeNotches, specs.PowerNotches, specs.AtsNotch, specs.B67Notch, specs.Cars) { }

            public ATS_VEHICLESPEC(int brakeNotches, int powerNotches, int atsNotch, int b67Notch, int cars) {
                BrakeNotches = brakeNotches;
                PowerNotches = powerNotches;
                AtsNotch = atsNotch;
                B67Notch = b67Notch;
                Cars = cars;
            }
        }

        // State Quantity of Vehicle
        public struct ATS_VEHICLESTATE {
            public double Location; // Train Position (Z-axis) (m)
            public float Speed; // Train Speed (km/h)
            public int Time; // Time (ms)
            public float BcPressure; // Pressure of Brake Cylinder (Pa)
            public float MrPressure; // Pressure of MR (Pa)
            public float ErPressure; // Pressure of ER (Pa)
            public float BpPressure; // Pressure of BP (Pa)
            public float SapPressure; // Pressure of SAP (Pa)
            public float Current; // Current (A)

            public ATS_VEHICLESTATE(ElapseData data) : this(data.Vehicle.Location,
                    (float)data.Vehicle.Speed.KilometersPerHour, (int)data.TotalTime.Milliseconds,
                    (float)data.Vehicle.BcPressure, (float)data.Vehicle.MrPressure, (float)data.Vehicle.ErPressure,
                    (float)data.Vehicle.BpPressure, (float)data.Vehicle.SapPressure, 0.0f) { }

            public ATS_VEHICLESTATE(double location, float kilometersPerHour, int milliseconds,
                float bcPressure, float mrPressure, float erPressure, float bpPressure, float sapPressure, float current) : this() {
                Location = location;
                Speed = kilometersPerHour;
                Time = milliseconds;
                BcPressure = bcPressure;
                MrPressure = mrPressure;
                ErPressure = erPressure;
                BpPressure = bpPressure;
                SapPressure = sapPressure;
                Current = current;
            }
        }

        // Received Data from Beacon
        public struct ATS_BEACONDATA {
            public int Type; // Type of Beacon
            public int Signal; // Signal of Connected Section
            public float Distance; // Distance to Connected Section (m)
            public int Optional; // Optional Data

            public ATS_BEACONDATA(BeaconData data) : this(data.Type, data.Signal.Aspect, (float)data.Signal.Distance, data.Optional) { }

            public ATS_BEACONDATA(int type, int aspect, float distance, int optional) : this() {
                Type = type;
                Signal = aspect;
                Distance = distance;
                Optional = optional;
            }
        }

        // Train Operation Instruction
        public struct ATS_HANDLES {
            public int Brake; // Brake Notch
            public int Power; // Power Notch
            public int Reverser; // Reverser Position
            public int ConstantSpeed; // Constant Speed Control

            public ATS_HANDLES(Handles handles) : this(handles.BrakeNotch, handles.PowerNotch, handles.Reverser,
                     handles.ConstSpeed ? ATS_CONSTANTSPEED_ENABLE : ATS_CONSTANTSPEED_DISABLE) { }

            public ATS_HANDLES(int brakeNotch, int powerNotch, int reverser, int constSpeed) : this() {
                Brake = brakeNotch;
                Power = powerNotch;
                Reverser = reverser;
                ConstantSpeed = constSpeed;
            }

            public override string ToString() {
                return string.Format("R{2} P{1} B{0}   C/SPD{3}]", Brake, Power, Reverser, ConstantSpeed);
            }

        }

        // Called when this plug-in is loaded
        [DllImport(MODULE_NAME)]
        public static extern void Load();

        // Called when this plug-in is unloaded
        [DllImport(MODULE_NAME)]
        public static extern void Dispose();

        // Returns the version numbers of ATS plug-in
        [DllImport(MODULE_NAME)]
        public static extern int GetPluginVersion();

        // Called when the train is loaded
        [DllImport(MODULE_NAME)]
        public static extern void SetVehicleSpec(ATS_VEHICLESPEC specs);

        // Called when the game is started
        [DllImport(MODULE_NAME)]
        public static extern void Initialize(int mode);

        // Called every frame
        [DllImport(MODULE_NAME)]
        public static extern ATS_HANDLES Elapse(ATS_VEHICLESTATE vehicleState, ref int[] panel, ref int[] sound);

        // Called when the power is changed
        [DllImport(MODULE_NAME)]
        public static extern void SetPower(int powerNotch);

        // Called when the brake is changed
        [DllImport(MODULE_NAME)]
        public static extern void SetBrake(int brakeNotch);

        // Called when the reverser is changed
        [DllImport(MODULE_NAME)]
        public static extern void SetReverser(int reverser);

        // Called when any ATS key is pressed
        [DllImport(MODULE_NAME)]
        public static extern void KeyDown(int key);

        // Called when any ATS key is released
        [DllImport(MODULE_NAME)]
        public static extern void KeyUp(int key);

        // Called when the horn is used
        [DllImport(MODULE_NAME)]
        public static extern void HornBlow(int type);

        // Called when the door is opened
        [DllImport(MODULE_NAME)]
        public static extern void DoorOpen();

        // Called when the door is closed
        [DllImport(MODULE_NAME)]
        public static extern void DoorClose();

        // Called when current signal is changed
        [DllImport(MODULE_NAME)]
        public static extern void SetSignal(int NamelessParameter);

        // Called when the beacon data is received
        [DllImport(MODULE_NAME)]
        public static extern void SetBeaconData(ATS_BEACONDATA NamelessParameter);
    }
}
