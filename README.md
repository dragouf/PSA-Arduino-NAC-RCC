# Arduino PSA Diag box

PSA NAC / RCC Diagbox Configuration software

fork of https://github.com/ludwig-v/arduino-psa-nac_rcc_cirocco 



## What's in this version

- This is the offline version (include seed/key algorithm). 
- You have an offline editor (to edit configuration at home before to send it to the car)
- Zone description instead of system name on main board (french only)
- .Net core 


You have the source code as well if you want to improve it and better understand how it is working.

Tuto in french: https://www.forum-peugeot.com/Forum/threads/tuto-t%C3%A9l%C3%A9codage-et-calibration-dun-nac-rcc-cirocco-sans-diagbox-via-arduino.121767/


Thanks to Vlud and Bagou91. 

## Sources

After some discussion with VLud he wants all the source of every files to be mention. So here is the list. The purpose of the repository is to unify their work.

ecu.md > Source: https://github.com/ludwig-v/psa-seedkey-algorithm/blob/main/ECU_KEYS.md  
protocol.md > Source: https://github.com/ludwig-v/arduino-psa-diag/blob/master/README.md  
arduino-psa-diag.ino > Source: https://github.com/ludwig-v/arduino-psa-diag/blob/master/arduino-psa-diag/arduino-psa-diag.ino   
arduino-psa-diag/SeedKeyGenerator.cs > Source: https://github.com/ludwig-v/psa-seedkey-algorithm/blob/main/algorithmSimplified.cs  
