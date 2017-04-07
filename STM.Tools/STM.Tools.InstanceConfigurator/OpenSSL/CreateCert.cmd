
set OPENSSL_CONF=%cd%\openssl.cnf
set RANDFILE=.rnd
OpenSSL.exe pkcs12 -export -in "%1" -inkey "%2" -certfile "%3" -password "%4" -name "%5" -out "%6"