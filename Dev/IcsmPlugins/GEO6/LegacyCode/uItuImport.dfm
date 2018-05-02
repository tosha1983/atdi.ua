object frmRrc06Import: TfrmRrc06Import
  Left = 533
  Top = 410
  Width = 647
  Height = 436
  Caption = 'ITU '#1080#1084#1087#1086#1088#1090
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnResize = FormResize
  DesignSize = (
    639
    409)
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 16
    Top = 12
    Width = 93
    Height = 13
    Caption = #1060#1072#1081#1083' '#1076#1083#1103' i'#1084#1087#1086#1088#1090#1091':'
  end
  object lblPb: TLabel
    Left = 48
    Top = 366
    Width = 22
    Height = 13
    Anchors = [akLeft, akBottom]
    Caption = 'lblPb'
    Visible = False
  end
  object lblDbSection: TLabel
    Left = 21
    Top = 339
    Width = 53
    Height = 13
    Anchors = [akLeft, akBottom]
    Caption = '&'#1056#1086#1079#1076'i'#1083' '#1041#1044':'
    FocusControl = cbxDbSection
  end
  object lblConstr: TLabel
    Left = 16
    Top = 40
    Width = 62
    Height = 13
    Caption = #1054#1073#1084#1077#1078#1077#1085#1085#1103':'
  end
  object lblConstrContent: TLabel
    Left = 88
    Top = 36
    Width = 489
    Height = 29
    Anchors = [akLeft, akTop, akRight]
    AutoSize = False
    Caption = #1053#1077#1084#1072' '#1086#1073#1084#1077#1078#1077#1085#1100
    WordWrap = True
  end
  object pb: TProgressBar
    Left = 8
    Top = 382
    Width = 449
    Height = 16
    Anchors = [akLeft, akRight, akBottom]
    Min = 0
    Max = 100
    Smooth = True
    Step = 1
    TabOrder = 7
    Visible = False
  end
  object lblError: TMemo
    Left = 8
    Top = 361
    Width = 449
    Height = 46
    Anchors = [akLeft, akRight, akBottom]
    BevelInner = bvNone
    BevelOuter = bvNone
    Ctl3D = True
    ParentColor = True
    ParentCtl3D = False
    TabOrder = 8
    Visible = False
  end
  object edtFileName: TEdit
    Left = 128
    Top = 8
    Width = 446
    Height = 21
    Anchors = [akLeft, akTop, akRight]
    Color = clBtnFace
    ReadOnly = True
    TabOrder = 1
  end
  object btnFile: TButton
    Left = 580
    Top = 8
    Width = 21
    Height = 21
    Anchors = [akTop, akRight]
    Caption = '...'
    TabOrder = 0
    OnClick = btnFileClick
  end
  object GroupBox1: TGroupBox
    Left = 7
    Top = 64
    Width = 623
    Height = 265
    Anchors = [akLeft, akTop, akRight, akBottom]
    Caption = ' '#1059#1084'i'#1089#1090' '
    TabOrder = 4
    object Splitter1: TSplitter
      Left = 2
      Top = 15
      Width = 3
      Height = 248
      Cursor = crHSplit
      Visible = False
    end
    object grdData: TStringGrid
      Left = 5
      Top = 15
      Width = 616
      Height = 248
      Align = alClient
      ColCount = 2
      DefaultRowHeight = 16
      RowCount = 2
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goColSizing, goRowSelect]
      TabOrder = 0
      OnDrawCell = grdDataDrawCell
      OnKeyDown = grdDataKeyDown
      OnKeyUp = grdDataKeyDown
      OnMouseUp = grdDataMouseUp
    end
  end
  object btnCancel: TButton
    Left = 555
    Top = 373
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = #1047#1072#1082#1088#1080#1090#1080
    TabOrder = 5
    OnClick = btnCancelClick
  end
  object btnImport: TButton
    Left = 472
    Top = 373
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'I'#1084#1087#1086#1088#1090
    Enabled = False
    TabOrder = 6
    OnClick = btnImportClick
  end
  object cbxDbSection: TComboBox
    Left = 78
    Top = 335
    Width = 195
    Height = 21
    Style = csDropDownList
    Anchors = [akLeft, akBottom]
    ItemHeight = 13
    TabOrder = 9
    OnChange = cbxDbSectionChange
  end
  object btnConstrSet: TButton
    Left = 580
    Top = 40
    Width = 21
    Height = 21
    Hint = #1042#1089#1090#1072#1085#1086#1074#1080#1090#1080' '#1086#1073#1084#1077#1078#1077#1085#1085#1103
    Anchors = [akTop, akRight]
    Caption = '...'
    TabOrder = 2
    OnClick = btnConstrSetClick
  end
  object btnConstrClear: TButton
    Left = 604
    Top = 40
    Width = 21
    Height = 21
    Hint = #1047#1082#1080#1085#1091#1090#1080' '#1086#1073#1084#1077#1078#1077#1085#1085#1103
    Anchors = [akTop, akRight]
    Caption = #1061
    TabOrder = 3
    OnClick = btnConstrClearClick
  end
  object dsSta: TDataSource
    Left = 16
    Top = 152
  end
  object dsCorr: TDataSource
    Left = 32
    Top = 224
  end
  object dsTempNet: TDataSource
    DataSet = cdsTempNet
    Left = 248
    Top = 231
  end
  object cdsTempNet: TClientDataSet
    Aggregates = <>
    Params = <>
    Left = 213
    Top = 231
  end
  object sqlAssgn: TIBDataSet
    BufferChunks = 1000
    CachedUpdates = False
    InsertSQL.Strings = (
      '  INSERT INTO TRANSMITTERS ('
      '    ID,'
      '    STAND_ID,'
      '    LATITUDE,'
      '    LONGITUDE,'
      '    SYSTEMCAST_ID,'
      '    TYPESYSTEM,'
      '    CHANNEL_ID,'
      '    ALLOTMENTBLOCKDAB_ID,'
      '    VIDEO_CARRIER,'
      '    VIDEO_OFFSET_LINE,'
      '    VIDEO_OFFSET_HERZ,'
      '    SOUND_OFFSET_PRIMARY,'
      '    EPR_VIDEO_MAX,'
      '    EPR_VIDEO_HOR,'
      '    EPR_VIDEO_VERT,'
      '    EPR_SOUND_MAX_PRIMARY,'
      '    EPR_SOUND_HOR_PRIMARY,'
      '    EPR_SOUND_VERT_PRIMARY,'
      '    EFFECTPOWERHOR,'
      '    EFFECTPOWERVER,'
      '    HEIGHTANTENNA,'
      '    HEIGHT_EFF_MAX,'
      '    EFFECTHEIGHT,'
      '    POLARIZATION,'
      '    ANTENNAGAIN,'
      '    DIRECTION,'
      '    ANT_DIAG_H,'
      '    ANT_DIAG_V,'
      '    IDENTIFIERSFN,'
      '    BLOCKCENTREFREQ,'
      '    REMARKS,'
      '    STATUS,'
      '    RPC,'
      '    RX_MODE,'
      '    ADM_ID,'
      '    IS_PUB_REQ,'
      '    ADM_REF_ID,'
      '    PLAN_ENTRY,'
      '    ASSGN_CODE,'
      '    ASSOCIATED_ADM_ALLOT_ID,'
      '    ASSOCIATED_ALLOT_SFN_ID,'
      '    CALL_SIGN,'
      '    D_EXPIRY,'
      '    OP_AGCY,'
      '    ADDR_CODE,'
      '    OP_HH_FR,'
      '    OP_HH_TO,'
      '    IS_RESUB,'
      '    REMARK_CONDS_MET,'
      '    SIGNED_COMMITMENT,'
      '    COORD,'
      '    FREQSTABILITY,'
      '    SYSTEMCOLOUR,'
      '    V_SOUND_RATIO_PRIMARY,'
      '    TYPEOFFSET'
      '    ,SOUND_CARRIER_PRIMARY'
      '    ,CARRIER'
      '    ,BANDWIDTH'
      '    )'
      '  VALUES ('
      '    :ID,'
      '    :STAND_ID,'
      '    :LATITUDE,'
      '    :LONGITUDE,'
      '    :SYSTEMCAST_ID,'
      '    :TYPESYSTEM,'
      '    :CHANNEL_ID,'
      '    :ALLOTMENTBLOCKDAB_ID,'
      '    :VIDEO_CARRIER,'
      '    :VIDEO_OFFSET_LINE,'
      '    :VIDEO_OFFSET_HERZ,'
      '    :SOUND_OFFSET_PRIMARY,'
      '    :EPR_VIDEO_MAX,'
      '    :EPR_VIDEO_HOR,'
      '    :EPR_VIDEO_VERT,'
      '    :EPR_SOUND_MAX_PRIMARY,'
      '    :EPR_SOUND_HOR_PRIMARY,'
      '    :EPR_SOUND_VERT_PRIMARY,'
      '    :EFFECTPOWERHOR,'
      '    :EFFECTPOWERVER,'
      '    :HEIGHTANTENNA,'
      '    :HEIGHT_EFF_MAX,'
      '    :EFFECTHEIGHT,'
      '    :POLARIZATION,'
      '    :ANTENNAGAIN,'
      '    :DIRECTION,'
      '    :ANT_DIAG_H,'
      '    :ANT_DIAG_V,'
      '    :SFN_ID_FK,'
      '    :BLOCKCENTREFREQ,'
      '    :REMARKS,'
      '    :DB_SECTION_ID,'
      '    :RPC,'
      '    :RX_MODE,'
      '    :ADM_ID,'
      '    :IS_PUB_REQ,'
      '    :ADM_REF_ID,'
      '    :PLAN_ENTRY,'
      '    :ASSGN_CODE,'
      '    :ASSOCIATED_ADM_ALLOT_ID,'
      '    :ASSOCIATED_ALLOT_SFN_ID,'
      '    :CALL_SIGN,'
      '    :D_EXPIRY,'
      '    :OP_AGCY,'
      '    :ADDR_CODE,'
      '    :OP_HH_FR,'
      '    :OP_HH_TO,'
      '    :IS_RESUB,'
      '    :REMARK_CONDS_MET,'
      '    :SIGNED_COMMITMENT,'
      '    :COORD,'
      '    :FREQSTABILITY,'
      '    :SYSTEMCOLOUR,'
      '    :V_SOUND_RATIO_PRIMARY,'
      '    :TYPEOFFSET'
      '    , :SOUND_CARRIER_PRIMARY'
      '    ,:CARRIER'
      '    ,:BANDWIDTH'
      '    )')
    ModifySQL.Strings = (
      '  UPDATE TRANSMITTERS'
      '  SET STAND_ID = :STAND_ID,'
      '      LATITUDE = :LATITUDE,'
      '      LONGITUDE = :LONGITUDE,'
      '      SYSTEMCAST_ID = :SYSTEMCAST_ID,'
      '      TYPESYSTEM = :TYPESYSTEM,'
      '      CHANNEL_ID = :CHANNEL_ID,'
      '      ALLOTMENTBLOCKDAB_ID = :ALLOTMENTBLOCKDAB_ID,'
      '      VIDEO_CARRIER = :VIDEO_CARRIER,'
      '      VIDEO_OFFSET_HERZ = :VIDEO_OFFSET_HERZ,'
      '      EPR_VIDEO_MAX = :EPR_VIDEO_MAX,'
      '      EPR_VIDEO_HOR = :EPR_VIDEO_HOR,'
      '      EPR_VIDEO_VERT = :EPR_VIDEO_VERT,'
      '      EFFECTPOWERHOR = :EFFECTPOWERHOR,'
      '      EFFECTPOWERVER = :EFFECTPOWERVER,'
      '      HEIGHTANTENNA = :HEIGHTANTENNA,'
      '      HEIGHT_EFF_MAX = :HEIGHT_EFF_MAX,'
      '      EFFECTHEIGHT = :EFFECTHEIGHT,'
      '      POLARIZATION = :POLARIZATION,'
      '      DIRECTION = :DIRECTION,'
      '      ANT_DIAG_H = :ANT_DIAG_H,'
      '      ANT_DIAG_V = :ANT_DIAG_V,'
      '      IDENTIFIERSFN = :SFN_ID_FK,'
      '      BLOCKCENTREFREQ = :BLOCKCENTREFREQ,'
      '      DATEINTENDUSE = :DATEINTENDUSE,'
      '      REMARKS = :REMARKS,'
      '      RPC = :RPC,'
      '      RX_MODE = :RX_MODE,'
      '      ADM_ID = :ADM_ID,'
      '      IS_PUB_REQ = :IS_PUB_REQ,'
      '      ADM_REF_ID = :ADM_REF_ID,'
      '      PLAN_ENTRY = :PLAN_ENTRY,'
      '      ASSGN_CODE = :ASSGN_CODE,'
      '      ASSOCIATED_ADM_ALLOT_ID = :ASSOCIATED_ADM_ALLOT_ID,'
      '      ASSOCIATED_ALLOT_SFN_ID = :ASSOCIATED_ALLOT_SFN_ID,'
      '      CALL_SIGN = :CALL_SIGN,'
      '      D_EXPIRY = :D_EXPIRY,'
      '      OP_AGCY = :OP_AGCY,'
      '      ADDR_CODE = :ADDR_CODE,'
      '      OP_HH_FR = :OP_HH_FR,'
      '      OP_HH_TO = :OP_HH_TO,'
      '      IS_RESUB = :IS_RESUB,'
      '      REMARK_CONDS_MET = :REMARK_CONDS_MET,'
      '      SIGNED_COMMITMENT = :SIGNED_COMMITMENT,'
      '      COORD = :COORD,'
      '      VIDEO_OFFSET_LINE = :VIDEO_OFFSET_LINE,'
      '      SOUND_OFFSET_PRIMARY = :SOUND_OFFSET_PRIMARY,'
      '      FREQSTABILITY = :FREQSTABILITY,'
      '      SYSTEMCOLOUR = :SYSTEMCOLOUR,'
      '      V_SOUND_RATIO_PRIMARY = :V_SOUND_RATIO_PRIMARY,'
      '      TYPEOFFSET = :TYPEOFFSET'
      '      , SOUND_CARRIER_PRIMARY = :SOUND_CARRIER_PRIMARY'
      '  WHERE (ID = :ID)')
    Left = 440
    Top = 80
  end
end
