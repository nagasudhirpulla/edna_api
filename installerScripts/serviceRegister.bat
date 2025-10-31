rem delete service if exists
call net stop edna_api
call net delete edna_api
rem insall service
call nssm.exe install edna_api "%cd%\EdnaApi.exe"
call nssm.exe set edna_api AppStdout "%programdata%\EdnaApi\edna_api.log"
call nssm.exe set edna_api AppStderr "%programdata%\EdnaApi\edna_api.log"
call nssm.exe set edna_api AppStdoutCreationDisposition 4
call nssm.exe set edna_api AppStderrCreationDisposition 4
call nssm.exe set edna_api AppRotateFiles 1
call nssm.exe set edna_api AppRotateOnline 1
call nssm.exe set edna_api AppRotateBytes 1048576
call net start edna_api