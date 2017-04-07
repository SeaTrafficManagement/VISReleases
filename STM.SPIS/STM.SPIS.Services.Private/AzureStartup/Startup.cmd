echo "Startup task running..."  >> "C:\Logs\StartupLog.txt" 2>&1
%windir%\System32\inetsrv\appcmd.exe unlock config /section:system.webServer/security/access >> "C:\Logs\StartupLog.txt" 2>&1
netsh http add urlacl url=http://+:9991/ user=Everyone >> "C:\Logs\StartupLog.txt" 2>&1
certutil -enterprise -addstore "Root" "..\AzureStartup\MaritimeCloud_Test_Root_Certificate.cer" >> "C:\Logs\StartupLog.txt" 2>&1
certutil -enterprise -addstore "Ca" "..\AzureStartup\MaritimeCloud_Test_Identity_Registry.cer" >> "C:\Logs\StartupLog.txt" 2>&1
certutil -enterprise -addstore "Ca" "..\AzureStartup\IALA_Members.cer" >> "C:\Logs\StartupLog.txt" 2>&1
certutil -enterprise -addstore "Ca" "..\AzureStartup\BIMCO_ExtraNet.cer" >> "C:\Logs\StartupLog.txt" 2>&1
certutil -enterprise -addstore "Root" "..\AzureStartup\SmaStmRootCertificate.cer" >> "C:\Logs\StartupLog.txt" 2>&1
schtasks /create /tn "Poll PortCDM queue" /f /sc MINUTE /MO 5 /ru "NT AUTHORITY\NETWORKSERVICE" /tr "%CD%\STM.SPIS.PollingQueueWorker.exe"  >> "C:\Logs\StartupLog.txt" 2>&1
EXIT /B 0