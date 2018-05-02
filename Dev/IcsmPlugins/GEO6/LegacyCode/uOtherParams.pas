unit uOtherParams;

interface

const
    osUnknown  = 0; 
    osAL    = 1; //** Aeronautical mobile (OR) system AL 26 10 000 A.4.1-2
    osCA    = 2; //** Fixed system CA 15 10 A.4.1-5
    osDA    = 3; //** Aeronautical mobile (OR) system DA 26 10 000 A.4.1-11
    osDB    = 4; //** Aeronautical mobile (OR) system DB 26 10 000 A.4.1-12
    osIA    = 5; //** Fixed system IA 48 10 A.4.1-6
    osMA    = 6; // Land mobile system MA 4 10 A.4.1-3
    osMT    = 7; // Mobile and fixed systems MT (transportable) 20 10 A.4.1-4
    osMU    = 8; //** Mobile system MU (low power) 54 10 A.4.1-7
    osM1    = 9; // Mobile system M1 (narrow-band FM, 12.5 kHz) interfered with by a single T-DAB block(1)(private mobile radio) 15 10 A.4.1-5
    osM2    = 10;//** Mobile system M2 (narrow-band), interfered with by two or more T-DAB blocks 36 10 A.4.1-5
    osRA1   = 11; //** Mobile system RA1 (narrow-band FM, 12.5 kHz) interfered with by a single T-DAB block(1) 15.0 1.5 A.4.1-5
    osRA2   = 12; //** Mobile system RA2 (narrow-band FM, 12.5 kHz) interfered with by a single T-DAB block(1) 7.0 20.0 A.4.1-5
    osR1    = 13; //** Land mobile system R1 (medical telemetry) 32.0 10.0 A.4.1-8
    osR3    = 14; //** Mobile system R3 (remote control) 30.0 10.0 A.4.1-7
    osR4    = 15; //** Mobile system R4 (remote control) 30.0 10.0 A.4.1-7
    osXA    = 16; //** Land mobile system XA (private mobile radio) 15.0 10.0 A.4.1-5
    osXB    = 17; //** Fixed system XB (alarm system) 37.0 10.0 A.4.1-9
    osXE    = 18; //** Aeronautical mobile (OR) system XE 0.0 0.0 A.4.1-10
    osXM    = 19; //** Land mobile system XM (radio microphones,VHF) 48.0 10.0 A.4.1-6

    osAA8   = 20; //BL8 Aeronautical radionavigation system BL8 (RSBN, 0.7 or 0.8 MHz) 42.0 10 000.0 A.4.2-24
    //osAA8 = ; // BN8 Aeronautical radionavigation system BN8 (RSBN, 3 MHz) 42.0 10.0 A.4.2-24
    //osAA8 = ; // BY8 Aeronautical radionavigation system BY8 (RSBN, 0.7 MHz) 42.0 10.0 A.4.2-24
    //osAA8 = ; // BX8 Aeronautical radionavigation system BX8 (RSBN, 3 MHz) 42.0 10 000.0 A.4.2-24
    osAB    = 21; // AB8N Aeronautical radionavigation system AB8N (RLS 1 Type 1, 6 MHz) 13.0 10.0 A.4.2-16
    //osAB  = ; // AB8C Aeronautical radionavigation system AB8C (RLS 1 Type 1, 6 MHz) 13.0 10.0 A.4.2-17
    //osAB  = ; // AC8N Aeronautical radionavigation system AC8N (RLS 1 Type 2, 3 MHz) 13.0 10.0 A.4.2-18
    //osAB  = ; // AC8C Aeronautical radionavigation system AC8C (RLS 1 Type 2, 3 MHz) 13.0 10.0 A.4.2-19
    osBA    = 22; // BA8N Aeronautical radionavigation system BA8N (RLS 2 Type 1) 29.0 10.0 A.4.2-20
    //osBA  = ; // BA8C Aeronautical radionavigation system BA8C (RLS 2 Type 1) 29.0 10.0 A.4.2-21

    osAA2   = 23; // BB8N Aeronautical radionavigation system BB8N (RLS 2 Type 2, airborne transmission, 8 MHz) 24.0 10.0 A.4.2-22
    //osAA2 = ; // BB8C Aeronautical radionavigation system BB8C (RLS 2 Type 2, airborne transmission, 8 MHz) 24.0 10.0 A.4.2-23
    osBC    = 24; // BC8N Aeronautical radionavigation system BC8N (RLS 2 Type 2, ground transmission, 3 MHz) 73.0 10 000.0 A.4.2-18
    //osBC  = ; // BC8C Aeronautical radionavigation system BC8C (RLS 2 Type 2, ground transmission, 3 MHz) 73.0 10 000.0 A.4.2-19
    osBD    = 25; // BD8N Aeronautical radionavigation system BD8N (RLS 2 Type 1, ground transmission, 4 MHz) 52.0 10 000.0 A.4.2-20
    //osBD  = ; // BD8C Aeronautical radionavigation system BD8C (RLS 2 Type 1, ground transmission, 4 MHz) 52.0 10 000.0 A.4.2-21
    osFF    = 26; // FF7 Fixed system FF7 (transportable, 7 MHz) 35.0 10.0 A.4.2-2
    //osFF  = ; // FF8 Fixed system FF8 (transportable, 8 MHz) 35.0 10.0 A.4.2-3
    osFH    = 27; // FH8 Fixed system FH8 (P-MP) 18.0 10.0 A.4.2-4

    osFK    = 43; //

    osFK7   = 28; // FK7N Generic fixed non-critical mask – 10.0 (See Note)
    //osFK7 = ; // FK7C Generic fixed sensitive mask – 10.0 (See Note)
    osFK8   = 29; // FK8N Generic fixed non-critical mask – 10.0 (See Note)
    //osFK8 = ; // FK8C Generic fixed sensitive mask – 10.0 (See Note)
    osNX    = 30; //** NX8 Land mobile system NX8 27.0 20.0 A.4.2-7
    osNR    = 31; //** NR7 Land mobile system NR7 (radio microphone, 7 MHz) 68.0 1.5 A.4.2-8
    //osNR  = ; //** NR8 Land mobile system NR8 (radio microphone, 8 MHz) 68.0 1.5 A.4.2-9
    osNS    = 32; //** NS7 Mobile system NS7 (OB link, stereo, non-companded) 86.0 10.0 A.4.2-10
    //osNS  = ; //** NS8 Mobile system NS8 (OB link, stereo, non-companded) 86.0 10.0 A.4.2-11

    osNT = 33; //** NT7 Mobile system NT7 (talkback, non-companded) 31.0 1.5 A.4.2-12
    //osNT = ; //** NT8 Mobile system NT8 (talkback, non-companded) 31.0 1.5 A.4.2-13
    osNA = 34; // NA8N Digital land mobile system NA8N (non-critical) 13.0 20.0 A.4.2-14
    //osNA = ; // NA8C Digital land mobile system NA8C (sensitive) 13.0 20.0 A.4.2-15
    osNB = 35; // NB7N Generic mobile non-critical mask – 10.0 (See Note)
    //osNB = ; // NB7C Generic mobile sensitive mask – 10.0 (See Note)
    //osNB = ; // NB8N Generic mobile non-critical mask – 10.0 (See Note)
    //osNB = ; // NB8C Generic mobile sensitive mask – 10.0 (See Note)

    osNB8 = 44;  //

    osXG = 36; // XG8 Aeronautical radionavigation system XG8 (on channel 36, 4 MHz airport radars, UK) –12.0 7.0 A.4.2-25
    osPL = 37; // PL8 Aeronautical radionavigation system PL8 (radars, artificial values) 0.0 1.5 A.4.2-25
    osNY = 38; // X7N Land mobile system X7N (VHF) 28.0 1.5 A.4.2-26
    //osNY = ; // X7C Land mobile system X7C (VHF) 28.0 1.5 A.4.2-27
    //osNY = ; // X8N Land mobile system X8N (VHF) 28.0 1.5 A.4.2-28
    //osNY = ; // X8C Land mobile system X8C (VHF) 28.0 1.5 A.4.2-29
    //osNY = ; // Y8N Land mobile system Y8N at 480 MHz 31.0 1.5 A.4.2-28
    //osNY = ; // Y8C Land mobile system Y8C at 480 MHz 31.0 1.5 A.4.2-29
    //osNY = ; // Z8N Land mobile system Z8C at 620 MHz 33.0 1.5 A.4.2-28
    //osNY = ; // Z8C Land mobile system Z8C at 620 MHz 33.0 1.5 A.4.2-29

    osXA8 = 39; //** ZA8C Radio astronomy single dish telescope sensitive DVB-T mask –39.0 50.0 A.4.2-5
    //osXA8 = ; //** ZA8N Radio astronomy single dish telescope non-critical DVB-T mask –39.0 50.0 A.4.2-6
    osXB8 = 40; //** ZB8C Radio astronomy VLBI sensitive DVB-T mask 2.0 50.0 A.4.2-5
    //osXB8 = ; //** ZB8N Radio astronomy VLBI non-critical DVB-T mask 2.0 50.0 A.4.2-6
    osZC8C = 41; //** Radio astronomy interferometry sensitive DVB-T mask –22.0 50.0 A.4.2-5
    osZC8N = 42; //** Radio astronomy interferometry non-critical DVB-T mask –22.0 50.0 A.4.2-6


implementation

end.
