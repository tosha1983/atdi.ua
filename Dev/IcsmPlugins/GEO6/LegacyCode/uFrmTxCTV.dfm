inherited frmTxCTV: TfrmTxCTV
  Left = 183
  Top = 98
  BorderStyle = bsToolWindow
  Caption = 'frmTxCTV'
  ClientHeight = 510
  OldCreateOrder = True
  PixelsPerInch = 96
  TextHeight = 13
  inherited pnlForAllTop: TPanel
    inherited lblTxType: TLabel
      Width = 39
      Caption = #1050#1058#1041
    end
    inherited edtID: TDBEdit
      Left = 798
    end
  end
  inherited pcData: TPageControl
    Height = 418
    inherited tshCommon: TTabSheet
      object PnlCTVChannel: TPanel [0]
        Left = 0
        Top = 0
        Width = 772
        Height = 141
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 0
        object lblChannekCount: TLabel
          Left = 10
          Top = 28
          Width = 121
          Height = 13
          Caption = #1050#1086#1083#1080#1095#1077#1089#1090#1074#1086' '#1058#1042' '#1082#1072#1085#1072#1083#1086#1074
        end
        object lblChannelFMCount: TLabel
          Left = 6
          Top = 74
          Width = 150
          Height = 13
          Caption = #1050#1086#1083#1080#1095#1077#1089#1090#1074#1086' '#1054#1042#1063' '#1063#1052' '#1082#1072#1085#1072#1083#1086#1074
        end
        object lblTabl: TLabel
          Left = 126
          Top = 4
          Width = 46
          Height = 13
          Caption = #1058#1072#1073#1083#1080#1094#1103':'
        end
        object lblAreaCoverage: TLabel
          Left = 14
          Top = 128
          Width = 74
          Height = 13
          Caption = #1047#1086#1085#1072' '#1087#1086#1082#1088#1080#1090#1090#1103
        end
        object edtChannelTVCount: TNumericEdit
          Left = 34
          Top = 47
          Width = 61
          Height = 21
          ReadOnly = True
          TabOrder = 0
          Text = 'edtChannelTVCount'
          Alignment = taRightJustify
          OldValue = 'edtChannelTVCount'
        end
        object edtChannelFMCount: TNumericEdit
          Left = 34
          Top = 94
          Width = 61
          Height = 21
          ReadOnly = True
          TabOrder = 1
          Text = 'edtChannelCount'
          Alignment = taRightJustify
          OldValue = 'edtChannelCount'
        end
        object dbgChannelCTV: TDBGrid
          Left = 182
          Top = 6
          Width = 587
          Height = 149
          DataSource = dsChannelCtv
          TabOrder = 2
          TitleFont.Charset = DEFAULT_CHARSET
          TitleFont.Color = clWindowText
          TitleFont.Height = -11
          TitleFont.Name = 'MS Sans Serif'
          TitleFont.Style = []
          OnEditButtonClick = dbgChannelCTVEditButtonClick
          Columns = <
            item
              Expanded = False
              FieldName = 'TYPESYSTEM'
              PickList.Strings = (
                #1095#1072#1089#1090#1086#1090#1072
                #1082#1072#1085#1072#1083)
              Width = 65
              Visible = True
            end
            item
              ButtonStyle = cbsEllipsis
              Expanded = False
              FieldName = 'RX_CHANNEL'
              ReadOnly = True
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'RX_FREQUENCY'
              ReadOnly = True
              Width = 68
              Visible = True
            end
            item
              ButtonStyle = cbsEllipsis
              Expanded = False
              FieldName = 'TX_CHANNEL'
              ReadOnly = True
              Width = 58
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'TX_FREQUENCY'
              ReadOnly = True
              Width = 69
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'NAMEPROGRAMM'
              Visible = True
            end
            item
              Expanded = False
              FieldName = 'TR_NAME'
              Visible = True
            end>
        end
        object Button1: TButton
          Left = 94
          Top = 124
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 3
        end
      end
      object pnlCTVInfo: TPanel [1]
        Left = 0
        Top = 141
        Width = 772
        Height = 46
        Align = alBottom
        TabOrder = 1
      end
      inherited pnlMaster: TPanel
        Top = 187
        TabOrder = 2
        inherited cbStateOutCode: TDBLookupComboBox
          Left = 35
        end
      end
      inherited gbxCoordination: TGroupBox
        Top = 353
        TabOrder = 3
      end
    end
    inherited tshCoordination: TTabSheet
      inherited gbOrganizations: TGroupBox
        Height = 224
        inherited dbgOrganizations: TDBGrid
          Height = 207
        end
      end
      inherited gbDocuments: TGroupBox
        Top = 224
        Height = 168
        inherited dbgDocuments: TDBGrid
          Height = 151
        end
      end
      inherited gbDoc: TGroupBox
        Height = 224
      end
      inherited gbOrganization: TGroupBox
        Height = 224
      end
    end
    inherited tshEquipment: TTabSheet
      inherited gbEquipment: TGroupBox
        Width = 783
        Height = 303
        inherited dbgEquipment: TDBGrid
          Width = 779
          Height = 286
        end
      end
      inherited pnlEquipButton: TPanel
        Top = 303
        Width = 783
      end
    end
    inherited tshLicenses: TTabSheet
      inherited btnAttach: TButton
        TabOrder = 41
      end
      inherited btnDetach: TButton
        TabOrder = 37
      end
      inherited gbHoursOfOp: TGroupBox
        TabOrder = 39
      end
      inherited gbUseDates: TGroupBox
        TabOrder = 40
      end
      object Panel1: TPanel
        Left = 296
        Top = 0
        Width = 465
        Height = 137
        TabOrder = 38
        object lblPermRegion: TLabel
          Left = 16
          Top = 48
          Width = 166
          Height = 13
          Caption = #1044#1086#1079#1074#1110#1083' '#1086#1073#1083#1072#1089#1090#1085#1086#1075#1086' '#1082#1086#1084#1110#1090#1077#1090#1091' '#1050#1058#1042
        end
        object lblRequestCTV: TLabel
          Left = 20
          Top = 111
          Width = 162
          Height = 13
          Caption = #1047#1072#1103#1074#1082#1072' '#1085#1072' '#1074#1080#1087#1080#1089#1082#1091' '#1088#1072#1093#1091#1085#1082#1091' '#1050#1058#1042
        end
        object lblDatePermRegion: TLabel
          Left = 58
          Top = 80
          Width = 124
          Height = 13
          Caption = #1044#1072#1090#1072' '#1074#1080#1076#1072#1095#1110' '#1076#1086#1079#1074#1110#1083#1091' '#1050#1058#1042
        end
        object lblOrganPerm: TLabel
          Left = 56
          Top = 16
          Width = 126
          Height = 13
          Caption = #1054#1088#1075#1072#1085', '#1097#1086' '#1085#1072#1076#1072#1074' '#1076#1086#1079#1074#1110#1083#1080
        end
        object edtNumPermRegionalCouncil: TDBEdit
          Left = 188
          Top = 44
          Width = 121
          Height = 21
          DataField = 'NUMPERMREGCOUNCIL'
          DataSource = dsLicenseCTV
          TabOrder = 0
        end
        object mmNoticeCount: TDBMemo
          Left = 188
          Top = 107
          Width = 102
          Height = 21
          DataField = 'NOTICECOUNT'
          DataSource = dsLicenseCTV
          TabOrder = 1
        end
        object edtRegionalCouncil: TDBEdit
          Left = 188
          Top = 12
          Width = 121
          Height = 21
          DataField = 'REGIONALCOUNCIL'
          DataSource = dsLicenseCTV
          TabOrder = 2
        end
        object btnNoticeCount: TButton
          Left = 290
          Top = 108
          Width = 19
          Height = 20
          Caption = '...'
          Font.Charset = DEFAULT_CHARSET
          Font.Color = clWindowText
          Font.Height = -11
          Font.Name = 'MS Sans Serif'
          Font.Style = [fsBold]
          ParentFont = False
          TabOrder = 3
        end
        object dtpPermRegion: TDateTimePicker
          Left = 192
          Top = 76
          Width = 117
          Height = 21
          CalAlignment = dtaLeft
          Date = 37843.5524871528
          Time = 37843.5524871528
          DateFormat = dfShort
          DateMode = dmComboBox
          Kind = dtkDate
          ParseInput = False
          TabOrder = 4
          OnChange = dtpPermRegionChange
        end
        object edtDatePermRegional: TDBEdit
          Left = 188
          Top = 76
          Width = 104
          Height = 21
          DataField = 'DATEPERMREGCOUNCIL'
          DataSource = dsLicenseCTV
          TabOrder = 5
        end
      end
    end
    inherited tshTestpoint: TTabSheet
      inherited gbTestpoints: TGroupBox
        Width = 773
        Height = 392
        inherited dbgTestpoints: TDBGrid
          Width = 769
          Height = 375
        end
      end
    end
    inherited tshChangeLog: TTabSheet
      inherited dbgGhangeLog: TDBGrid
        Width = 783
        Height = 345
      end
      inherited panList: TPanel
        Width = 783
        Height = 345
        inherited dgrList: TDBGrid
          Width = 783
          Height = 320
        end
        inherited panSearch: TPanel
          Top = 320
          Width = 783
        end
      end
    end
    inherited tshMap: TTabSheet
      inherited cmf: TCustomMapFrame
        Height = 392
        inherited sb: TStatusBar
          Top = 373
        end
        inherited bmf: TBaseMapFrame
          Height = 344
        end
        inherited tb: TToolBar
          Images = cmf.bmf.iml
          inherited tb1: TToolButton
            Action = cmf.bmf.actNone
          end
          inherited tb2: TToolButton
            Action = cmf.bmf.actPan
          end
          inherited tb3: TToolButton
            Action = cmf.bmf.actZoomInTwice
          end
          inherited tb4: TToolButton
            Action = cmf.bmf.actZoomOutTwice
          end
          inherited tb5: TToolButton
            Action = cmf.bmf.actConf
          end
          inherited tb6: TToolButton
            Action = cmf.bmf.actArrows
          end
        end
      end
    end
  end
  inherited pnlForAllBottom: TPanel
    Top = 472
    Height = 38
  end
  inherited dsStantionsBase: TDataSource
    Top = 4
  end
  inherited ibqAccCondNameOut: TIBQuery
    Left = 144
  end
  inherited dsStand: TDataSource
    Top = 456
  end
  inherited ibqUserName: TIBQuery
    Left = 176
  end
  inherited ibqTRKName: TIBQuery
    Left = 212
  end
  inherited ibqSystemCastName: TIBQuery
    Left = 276
  end
  object ibdsLicenseCTV: TIBDataSet [27]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    AfterEdit = ibdsLicenseCTVAfterEdit
    BufferChunks = 1000
    CachedUpdates = False
    RefreshSQL.Strings = (
      'select '#9'ID,'
      #9'REGIONALCOUNCIL, '
      #9'NUMPERMREGCOUNCIL, '
      #9'DATEPERMREGCOUNCIL, '
      #9'NOTICECOUNT '
      'from TRANSMITTERS where ID = :ID')
    SelectSQL.Strings = (
      'select '#9'ID,'
      #9'REGIONALCOUNCIL, '
      #9'NUMPERMREGCOUNCIL, '
      #9'DATEPERMREGCOUNCIL, '
      #9'NOTICECOUNT '
      'from TRANSMITTERS where ID = :ID')
    ModifySQL.Strings = (
      'update TRANSMITTERS set'
      #9'REGIONALCOUNCIL = :REGIONALCOUNCIL, '
      #9'NUMPERMREGCOUNCIL = :NUMPERMREGCOUNCIL, '
      #9'DATEPERMREGCOUNCIL = :DATEPERMREGCOUNCIL, '
      #9'NOTICECOUNT = :NOTICECOUNT '
      'where ID = :ID')
    Left = 536
    Top = 388
    object ibdsLicenseCTVREGIONALCOUNCIL: TIBStringField
      FieldName = 'REGIONALCOUNCIL'
      Origin = 'TRANSMITTERS.REGIONALCOUNCIL'
      Size = 64
    end
    object ibdsLicenseCTVNUMPERMREGCOUNCIL: TIBStringField
      FieldName = 'NUMPERMREGCOUNCIL'
      Origin = 'TRANSMITTERS.NUMPERMREGCOUNCIL'
      Size = 64
    end
    object ibdsLicenseCTVDATEPERMREGCOUNCIL: TDateField
      FieldName = 'DATEPERMREGCOUNCIL'
      Origin = 'TRANSMITTERS.DATEPERMREGCOUNCIL'
    end
    object ibdsLicenseCTVNOTICECOUNT: TBlobField
      FieldName = 'NOTICECOUNT'
      Origin = 'TRANSMITTERS.NOTICECOUNT'
      Size = 8
    end
    object ibdsLicenseCTVID: TIntegerField
      FieldName = 'ID'
      Origin = 'TRANSMITTERS.ID'
      Required = True
    end
  end
  object dsLicenseCTV: TDataSource [28]
    DataSet = ibdsLicenseCTV
    Left = 568
    Top = 388
  end
  object ibdsChannelCtv: TIBDataSet [29]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BeforeDelete = ibdsChannelCtvBeforeDelete
    BeforePost = ibdsChannelCtvBeforePost
    BufferChunks = 1000
    CachedUpdates = False
    DeleteSQL.Strings = (
      'delete from CHANELSRETRANSLATE'
      'where'
      '  ID = :OLD_ID ')
    InsertSQL.Strings = (
      'insert into CHANELSRETRANSLATE'
      '  (ID, TRANSMITTERS_ID, TYPESYSTEM, TYPERECEIVE_ID, RX_CHANNEL, '
      'RX_FREQUENCY, '
      '   TX_CHANNEL, TX_FREQUENCY, NAMEPROGRAMM)'
      'values'
      
        '  (:ID, :TRANSMITTERS_ID, :TYPESYSTEM, :TYPERECEIVE_ID, :RX_CHAN' +
        'NEL, '
      ':RX_FREQUENCY, '
      '   :TX_CHANNEL, :TX_FREQUENCY, :NAMEPROGRAMM)')
    RefreshSQL.Strings = (
      
        'select CHR.ID, CHR.TRANSMITTERS_ID, CHR.TYPESYSTEM, CHR.TYPERECE' +
        'IVE_ID, CHR.RX_CHANNEL, CHR.RX_FREQUENCY, CHR.TX_CHANNEL, CHR.TX' +
        '_FREQUENCY, CHR.NAMEPROGRAMM, TR.NAME TR_NAME'
      'from CHANELSRETRANSLATE CHR'
      'left outer join  TYPERECEIVE TR on (TR.ID = CHR.TYPERECEIVE_ID)'
      'where CHR.ID = :ID')
    SelectSQL.Strings = (
      
        'select CHR.ID, CHR.TRANSMITTERS_ID, CHR.TYPESYSTEM, CHR.TYPERECE' +
        'IVE_ID, CHR.RX_CHANNEL, CHR.RX_FREQUENCY, CHR.TX_CHANNEL, CHR.TX' +
        '_FREQUENCY, CHR.NAMEPROGRAMM, TR.NAME TR_NAME'
      'from CHANELSRETRANSLATE CHR'
      'left outer join  TYPERECEIVE TR on (TR.ID = CHR.TYPERECEIVE_ID)'
      'where CHR.TRANSMITTERS_ID = :TX_ID')
    ModifySQL.Strings = (
      'update CHANELSRETRANSLATE'
      'set'
      '  ID = :ID,'
      '  TRANSMITTERS_ID = :TRANSMITTERS_ID,'
      '  TYPESYSTEM = :TYPESYSTEM,'
      '  TYPERECEIVE_ID = :TYPERECEIVE_ID,'
      '  RX_CHANNEL = :RX_CHANNEL,'
      '  RX_FREQUENCY = :RX_FREQUENCY,'
      '  TX_CHANNEL = :TX_CHANNEL,'
      '  TX_FREQUENCY = :TX_FREQUENCY,'
      '  NAMEPROGRAMM = :NAMEPROGRAMM'
      'where'
      '  ID = :OLD_ID ')
    Left = 310
    Top = 370
    object ibdsChannelCtvID: TIntegerField
      FieldName = 'ID'
      Origin = 'CHANELSRETRANSLATE.ID'
      Required = True
      Visible = False
    end
    object ibdsChannelCtvTRANSMITTERS_ID: TIntegerField
      FieldName = 'TRANSMITTERS_ID'
      Origin = 'CHANELSRETRANSLATE.TRANSMITTERS_ID'
      Required = True
      Visible = False
    end
    object ibdsChannelCtvTYPESYSTEM: TSmallintField
      DisplayLabel = #1058#1080#1087' '#1082#1072#1085#1072#1083#1072
      DisplayWidth = 12
      FieldName = 'TYPESYSTEM'
      Origin = 'CHANELSRETRANSLATE.TYPESYSTEM'
      OnGetText = ibdsChannelCtvTYPESYSTEMGetText
      OnSetText = ibdsChannelCtvTYPESYSTEMSetText
    end
    object ibdsChannelCtvTYPERECEIVE_ID: TIntegerField
      FieldName = 'TYPERECEIVE_ID'
      Origin = 'CHANELSRETRANSLATE.TYPERECEIVE_ID'
      Required = True
      Visible = False
    end
    object ibdsChannelCtvRX_CHANNEL: TIBStringField
      DisplayLabel = #1050#1072#1085#1072#1083' '#1087#1088#1080#1081#1086#1084#1091
      DisplayWidth = 16
      FieldName = 'RX_CHANNEL'
      Origin = 'CHANELSRETRANSLATE.RX_CHANNEL'
      Size = 4
    end
    object ibdsChannelCtvRX_FREQUENCY: TFloatField
      DisplayLabel = #1063#1072#1089#1090#1086#1090#1072' '#1087#1088#1080#1081#1086#1084#1091
      DisplayWidth = 18
      FieldName = 'RX_FREQUENCY'
      Origin = 'CHANELSRETRANSLATE.RX_FREQUENCY'
    end
    object ibdsChannelCtvTX_FREQUENCY: TFloatField
      DisplayLabel = #1042#1080#1093#1110#1076#1085#1072' '#1095#1072#1089#1090#1086#1090#1072
      DisplayWidth = 16
      FieldName = 'TX_FREQUENCY'
      Origin = 'CHANELSRETRANSLATE.TX_FREQUENCY'
    end
    object ibdsChannelCtvTX_CHANNEL: TIBStringField
      DisplayLabel = #1050#1072#1085#1072#1083' '#1074#1080#1093#1110#1076#1085#1080#1081
      DisplayWidth = 16
      FieldName = 'TX_CHANNEL'
      Origin = 'CHANELSRETRANSLATE.TX_CHANNEL'
      Size = 4
    end
    object ibdsChannelCtvNAMEPROGRAMM: TIBStringField
      DisplayLabel = #1055#1088#1086#1075#1088#1072#1084#1072
      DisplayWidth = 16
      FieldName = 'NAMEPROGRAMM'
      Origin = 'CHANELSRETRANSLATE.NAMEPROGRAMM'
      Size = 18
    end
    object ibdsChannelCtvTR_NAME: TIBStringField
      DisplayLabel = #1058#1080#1087' '#1087#1088#1080#1081#1086#1084#1091
      DisplayWidth = 19
      FieldKind = fkLookup
      FieldName = 'TR_NAME'
      LookupDataSet = ibqTypeRec
      LookupKeyFields = 'ID'
      LookupResultField = 'NAME'
      KeyFields = 'TYPERECEIVE_ID'
      Size = 16
      Lookup = True
    end
  end
  object dsChannelCtv: TDataSource [30]
    DataSet = ibdsChannelCtv
    Left = 342
    Top = 370
  end
  object ibqTypeRec: TIBQuery [31]
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SQL.Strings = (
      'select ID, NAME from TYPERECEIVE')
    Left = 266
    Top = 370
  end
  inherited ibqStand: TIBQuery
    Left = 12
  end
end
