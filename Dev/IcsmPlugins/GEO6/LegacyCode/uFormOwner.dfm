object frmFormTRK: TfrmFormTRK
  Left = 270
  Top = 276
  Width = 652
  Height = 346
  Caption = #1056#1077#1076#1072#1075#1091#1074#1072#1085#1085#1103' '#1058#1056#1050'/'#1054#1087#1077#1088#1072#1090#1086#1088#1072
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = True
  Position = poDefaultPosOnly
  Visible = True
  OnClose = FormClose
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 4
    Top = 15
    Width = 11
    Height = 13
    Caption = 'ID'
    FocusControl = dbedtID
  end
  object Label2: TLabel
    Left = 64
    Top = 15
    Width = 32
    Height = 13
    Caption = #1053#1072#1079#1074#1072
    FocusControl = dbedtName
  end
  object Label3: TLabel
    Left = 56
    Top = 127
    Width = 45
    Height = 13
    Caption = #1058#1077#1083#1077#1092#1086#1085
    FocusControl = dbedtTelefon
  end
  object Label4: TLabel
    Left = 256
    Top = 127
    Width = 29
    Height = 13
    Caption = #1060#1072#1082#1089
    FocusControl = dbedtFax
  end
  object Label5: TLabel
    Left = 47
    Top = 56
    Width = 47
    Height = 13
    Caption = #1028#1044#1056#1055#1054#1059
    FocusControl = dbedtEDRPOU
  end
  object Label7: TLabel
    Left = 72
    Top = 92
    Width = 25
    Height = 13
    Caption = #1041#1072#1085#1082
    FocusControl = dbedtBankName
  end
  object Label8: TLabel
    Left = 412
    Top = 56
    Width = 84
    Height = 13
    Caption = #1057#1074#1080#1076'. '#1087#1083#1072#1090'. '#1053#1044#1057
    FocusControl = dbedtNumTax
  end
  object Label9: TLabel
    Left = 248
    Top = 56
    Width = 19
    Height = 13
    Caption = #1030#1055#1053
    FocusControl = dbedtIPN
  end
  object Label10: TLabel
    Left = 528
    Top = 15
    Width = 22
    Height = 13
    Caption = #1060#1110#1085'.'
    FocusControl = dbedtFin
  end
  object Label11: TLabel
    Left = 39
    Top = 159
    Width = 57
    Height = 13
    Caption = #1070#1088'. '#1072#1076#1088#1077#1089#1072
    FocusControl = dbedtUrAddress
  end
  object Label12: TLabel
    Left = 35
    Top = 195
    Width = 61
    Height = 13
    Caption = #1060#1110#1079'. '#1072#1076#1088#1077#1089#1072
    FocusControl = dbedtFizAddress
  end
  object Label13: TLabel
    Left = 51
    Top = 228
    Width = 45
    Height = 13
    Caption = #1050#1077#1088#1110#1074#1085#1080#1082
    FocusControl = dbedtNameBoss
  end
  object Label14: TLabel
    Left = 71
    Top = 263
    Width = 27
    Height = 13
    Caption = 'e-mail'
    FocusControl = dbedtEmale
  end
  object Label15: TLabel
    Left = 416
    Top = 92
    Width = 82
    Height = 13
    Caption = #1056#1086#1079#1088#1072#1093'. '#1088#1072#1093#1091#1085#1086#1082
    FocusControl = dbedtNumRozr
    WordWrap = True
  end
  object gbxActivity: TGroupBox
    Left = 504
    Top = 128
    Width = 113
    Height = 129
    Caption = #1044#1077#1103#1090#1077#1083#1100#1085#1086#1089#1090#1100
    TabOrder = 15
    object servAab: TDBCheckBox
      Left = 16
      Top = 48
      Width = 75
      Height = 17
      Caption = 'FM'
      DataField = 'AAB'
      DataSource = dsrList
      TabOrder = 0
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
    object servAtb: TDBCheckBox
      Left = 16
      Top = 24
      Width = 75
      Height = 17
      Caption = 'TV'
      DataField = 'AVB'
      DataSource = dsrList
      TabOrder = 1
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
    object servDvb: TDBCheckBox
      Left = 16
      Top = 72
      Width = 75
      Height = 17
      Caption = 'DVB-T'
      DataField = 'DVB'
      DataSource = dsrList
      TabOrder = 2
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
    object servDab: TDBCheckBox
      Left = 16
      Top = 96
      Width = 75
      Height = 17
      Caption = 'T-DAB'
      DataField = 'DAB'
      DataSource = dsrList
      TabOrder = 3
      ValueChecked = '1'
      ValueUnchecked = '0'
    end
  end
  object dbedtID: TDBEdit
    Left = 20
    Top = 12
    Width = 33
    Height = 21
    DataField = 'ID'
    DataSource = dsrList
    ParentColor = True
    ReadOnly = True
    TabOrder = 0
  end
  object dbedtName: TDBEdit
    Left = 104
    Top = 12
    Width = 409
    Height = 21
    DataField = 'NAMEORGANIZATION'
    DataSource = dsrList
    TabOrder = 1
  end
  object dbedtTelefon: TDBEdit
    Left = 104
    Top = 124
    Width = 113
    Height = 21
    DataField = 'PHONE'
    DataSource = dsrList
    TabOrder = 9
  end
  object dbedtFax: TDBEdit
    Left = 288
    Top = 124
    Width = 113
    Height = 21
    DataField = 'FAX'
    DataSource = dsrList
    TabOrder = 10
  end
  object dbedtEDRPOU: TDBEdit
    Left = 104
    Top = 52
    Width = 108
    Height = 21
    DataField = 'NUMIDENTYCODACCOUNT'
    DataSource = dsrList
    TabOrder = 3
  end
  object dbedtBankName: TDBEdit
    Left = 104
    Top = 88
    Width = 273
    Height = 21
    DataField = 'B_NAME'
    DataSource = dsrList
    ReadOnly = True
    TabOrder = 6
  end
  object dbedtNumTax: TDBEdit
    Left = 504
    Top = 52
    Width = 121
    Height = 21
    DataField = 'NUMNDS'
    DataSource = dsrList
    TabOrder = 5
  end
  object dbedtIPN: TDBEdit
    Left = 276
    Top = 52
    Width = 109
    Height = 21
    DataField = 'NUMIDENTYCOD'
    DataSource = dsrList
    TabOrder = 4
  end
  object dbedtFin: TDBEdit
    Left = 552
    Top = 12
    Width = 69
    Height = 21
    DataField = 'TYPEFINANCE'
    DataSource = dsrList
    TabOrder = 2
  end
  object dbedtUrAddress: TDBEdit
    Left = 103
    Top = 156
    Width = 297
    Height = 21
    DataField = 'ADDRESSJURE'
    DataSource = dsrList
    TabOrder = 11
  end
  object dbedtFizAddress: TDBEdit
    Left = 103
    Top = 192
    Width = 297
    Height = 21
    DataField = 'ADDRESSPHYSICAL'
    DataSource = dsrList
    TabOrder = 12
  end
  object dbedtNameBoss: TDBEdit
    Left = 103
    Top = 224
    Width = 297
    Height = 21
    DataField = 'NAMEBOSS'
    DataSource = dsrList
    TabOrder = 13
  end
  object dbedtEmale: TDBEdit
    Left = 103
    Top = 260
    Width = 297
    Height = 21
    DataField = 'MAIL'
    DataSource = dsrList
    TabOrder = 14
  end
  object dbedtNumRozr: TDBEdit
    Left = 504
    Top = 88
    Width = 121
    Height = 21
    DataField = 'NUMSETTLEMENTACCOUNT'
    DataSource = dsrList
    TabOrder = 8
  end
  object btnSelectBank: TButton
    Left = 380
    Top = 88
    Width = 25
    Height = 21
    Caption = '...'
    TabOrder = 7
    OnClick = btnSelectBankClick
  end
  object btnOK: TButton
    Left = 448
    Top = 284
    Width = 75
    Height = 25
    Caption = 'Ok'
    TabOrder = 16
    OnClick = btnOKClick
  end
  object btmCancel: TButton
    Left = 536
    Top = 284
    Width = 75
    Height = 25
    Caption = #1054#1090#1084#1077#1085#1072
    TabOrder = 17
    OnClick = btmCancelClick
  end
  object dstObj: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BeforePost = dstObjBeforePost
    BufferChunks = 1000
    CachedUpdates = False
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
      '   LEFT OUTER JOIN BANK B ON (b.ID = o.BANK_ID)'
      'where'
      '  o.ID = :ID')
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
    Filtered = True
    Left = 416
    Top = 144
    object dstObjID: TIntegerField
      FieldName = 'ID'
      Origin = 'OWNER.ID'
      Required = True
      Visible = False
    end
    object dstObjNAMEORGANIZATION: TIBStringField
      DisplayLabel = #1053#1072#1079#1074#1072
      DisplayWidth = 32
      FieldName = 'NAMEORGANIZATION'
      Origin = 'OWNER.NAMEORGANIZATION'
      Size = 256
    end
    object dstObjPHONE: TIBStringField
      DisplayLabel = #1058#1077#1083#1077#1092#1086#1085
      FieldName = 'PHONE'
      Origin = 'OWNER.PHONE'
      Size = 16
    end
    object dstObjFAX: TIBStringField
      FieldName = 'FAX'
      Origin = 'OWNER.FAX'
      Size = 16
    end
    object dstObjSERV_LIST: TStringField
      DisplayLabel = #1057#1083#1091#1078#1073#1099
      FieldKind = fkCalculated
      FieldName = 'SERV_LIST'
      Size = 15
      Calculated = True
    end
    object dstObjNUMIDENTYCODACCOUNT: TIBStringField
      DisplayLabel = #1028#1044#1056#1055#1054#1059
      DisplayWidth = 8
      FieldName = 'NUMIDENTYCODACCOUNT'
      Origin = 'OWNER.NUMIDENTYCODACCOUNT'
      Size = 16
    end
    object dstObjBANK_ID: TIntegerField
      Tag = 6
      FieldName = 'BANK_ID'
      Origin = 'OWNER.BANK_ID'
      Required = True
      Visible = False
    end
    object dstObjB_NAME: TIBStringField
      Tag = 5
      DisplayLabel = #1041#1072#1085#1082
      DisplayWidth = 32
      FieldName = 'B_NAME'
      Origin = 'BANK.NAME'
      Size = 64
    end
    object dstObjNUMNDS: TIBStringField
      DisplayLabel = #1057#1074#1080#1076'. '#1087#1083#1072#1090'. '#1053#1044#1057
      DisplayWidth = 12
      FieldName = 'NUMNDS'
      Origin = 'OWNER.NUMNDS'
      Size = 16
    end
    object dstObjNUMIDENTYCOD: TIBStringField
      DisplayLabel = #1030#1055#1053
      DisplayWidth = 12
      FieldName = 'NUMIDENTYCOD'
      Origin = 'OWNER.NUMIDENTYCOD'
      Size = 16
    end
    object dstObjTYPEFINANCE: TSmallintField
      DisplayLabel = #1060#1110#1085'.'
      DisplayWidth = 5
      FieldName = 'TYPEFINANCE'
      Origin = 'OWNER.TYPEFINANCE'
    end
    object dstObjADDRESSJURE: TIBStringField
      DisplayLabel = #1070#1088'. '#1072#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESSJURE'
      Origin = 'OWNER.ADDRESSJURE'
      Size = 128
    end
    object dstObjADDRESSPHYSICAL: TIBStringField
      DisplayLabel = #1060#1110#1079'. '#1072#1076#1088#1077#1089#1072
      DisplayWidth = 32
      FieldName = 'ADDRESSPHYSICAL'
      Origin = 'OWNER.ADDRESSPHYSICAL'
      Size = 128
    end
    object dstObjNAMEBOSS: TIBStringField
      DisplayLabel = #1050#1077#1088#1110#1074#1085#1080#1082
      DisplayWidth = 20
      FieldName = 'NAMEBOSS'
      Origin = 'OWNER.NAMEBOSS'
      Size = 64
    end
    object dstObjMAIL: TIBStringField
      DisplayLabel = 'e-mail'
      FieldName = 'MAIL'
      Origin = 'OWNER.MAIL'
      Size = 32
    end
    object dstObjNUMSETTLEMENTACCOUNT: TIBStringField
      DisplayLabel = #1056#1086#1079#1088#1072#1093'. '#1088#1072#1093#1091#1085#1086#1082
      FieldName = 'NUMSETTLEMENTACCOUNT'
      Origin = 'OWNER.NUMSETTLEMENTACCOUNT'
      Size = 16
    end
    object dstObjAVB: TSmallintField
      FieldName = 'AVB'
      Origin = 'OWNER.AVB'
      Visible = False
    end
    object dstObjAAB: TSmallintField
      FieldName = 'AAB'
      Origin = 'OWNER.AAB'
      Visible = False
    end
    object dstObjDVB: TSmallintField
      FieldName = 'DVB'
      Origin = 'OWNER.DVB'
      Visible = False
    end
    object dstObjDAB: TSmallintField
      FieldName = 'DAB'
      Origin = 'OWNER.DAB'
      Visible = False
    end
  end
  object dsrList: TDataSource
    DataSet = dstObj
    Left = 368
    Top = 144
  end
end
