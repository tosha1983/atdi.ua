object frmMain: TfrmMain
  Left = 399
  Top = 120
  Width = 635
  Height = 625
  HorzScrollBar.Visible = False
  VertScrollBar.Visible = False
  Caption = 'frmMain'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIForm
  Menu = MainMenu1
  OldCreateOrder = False
  ShowHint = True
  WindowState = wsMaximized
  OnClose = FormClose
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object leftSplitter: TSplitter
    Left = 0
    Top = 0
    Width = 3
    Height = 536
    Cursor = crHSplit
  end
  object rightSplitter: TSplitter
    Left = 624
    Top = 0
    Width = 3
    Height = 536
    Cursor = crHSplit
    Align = alRight
  end
  object StatusBar1: TStatusBar
    Left = 0
    Top = 560
    Width = 627
    Height = 19
    Panels = <
      item
        Width = 100
      end
      item
        Width = 350
      end
      item
        Width = 115
      end
      item
        Width = 100
      end
      item
        Width = 100
      end
      item
        Width = 120
      end
      item
        Width = 50
      end>
    SimplePanel = False
    OnResize = StatusBar1Resize
  end
  object tbrShortcut: TToolBar
    Left = 0
    Top = 536
    Width = 627
    Height = 24
    Align = alBottom
    ButtonHeight = 21
    ButtonWidth = 97
    Caption = 'tbrShortcut'
    Flat = True
    ShowCaptions = True
    TabOrder = 1
    Visible = False
  end
  object pnlLeftDockPanel: TPanel
    Left = 3
    Top = 0
    Width = 0
    Height = 536
    Align = alLeft
    DockSite = True
    TabOrder = 2
    OnDockDrop = DockPanelDockDrop
    OnDockOver = DockPanelDockOver
    OnGetSiteInfo = DockPanelGetSiteInfo
    OnUnDock = DockPanelUnDock
  end
  object pnlRightDockPanel: TPanel
    Left = 624
    Top = 0
    Width = 0
    Height = 536
    Align = alRight
    DockSite = True
    TabOrder = 3
    OnDockDrop = DockPanelDockDrop
    OnDockOver = DockPanelDockOver
    OnGetSiteInfo = DockPanelGetSiteInfo
    OnUnDock = DockPanelUnDock
  end
  object pb: TProgressBar
    Left = 416
    Top = 512
    Width = 193
    Height = 16
    Min = 0
    Max = 100
    Smooth = True
    Step = 1
    TabOrder = 4
    Visible = False
  end
  object MainMenu1: TMainMenu
    Left = 248
    Top = 8
    object miFile: TMenuItem
      Caption = '&'#1060#1072#1081#1083
      object N63: TMenuItem
        Action = actExport
      end
      object N64: TMenuItem
        Action = actImport
      end
      object N65: TMenuItem
        Caption = '-'
      end
      object N2: TMenuItem
        Action = actFileExit
      end
      object actPrint1: TMenuItem
        Action = actPrint
      end
    end
    object miTxDb: TMenuItem
      Caption = '&'#1041#1072#1079#1072
      object N1: TMenuItem
        Caption = '-'
      end
      object N35: TMenuItem
        Action = actNewTx
      end
      object N75: TMenuItem
        Caption = '-'
      end
      object N13: TMenuItem
        Action = actListStand
      end
      object N62: TMenuItem
        Action = actListSynchroNets
      end
      object N30: TMenuItem
        Caption = '-'
      end
      object N73: TMenuItem
        Action = actUserActivityLog
      end
    end
    object miDictDb: TMenuItem
      Caption = '&'#1044#1086#1074#1110#1076#1085#1080#1082#1080
      object N21: TMenuItem
        Action = actListSystemCast
      end
      object actListAnalTeleSyst1: TMenuItem
        Action = actListAnalogTeleSystem
      end
      object actDictListRadioSyst1: TMenuItem
        Action = actListAnalogRadioSystem
      end
      object actDictDigitSyst1: TMenuItem
        Action = actListDigitalSystem
      end
      object N32: TMenuItem
        Action = actListFrequencyGrid
      end
      object actDictChannels1: TMenuItem
        Action = actListChannel
      end
      object N61: TMenuItem
        Action = actListSynchroNetTypes
      end
      object N37: TMenuItem
        Action = actListBlockDAB
      end
      object N11: TMenuItem
        Caption = '-'
      end
      object N22: TMenuItem
        Action = actListCarrierGuardInterval
      end
      object N28: TMenuItem
        Action = actListUncompatibleChannels
      end
      object N25: TMenuItem
        Action = actListMinFieldStrength
      end
      object N12: TMenuItem
        Caption = '-'
      end
      object actDictEquipment1: TMenuItem
        Action = actListEquipment
      end
      object N26: TMenuItem
        Action = actListRadioService
      end
      object N27: TMenuItem
        Action = actListTypeReceive
      end
      object N23: TMenuItem
        Action = actListOffsetCarryFreqTVA
      end
      object N18: TMenuItem
        Tag = 1
        Action = actListAccountCondition
      end
      object N60: TMenuItem
        Action = actListTPOnBorder
      end
      object N72: TMenuItem
        Caption = '-'
      end
      object actListCountries1: TMenuItem
        Action = actListCountry
      end
      object N14: TMenuItem
        Action = actListArea
      end
      object N24: TMenuItem
        Action = actListDistrict
      end
      object N15: TMenuItem
        Action = actListCitiy
      end
      object N34: TMenuItem
        Action = actListTelecomOrganization
      end
      object N31: TMenuItem
        Action = actListOwner
      end
      object actListLicense1: TMenuItem
        Action = actListLicense
      end
      object N33: TMenuItem
        Action = actListBank
      end
    end
    object miRrc06: TMenuItem
      Caption = #1062#1080#1092#1088#1072
      object N76: TMenuItem
        Action = actListDigSubareas
      end
      object N29: TMenuItem
        Action = actListDigAllocations
      end
    end
    object miDocs: TMenuItem
      Caption = #1044#1086#1082#1091#1084#1077#1085#1090#1080
      object miDocuments: TMenuItem
        Action = actDocuments
        Caption = #1055#1086#1090#1088#1077#1073#1091#1102#1090#1100' '#1085#1077#1075#1072#1081#1085#1086#1111' '#1074#1110#1076#1087#1086#1074#1110#1076#1110
      end
    end
    object miView: TMenuItem
      Caption = '&'#1042#1080#1076
      object N66: TMenuItem
        Action = actMap
      end
      object N67: TMenuItem
        Action = actExplorer
      end
      object N6: TMenuItem
        Action = actShowPlanning
      end
    end
    object mniForms: TMenuItem
      Caption = '&'#1042#1110#1082#1085#1072
      OnClick = mniFormsClick
      object N7: TMenuItem
        Action = WindowClose1
      end
      object N8: TMenuItem
        Action = WindowCascade1
      end
      object N9: TMenuItem
        Action = WindowArrange1
      end
      object N70: TMenuItem
        Action = WindowTileHorizontal1
      end
      object N71: TMenuItem
        Action = WindowTileVertical1
      end
      object N10: TMenuItem
        Action = WindowMinimizeAll1
      end
      object mniFormsSeparator: TMenuItem
        Caption = '-'
        OnClick = mniChildWindowClick
      end
    end
    object miTools: TMenuItem
      Caption = #1030#1085#1089#1090#1088#1091#1084#1077#1085#1090#1080
      object N20: TMenuItem
        Action = actCustomizeActionBars
      end
      object N69: TMenuItem
        Action = actParams
      end
      object miMemInfo: TMenuItem
        Action = actMemInfo
      end
      object miSep: TMenuItem
        Caption = '-'
      end
      object miUnloadAll: TMenuItem
        Action = actUnloadAll
      end
      object miUnloadAllForced: TMenuItem
        Action = actUnloadAllForced
      end
    end
    object N3: TMenuItem
      Caption = '&?'
      object N4: TMenuItem
        Action = actAbout
      end
    end
  end
  object ActionList1: TActionList
    Left = 288
    Top = 8
    object actAbout: TAction
      Caption = '&'#1055#1088#1086' '#1087#1088#1086#1075#1088#1072#1084#1091
      OnExecute = actAboutExecute
    end
    object actFileExit: TFileExit
      Category = #1060#1072#1081#1083
      Caption = #1042'&'#1080#1093#1110#1076
      Hint = #1042#1099#1093#1086#1076'|'#1042#1099#1093#1086#1076' '#1080#1079' '#1087#1088#1080#1083#1086#1078#1077#1085#1080#1103
      ImageIndex = 43
    end
    object actListSystemCast: TAction
      Tag = 35
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1080#1089#1090#1077#1084#1080' &'#1084#1086#1074#1083#1077#1085#1085#1103
      OnExecute = actListExecute
    end
    object actListAccountCondition: TAction
      Tag = 1
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1054#1073#1083#1110#1082#1086#1074#1110' &'#1089#1090#1072#1085#1080' '#1087#1077#1088#1077#1076#1072#1090#1095#1080#1082#1110#1074
      OnExecute = actListExecute
    end
    object actListAnalogRadioSystem: TAction
      Tag = 3
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' &'#1088#1072#1076#1110#1086#1084#1086#1074#1083#1077#1085#1085#1103
      OnExecute = actListExecute
    end
    object actListAnalogTeleSystem: TAction
      Tag = 4
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1080#1089#1090#1077#1084#1080' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' &'#1058#1041
      OnExecute = actListExecute
    end
    object actListChannel: TAction
      Tag = 10
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1050#1072#1085#1072#1083#1080' / '#1095#1072#1089#1090#1086#1090#1080
      OnExecute = actListExecute
    end
    object actListEquipment: TAction
      Tag = 20
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1059#1089#1090#1072#1090#1082#1091#1074#1072#1085#1085#1103
      OnExecute = actListExecute
    end
    object actListCountry: TAction
      Tag = 14
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1050#1088#1072#1111#1085#1080
      OnExecute = actListExecute
    end
    object WindowClose1: TWindowClose
      Category = #1054#1082#1085#1086
      Caption = '&'#1047#1072#1082#1088#1080#1090#1080
      Enabled = False
      Hint = 'Close'
    end
    object WindowCascade1: TWindowCascade
      Category = #1054#1082#1085#1086
      Caption = '&'#1050#1072#1089#1082#1072#1076#1086#1084
      Enabled = False
      Hint = 'Cascade'
      ImageIndex = 17
    end
    object WindowMinimizeAll1: TWindowMinimizeAll
      Category = #1054#1082#1085#1086
      Caption = '&'#1047#1074#1077#1088#1085#1091#1090#1080' '#1074#1089#1110
      Enabled = False
      Hint = 'Minimize All'
    end
    object WindowArrange1: TWindowArrange
      Category = #1054#1082#1085#1086
      Caption = '&'#1042#1080#1088#1110#1074#1085#1103#1090#1080
      Enabled = False
    end
    object actListArea: TAction
      Tag = 5
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1056#1077#1075#1110#1086#1085#1080
      OnExecute = actListExecute
    end
    object actListCitiy: TAction
      Tag = 11
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1053#1072#1089#1077#1083#1077#1085#1110' '#1087#1091#1085#1082#1090#1080
      OnExecute = actListExecute
    end
    object actListStreet: TAction
      Tag = 34
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1042#1091#1083#1080#1094#1110
      OnExecute = actListExecute
    end
    object actListDigitalSystem: TAction
      Tag = 17
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1080#1089#1090#1077#1084#1080' &'#1094#1080#1092#1088#1086#1074#1086#1075#1086' '#1058#1041
      OnExecute = actListExecute
    end
    object actListCarrierGuardInterval: TAction
      Tag = 8
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1047#1072#1093#1080#1089#1085#1110' '#1110#1085#1090#1077#1088#1074#1072#1083#1080' '#1062#1058#1041
      OnExecute = actListExecute
    end
    object actListOffsetCarryFreqTVA: TAction
      Tag = 24
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1058#1080#1087#1080' '#1047'&'#1053#1063' '#1072#1085#1072#1083#1086#1075#1086#1074#1086#1075#1086' '#1058#1041
      OnExecute = actListExecute
    end
    object actListDistrict: TAction
      Tag = 18
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1056#1072'&'#1081#1086#1085#1080
      OnExecute = actListExecute
    end
    object actListMinFieldStrength: TAction
      Tag = 23
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1052#1110#1085#1110#1084#1072#1083#1100#1085#1110' '#1085#1072#1087#1088#1091#1078#1077#1085#1085#1086#1089#1090#1110' '#1087#1086#1083#1103
      OnExecute = actListExecute
    end
    object actListRadioService: TAction
      Tag = 30
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1056'&'#1072#1076#1110#1086#1089#1083#1091#1078#1073#1080
      OnExecute = actListExecute
    end
    object actListTypeReceive: TAction
      Tag = 40
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1058#1080#1087#1080' &'#1087#1088#1080#1081#1086#1084#1091
      OnExecute = actListExecute
    end
    object actListUncompatibleChannels: TAction
      Tag = 41
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1053'&'#1077#1089#1091#1084#1110#1089#1085#1110' '#1082#1072#1085#1072#1083#1080' '#1082#1072#1073#1077#1083#1100#1085#1086#1075#1086' '#1058#1041
      OnExecute = actListExecute
    end
    object actListStand: TAction
      Tag = 33
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1054#1087#1086#1088#1080
      OnExecute = actListExecute
    end
    object actListOwner: TAction
      Tag = 25
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1058#1056#1050' / '#1054#1087#1077#1088#1072#1090#1086#1088#1080
      OnExecute = actListExecute
    end
    object actListBank: TAction
      Tag = 6
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1041#1072#1085#1082#1080
      OnExecute = actListExecute
    end
    object actListTelecomOrganization: TAction
      Tag = 36
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1040#1076#1084#1110#1085#1110#1089#1090#1088#1072#1094#1110#1111' '#1079#1074#39#1103#1079#1082#1091
      OnExecute = actListExecute
    end
    object actNewTx: TAction
      Category = #1054#1073#1098#1077#1082#1090#1099
      Caption = '&'#1053#1086#1074#1080#1081' '#1087#1077#1088#1077#1076#1072#1074#1072#1095'...'
      OnExecute = actNewTxExecute
    end
    object actListBlockDAB: TAction
      Tag = 7
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1063#1072#1089#1090#1086#1090#1085#1110' '#1073#1083#1086#1082#1080' '#1062#1056#1052
      OnExecute = actListExecute
    end
    object actListTPOnBorder: TAction
      Tag = 13
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080' '#1085#1072' '#1082#1086#1088#1076#1086#1085#1072#1093
      OnExecute = actListExecute
    end
    object actListLicense: TAction
      Tag = 22
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1051#1110#1094#1077#1085#1079#1110#1111
      OnExecute = actListExecute
    end
    object actListSynchroNetTypes: TAction
      Tag = 44
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1058#1080#1087#1080' '#1089#1110#1085#1093#1088#1086#1085#1085#1080#1093' '#1084#1077#1088#1077#1078
      OnExecute = actListExecute
    end
    object actListSynchroNets: TAction
      Tag = 45
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1110#1085#1093#1088#1086#1085#1085#1110' '#1084#1077#1088#1077#1078#1110
      OnExecute = actListExecute
    end
    object actExplorer: TAction
      Category = #1042#1080#1076
      Caption = '&'#1054#1073#39#1108#1082#1090#1080' '#1072#1085#1072#1083#1110#1079#1091
      ShortCut = 122
      OnExecute = actExplorerExecute
    end
    object actCustomizeActionBars: TCustomizeActionBars
      Category = #1053#1072#1089#1090#1088#1086#1081#1082#1072
      Caption = '&'#1053#1072#1089#1090#1088#1086#1081#1082#1072
    end
    object actMap: TAction
      Category = #1042#1080#1076
      Caption = '&'#1050#1072#1088#1090#1072
      ShortCut = 123
      Visible = False
    end
    object actParams: TAction
      Category = #1053#1072#1089#1090#1088#1086#1081#1082#1072
      Caption = #1055#1072#1088#1072#1084#1077#1090#1088#1080' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1110#1074
      OnExecute = actParamsExecute
    end
    object WindowTileHorizontal1: TWindowTileHorizontal
      Category = #1054#1082#1085#1086
      Caption = #1042#1080#1082#1083#1072#1089#1090#1080' &'#1075#1086#1088#1080#1079#1086#1085#1090#1072#1083#1100#1085#1086
      Enabled = False
      Hint = #1042#1080#1082#1083#1072#1089#1090#1080' '#1075#1086#1088#1080#1079#1086#1085#1090#1072#1083#1100#1085#1086
      ImageIndex = 15
    end
    object WindowTileVertical1: TWindowTileVertical
      Category = #1054#1082#1085#1086
      Caption = '&'#1042#1080#1082#1083#1072#1089#1090#1080' '#1074#1077#1088#1090#1080#1082#1072#1083#1100#1085#1086
      Enabled = False
      Hint = #1042#1080#1082#1083#1072#1089#1090#1080' '#1074#1077#1088#1090#1080#1082#1072#1083#1100#1085#1086
      ImageIndex = 16
    end
    object actUserActivityLog: TAction
      Tag = 46
      Category = #1054#1073#1098#1077#1082#1090#1099
      Caption = '&'#1046#1091#1088#1085#1072#1083' '#1072#1082#1090#1080#1074#1085#1086#1089#1090#1110' '#1082#1086#1088#1080#1089#1090#1091#1074#1072#1095#1110#1074
      Hint = #1046#1091#1088#1085#1072#1083' '#1072#1082#1090#1080#1074#1085#1086#1089#1090#1110' '#1082#1086#1088#1080#1089#1090#1091#1074#1072#1095#1110#1074
      OnExecute = actListExecute
    end
    object actSearch: TAction
      Category = #1054#1073#1098#1077#1082#1090#1099
      Caption = '&'#1055#1086#1096#1091#1082'...'
      Hint = #1055#1086#1096#1091#1082' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      OnExecute = actSearchExecute
    end
    object actExport: TAction
      Category = #1060#1072#1081#1083
      Caption = '&'#1045#1082#1089#1087#1086#1088#1090'...'
    end
    object actImport: TAction
      Category = #1060#1072#1081#1083
      Caption = '&'#1030#1084#1087#1086#1088#1090
    end
    object actPrint: TAction
      Category = #1060#1072#1081#1083
      Caption = '&'#1044#1088#1091#1082
    end
    object actListFrequencyGrid: TAction
      Tag = 47
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = #1057#1110#1090#1082#1080' '#1082#1072#1085#1072#1083#1110#1074' / '#1095#1072#1089#1090#1086#1090
      OnExecute = actListExecute
    end
    object actShowPlanning: TAction
      Category = #1042#1080#1076
      Caption = #1042#1110#1082#1085#1086' '#1087#1083#1072#1085#1091#1074#1072#1085#1085#1103
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1074#1110#1082#1085#1086' '#1087#1083#1072#1085#1091#1074#1072#1085#1085#1103
      OnExecute = actShowPlanningExecute
    end
    object actDocuments: TAction
      Caption = #1044#1086#1082#1091#1084#1077#1085#1090#1080
      OnExecute = actDocumentsExecute
    end
    object actListDigAllocations: TAction
      Tag = 48
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1062#1080#1092#1088#1086#1074#1110' '#1074#1080#1076#1110#1083#1077#1085#1085#1103
      OnExecute = actListExecute
    end
    object actListDigSubareas: TAction
      Tag = 49
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = '&'#1050#1086#1085#1090#1091#1088#1080' '#1079#1086#1085
      OnExecute = actListExecute
    end
    object actImpRrc06: TAction
      Category = #1048#1084#1087#1086#1088#1090
      OnExecute = actImpRrc06Execute
    end
    object actMemInfo: TAction
      Category = 'Debug'
      Caption = #1055#1072#1084#1103#1090#1100'...'
      ShortCut = 49229
      OnExecute = actMemInfoExecute
    end
    object actUnloadAll: TAction
      Category = 'Debug'
      Caption = #1042#1099#1075#1088#1091#1079#1080#1090#1100' '#1087#1077#1088#1077#1076#1072#1090#1095#1080#1082#1080
      ShortCut = 49222
      OnExecute = actUnloadAllExecute
    end
    object actUnloadAllForced: TAction
      Category = 'Debug'
      Caption = #1042#1099#1075#1088#1091#1079#1080#1090#1100' '#1092#1086#1088#1089#1080#1088#1086#1074#1072#1085#1085#1086
      ShortCut = 49222
      OnExecute = actUnloadAllForcedExecute
      OnUpdate = actUnloadAllForcedUpdate
    end
    object actList: TAction
      Category = #1057#1087#1088#1072#1074#1086#1095#1085#1080#1082#1080
      Caption = 'actList'
      OnExecute = actListExecute
    end
    object actTxList: TAction
      OnExecute = actTxListExecute
    end
  end
  object CustomizeDlg1: TCustomizeDlg
    StayOnTop = False
    Left = 352
    Top = 8
  end
  object ApplicationEvents1: TApplicationEvents
    Left = 408
    Top = 8
  end
end
