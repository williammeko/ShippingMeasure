[System Requirements]

Microsoft .NET Framework 3.5 Service pack 1
	• Supported Operating System
		Windows Server 2003, Windows Server 2008, Windows Vista, Windows XP 
	• Processor: 400 MHz Pentium processor or equivalent (Minimum); 1GHz Pentium processor or equivalent (Recommended)
	• RAM: 96 MB (Minimum); 256 MB (Recommended)
	• Hard Disk: Up to 500 MB of available space may be required
	• CD or DVD Drive:  Not required
	• Display: 800 x 600, 256 colors (Minimum); 1024 x 768 high color, 32-bit (Recommended)

Microsoft Office 2000
	• Word 2000
	• Excel 2000

Printer
	• Microsoft XPS Document Writer

Screen resolution
	• Recommended: 1280 X 720, or higher


[Event Log]

To enable the app to write Event Log, please follow the steps below. (Only follow the steps once by each computer)
Steps to enable event log
	1. Run \Deploy.ps1, with an elevated (run as Administrator) PowerShell console.
		In order to prepare Windows Event Log to let the app write log to.
		After running \Deploy.ps1, the app can write log to Windows Event Log, User also can view events of the app with Event Viewer.
		(If execution security issue encountered, please try the command first: Set-ExecutionPolicy -ExecutionPolicy Unrestricted)
	2. Make sure the configuration is enabled.
		There is a configuration node in App.config file, which named "<system.diagnostics>", by default is commented. You can un-comment the node to enable the app to write event log.


[Components]

These components are optional to install, only while ShippingMeasure.exe cannot be run normally
	• Microsoft .NET Framework 3.5 Service pack 1 (Full Package)
		dotnetfx35.exe
	• Microsoft Access Database Engine 2010 Redistributable
		AccessDatabaseEngine.exe (for 64-bit Office: AccessDatabaseEngine_x64.exe)


[Build]

In order to allow user to run the app on their platform normallly, please create builds for both x86 and x64 platforms.
Steps to create a build (for example: x86)
	1. Open ShippingMeasure.sln with Visual Studio 2015.
	2. In Solution Explorer, right-click project ShippingMeasure, select Properties.
	3. In Properties page, select "Build" tab.
	4. Select "x86" from dropdown "Platform target".
	5. Press [Ctrl]-[Shift]-[S] to save settings.
	6. Right-click project ShippingMeasure from Solution Explorer, select "Build".
	7. Wait for message "Build Succeeded".
	8. Open the folder of project ShippingMeasure, copy the sub-folder "bin\Debug" (or "bin\Release", determined by what you selected to build), to where you will copy to user.
	9. Rename the target folder from "Debug" (or "Release"), to "ShippingMeasure_x86"


[Steps to create a build to a customer]

As each customer would hold one build of the app, which is different from other customers'. We should do some modification and create one build for each customer.
Steps as below
	- Prerequisite
		Cust1Db (new copy of Data.mdb)
		Cust1Branch (new copy of Main branch)
		Cust1Drop (built with Cust1Branch)
	- Make a copy of Data.mdb.
		Here we call the new Data.mdb: Cust1Db
	- Run ShippingMeasure.DataConverting tool, import tank data to Cust1Db.
	- Make a copy of the Main branch. One branch means one folder copy of source code files.
		Here we call the new branch: Cust1Branch
	- Open Cust1Branch with Visual Studio 2015, and do some modifications on it.
		- Modify App.config
			Language: for default to English set as "en-us", for default to Chinese set as "zh-cn"
			DataConnectionString: if path of Data.mdb changed
	- Build project ShippingMeasure, copy the bin\Debug (or bin\Release) folder to a new folder named Cust1Drop
	- Copy Cust1Db to Cust1Drop folder.
	- Run ShippingMeasure.exe in Cust1Drop to test.
	- If anything is OK, copy/burn Cust1Drop to a disk to deliver to customer.

