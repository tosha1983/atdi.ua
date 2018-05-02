unit UProtectOverlapping;

interface

const _DIM_ARRAY = 11;


 _PR_PAL = 0;
 _PR_SECAM = 1;

 _PR_HIK1L = 0;
 _PR_BGDK = 1;

 _PR_NO = 0;
 _PR_PO = 1;



type
  TPROverlappingCurve = array [1.. _DIM_ARRAY] of double;

const
{
  Значения разности частоты видео и несущей мешающего сигнала
}
  PR_Curve_Delta_F: TPROverlappingCurve =
  (-1.25, -0.5,    0.0,    0.5,   1.0,   2.0,   3.0,   3.6,   4.8,   5.7,   6.0);
  PR_Curve_Delta_F_BG: TPROverlappingCurve =
  (-1.25, -0.5,    0.0,    0.5,   1.0,   2.0,   3.0,   3.6,   4.8,   5.3,   6.0);
{
  Кривые для защитных отношений (длительная помеха)
}
//////////////////////////////////  offset 0   ///////////////////////////////////////
  PR_Curve_C_NO_0_PAL_HIK1L: TPROverlappingCurve =
  (40,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_0_PAL_HIK1L: TPROverlappingCurve =
  (30,       37,     38,     44,    44,    42,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_0_PAL_BGDK: TPROverlappingCurve =
  (32,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_0_PAL_BGDK: TPROverlappingCurve =
  (22,       37,     38,     44,    44,    42,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_0_SECAM_HIK1L: TPROverlappingCurve =
  (40,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_0_SECAM_HIK1L: TPROverlappingCurve =
  (30,       37,     38,     44,    44,    42,    36,    37,    37,    21,    21);

  PR_Curve_C_NO_0_SECAM_BGDK: TPROverlappingCurve =
  (32,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_0_SECAM_BGDK: TPROverlappingCurve =
  (22,       37,     38,     44,    44,    42,    36,    37,    37,    21,    21);

//////////////////////////////////  offset 1   ///////////////////////////////////////
  PR_Curve_C_NO_1_PAL_HIK1L: TPROverlappingCurve =
  (38,       49,     53,     57,    57,    53,    43,    48,    48,    32,    32);
  PR_Curve_C_PO_1_PAL_HIK1L: TPROverlappingCurve =
  (29,       38,     40,     42,    42,    41,    36,    36,    36,    22,    22);

  PR_Curve_C_NO_1_PAL_BGDK: TPROverlappingCurve =
  (30,       49,     53,     57,    57,    53,    43,    48,    48,    32,    32);
  PR_Curve_C_PO_1_PAL_BGDK: TPROverlappingCurve =
  (22,       38,     40,     42,    42,    41,    36,    36,    36,    22,    22);

  PR_Curve_C_NO_1_SECAM_HIK1L: TPROverlappingCurve =
  (38,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_1_SECAM_HIK1L: TPROverlappingCurve =
  (29,       38,     40,     42,    42,    41,    36,    37,    37,    21,    21);

  PR_Curve_C_NO_1_SECAM_BGDK: TPROverlappingCurve =
  (30,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_1_SECAM_BGDK: TPROverlappingCurve =
  (22,       38,     40,     42,    42,    41,    36,    37,    37,    21,    21);

//////////////////////////////////  offset 2   ///////////////////////////////////////
  PR_Curve_C_NO_2_PAL_HIK1L: TPROverlappingCurve =
  (34,       46,     50,     55,    55,    51,    41,    51,    51,    33,    33);
  PR_Curve_C_PO_2_PAL_HIK1L: TPROverlappingCurve =
  (27,       34,     36,     38,    38,    37,    34,    39,    39,    24,    24);

  PR_Curve_C_NO_2_PAL_BGDK: TPROverlappingCurve =
  (27,       46,     50,     55,    55,    51,    41,    51,    51,    33,    33);
  PR_Curve_C_PO_2_PAL_BGDK: TPROverlappingCurve =
  (20,       34,     36,     38,    38,    37,    34,    39,    39,    24,    24);

  PR_Curve_C_NO_2_SECAM_HIK1L: TPROverlappingCurve =
  (34,       46,     50,     55,    55,    51,    41,    45,    45,    30,    30);
  PR_Curve_C_PO_2_SECAM_HIK1L: TPROverlappingCurve =
  (27,       34,     36,     38,    38,    37,    34,    37,    37,    21,    21);

  PR_Curve_C_NO_2_SECAM_BGDK: TPROverlappingCurve =
  (27,       46,     50,     55,    55,    51,    41,    45,    45,    30,    30);
  PR_Curve_C_PO_2_SECAM_BGDK: TPROverlappingCurve =
  (20,       34,     36,     38,    38,    37,    34,    37,    37,    21,    21);

//////////////////////////////////  offset 3   ///////////////////////////////////////
  PR_Curve_C_NO_3_PAL_HIK1L: TPROverlappingCurve =
  (30,       42,     46,     50,    50,    46,    38,    53,    53,    35,    35);
  PR_Curve_C_PO_3_PAL_HIK1L: TPROverlappingCurve =
  (24,       30,     32,     34,    34,    33,    31,    40,    40,    26,    26);

  PR_Curve_C_NO_3_PAL_BGDK: TPROverlappingCurve =
  (23,       42,     46,     50,    50,    46,    38,    53,    53,    35,    35);
  PR_Curve_C_PO_3_PAL_BGDK: TPROverlappingCurve =
  (17,       30,     32,     34,    34,    33,    31,    40,    40,    26,    26);

  PR_Curve_C_NO_3_SECAM_HIK1L: TPROverlappingCurve =
  (30,       42,     46,     50,    50,    46,    38,    45,    45,    30,    30);
  PR_Curve_C_PO_3_SECAM_HIK1L: TPROverlappingCurve =
  (24,       30,     32,     34,    34,    33,    31,    37,    37,    21,    21);

  PR_Curve_C_NO_3_SECAM_BGDK: TPROverlappingCurve =
  (23,       42,     46,     50,    50,    46,    38,    45,    45,    30,    30);
  PR_Curve_C_PO_3_SECAM_BGDK: TPROverlappingCurve =
  (17,       30,     32,     34,    34,    33,    31,    37,    37,    21,    21);

//////////////////////////////////  offset 4   ///////////////////////////////////////
  PR_Curve_C_NO_4_PAL_HIK1L: TPROverlappingCurve =
  (28,       38,     42,     45,    45,    42,    35,    51,    51,    33,    33);
  PR_Curve_C_PO_4_PAL_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    39,    39,    24,    24);

  PR_Curve_C_NO_4_PAL_BGDK: TPROverlappingCurve =
  (21,       38,     42,     45,    45,    42,    35,    51,    51,    33,    33);
  PR_Curve_C_PO_4_PAL_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    39,    39,    24,    24);

  PR_Curve_C_NO_4_SECAM_HIK1L: TPROverlappingCurve =
  (28,       38,     42,     45,    45,    42,    35,    45,    45,    30,    30);
  PR_Curve_C_PO_4_SECAM_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

  PR_Curve_C_NO_4_SECAM_BGDK: TPROverlappingCurve =
  (21,       38,     42,     45,    45,    42,    35,    45,    45,    30,    30);
  PR_Curve_C_PO_4_SECAM_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

//////////////////////////////////  offset 5   ///////////////////////////////////////
  PR_Curve_C_NO_5_PAL_HIK1L: TPROverlappingCurve =
  (26,       35,     38,     41,    41,    38,    32,    48,    48,    32,    32);
  PR_Curve_C_PO_5_PAL_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    36,    36,    22,    22);

  PR_Curve_C_NO_5_PAL_BGDK: TPROverlappingCurve =
  (19,       35,     38,     41,    41,    38,    32,    48,    48,    32,    32);
  PR_Curve_C_PO_5_PAL_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    36,    36,    22,    22);

  PR_Curve_C_NO_5_SECAM_HIK1L: TPROverlappingCurve =
  (26,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_5_SECAM_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

  PR_Curve_C_NO_5_SECAM_BGDK: TPROverlappingCurve =
  (19,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_5_SECAM_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

//////////////////////////////////  offset 6   ///////////////////////////////////////
  PR_Curve_C_NO_6_PAL_HIK1L: TPROverlappingCurve =
  (24,       33,     35,     37,    37,    36,    30,    45,    45,    30,    30);
  PR_Curve_C_PO_6_PAL_HIK1L: TPROverlappingCurve =
  (23,       29,     32,     33,    33,    32,    30,    34,    34,    21,    21);

  PR_Curve_C_NO_6_PAL_BGDK: TPROverlappingCurve =
  (17,       33,     35,     37,    37,    36,    30,    45,    45,    30,    30);
  PR_Curve_C_PO_6_PAL_BGDK: TPROverlappingCurve =
  (16,       29,     32,     33,    33,    32,    30,    34,    34,    21,    21);

  PR_Curve_C_NO_6_SECAM_HIK1L: TPROverlappingCurve =
  (24,       33,     35,     37,    37,    36,    30,    45,    45,    30,    30);
  PR_Curve_C_PO_6_SECAM_HIK1L: TPROverlappingCurve =
  (23,       29,     32,     33,    33,    32,    30,    37,    37,    21,    21);

  PR_Curve_C_NO_6_SECAM_BGDK: TPROverlappingCurve =
  (17,       33,     35,     37,    37,    36,    30,    45,    45,    30,    30);
  PR_Curve_C_PO_6_SECAM_BGDK: TPROverlappingCurve =
  (16,       29,     32,     33,    33,    32,    30,    37,    37,    21,    21);

//////////////////////////////////  offset 7   ///////////////////////////////////////
  PR_Curve_C_NO_7_PAL_HIK1L: TPROverlappingCurve =
  (26,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_7_PAL_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    34,    34,    21,    21);

  PR_Curve_C_NO_7_PAL_BGDK: TPROverlappingCurve =
  (19,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_7_PAL_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    34,    34,    21,    21);

  PR_Curve_C_NO_7_SECAM_HIK1L: TPROverlappingCurve =
  (26,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_7_SECAM_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

  PR_Curve_C_NO_7_SECAM_BGDK: TPROverlappingCurve =
  (19,       35,     38,     41,    41,    38,    32,    45,    45,    30,    30);
  PR_Curve_C_PO_7_SECAM_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

//////////////////////////////////  offset 8   ///////////////////////////////////////
  PR_Curve_C_NO_8_PAL_HIK1L: TPROverlappingCurve =
  (28,       38,     42,     45,    45,    42,    35,    48,    48,    32,    32);
  PR_Curve_C_PO_8_PAL_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    36,    36,    22,    22);

  PR_Curve_C_NO_8_PAL_BGDK: TPROverlappingCurve =
  (21,       38,     42,     45,    45,    42,    35,    48,    48,    32,    32);
  PR_Curve_C_PO_8_PAL_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    36,    36,    22,    22);

  PR_Curve_C_NO_8_SECAM_HIK1L: TPROverlappingCurve =
  (28,       38,     42,     45,    45,    42,    35,    45,    45,    30,    30);
  PR_Curve_C_PO_8_SECAM_HIK1L: TPROverlappingCurve =
  (22,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

  PR_Curve_C_NO_8_SECAM_BGDK: TPROverlappingCurve =
  (21,       38,     42,     45,    45,    42,    35,    45,    45,    30,    30);
  PR_Curve_C_PO_8_SECAM_BGDK: TPROverlappingCurve =
  (15,       27,     29,     31,    31,    31,    30,    37,    37,    21,    21);

//////////////////////////////////  offset 9   ///////////////////////////////////////
  PR_Curve_C_NO_9_PAL_HIK1L: TPROverlappingCurve =
  (30,       42,     46,     50,    50,    46,    38,    51,    51,    33,    33);
  PR_Curve_C_PO_9_PAL_HIK1L: TPROverlappingCurve =
  (24,       30,     32,     34,    34,    33,    31,    39,    39,    24,    24);

  PR_Curve_C_NO_9_PAL_BGDK: TPROverlappingCurve =
  (23,       42,     46,     50,    50,    46,    38,    51,    51,    33,    33);
  PR_Curve_C_PO_9_PAL_BGDK: TPROverlappingCurve =
  (17,       30,     32,     34,    34,    33,    31,    39,    39,    24,    24);

  PR_Curve_C_NO_9_SECAM_HIK1L: TPROverlappingCurve =
  (30,       42,     46,     50,    50,    46,    38,    45,    45,    30,    30);
  PR_Curve_C_PO_9_SECAM_HIK1L: TPROverlappingCurve =
  (24,       30,     32,     34,    34,    33,    31,    37,    37,    21,    21);

  PR_Curve_C_NO_9_SECAM_BGDK: TPROverlappingCurve =
  (23,       42,     46,     50,    50,    46,    38,    45,    45,    30,    30);
  PR_Curve_C_PO_9_SECAM_BGDK: TPROverlappingCurve =
  (17,       30,     32,     34,    34,    33,    31,    37,    37,    21,    21);

//////////////////////////////////  offset 10   ///////////////////////////////////////
  PR_Curve_C_NO_10_PAL_HIK1L: TPROverlappingCurve =
  (34,       46,     50,     55,    55,    51,    41,    48,    48,    32,    32);
  PR_Curve_C_PO_10_PAL_HIK1L: TPROverlappingCurve =
  (27,       34,     36,     38,    38,    37,    34,    36,    36,    22,    22);

  PR_Curve_C_NO_10_PAL_BGDK: TPROverlappingCurve =
  (27,       46,     50,     55,    55,    51,    41,    48,    48,    32,    32);
  PR_Curve_C_PO_10_PAL_BGDK: TPROverlappingCurve =
  (20,       34,     36,     38,    38,    37,    34,    36,    36,    22,    22);

  PR_Curve_C_NO_10_SECAM_HIK1L: TPROverlappingCurve =
  (34,       46,     50,     55,    55,    51,    41,    45,    45,    30,    30);
  PR_Curve_C_PO_10_SECAM_HIK1L: TPROverlappingCurve =
  (27,       34,     36,     38,    38,    37,    34,    37,    37,    21,    21);

  PR_Curve_C_NO_10_SECAM_BGDK: TPROverlappingCurve =
  (27,       46,     50,     55,    55,    51,    41,    45,    45,    30,    30);
  PR_Curve_C_PO_10_SECAM_BGDK: TPROverlappingCurve =
  (20,       34,     36,     38,    38,    37,    34,    37,    37,    21,    21);

//////////////////////////////////  offset 11   ///////////////////////////////////////
  PR_Curve_C_NO_11_PAL_HIK1L: TPROverlappingCurve =
  (38,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_11_PAL_HIK1L: TPROverlappingCurve =
  (29,       38,     40,     42,    42,    41,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_11_PAL_BGDK: TPROverlappingCurve =
  (30,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_11_PAL_BGDK: TPROverlappingCurve =
  (22,       38,     40,     42,    42,    41,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_11_SECAM_HIK1L: TPROverlappingCurve =
  (38,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_11_SECAM_HIK1L: TPROverlappingCurve =
  (29,       38,     40,     42,    42,    41,    36,    37,    37,    21,    21);

  PR_Curve_C_NO_11_SECAM_BGDK: TPROverlappingCurve =
  (30,       49,     53,     57,    57,    53,    43,    45,    45,    30,    30);
  PR_Curve_C_PO_11_SECAM_BGDK: TPROverlappingCurve =
  (22,       38,     40,     42,    42,    41,    36,    37,    37,    21,    21);

//////////////////////////////////  offset 12   ///////////////////////////////////////
  PR_Curve_C_NO_12_PAL_HIK1L: TPROverlappingCurve =
  (40,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_12_PAL_HIK1L: TPROverlappingCurve =
  (30,       37,     44,     44,    44,    42,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_12_PAL_BGDK: TPROverlappingCurve =
  (32,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_12_PAL_BGDK: TPROverlappingCurve =
  (22,       37,     44,     44,    44,    42,    36,    34,    34,    21,    21);

  PR_Curve_C_NO_12_SECAM_HIK1L: TPROverlappingCurve =
  (40,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_12_SECAM_HIK1L: TPROverlappingCurve =
  (30,       37,     44,     44,    44,    42,    36,    37,    37,    21,    21);

  PR_Curve_C_NO_12_SECAM_BGDK: TPROverlappingCurve =
  (32,       50,     54,     58,    58,    54,    44,    45,    45,    30,    30);
  PR_Curve_C_PO_12_SECAM_BGDK: TPROverlappingCurve =
  (22,       37,     44,     44,    44,    42,    36,    37,    37,    21,    21);



{
  Кривые для защитных отношений (тропосферная помеха)
}
//////////////////////////////////  offset 0   ///////////////////////////////////////
  PR_Curve_T_NO_0_PAL_HIK1L: TPROverlappingCurve =
  (32,       44,     47,     50,    50,    44,    36,    35,    35,    18,    18);
  PR_Curve_T_PO_0_PAL_HIK1L: TPROverlappingCurve =
  (23,       32,     34,     40,    40,    37,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_0_PAL_BGDK: TPROverlappingCurve =
  (23,       44,     47,     50,    50,    44,    36,    35,    35,    18,    18);
  PR_Curve_T_PO_0_PAL_BGDK: TPROverlappingCurve =
  (11,       32,     34,     40,    40,    37,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_0_SECAM_HIK1L: TPROverlappingCurve =
  (32,       44,     47,     50,    50,    44,    36,    40,    40,    25,    25);
  PR_Curve_T_PO_0_SECAM_HIK1L: TPROverlappingCurve =
  (23,       32,     34,     40,    40,    37,    31,    33,    33,    18,    18);

  PR_Curve_T_NO_0_SECAM_BGDK: TPROverlappingCurve =
  (23,       44,     47,     50,    50,    44,    36,    40,    40,    25,    25);
  PR_Curve_T_PO_0_SECAM_BGDK: TPROverlappingCurve =
  (11,       32,     34,     40,    40,    37,    31,    33,    33,    18,    18);

//////////////////////////////////  offset 1   ///////////////////////////////////////
  PR_Curve_T_NO_1_PAL_HIK1L: TPROverlappingCurve =
  (31,       43,     46,     49,    49,    42,    34,    39,    39,    20,    20);
  PR_Curve_T_PO_1_PAL_HIK1L: TPROverlappingCurve =
  (23,       33,     36,     39,    39,    36,    31,    31,    31,    16,    16);

  PR_Curve_T_NO_1_PAL_BGDK: TPROverlappingCurve =
  (20,       43,     46,     49,    49,    42,    34,    39,    39,    20,    20);
  PR_Curve_T_PO_1_PAL_BGDK: TPROverlappingCurve =
  (11,       33,     36,     39,    39,    36,    31,    31,    31,    16,    16);

  PR_Curve_T_NO_1_SECAM_HIK1L: TPROverlappingCurve =
  (31,       43,     46,     49,    49,    42,    34,    40,    40,    25,    25);
  PR_Curve_T_PO_1_SECAM_HIK1L: TPROverlappingCurve =
  (23,       33,     36,     39,    39,    36,    31,    33,    33,    18,    18);

  PR_Curve_T_NO_1_SECAM_BGDK: TPROverlappingCurve =
  (20,       43,     46,     49,    49,    42,    34,    40,    40,    25,    25);
  PR_Curve_T_PO_1_SECAM_BGDK: TPROverlappingCurve =
  (11,       33,     36,     39,    39,    36,    31,    33,    33,    18,    18);

//////////////////////////////////  offset 2   ///////////////////////////////////////
  PR_Curve_T_NO_2_PAL_HIK1L: TPROverlappingCurve =
  (28,       39,     42,     45,    45,    39,    32,    42,    42,    22,    22);
  PR_Curve_T_PO_2_PAL_HIK1L: TPROverlappingCurve =
  (21,       29,     32,     35,    35,    33,    29,    34,    34,    17,    17);

  PR_Curve_T_NO_2_PAL_BGDK: TPROverlappingCurve =
  (17,       39,     42,     45,    45,    39,    32,    42,    42,    22,    22);
  PR_Curve_T_PO_2_PAL_BGDK: TPROverlappingCurve =
  (9,       29,     32,     35,    35,    33,    29,    34,    34,    17,    17);

  PR_Curve_T_NO_2_SECAM_HIK1L: TPROverlappingCurve =
  (28,       39,     42,     45,    45,    39,    32,    40,    40,    25,    25);
  PR_Curve_T_PO_2_SECAM_HIK1L: TPROverlappingCurve =
  (21,       29,     32,     35,    35,    33,    29,    33,    33,    18,    18);

  PR_Curve_T_NO_2_SECAM_BGDK: TPROverlappingCurve =
  (17,       39,     42,     45,    45,    39,    32,    40,    40,    25,    25);
  PR_Curve_T_PO_2_SECAM_BGDK: TPROverlappingCurve =
  (09,       29,     32,     35,    35,    33,    29,    33,    33,    18,    18);

//////////////////////////////////  offset 3   ///////////////////////////////////////
  PR_Curve_T_NO_3_PAL_HIK1L: TPROverlappingCurve =
  (25,       34,     36,     39,    39,    35,    29,    45,    45,    25,    25);
  PR_Curve_T_PO_3_PAL_HIK1L: TPROverlappingCurve =
  (19,       25,     28,     31,    31,    29,    26,    35,    35,    18,    18);

  PR_Curve_T_NO_3_PAL_BGDK: TPROverlappingCurve =
  (13,       34,     36,     39,    39,    35,    29,    45,    45,    25,    25);
  PR_Curve_T_PO_3_PAL_BGDK: TPROverlappingCurve =
  (07,       25,     28,     31,    31,    29,    26,    35,    35,    18,    18);

  PR_Curve_T_NO_3_SECAM_HIK1L: TPROverlappingCurve =
  (25,       34,     36,     39,    39,    35,    29,    40,    40,    25,    25);
  PR_Curve_T_PO_3_SECAM_HIK1L: TPROverlappingCurve =
  (19,       25,     28,     31,    31,    29,    26,    33,    33,    18,    18);

  PR_Curve_T_NO_3_SECAM_BGDK: TPROverlappingCurve =
  (13,       34,     36,     39,    39,    35,    29,    40,    40,    25,    25);
  PR_Curve_T_PO_3_SECAM_BGDK: TPROverlappingCurve =
  (07,       25,     28,     31,    31,    29,    26,    33,    33,    18,    18);

//////////////////////////////////  offset 4   ///////////////////////////////////////
  PR_Curve_T_NO_4_PAL_HIK1L: TPROverlappingCurve =
  (22,       30,     32,     35,    35,    32,    27,    42,    42,    22,    22);
  PR_Curve_T_PO_4_PAL_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    34,    34,    17,    17);

  PR_Curve_T_NO_4_PAL_BGDK: TPROverlappingCurve =
  (10,       30,     32,     35,    35,    32,    27,    42,    42,    22,    22);
  PR_Curve_T_PO_4_PAL_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    34,    34,    17,    17);

  PR_Curve_T_NO_4_SECAM_HIK1L: TPROverlappingCurve =
  (22,       30,     32,     35,    35,    32,    27,    40,    40,    25,    25);
  PR_Curve_T_PO_4_SECAM_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

  PR_Curve_T_NO_4_SECAM_BGDK: TPROverlappingCurve =
  (10,       30,     32,     35,    35,    32,    27,    40,    40,    25,    25);
  PR_Curve_T_PO_4_SECAM_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

//////////////////////////////////  offset 5   ///////////////////////////////////////
  PR_Curve_T_NO_5_PAL_HIK1L: TPROverlappingCurve =
  (20,       28,     30,     32,    32,    30,    25,    39,    39,    20,    20);
  PR_Curve_T_PO_5_PAL_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    31,    31,    16,    16);

  PR_Curve_T_NO_5_PAL_BGDK: TPROverlappingCurve =
  (08,       28,     30,     32,    32,    30,    25,    39,    39,    20,    20);
  PR_Curve_T_PO_5_PAL_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    31,    31,    16,    16);

  PR_Curve_T_NO_5_SECAM_HIK1L: TPROverlappingCurve =
  (20,       28,     30,     32,    32,    30,    25,    40,    40,    25,    25);
  PR_Curve_T_PO_5_SECAM_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

  PR_Curve_T_NO_5_SECAM_BGDK: TPROverlappingCurve =
  (08,       28,     30,     32,    32,    30,    25,    40,    40,    25,    25);
  PR_Curve_T_PO_5_SECAM_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

//////////////////////////////////  offset 6   ///////////////////////////////////////
  PR_Curve_T_NO_6_PAL_HIK1L: TPROverlappingCurve =
  (19,       27,     29,     31,    31,    29,    24,    35,    35,    18,    18);
  PR_Curve_T_PO_6_PAL_HIK1L: TPROverlappingCurve =
  (17,       24,     26,     28,    28,    26,    24,    28,    28,    15,    15);

  PR_Curve_T_NO_6_PAL_BGDK: TPROverlappingCurve =
  (07,       27,     29,     31,    31,    29,    24,    35,    35,    18,    18);
  PR_Curve_T_PO_6_PAL_BGDK: TPROverlappingCurve =
  (05,       24,     26,     28,    28,    26,    24,    28,    28,    15,    15);

  PR_Curve_T_NO_6_SECAM_HIK1L: TPROverlappingCurve =
  (19,       27,     29,     31,    31,    29,    24,    40,    40,    25,    25);
  PR_Curve_T_PO_6_SECAM_HIK1L: TPROverlappingCurve =
  (17,       24,     26,     28,    28,    26,    24,    33,    33,    18,    18);

  PR_Curve_T_NO_6_SECAM_BGDK: TPROverlappingCurve =
  (07,       27,     29,     31,    31,    29,    24,    40,    40,    25,    25);
  PR_Curve_T_PO_6_SECAM_BGDK: TPROverlappingCurve =
  (05,       24,     26,     28,    28,    26,    24,    33,    33,    18,    18);

//////////////////////////////////  offset 7   ///////////////////////////////////////
  PR_Curve_T_NO_7_PAL_HIK1L: TPROverlappingCurve =
  (20,       28,     30,     32,    32,    30,    25,    35,    35,    18,    18);
  PR_Curve_T_PO_7_PAL_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    28,    28,    15,    15);

  PR_Curve_T_NO_7_PAL_BGDK: TPROverlappingCurve =
  (08,       28,     30,     32,    32,    30,    25,    35,    35,    18,    18);
  PR_Curve_T_PO_7_PAL_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    28,    28,    15,    15);

  PR_Curve_T_NO_7_SECAM_HIK1L: TPROverlappingCurve =
  (20,       28,     30,     32,    32,    30,    25,    40,    40,    25,    25);
  PR_Curve_T_PO_7_SECAM_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

  PR_Curve_T_NO_7_SECAM_BGDK: TPROverlappingCurve =
  (08,       28,     30,     32,    32,    30,    25,    40,    40,    25,    25);
  PR_Curve_T_PO_7_SECAM_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

//////////////////////////////////  offset 8   ///////////////////////////////////////
  PR_Curve_T_NO_8_PAL_HIK1L: TPROverlappingCurve =
  (22,       30,     32,     35,    35,    32,    27,    39,    39,    20,    20);
  PR_Curve_T_PO_8_PAL_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    31,    31,    16,    16);

  PR_Curve_T_NO_8_PAL_BGDK: TPROverlappingCurve =
  (10,       30,     32,     35,    35,    32,    27,    39,    39,    20,    20);
  PR_Curve_T_PO_8_PAL_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    31,    31,    16,    16);

  PR_Curve_T_NO_8_SECAM_HIK1L: TPROverlappingCurve =
  (22,       30,     32,     35,    35,    32,    27,    40,    40,    25,    25);
  PR_Curve_T_PO_8_SECAM_HIK1L: TPROverlappingCurve =
  (17,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

  PR_Curve_T_NO_8_SECAM_BGDK: TPROverlappingCurve =
  (10,       30,     32,     35,    35,    32,    27,    40,    40,    25,    25);
  PR_Curve_T_PO_8_SECAM_BGDK: TPROverlappingCurve =
  (05,       22,     24,     26,    26,    25,    24,    33,    33,    18,    18);

//////////////////////////////////  offset 9   ///////////////////////////////////////
  PR_Curve_T_NO_9_PAL_HIK1L: TPROverlappingCurve =
  (25,       34,     36,     39,    39,    35,    29,    42,    42,    22,    22);
  PR_Curve_T_PO_9_PAL_HIK1L: TPROverlappingCurve =
  (19,       25,     28,     31,    31,    29,    26,    34,    34,    17,    17);

  PR_Curve_T_NO_9_PAL_BGDK: TPROverlappingCurve =
  (13,       34,     36,     39,    39,    35,    29,    42,    42,    22,    22);
  PR_Curve_T_PO_9_PAL_BGDK: TPROverlappingCurve =
  (07,       25,     28,     31,    31,    29,    26,    34,    34,    17,    17);

  PR_Curve_T_NO_9_SECAM_HIK1L: TPROverlappingCurve =
  (25,       34,     36,     39,    39,    35,    29,    40,    40,    25,    25);
  PR_Curve_T_PO_9_SECAM_HIK1L: TPROverlappingCurve =
  (19,       25,     28,     31,    31,    29,    26,    33,    33,    18,    18);

  PR_Curve_T_NO_9_SECAM_BGDK: TPROverlappingCurve =
  (13,       34,     36,     39,    39,    35,    29,    40,    40,    25,    25);
  PR_Curve_T_PO_9_SECAM_BGDK: TPROverlappingCurve =
  (07,       25,     28,     31,    31,    29,    26,    33,    33,    18,    18);

//////////////////////////////////  offset 10   ///////////////////////////////////////
  PR_Curve_T_NO_10_PAL_HIK1L: TPROverlappingCurve =
  (28,       39,     42,     45,    45,    39,    32,    39,    39,    20,    20);
  PR_Curve_T_PO_10_PAL_HIK1L: TPROverlappingCurve =
  (21,       29,     32,     35,    35,    33,    29,    31,    31,    16,    16);

  PR_Curve_T_NO_10_PAL_BGDK: TPROverlappingCurve =
  (17,       39,     42,     45,    45,    39,    32,    39,    39,    20,    20);
  PR_Curve_T_PO_10_PAL_BGDK: TPROverlappingCurve =
  (09,       29,     32,     35,    35,    33,    29,    31,    31,    16,    16);

  PR_Curve_T_NO_10_SECAM_HIK1L: TPROverlappingCurve =
  (28,       39,     42,     45,    45,    39,    32,    40,    40,    25,    25);
  PR_Curve_T_PO_10_SECAM_HIK1L: TPROverlappingCurve =
  (21,       29,     32,     35,    35,    33,    29,    33,    33,    18,    18);

  PR_Curve_T_NO_10_SECAM_BGDK: TPROverlappingCurve =
  (17,       39,     42,     45,    45,    39,    32,    40,    40,    25,    25);
  PR_Curve_T_PO_10_SECAM_BGDK: TPROverlappingCurve =
  (09,       29,     32,     35,    35,    33,    29,    33,    33,    18,    18);

//////////////////////////////////  offset 11   ///////////////////////////////////////
  PR_Curve_T_NO_11_PAL_HIK1L: TPROverlappingCurve =
  (31,       43,     46,     49,    49,    42,    34,    35,    35,    18,    18);
  PR_Curve_T_PO_11_PAL_HIK1L: TPROverlappingCurve =
  (23,       33,     36,     39,    39,    36,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_11_PAL_BGDK: TPROverlappingCurve =
  (20,       43,     46,     49,    49,    42,    34,    35,    35,    18,    18);
  PR_Curve_T_PO_11_PAL_BGDK: TPROverlappingCurve =
  (11,       33,     36,     39,    39,    36,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_11_SECAM_HIK1L: TPROverlappingCurve =
  (31,       43,     46,     49,    49,    42,    34,    40,    40,    25,    25);
  PR_Curve_T_PO_11_SECAM_HIK1L: TPROverlappingCurve =
  (23,       33,     36,     39,    39,    36,    31,    33,    33,    18,    18);

  PR_Curve_T_NO_11_SECAM_BGDK: TPROverlappingCurve =
  (20,       43,     46,     49,    49,    42,    34,    40,    40,    25,    25);
  PR_Curve_T_PO_11_SECAM_BGDK: TPROverlappingCurve =
  (11,       33,     36,     39,    39,    36,    31,    33,    33,    18,    18);

//////////////////////////////////  offset 12   ///////////////////////////////////////
  PR_Curve_T_NO_12_PAL_HIK1L: TPROverlappingCurve =
  (32,       44,     47,     50,    50,    44,    36,    35,    35,    18,    18);
  PR_Curve_T_PO_12_PAL_HIK1L: TPROverlappingCurve =
  (23,       32,     40,     40,    40,    37,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_12_PAL_BGDK: TPROverlappingCurve =
  (23,       44,     47,     50,    50,    44,    36,    35,    35,    18,    18);
  PR_Curve_T_PO_12_PAL_BGDK: TPROverlappingCurve =
  (11,       32,     40,     40,    40,    37,    31,    28,    28,    15,    15);

  PR_Curve_T_NO_12_SECAM_HIK1L: TPROverlappingCurve =
  (32,       44,     47,     50,    50,    44,    36,    40,    40,    25,    25);
  PR_Curve_T_PO_12_SECAM_HIK1L: TPROverlappingCurve =
  (23,       32,     40,     40,    40,    37,    31,    33,    33,    18,    18);

  PR_Curve_T_NO_12_SECAM_BGDK: TPROverlappingCurve =
  (23,       44,     47,     50,    50,    44,    36,    40,    40,    25,    25);
  PR_Curve_T_PO_12_SECAM_BGDK: TPROverlappingCurve =
  (11,       32,     40,     40,    40,    37,    31,    33,    33,    18,    18);


function GetCurveValue(df_curve, pr_curve: TPROverlappingCurve; df: double): double;

implementation
uses UShare;



{
  Возвращает значение защ. отношения из кривой, заданной параметром pr_curve.
  df_curve задает значени разности частот между несущими WANTED и  UNWANTED:

  pr_curve (30, 40, 50, 40, 30 ...)
          ^
      50  |       _____
          |     /      \
      40  |   /          \
          | /             \
      30 -|------------------------------> df_curve (-1.25, 0, 1.25 ...)
           -1.25  0  1.25  2   3   4 ...
}
function GetCurveValue(df_curve, pr_curve: TPROverlappingCurve; df: double): double;
var fmin, fmax, f: double;
    i, imin, imax: integer;
    prmin, prmax: double;
begin

 if (df < df_curve[1]) or (df > df_curve[_DIM_ARRAy]) then
 begin
   result := NO_INTERFERENCE;
   Exit;
 end;

 i := 1;
 f := df_curve[i];

 while (df >= f) and (i <= _DIM_ARRAY) do
 begin
   i := i + 1;
   f := df_curve[i];
 end;

 imin := i-1;
 imax := i;

 fmin := df_curve[imin];
 fmax := df_curve[imax];
 prmin := pr_curve[imin];
 prmax := pr_curve[imax];

 result := prmin + ((df - fmin) / (fmax - fmin)) * (prmax - prmin);
end;


end.
