object frmDocumentsMustAnswer: TfrmDocumentsMustAnswer
  Left = 515
  Top = 436
  Width = 677
  Height = 280
  Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1094#1110#1103':'#1044#1086#1082#1091#1084#1077#1085#1090#1080', '#1097#1086' '#1087#1086#1090#1088#1077#1073#1091#1102#1090#1100' '#1074#1110#1076#1087#1086#1074#1110#1076#1110
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = False
  Position = poDefault
  Visible = True
  OnClose = FormClose
  OnCreate = FormCreate
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object dbgDocMausAnswer: TDBGrid
    Left = 0
    Top = 0
    Width = 669
    Height = 253
    Align = alClient
    DataSource = dsDocMustAnswer
    ReadOnly = True
    TabOrder = 0
    TitleFont.Charset = DEFAULT_CHARSET
    TitleFont.Color = clWindowText
    TitleFont.Height = -11
    TitleFont.Name = 'MS Sans Serif'
    TitleFont.Style = []
    OnDblClick = dbgDocMausAnswerDblClick
  end
  object dsDocMustAnswer: TDataSource
    DataSet = ibqDocMustAnswer
    Left = 136
    Top = 12
  end
  object ibqDocMustAnswer: TIBQuery
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'SELECT '
      #9'TR.ID TR_ID,'
      #9'TR.ADMINISTRATIONID,'
      #9'SC.CODE,'
      #9'LT.TYPELETTER,'#9
      #9'LT.ACCOUNTCONDITION_ID,'
      #9'LT.TELECOMORGANIZATION_ID,'
      #9'LT.CREATEDATEIN,'
      #9'LT.NUMIN,'
      #9'LT.CREATEDATEOUT,'
      #9'LT.NUMOUT,'
      #9'LT.ANSWERDATE,'
      #9'LT.ANSWERIS,'
      #9'SC.ENUMVAL'
      'FROM LETTERS LT'
      'left outer join TRANSMITTERS TR on (LT.TRANSMITTERS_ID =  TR.ID)'
      'left outer join SYSTEMCAST SC on (TR.SYSTEMCAST_ID  =  SC.ID)'
      'where  ANSWERDATE < :DATE')
    Left = 172
    Top = 12
    ParamData = <
      item
        DataType = ftUnknown
        Name = 'DATE'
        ParamType = ptUnknown
      end>
    object ibqDocMustAnswerADMINISTRATIONID: TIBStringField
      DisplayLabel = #1050#1086#1076' '#1087#1088#1076#1095
      FieldName = 'ADMINISTRATIONID'
      Origin = 'TRANSMITTERS.ADMINISTRATIONID'
      Size = 4
    end
    object ibqDocMustAnswerCODE: TIBStringField
      DisplayLabel = #1058#1080#1087' '#1087#1088#1076#1095
      FieldName = 'CODE'
      Origin = 'SYSTEMCAST.CODE'
      Size = 4
    end
    object ibqDocMustAnswerTYPELETTER: TSmallintField
      DisplayLabel = #1058#1080#1087
      DisplayWidth = 6
      FieldName = 'TYPELETTER'
      Origin = 'LETTERS.TYPELETTER'
      OnGetText = ibqDocMustAnswerTYPELETTERGetText
    end
    object ibqDocMustAnswerACC_NAME: TStringField
      DisplayLabel = #1057#1090#1072#1085
      DisplayWidth = 20
      FieldKind = fkLookup
      FieldName = 'ACC_NAME'
      LookupDataSet = dmMain.ibdsAccCondList
      LookupKeyFields = 'ID'
      LookupResultField = 'NAME'
      KeyFields = 'ACCOUNTCONDITION_ID'
      Size = 32
      Lookup = True
    end
    object ibqDocMustAnswerCREATEDATEIN: TDateField
      DisplayLabel = #1042#1093#1110#1076#1085#1072' '#1076#1072#1090#1072
      DisplayWidth = 7
      FieldName = 'CREATEDATEIN'
      Origin = 'LETTERS.CREATEDATEIN'
    end
    object ibqDocMustAnswerNUMIN: TIntegerField
      DisplayLabel = #1042#1093#1110#1076#1085#1080#1081' '#1085#1086#1084#1077#1088
      FieldName = 'NUMIN'
      Origin = 'LETTERS.NUMIN'
    end
    object ibqDocMustAnswerCREATEDATEOUT: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1074#1080#1093
      DisplayWidth = 7
      FieldName = 'CREATEDATEOUT'
      Origin = 'LETTERS.CREATEDATEOUT'
    end
    object ibqDocMustAnswerNUMOUT: TIntegerField
      DisplayLabel = #1042#1080#1093#1110#1076#1085#1080#1081
      FieldName = 'NUMOUT'
      Origin = 'LETTERS.NUMOUT'
    end
    object ibqDocMustAnswerANSWERDATE: TDateField
      DisplayLabel = #1044#1072#1090#1072' '#1074#1110#1076#1087#1086#1074
      FieldName = 'ANSWERDATE'
      Origin = 'LETTERS.ANSWERDATE'
    end
    object ibqDocMustAnswerANSWERIS: TSmallintField
      DisplayLabel = #1042#1110#1076#1087#1086#1074
      DisplayWidth = 5
      FieldName = 'ANSWERIS'
      Origin = 'LETTERS.ANSWERIS'
    end
    object ibqDocMustAnswerTELECOMORGANIZATION_ID: TIntegerField
      FieldName = 'TELECOMORGANIZATION_ID'
      Origin = 'LETTERS.TELECOMORGANIZATION_ID'
      Required = True
      Visible = False
    end
    object ibqDocMustAnswerACCOUNTCONDITION_ID: TIntegerField
      FieldName = 'ACCOUNTCONDITION_ID'
      Origin = 'LETTERS.ACCOUNTCONDITION_ID'
      Required = True
      Visible = False
    end
    object ibqDocMustAnswerTR_ID: TIntegerField
      FieldName = 'TR_ID'
      Origin = 'TRANSMITTERS.ID'
      Visible = False
    end
    object ibqDocMustAnswerENUMVAL: TIntegerField
      FieldName = 'ENUMVAL'
      Origin = 'SYSTEMCAST.ENUMVAL'
      Visible = False
    end
  end
end
