## UDS Commands (NAC/RCC_CN/RCC)

| Command | Description |
|--|--|
| 3E00 | Keep-Alive session |
| 190209 | List of current faults |
| 14FFFFFF | Clear faults |
| 37 | Flash autocontrol (Unit must be unlocked first) |
| 1001 | End of communication |
| 1002 | Open Download session |
| 1003 | Open Diagnostic session |
| 1103 | Reboot |
| 2701 | Unlocking service for download (Diagnostic session must be enabled first) - SEED |
| 2703 | Unlocking service for configuration (Diagnostic session must be enabled first) - SEED |
| 2702XXXXXXXX  | Unlocking response for download - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation |
| 2704XXXXXXXX  | Unlocking response for configuration - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation |
| 22XXXX | Read Zone XXXX (2 bytes) |
| 2EXXXXYYYYYYYYYYYY | Write Zone XXXX with data YYYYYYYYYYYY (Unit must be unlocked first) |
| 3101FF0081F05A | Empty flash memory (Unit must be unlocked first) |
| 3103FF00 | Empty flash memory (Unit must be unlocked first) |
| 3481110000 | Prepare flash writing (Unit must be unlocked first) |
| 3101FF04 | Empty ZI Zone (Unit must be unlocked first) |
| 3103FF04 | Empty ZI Zone (Unit must be unlocked first) |
| 3483110000 | Prepare ZI zone writing (Unit must be unlocked first) |

## KWP2000 Commands (SMEG/AIO/MRN)

| Command | Description |
|--|--|
| 3E | Keep-Alive session |
| 17FF00 | List of current faults |
| 14FF00 | Clear faults |
| 1081 | End of communication |
| 10C0 | Open Diagnostic session |
| 31A800 | Reboot |
| 31A801 | Reboot 2 |
| 2781 | Unlocking service for download (Diagnostic session must be enabled first) - SEED |
| 2783 | Unlocking service for configuration (Diagnostic session must be enabled first) - SEED |
| 2782XXXXXXXX  | Unlocking response for download - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation |
| 2784XXXXXXXX  | Unlocking response for configuration - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation |
| 21XX | Read Zone XX (1 byte) |
| 3BXXYYYYYYYYYYYY | Write Zone XX with data YYYYYYYYYYYY (Unit must be unlocked first) |
| 318181F05A | Empty flash memory (Unit must be unlocked first) |
| 318101 | Empty flash memory (Unit must be unlocked first) |
| 348100000D07 | Prepare flash writing (Unit must be unlocked first) |

## Test Commands (NAC/RCC_CN/RCC)
| Command | Description |
|--|--|
| 22D4XX | Measures |
| 22D405 | Visible satellites |
| 3101DF07 | Tactile Test Screen |
| 2FD6000300 | Black screen |
| 2FD60000 | Restore the screen |
| 2FD66000 | Stop Camera display control |
| 2FD66003 | Camera display control |
| 2FD6700330 | 180° Camera display control - Standard view |
| 2FD6700340 | 180° Camera display control - Zoom view |
| 2FD6700350 | 180° Camera display control - Lateral view |
| 2FD62000 | Stop Sound Test |
| 082FD620030108 | Sound test Front Right |
| 082FD62003010A | Sound test Front Right |
| 082FD620030208 | Sound test Front Left |
| 082FD62003020A | Sound test Front Left |
| 082FD620030308 | Sound test Back Right |
| 082FD62003030A | Sound test Back Right |
| 082FD620030308 | Sound test Back Left |
| 082FD62003030A | Sound test Back Left |


## Test Commands (SMEG/AIO/MRN)
| Command | Description |
|--|--|
| 308300 | Black screen |
| 308301 | Black screen Keep-Alive |
| 308302 | Stop Black screen |
| 308311 | Abort Black screen |
| 308200 | Blank screen |
| 308301 | Blank screen Keep-Alive |
| 308202 | Stop Blank screen |
| 308211 | Abort Blank screen |
| 308700 | Camera display |
| 308701 | Camera display Keep-Alive |
| 308702 | Stop Camera display |
| 308711 | Abort Camera display |
| 30850001006419 | Sound testing Front Right |
| 30850002006419 | Sound testing Front Left |
| 30850003006419 | Sound testing Rear Right |
| 30850004006419 | Sound testing Rear Left |
| 308501 | Sound testing Keep-Alive |
| 308502 | Stop Sound testing |
| 308511 | Abort Sound testing |

## UDS Answers (NAC/RCC_CN/RCC)

| Answer | Description |
|--|--|
| 7E00 | Keep-Alive reply |
| 54 | Faults cleared |
| 7101FF0001 | Flash erased successfully |
| 7103FF0002 | Flash erased successfully |
| 7101FF0401 | ZI erased successfully |
| 7103FF0402 | ZI erased successfully |
| 77 | Flash autocontrol OK |
| 5001XXXXXXXX | Communication closed |
| 5002XXXXXXXX | Download session opened |
| 5003XXXXXXXX | Diagnostic session opened |
| 5103 | Reboot OK |
| 62XXXXYYYYYYYYYYYY  | Successfull read of Zone XXXX - YYYYYYYYYYYY = DATA |
| 6701XXXXXXXX | Seed generated for download - XXXXXXXX = SEED |
| 6703XXXXXXXX | Seed generated for configuration - XXXXXXXX = SEED |
| 6702 | Unlocked successfully for download - Unit will be locked again if no command is issued within 5 seconds |
| 6704 | Unlocked successfully for configuration - Unit will be locked again if no command is issued within 5 seconds |
| 6EXXXX | Successfull Configuration Write of Zone XXXX |
| 741000 | Download Writing ready |
| 76XX02 | Download frame XX injected with success |
| 76XX0A | Invalid checksum on download frame XX |
| 7F3478 | Download Writing in progress |
| 7F3778 | Flash autocontrol in progress |
| 7F2724 | Anti-Bruteforce active |
| 7F2713 | Invalid SEED Answer (KEY) |
| 7F2E78 | Configuration Write in progress |
| 7F2E13 | Failed Configuration Write - Invalid Zone data |
| 7F2E7E | Failed Configuration Write - Unit is locked |
| 7F2E31 | Failed Configuration Write - Not allowed operation |
| 7F2EXX | Failed Configuration Write |
| 7F2E31 | Failed Configuration Read - Not allowed operation |
| 7F22XX | Failed Configuration Read |
| 7FXXYY | Error - XX = Service / YY = Error Number |

## KWP2000 Answers (SMEG/AIO/MRN)

| Answer | Description |
|--|--|
| 7E | Keep-Alive reply |
| 5081 | Communication closed |
| 50C0 | Diagnostic session opened |
| 71A801 | Reboot |
| 71A802 | Reboot 2 |
| 61XXYYYYYYYYYYYY  | Successfull read of Zone XX - YYYYYYYYYYYY = DATA |
| 6781XXXXXXXX | Seed generated for download - XXXXXXXX = SEED |
| 6783XXXXXXXX | Seed generated for configuration - XXXXXXXX = SEED |
| 6782 | Unlocked successfully for download - Unit will be locked again if no command is issued within 5 seconds |
| 6784 | Unlocked successfully for configuration - Unit will be locked again if no command is issued within 5 seconds |

## PSA Seed/Key algorithm

[Algorithm can be found here with some example source code](https://github.com/ludwig-v/psa-seedkey-algorithm)

## Zones (NAC/RCC_CN/RCC)

| Zone ID | Description |
|--|--|
| F0FE | ZI Zone (Last 6 characters: current calibration) |
| F080 | ZA Zone |
| F190 | VIN |
| F18C | Serial number |
| 2100 | Telecoding_Fct_AAS |
| 2101 | Telecoding_Fct_AFIL |
| 1234 | HU_CODING_ADDONS |
| 2103 | Telecoding_Fct_ARTIV |
| 2104 | Telecoding_Fct_AUDIO |
| 2106 | Telecoding_Fct_AVR |
| 2107 | Telecoding_Fct_BT |
| 2108 | Telecoding_Fct_BTEL |
| 2109 | Telecoding_Fct_CAFR |
| 210A | Telecoding_Fct_CHANGER_CD |
| 210B | Telecoding_Fct_CHECK |
| 210C | Telecoding_Fct_CITYPARK |
| 210D | Telecoding_Fct_CLIM |
| 210E | Telecoding_Fct_DSG |
| 210F | Telecoding_Fct_ECRAN_PRINCIPALE |
| 2110 | Telecoding_Fct_ECRAN_SECONDAIRE |
| 2112 | Telecoding_Fct_HDC |
| 2113 | Telecoding_Fct_HY |
| 2114 | Telecoding_Fct_INTERNET |
| 2115 | Telecoding_Fct_MPD |
| 2117 | Telecoding_Fct_RADIO |
| 2118 | Telecoding_Fct_RADIO_NUM |
| 2119 | Telecoding_Fct_SAM |
| 211A | Telecoding_Fct_STT |
| 211B | Telecoding_Fct_XVV |
| 211C | Telecoding_Fct_WIFI |
| 211D | Telecoding_Fct_ASR |
| 211E | Telecoding_Fct_ADML |
| 211F | Telecoding_Fct_LANG |
| 2120 | Telecoding_Fct_LKA |
| 2121 | Telecoding_Fct_ACV |
| 2124 | Telecoding_Fct_LUM |
| 2125 | Telecoding_Fct_OBC |
| 2126 | Telecoding_Fct_CPUSH |
| 2128 | Telecoding_Fct_SPY |
| 2127 | Telecoding_Fct_IHM |
| 2116 | Telecoding_Fct_NAV |
| 0100 | Calibration_Fct_AAS |
| 0105 | Calibration_Fct_AVR |
| 0106 | Calibration_Fct_BT |
| 0107 | Calibration_Fct_BTEL |
| 010A | Calibration_Fct_CITYPARK |
| 010C | Calibration_Fct_ENTREE_VIDEO |
| 010D | Calibration_Fct_FAN |
| 010E | Calibration_Fct_FMUX |
| 010F | Calibration_Fct_HY |
| 0110 | Calibration_Fct_INTERNET |
| 0112 | Calibration_Fct_NAV |
| 0115 | Calibration_Fct_STT |
| 0116 | Calibration_Fct_XVV |
| 0117 | Calibration_Fct_LUM |
| 011A | Calibration_Fct_WIFI |
| 0119 | Calibration_Fct_LANG |
| 0118 | Calibration_Fct_OBC |
| 011B | Calibration_Fct_VIDEOTIMING |
| 011E | Calibration_Fct_SVR |
| 2123 | Telecoding_Fct_Alarm_2 |
| 0103 | Calibration_Fct_AUDIO |
| 2129 | Telecoding_Fct_LVDS |
| 2105 | Telecoding_Fct_AVP |
| 212A | Telecoding_Fct_VISIOPARK |
| 0120 | Calibration_Fct_VISIOPARK |
| FFF1 | Calibration_Fct_COLOR_CORRECTION |
| 011D | Calibration_Fct_HDC |
| 011F | Calibration_Fct_LVDS |
| 212C | Telecoding_Fct_ION |
| 212D | Telecoding_Fct_PPS |
| 212E | Telecoding_Fct_IDVR |
| 212F | Telecoding_Fct_AUDIO2 |
| 2130 | Telecoding_Fct_BTA |
| 0104 | Calibration_Fct_AVP |
| 011C | Calibration_Fct_CLIM |
| 0121 | Calibration_Fct_ION |
| 0122 | Calibration_Fct_PPS |
| 2131 | Telecoding_Fct_ANDROID |
| 2132 | Telecoding_Fct_IDVR_HMI |
| 0123 | Calibration_Fct_VIDEOTIMING_2 |
| 2133 | Telecoding_Fct_WAVE3 |
| 0124 | Calibration_Fct_LVDS_EXPORT |
| 0125 | Calibration_Fct_HW_VERSION |
| 0126 | Calibration_Fct_BEIDOU |
| 0127 | Calibration_Fct_DGT |
| 0128 | Calibration_Fct_MASS |
| 0129 | Calibration_Fct_PPS2 |
| 012A | Calibration_Fct_VP1_HW |
| 012B | Calibration_Fct_USB |
| 00DE | Calibration_Fct_PUSH_LUM |
| 00DD | Calibration_Fct_AEE_SEL |
| 2145 | Telecoding_Fct_AIO |

## Zones (SMEG/AIO/MRN)

| Zone ID | Description |
|--|--|
| FE | ZI Zone (Last 6 characters: current calibration) |
| 80 | ZA Zone |
| B0 | VIN |
| 82 | Serial number |
| A0 | ? (SMEG) |
| B1 | ? (AIO) |
| B2 | ? (SMEG, AIO) |
| B3 | Personalization menus (SMEG) |
| B5 | ? (SMEG) |
| B9 | ? (SMEG) |
| C0 | ? (SMEG) |
| D0 | Display details - Part number, Software (SMEG) |
| BC | GPS Measures (SMEG) |

## Diagnostic frames explanation / What the Sketch is doing

CAN-BUS is limited to 8 bytes per frame, to send larger data PSA chose a simple algorythm to truncate the data into multiple parts

### To send data smaller or equal to 7 bytes:
#### [SEND] Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| Full Data Length | Data[0] | Data[1] | Data[2] | Data[3] | Data[4] | Data[5] | Data[6] |

### To send data larger than 7 bytes:
#### [SEND] First Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x10 | Full Data Length | Data[0] | Data[1] | Data[2] | Data[3] | Data[4] | Data[5] |
#### [RECEIVE] Write Acknowledgement Frame:
| Byte 1 | Byte 2 | Byte 3 |
|--|--|--|
| 0x30 | 0x00 | 0x0A |
#### [SEND] Second Frame:
##### ID starting at 0x21 and increasing by 1 for every extra frame needed, after 0x2F reverting back to 0x20
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x21 | Data[6] | Data[7] | Data[8] | Data[9] | Data[10] | Data[11] | Data[12] |
#### [SEND] Third Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x22 | Data[13] | Data[14] | Data[15] | Data[16] | Data[17] | Data[18] | Data[19] |

### To receive data smaller or equal to 7 bytes:
#### [RECEIVE] Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| Full Data Length | Data[0] | Data[1] | Data[2] | Data[3] | Data[4] | Data[5] | Data[6] |

### To receive data larger than 7 bytes:
#### [RECEIVE] First Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x10 | Full Data Length | Data[0] | Data[1] | Data[2] | Data[3] | Data[4] | Data[5] |
#### [SEND] Read Acknowledgement Frame:
| Byte 1 | Byte 2 | Byte 3 |
|--|--|--|
| 0x30 | 0x00 | 0x05 |
#### [SEND] Second Frame:
##### ID starting at 0x21 and increasing by 1 for every extra frame needed, after 0x2F reverting back to 0x20
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x21 | Data[6] | Data[7] | Data[8] | Data[9] | Data[10] | Data[11] | Data[12] |
#### [SEND] Third Frame:
| Byte 1 | Byte 2 | Byte 3 | Byte 4 | Byte 5 | Byte 6 | Byte 7 | Byte 8 |
|--|--|--|--|--|--|--|--|
| 0x22 | Data[13] | Data[14] | Data[15] | Data[16] | Data[17] | Data[18] | Data[19] |

> Received frames could be out-of-order, ID must be used to append parts at the correct position into the final data whose size is known

## Calibration file (.cal) explanation

Every line of calibration files has this form:

### Content Size
| TYPE | LENGTH | ADDRESS | LENGTH2 | ZONE | DATA | CHECKSUM | CHECKSUM2 |
|--|--|--|--|--|--|--|--|
| 1h | 1h | Variable Length | 1h | 2h | Variable Length | 2h| 1h |

1h = 1 HEX Byte or 2 characters in the .cal file

### Content Data
| Line Part | Line Detail |
|--|--|
| **TYPE** | S1 = ZI Zone / S2 / S8 |
| **LENGTH** | Hex Length of ADDRESS+ZONE+DATA+CHECKSUM+CHECKSUM2 |
| **ADDRESS** |  |
| **LENGTH2** | Hex Length of ZONE+DATA+CHECKSUM  |
| **ZONE** |  |
| **DATA** |   |
| **CHECKSUM** | *CRC-16/X-25*(DATA) with this order CRC[1] CRC[0] |
| **CHECKSUM2** | *CRC-8/2s_complement*(ADDRESS+ZONE+DATA+CHECKSUM) - 1 |

## PINOUT

| PIN | Description |
|--|--|
| 3 | CAN-BUS Diagnostic High |
| 8 | CAN-BUS Diagnostic Low |
