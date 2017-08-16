echo "Startup task running..." >> "C:\Logs\StartupLog.txt" 2>&1
%windir%\System32\inetsrv\appcmd.exe unlock config /section:system.webServer/security/access >> "C:\Logs\StartupLog.txt" 2>&1
%windir%\System32\inetsrv\appcmd.exe unlock config /section:system.webServer/serverRuntime >> "C:\Logs\StartupLog.txt" 2>&1

if "%Environment%" == "DEV" (
	certutil -enterprise -addstore "Root" "..\AzureStartup\staging\mc-root-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-idreg-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-iala-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-bimco-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Root" "..\AzureStartup\staging\SmaStmRootCertificate.cer" >> "C:\Logs\StartupLog.txt" 2>&1
)

if "%Environment%" == "TEST" (
	echo "TEST" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Root" "..\AzureStartup\staging\mc-root-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-idreg-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-iala-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\staging\mc-bimco-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Root" "..\AzureStartup\staging\SmaStmRootCertificate.cer" >> "C:\Logs\StartupLog.txt" 2>&1
)

if "%Environment%" == "PROD" (
	echo "PROD" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Root" "..\AzureStartup\Production\mc-root-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\Production\mc-idreg-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\Production\mc-iala-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
	certutil -enterprise -addstore "Ca" "..\AzureStartup\Production\mc-bimco-cert.cer" >> "C:\Logs\StartupLog.txt" 2>&1
)

EXIT /B 0