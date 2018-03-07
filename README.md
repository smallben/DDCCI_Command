# DDCCI_Command
Using WPF to implement the application which could sending command to Monitor to get the specific data via MSDN API (High-Level / Low-Level)

# DDCCI Command with WPF

This is a testing application for getting the needed data from Scalar(Monitor).
Example: Brightness, Contrast, Capabilities or etc...
(which is defined on the MSDN API.. [MSDN Monitor Configuration API](https://msdn.microsoft.com/zh-tw/library/windows/desktop/dd692964%28v=vs.85%29.aspx))
**Note**. Before tracing code, we have to prepare the basic knowledge of the difference between of High-level / Low-level configuration API.
[High-Level API](https://msdn.microsoft.com/zh-tw/library/windows/desktop/dd692981%28v=vs.85%29.aspx)
[Low-Level API](https://msdn.microsoft.com/zh-tw/library/windows/desktop/dd692982%28v=vs.85%29.aspx)

# Goal

Get the data what I needed.
Example: Get Brightness value from monitor.
Method 1: Using High-level function

    GetMonitorBrightness()

Method 2: Using Low-level function

    GetVCPFeatureAndVCPFeatureReply()

The above method is working on any monitor which turn on DDCCI and follow the specification of VESA standard.

# Environment
OS: Win 7
Platform: Notebook
Monitor: VGA / HDMI monitor which turn on the DDCCI and implement the VESA standard function in Scalar Chip.
Compiler: Visual Studio 2017
Code Language: C# - WPF

# Operation

 1. Turn on *.sln file to open the project
 2. Run

# Information
If you have any question, please leave the message to me. Or send me the E-mail: ben.tseng@coretronic.com
