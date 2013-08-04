using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GP = GHI.Premium;

using Micro1140.Cpu;

namespace Micro1140
{
    public partial class Program
    {
        private readonly Font font = Resources.GetFont(Resources.FontResources.NinaB);
        
        private ArrayList lines = new ArrayList();
        private int currentLine = 0;

        private void OnKeyboardConnected(GTM.GHIElectronics.UsbHost host, GP.USBHost.USBH_Keyboard kb)
        {
            Debug.Print("keyboard connected");

            kb.CharUp += OnCharUp;
        }

        private void OnCharUp(GP.USBHost.USBH_Keyboard kb, GP.USBHost.USBH_KeyboardEventArgs e)
        {
            Debug.Print("char up: " + e.KeyAscii);
            
            if (e.KeyAscii == '\n')
            {
                Debug.Print("LF");
                currentLine += 1;
                lines.Add("");
            }
            else if (e.KeyAscii == '\r')
            {
                Debug.Print("suppressing CR");
            }
            else
            {
                string s = (string)lines[currentLine];

                s += e.KeyAscii;
                lines[currentLine] = s;
            }

            display_T35.SimpleGraphics.ClearNoRedraw();

            int lineSpacing = font.Height;

            for (int i = 0; i < lines.Count; i++)
            {
                display_T35.SimpleGraphics.DisplayText((string)lines[i], font, Color.White, 0, (uint)(i * lineSpacing));
            }
        }

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/

            lines.Add("");

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            usbHost.KeyboardConnected += OnKeyboardConnected;
        }
    }
}
