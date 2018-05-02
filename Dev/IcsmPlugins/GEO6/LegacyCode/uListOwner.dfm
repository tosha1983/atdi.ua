inherited frmListOwner: TfrmListOwner
  Tag = 25
  Left = 216
  Top = 77
  Width = 799
  Height = 405
  Caption = #1058#1056#1050' / '#1054#1087#1077#1088#1072#1090#1086#1088#1080
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited tbrList: TToolBar
    Width = 791
  end
  inherited panList: TPanel
    Width = 791
    Height = 353
    Caption = #1058#1056#1050' / '#1054#1087#1077#1088#1072#1090#1086#1088#1080
    inherited dgrList: TDBGrid
      Width = 791
      Height = 312
      ReadOnly = True
    end
    inherited panSearch: TPanel
      Top = 312
      Width = 791
    end
  end
  inherited aclList: TActionList
    Left = 296
  end
  inherited dstList: TIBDataSet
    AfterOpen = dstListAfterOpen
    OnCalcFields = dstListCalcFields
    DeleteSQL.Strings = (
      'delete from OWNER'
      'where'
      '  ID = :OLD_ID')
    InsertSQL.Strings = (
      'insert into OWNER'
      
        '  (ID, NAMEORGANIZATION, ADDRESSJURE, ADDRESSPHYSICAL, NUMIDENTY' +
        'COD, NUMNDS, '
      
        '   TYPEFINANCE, NAMEBOSS, PHONE, FAX, MAIL, NUMSETTLEMENTACCOUNT' +
        ', NUMIDENTYCODACCOUNT, '
      '   BANK_ID, AVB, AAB, DVB, DAB)'
      'values'
      
        '  (:ID, :NAMEORGANIZATION, :ADDRESSJURE, :ADDRESSPHYSICAL, :NUMI' +
        'DENTYCOD, '
      
        '   :NUMNDS, :TYPEFINANCE, :NAMEBOSS, :PHONE, :FAX, :MAIL, :NUMSE' +
        'TTLEMENTACCOUNT, '
      '   :NUMIDENTYCODACCOUNT, :BANK_ID, :AVB, :AAB, :DVB, :DAB)')
    RefreshSQL.Strings = (
      
        'SELECT o.ID, o.NAMEORGANIZATION, o.ADDRESSJURE, o.ADDRESSPHYSICA' +
        'L, o.NUMIDENTYCOD, o.NUMNDS, o.TYPEFINANCE, o.NAMEBOSS, o.PHONE,' +
        ' o.FAX, o.MAIL, o.NUMSETTLEMENTACCOUNT, o.NUMIDENTYCODACCOUNT, o' +
        '.BANK_ID, b.NAME B_NAME, o.AVB, o.AAB, o.DVB, o.DAB'
      'FROM OWNER O'
      '   LEFT OUTER JOIN BANK B ON (b.ID = o.BANK_ID)'
      'where'
      '  o.ID = :ID')
    SelectSQL.Strings = (
      
        'SELECT o.ID, o.NAMEORGANIZATION, o.ADDRESSJURE, o.ADDRESSPHYSICA' +
        'L, o.NUMIDENTYCOD, o.NUMNDS, o.TYPEFINANCE, o.NAMEBOSS, o.PHONE,' +
        ' o.FAX, o.MAIL, o.NUMSETTLEMENTACCOUNT, o.NUMIDENTYCODACCOUNT, o' +
        '.BANK_ID, b.NAME B_NAME,'
      'o.AVB, o.AAB, o.DVB, o.DAB'
      'FROM OWNER O'
      '   LEFT OUTER JOIN BANK B ON (b.ID = o.BANK_ID)')
    ModifySQL.Strings = (
      'update OWNER'
      'set'
      '  NAMEORGANIZATION = :NAMEORGANIZATION,'
      '  ADDRESSJURE = :ADDRESSJURE,'
      '  ADDRESSPHYSICAL = :ADDRESSPHYSICAL,'
      '  NUMIDENTYCOD = :NUMIDENTYCOD,'
      '  NUMNDS = :NUMNDS,'
      '  TYPEFINANCE = :TYPEFINANCE,'
      '  NAMEBOSS = :NAMEBOSS,'
      '  PHONE = :PHONE,'
      '  FAX = :FAX,'
      '  MAIL = :MAIL,'
      '  NUMSETTLEMENTACCOUNT = :NUMSETTLEMENTACCOUNT,'
      '  NUMIDENTYCODACCOUNT = :NUMIDENTYCODACCOUNT,'
      '  BANK_ID = :BANK_ID,'
      '  AVB = :AVB, '
      '  AAB = :AAB, '
      '  DVB = :DVB, '
      '  DAB = :DAB'
      'where'
      '  ID = :OLD_ID')
    object dstListID: TIntegerField
      FieldName = 'ID'
      Origin = 'OWNER.ID'
      Required = True
      Visible = False
    end
    object dstListNAMEORGANIZATION: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      DisplayWidth = 32
      FieldName = 'NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object dstListPHONE: TIBStringField
      DisplayLabel = #1058#1077#1083#1077#1092#1086#1085
      FieldName = 'PHONE'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object dstListFAX: TIBStringField
      FieldName = 'FAX'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object dstListSERV_LIST: TStringField
      DisplayLabel = #1057#1083#1091#1078#1073#1099
      FieldKind = fkCalculated
      FieldName = 'SERV_LIST'
      Size = 15
      Calculated = True
    end
    object dstListNUMIDENTYCODACCOUNT: TIBStringField
      DisplayLabel = #1028#1044#1056#1055#1054#1059
      DisplayWidth = 8
      FieldName = 'NUMIDENTYCODACCOUNT'
      Origin = 'OWNER.NUMIDENTYCODACCOUNT'
      Size = 16
    end
    object dstListBANK_ID: TIntegerField
      Tag = 6
      FieldName = 'BANK_ID'
      Origin = 'OWNER.BANK_ID'
      Required = True
      Visible = False
    end
    object dstListB_NAME: TIBStringField
      Tag = 5
      DisplayLabel = #1041#1072#1085#1082
      DisplayWidth = 32
      FieldName = 'B_NAME'
      Origin = 'BANK.NAME'
      Size = 64
    end
    object dstListNUMNDS: TIBStringField
      DisplayLabel = #1057#1074#1080#1076'. '#1087#1083#1072#1090'. '#1053#1044#1057
      DisplayWidth = 12
      FieldName = 'NUMNDS'
      Origin = 'OWNER.NUMNDS'
      Size = 16
    end
    object dstListNUMIDENTYCOD: TIBStringField
      DisplayLabel = #1030#1055#1053
      DisplayWidth = 12
      FieldName = 'NUMIDENTYCOD'
      Origin = 'OWNER.NUMIDENTYCOD'
      Size = 16
    end
    object dstListTYPEFINANCE: TSmallintField
      DisplayLabel = #1060#1110#1085'.'
      DisplayWidth = 5
      FieldName = 'TYPEFINANCE'
      Origin = 'OWNER.TYPEFINANCE'
    end
    object dstListADDRESSJURE: TIBStringField
      DisplayLabel = #1070#1088'. '#1072#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESSJURE'
      Origin = 'OWNER.ADDRESSJURE'
      Size = 128
    end
    object dstListADDRESSPHYSICAL: TIBStringField
      DisplayLabel = #1060#1110#1079'. '#1072#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESSPHYSICAL'
      Origin = 'OWNER.ADDRESSPHYSICAL'
      Size = 128
    end
    object dstListNAMEBOSS: TIBStringField
      DisplayLabel = #1050#1077#1088#1110#1074#1085#1080#1082
      DisplayWidth = 20
      FieldName = 'NAMEBOSS'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object dstListMAIL: TIBStringField
      DisplayLabel = 'e-mail'
      FieldName = 'MAIL'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object dstListNUMSETTLEMENTACCOUNT: TIBStringField
      DisplayLabel = #1056#1086#1079#1088#1072#1093'. '#1088#1072#1093#1091#1085#1086#1082
      FieldName = 'NUMSETTLEMENTACCOUNT'
      Origin = 'OWNER.NUMSETTLEMENTACCOUNT'
      Size = 16
    end
    object dstListAVB: TSmallintField
      FieldName = 'AVB'
      Origin = 'OWNER.AVB'
      Visible = False
    end
    object dstListAAB: TSmallintField
      FieldName = 'AAB'
      Origin = 'OWNER.AAB'
      Visible = False
    end
    object dstListDVB: TSmallintField
      FieldName = 'DVB'
      Origin = 'OWNER.DVB'
      Visible = False
    end
    object dstListDAB: TSmallintField
      FieldName = 'DAB'
      Origin = 'OWNER.DAB'
      Visible = False
    end
  end
end
