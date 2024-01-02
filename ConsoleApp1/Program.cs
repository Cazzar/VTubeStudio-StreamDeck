using System.IO;
using System.Threading;

/*
This is a test program to allow the streamdeck plugin to be debugged without having to run the streamdeck software.
To use this, replace the binary of the plugin with this program, and then run the streamdeck software.

This program will write the arguments it receives to a file called argv.txt in the same directory as the plugin.
You can then debug the streamdeck-vtubestudio project, and it will read the arguments from the file.
You may need to change the path to the file in streamdeck-vtubestudio/Program.cs.
You also may need to restart the streamdeck software after a few debugging attempts as it has limited restarts.
 */

File.WriteAllLines("argv.txt", args);
while (true)
{
    Thread.Sleep(100_000);
}