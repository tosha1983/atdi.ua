inherited frmListDigitalSystem: TfrmListDigitalSystem
  Tag = 17
  Caption = #1057#1080#1089#1090#1077#1084#1080' '#1094#1080#1092#1088#1086#1074#1086#1075#1086' '#1058#1041
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited panList: TPanel
    Caption = #1057#1080#1089#1090#1077#1084#1080' '#1094#1080#1092#1088#1086#1074#1086#1075#1086' '#1058#1041
  end
  inherited trList: TIBTransaction
    Active = True
  end
  inherited dstList: TIBDataSet
    DeleteSQL.Strings = (
      'delete from DIGITALTELESYSTEM'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into DIGITALTELESYSTEM'
      '  (ID, NAMESYSTEM, MODULATION, CODERATE, GAUSSIANCHANNEL, '
      'RICEANCHANNEL, '
      '   RAYLEIGHCHANNEL, NETBITRATEGUARD4, NETBITRATEGUARD8, '
      'NETBITRATEGUARD16, '
      '   NETBITRATEGUARD32, ENUMVAL, DESCR)'
      'values'
      '  (:ID, :NAMESYSTEM, :MODULATION, :CODERATE, :GAUSSIANCHANNEL, '
      ':RICEANCHANNEL, '
      '   :RAYLEIGHCHANNEL, :NETBITRATEGUARD4, :NETBITRATEGUARD8, '
      ':NETBITRATEGUARD16, '
      '   :NETBITRATEGUARD32, :ENUMVAL, :DESCR)')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  NAMESYSTEM,'
      '  MODULATION,'
      '  CODERATE,'
      '  GAUSSIANCHANNEL,'
      '  RICEANCHANNEL,'
      '  RAYLEIGHCHANNEL,'
      '  NETBITRATEGUARD4,'
      '  NETBITRATEGUARD8,'
      '  NETBITRATEGUARD16,'
      '  NETBITRATEGUARD32,'
      '  ENUMVAL,'
      '  DESCR'
      'from DIGITALTELESYSTEM '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      
        'select ID, NAMESYSTEM, MODULATION, CODERATE, GAUSSIANCHANNEL, RI' +
        'CEANCHANNEL, RAYLEIGHCHANNEL, NETBITRATEGUARD4, NETBITRATEGUARD8' +
        ', NETBITRATEGUARD16, NETBITRATEGUARD32, ENUMVAL, DESCR '
      'from DIGITALTELESYSTEM'
      'ORDER BY 2')
    ModifySQL.Strings = (
      'update DIGITALTELESYSTEM'
      'set'
      '  NAMESYSTEM = :NAMESYSTEM,'
      '  MODULATION = :MODULATION,'
      '  CODERATE = :CODERATE,'
      '  GAUSSIANCHANNEL = :GAUSSIANCHANNEL,'
      '  RICEANCHANNEL = :RICEANCHANNEL,'
      '  RAYLEIGHCHANNEL = :RAYLEIGHCHANNEL,'
      '  NETBITRATEGUARD4 = :NETBITRATEGUARD4,'
      '  NETBITRATEGUARD8 = :NETBITRATEGUARD8,'
      '  NETBITRATEGUARD16 = :NETBITRATEGUARD16,'
      '  NETBITRATEGUARD32 = :NETBITRATEGUARD32,'
      '  ENUMVAL = :ENUMVAL,'
      '  DESCR = :DESCR'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'DIGITALTELESYSTEM.ID'
      Required = True
      Visible = False
    end
    object dstListNAMESYSTEM: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      FieldName = 'NAMESYSTEM'
      Origin = 'DIGITALTELESYSTEM.NAMESYSTEM'
      Size = 4
    end
    object dstListMODULATION: TIBStringField
      DisplayLabel = #1052#1086#1076#1091#1083#1103#1094#1110#1103
      FieldName = 'MODULATION'
      Origin = 'DIGITALTELESYSTEM.MODULATION'
      Size = 8
    end
    object dstListCODERATE: TIBStringField
      DisplayLabel = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1082#1086#1076#1091
      FieldName = 'CODERATE'
      Origin = 'DIGITALTELESYSTEM.CODERATE'
      Size = 4
    end
    object dstListGAUSSIANCHANNEL: TFloatField
      DisplayLabel = #1043#1072#1091#1089#1089#1110#1074' '#1082#1072#1085#1072#1083
      FieldName = 'GAUSSIANCHANNEL'
      Origin = 'DIGITALTELESYSTEM.GAUSSIANCHANNEL'
      DisplayFormat = '0.0'
    end
    object dstListRICEANCHANNEL: TFloatField
      DisplayLabel = 'Ricean '#1082#1072#1085#1072#1083
      FieldName = 'RICEANCHANNEL'
      Origin = 'DIGITALTELESYSTEM.RICEANCHANNEL'
      DisplayFormat = '0.0'
    end
    object dstListRAYLEIGHCHANNEL: TFloatField
      DisplayLabel = 'Rayleigh '#1082#1072#1085#1072#1083
      FieldName = 'RAYLEIGHCHANNEL'
      Origin = 'DIGITALTELESYSTEM.RAYLEIGHCHANNEL'
      DisplayFormat = '0.0'
    end
    object dstListNETBITRATEGUARD4: TFloatField
      DisplayLabel = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1076#1083#1103' 1/4'
      FieldName = 'NETBITRATEGUARD4'
      Origin = 'DIGITALTELESYSTEM.NETBITRATEGUARD4'
      DisplayFormat = '0.00'
    end
    object dstListNETBITRATEGUARD8: TFloatField
      DisplayLabel = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1076#1083#1103' 1/8'
      FieldName = 'NETBITRATEGUARD8'
      Origin = 'DIGITALTELESYSTEM.NETBITRATEGUARD8'
      DisplayFormat = '0.00'
    end
    object dstListNETBITRATEGUARD16: TFloatField
      DisplayLabel = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1076#1083#1103' 1/16'
      FieldName = 'NETBITRATEGUARD16'
      Origin = 'DIGITALTELESYSTEM.NETBITRATEGUARD16'
      DisplayFormat = '0.00'
    end
    object dstListNETBITRATEGUARD32: TFloatField
      DisplayLabel = #1064#1074#1080#1076#1082#1110#1089#1090#1100' '#1076#1083#1103' 1/32'
      FieldName = 'NETBITRATEGUARD32'
      Origin = 'DIGITALTELESYSTEM.NETBITRATEGUARD32'
      DisplayFormat = '0.00'
    end
    object dstListENUMVAL: TIntegerField
      DisplayLabel = #1050#1086#1085#1089#1090
      DisplayWidth = 4
      FieldName = 'ENUMVAL'
      Origin = 'DIGITALTELESYSTEM.ENUMVAL'
      Required = True
    end
    object dstListDESCR: TIBStringField
      DisplayLabel = #1054#1087#1080#1089#1072#1085#1080#1077
      DisplayWidth = 40
      FieldName = 'DESCR'
      Size = 128
    end
  end
end
