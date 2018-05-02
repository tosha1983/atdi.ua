object frmSelection: TfrmSelection
  Left = 234
  Top = 277
  Width = 918
  Height = 648
  ActiveControl = cbxWantedTx
  Caption = #1045#1082#1089#1087#1077#1088#1090#1080#1079#1072
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = False
  Position = poDefault
  ShowHint = True
  Visible = True
  OnClose = FormClose
  OnCloseQuery = FormCloseQuery
  OnResize = FormResize
  OnShow = FormShow
  DesignSize = (
    910
    621)
  PixelsPerInch = 96
  TextHeight = 13
  object pcSelection: TPageControl
    Left = 0
    Top = 0
    Width = 910
    Height = 602
    ActivePage = tshMap
    Align = alClient
    TabIndex = 2
    TabOrder = 0
    OnChange = pcSelectionChange
    object tshSelection: TTabSheet
      Caption = #1042#1080#1073#1110#1088#1082#1072
      DesignSize = (
        902
        574)
      object pb: TProgressBar
        Left = 16
        Top = 312
        Width = 319
        Height = 13
        Anchors = [akLeft, akTop, akRight]
        Min = 0
        Max = 100
        Smooth = True
        Step = 1
        TabOrder = 2
        Visible = False
      end
      object tbrSelection: TToolBar
        Left = 0
        Top = 0
        Width = 902
        Height = 29
        Caption = 'tbrSelection'
        Flat = True
        Images = imlSelection
        TabOrder = 1
        object tbtAdd: TToolButton
          Left = 0
          Top = 0
          Action = actAddTx
        end
        object tbtRemove: TToolButton
          Left = 23
          Top = 0
          Action = actRemoveTx
        end
        object tbtSep1: TToolButton
          Left = 46
          Top = 0
          Width = 8
          Caption = 'tbtSep1'
          ImageIndex = 4
          Style = tbsSeparator
        end
        object tbtDuelInterfere: TToolButton
          Left = 54
          Top = 0
          Action = actDuelInterfere
        end
        object tbtSortFrom: TToolButton
          Left = 77
          Top = 0
          Action = actSortFromUs
        end
        object tbtSortTo: TToolButton
          Left = 100
          Top = 0
          Action = actSortToUs
        end
        object tbtSep2: TToolButton
          Left = 123
          Top = 0
          Width = 8
          Caption = 'tbtSep2'
          ImageIndex = 0
          Style = tbsSeparator
        end
        object tbtFirst20: TToolButton
          Left = 131
          Top = 0
          Action = actSelect20
        end
        object tbtSelectAll: TToolButton
          Left = 154
          Top = 0
          Action = actSelectAll
        end
        object tbtDeselectAll: TToolButton
          Left = 177
          Top = 0
          Action = actDeselectAll
        end
        object tbtRevertSelection: TToolButton
          Left = 200
          Top = 0
          Action = actRevertAll
        end
        object tbtSep5: TToolButton
          Left = 223
          Top = 0
          Width = 8
          Caption = 'tbtSep5'
          ImageIndex = 14
          Style = tbsSeparator
        end
        object tbtDuel: TToolButton
          Left = 231
          Top = 0
          Action = actCalcDuel
        end
        object tbtSaveResTo: TToolButton
          Left = 254
          Top = 0
          Action = actExport
        end
        object ToolButton1: TToolButton
          Left = 277
          Top = 0
          Action = actExportSelectionToExcel
          ImageIndex = 21
        end
        object tbtSep4: TToolButton
          Left = 300
          Top = 0
          Width = 8
          Caption = 'tbtSep4'
          ImageIndex = 13
          Style = tbsSeparator
        end
        object tbtShowTx: TToolButton
          Left = 308
          Top = 0
          Hint = #1050#1072#1088#1090#1082#1072' '#1086#1073#39#1108#1082#1090#1072' '#1077#1082#1089#1087#1077#1088#1090#1080#1079#1080
          Action = actShowTx
          ImageIndex = 20
        end
      end
      inline grid: TLisObjectGrid
        Left = 0
        Top = 29
        Width = 902
        Height = 545
        Align = alClient
        TabOrder = 0
        TabStop = True
        inherited dg: TStringGrid
          Width = 902
          Height = 526
          PopupMenu = pmnSelection
          OnDblClick = griddgDblClick
          OnDragDrop = griddgDragDrop
          OnDragOver = griddgDragOver
          OnDrawCell = griddgDrawCell
          OnKeyDown = griddgKeyDown
          OnKeyUp = griddgKeyUp
          OnMouseUp = griddgMouseUp
        end
        inherited hd: THeaderControl
          Width = 902
        end
      end
    end
    object tshChannels: TTabSheet
      Caption = #1050#1072#1085#1072#1083#1080'/'#1095#1072#1089#1090#1086#1090#1080
      ImageIndex = 2
      object sgrChannelList: TStringGrid
        Left = 0
        Top = 0
        Width = 902
        Height = 574
        Align = alClient
        ColCount = 2
        DefaultRowHeight = 16
        RowCount = 2
        TabOrder = 0
      end
    end
    object tshMap: TTabSheet
      Caption = #1050#1072#1088#1090#1072
      ImageIndex = 3
      object Splitter1: TSplitter
        Left = 652
        Top = 0
        Width = 4
        Height = 574
        Cursor = crHSplit
        Align = alRight
        Beveled = True
        ResizeStyle = rsUpdate
      end
      object panCalcResult: TPanel
        Left = 656
        Top = 0
        Width = 246
        Height = 574
        Align = alRight
        BevelOuter = bvNone
        TabOrder = 0
        OnResize = panCalcResultResize
        object panGraph: TPanel
          Left = 0
          Top = 0
          Width = 246
          Height = 188
          Align = alTop
          ParentColor = True
          TabOrder = 0
          DesignSize = (
            246
            188)
          object cbxWantedTx: TComboBox
            Left = 4
            Top = 164
            Width = 238
            Height = 19
            BevelKind = bkTile
            Style = csOwnerDrawFixed
            Anchors = [akLeft, akRight, akBottom]
            Ctl3D = False
            ItemHeight = 13
            ParentColor = True
            ParentCtl3D = False
            TabOrder = 0
            OnChange = cbxWantedTxChange
            OnDrawItem = cbxWantedTxDrawItem
          end
          object txtNo: TStaticText
            Left = 7
            Top = 144
            Width = 13
            Height = 17
            Anchors = [akLeft, akBottom]
            Caption = '-1'
            TabOrder = 1
          end
          object txtDesc: TStaticText
            Left = 222
            Top = 146
            Width = 15
            Height = 17
            Alignment = taRightJustify
            Anchors = [akRight, akBottom]
            Caption = '-/-'
            TabOrder = 2
          end
        end
        object pcCalcResult: TPageControl
          Left = 0
          Top = 329
          Width = 246
          Height = 245
          ActivePage = tshDuel
          Align = alClient
          TabIndex = 3
          TabOrder = 2
          TabPosition = tpBottom
          OnChange = pcCalcResultChange
          object tshZone: TTabSheet
            Caption = #1047#1086#1085#1072
            DesignSize = (
              238
              219)
            object grdZones: TStringGrid
              Left = 0
              Top = 24
              Width = 238
              Height = 193
              Anchors = [akLeft, akTop, akRight, akBottom]
              ColCount = 4
              Ctl3D = False
              DefaultColWidth = 40
              DefaultRowHeight = 14
              RowCount = 37
              ParentColor = True
              ParentCtl3D = False
              ScrollBars = ssVertical
              TabOrder = 2
              OnDrawCell = grdZonesDrawCell
            end
            object btnGetZones: TButton
              Left = 0
              Top = 0
              Width = 198
              Height = 20
              Action = actGetTxZones
              Anchors = [akLeft, akTop, akRight]
              TabOrder = 0
            end
            object btnDelZones: TButton
              Left = 198
              Top = 0
              Width = 40
              Height = 20
              Hint = #1042#1080#1076#1072#1083#1077#1085#1085#1103' '#1079#1086#1085' '#1079' '#1088#1077#1079#1091#1083#1100#1090#1072#1090#1110#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091
              Anchors = [akTop, akRight]
              Caption = 'X'
              TabOrder = 1
              OnClick = btnDelZonesClick
            end
          end
          object tshPoint: TTabSheet
            Caption = #1058#1086#1095#1082#1072
            ImageIndex = 1
            DesignSize = (
              238
              219)
            object lblPointParam1: TLabel
              Left = 0
              Top = 0
              Width = 60
              Height = 13
              Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1090#1080
            end
            object lblPointParam2: TLabel
              Left = 0
              Top = 24
              Width = 44
              Height = 13
              Caption = #1042#1110#1076#1089#1090#1072#1085#1100
            end
            object lblPointParam3: TLabel
              Left = 120
              Top = 24
              Width = 37
              Height = 13
              Caption = #1040#1079#1080#1084#1091#1090
            end
            object lblPointData1: TLabel
              Left = 72
              Top = 0
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPointData2: TLabel
              Left = 72
              Top = 24
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPointData3: TLabel
              Left = 192
              Top = 24
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPointParam8: TLabel
              Left = 0
              Top = 40
              Width = 26
              Height = 13
              Caption = #1045' '#1084#1110#1085
            end
            object lblPointData8: TLabel
              Left = 72
              Top = 40
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPointParam9: TLabel
              Left = 120
              Top = 40
              Width = 33
              Height = 13
              Caption = #1045' '#1089#1080#1075#1085
            end
            object lblPointData9: TLabel
              Left = 192
              Top = 40
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPoint2Param2: TLabel
              Left = 0
              Top = 76
              Width = 44
              Height = 13
              Caption = #1042#1110#1076#1089#1090#1072#1085#1100
            end
            object lblPoint2Param8: TLabel
              Left = 0
              Top = 92
              Width = 26
              Height = 13
              Caption = #1045' '#1084#1110#1085
            end
            object lblPoint2Data2: TLabel
              Left = 72
              Top = 76
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPoint2Data8: TLabel
              Left = 72
              Top = 92
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPoint2Param3: TLabel
              Left = 120
              Top = 76
              Width = 37
              Height = 13
              Caption = #1040#1079#1080#1084#1091#1090
            end
            object lblPoint2Param9: TLabel
              Left = 120
              Top = 92
              Width = 33
              Height = 13
              Caption = #1045' '#1089#1080#1075#1085
            end
            object lblPoint2Data3: TLabel
              Left = 192
              Top = 76
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPoint2Data9: TLabel
              Left = 192
              Top = 92
              Width = 11
              Height = 13
              Caption = '-/-'
            end
            object lblPoint2Data1: TLabel
              Left = 0
              Top = 60
              Width = 40
              Height = 13
              Caption = #1047#1072#1074#1072#1076#1072':'
            end
            object grdPoint: TStringGrid
              Left = 0
              Top = 137
              Width = 238
              Height = 61
              Anchors = [akLeft, akTop, akRight]
              ColCount = 3
              Ctl3D = False
              DefaultColWidth = 60
              DefaultRowHeight = 14
              RowCount = 4
              ParentColor = True
              ParentCtl3D = False
              ScrollBars = ssVertical
              TabOrder = 2
              OnDrawCell = grdPointDrawCell
              RowHeights = (
                14
                14
                14
                14)
            end
            object chbShowInDetail: TCheckBox
              Left = 120
              Top = 112
              Width = 81
              Height = 17
              Caption = #1055#1086#1076#1088#1086#1073#1080#1094#1110
              TabOrder = 1
              OnClick = chbShowInDetailClick
            end
            object btnCoordinates: TButton
              Left = 4
              Top = 112
              Width = 101
              Height = 20
              Caption = #1059#1082#1072#1079#1072#1090#1080' '#1090#1086#1095#1082#1091
              TabOrder = 0
              OnClick = btnCoordinatesClick
            end
          end
          object tshCoordination: TTabSheet
            Caption = #1050#1086#1086#1088#1076#1080#1085#1072#1094#1110#1103
            ImageIndex = 2
            object panCoordination: TPanel
              Left = 0
              Top = 0
              Width = 238
              Height = 38
              Align = alTop
              BevelOuter = bvNone
              TabOrder = 0
              object lblCoordination: TLabel
                Left = 0
                Top = 0
                Width = 238
                Height = 38
                Align = alClient
                Alignment = taCenter
                AutoSize = False
                Caption = 'lblCoordination'
                WordWrap = True
              end
            end
            object memCountryList: TMemo
              Left = 0
              Top = 38
              Width = 238
              Height = 181
              Align = alClient
              ParentColor = True
              ReadOnly = True
              TabOrder = 1
            end
          end
          object tshDuel: TTabSheet
            Caption = #1044#1091#1077#1083#1100
            ImageIndex = 3
            DesignSize = (
              238
              219)
            object lblAData: TLabel
              Left = 16
              Top = 0
              Width = 222
              Height = 25
              Anchors = [akLeft, akTop, akRight]
              AutoSize = False
              Caption = 'lblAData'
              WordWrap = True
            end
            object lblBData: TLabel
              Left = 16
              Top = 32
              Width = 222
              Height = 25
              Anchors = [akLeft, akTop, akRight]
              AutoSize = False
              Caption = 'lblBData'
              WordWrap = True
            end
            object lblEminA: TLabel
              Left = 0
              Top = 64
              Width = 40
              Height = 13
              Caption = 'lblEminA'
            end
            object lblEminB: TLabel
              Left = 0
              Top = 80
              Width = 40
              Height = 13
              Caption = 'lblEminB'
            end
            object lblEa: TLabel
              Left = 96
              Top = 64
              Width = 23
              Height = 13
              Caption = 'lblEa'
            end
            object lblEb: TLabel
              Left = 96
              Top = 80
              Width = 23
              Height = 13
              Caption = 'lblEb'
            end
            object lblA: TLabel
              Left = 0
              Top = 0
              Width = 10
              Height = 13
              Caption = 'A:'
            end
            object lblB: TLabel
              Left = 0
              Top = 32
              Width = 10
              Height = 13
              Caption = 'B:'
            end
            object lblEminAData: TLabel
              Left = 40
              Top = 64
              Width = 63
              Height = 13
              Caption = 'lblEminAData'
            end
            object lblEminBData: TLabel
              Left = 40
              Top = 80
              Width = 63
              Height = 13
              Caption = 'lblEminBData'
            end
            object grdDuelPoints: TStringGrid
              Left = 0
              Top = 64
              Width = 238
              Height = 155
              Anchors = [akLeft, akTop, akRight, akBottom]
              Ctl3D = False
              DefaultColWidth = 40
              DefaultRowHeight = 16
              Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine]
              ParentColor = True
              ParentCtl3D = False
              ScrollBars = ssVertical
              TabOrder = 0
              OnDrawCell = grdDuelPointsDrawCell
            end
          end
          object tshOffsetPick: TTabSheet
            Caption = #1047#1053#1063
            ImageIndex = 4
            object memOffset: TMemo
              Left = 0
              Top = 0
              Width = 238
              Height = 244
              Align = alClient
              ParentColor = True
              TabOrder = 0
            end
          end
          object tshErpPick: TTabSheet
            Caption = #1045#1042#1055
            ImageIndex = 5
            object memErp: TMemo
              Left = 0
              Top = 0
              Width = 238
              Height = 244
              Align = alClient
              ParentColor = True
              TabOrder = 0
            end
          end
        end
        object panData: TPanel
          Left = 0
          Top = 188
          Width = 246
          Height = 141
          Align = alTop
          TabOrder = 1
          DesignSize = (
            246
            141)
          object lblDataParam1: TLabel
            Left = 4
            Top = 4
            Width = 69
            Height = 13
            Caption = 'lblDataParam1'
          end
          object lblDataParam2: TLabel
            Left = 4
            Top = 20
            Width = 69
            Height = 13
            Caption = 'lblDataParam2'
          end
          object lblDataParam3: TLabel
            Left = 4
            Top = 36
            Width = 69
            Height = 13
            Caption = 'lblDataParam3'
          end
          object lblDataVal1: TLabel
            Left = 56
            Top = 4
            Width = 54
            Height = 13
            Caption = 'lblDataVal1'
          end
          object lblDataVal2: TLabel
            Left = 56
            Top = 20
            Width = 54
            Height = 13
            Caption = 'lblDataVal2'
          end
          object lblDataVal3: TLabel
            Left = 56
            Top = 36
            Width = 54
            Height = 13
            Caption = 'lblDataVal3'
          end
          object lblDataParam4: TLabel
            Left = 112
            Top = 4
            Width = 69
            Height = 13
            Caption = 'lblDataParam4'
          end
          object lblDataParam5: TLabel
            Left = 112
            Top = 20
            Width = 69
            Height = 13
            Caption = 'lblDataParam5'
          end
          object lblDataParam6: TLabel
            Left = 112
            Top = 36
            Width = 69
            Height = 13
            Caption = 'lblDataParam6'
          end
          object lblDataVal4: TLabel
            Left = 176
            Top = 4
            Width = 54
            Height = 13
            Caption = 'lblDataVal4'
          end
          object lblDataVal5: TLabel
            Left = 176
            Top = 20
            Width = 54
            Height = 13
            Caption = 'lblDataVal5'
          end
          object lblDataVal6: TLabel
            Left = 176
            Top = 36
            Width = 54
            Height = 13
            Caption = 'lblDataVal6'
          end
          object lblUnwantedTx: TLabel
            Left = 4
            Top = 55
            Width = 37
            Height = 13
            Caption = #1047#1072#1074#1072#1076#1072
          end
          object lblUnwantedTx2: TLabel
            Left = 4
            Top = 95
            Width = 46
            Height = 13
            Caption = #1047#1072#1074#1072#1076#1072' 2'
            Color = clBtnFace
            ParentColor = False
          end
          object lbUnwantedList: TLabel
            Left = 80
            Top = 120
            Width = 161
            Height = 13
            Anchors = [akLeft, akTop, akRight]
            AutoSize = False
            Caption = '<'#1087#1086#1090#1086#1095#1085#1072'>'
          end
          object cbxUnwantedTx: TComboBox
            Left = 54
            Top = 52
            Width = 189
            Height = 19
            BevelKind = bkTile
            Style = csOwnerDrawFixed
            Anchors = [akLeft, akTop, akRight]
            Ctl3D = False
            ItemHeight = 13
            ParentColor = True
            ParentCtl3D = False
            TabOrder = 0
            OnChange = cbxUnwantedTxChange
            OnDrawItem = cbxWantedTxDrawItem
          end
          object cbxUnwantedTx2: TComboBox
            Left = 54
            Top = 92
            Width = 189
            Height = 19
            BevelKind = bkTile
            Style = csOwnerDrawFixed
            Anchors = [akLeft, akTop, akRight]
            Ctl3D = False
            ItemHeight = 13
            ParentColor = True
            ParentCtl3D = False
            TabOrder = 1
            OnChange = cbxUnwantedTxChange
            OnDrawItem = cbxWantedTxDrawItem
          end
          object chbTwoUnwantedTxs: TCheckBox
            Left = 7
            Top = 75
            Width = 114
            Height = 17
            Caption = #1055#1086#1088#1110#1074#1085#1103#1090#1080' '#1079#1072#1074#1072#1076#1080
            TabOrder = 2
            OnClick = chbTwoUnwantedTxsClick
          end
          object btUnwantedList: TButton
            Left = 4
            Top = 116
            Width = 70
            Height = 21
            Caption = #1047#1072#1074#1072#1076#1080'...'
            TabOrder = 3
            OnClick = btUnwantedListClick
          end
        end
      end
      object panMap: TPanel
        Left = 0
        Top = 0
        Width = 652
        Height = 574
        Align = alClient
        BevelOuter = bvNone
        TabOrder = 1
        object tbrMap: TToolBar
          Left = 0
          Top = 0
          Width = 652
          Height = 29
          ButtonWidth = 24
          Caption = 'ToolBar1'
          Flat = True
          Images = imlMap
          TabOrder = 0
          object tbtClear: TToolButton
            Left = 0
            Top = 0
            Action = actClear
            Grouped = True
          end
          object tbtSep6: TToolButton
            Left = 24
            Top = 0
            Width = 8
            Caption = 'tbtSep1'
            Grouped = True
            ImageIndex = 9
            Style = tbsSeparator
          end
          object tbtNone: TToolButton
            Left = 32
            Top = 0
            Action = actNone
            Grouped = True
            Style = tbsCheck
          end
          object tbtDistance: TToolButton
            Left = 56
            Top = 0
            Action = actDistance
            Grouped = True
            Style = tbsCheck
          end
          object tbtGetRelief: TToolButton
            Left = 80
            Top = 0
            Action = actGetRelief
            Grouped = True
            Style = tbsCheck
          end
          object tbtSector: TToolButton
            Left = 104
            Top = 0
            Action = actCalcCoverSector
            Grouped = True
            Style = tbsCheck
          end
          object tbtPan: TToolButton
            Left = 128
            Top = 0
            Action = actPan
            Grouped = True
            Style = tbsCheck
          end
          object tbtZoomIn: TToolButton
            Left = 152
            Top = 0
            Action = actZoomIn
            Grouped = True
            Style = tbsCheck
          end
          object tbtZoomOut: TToolButton
            Left = 176
            Top = 0
            Action = actZoomOut
            Grouped = True
            Style = tbsCheck
          end
          object tbtZoomFit: TToolButton
            Left = 200
            Top = 0
            Action = actZoomFit
          end
          object tbtHideBtns: TToolButton
            Left = 224
            Top = 0
            Hint = #1057#1087#1088#1103#1090#1072#1090#1100' '#1082#1085#1086#1087#1082#1080' '#1089#1084#1077#1097#1077#1085#1080#1103' '#1082#1072#1088#1090#1099
            ImageIndex = 28
            Style = tbsCheck
          end
          object tbtSep7: TToolButton
            Left = 248
            Top = 0
            Width = 8
            Caption = 'tbtSep2'
            Grouped = True
            ImageIndex = 9
            Style = tbsSeparator
          end
          object tbtLayers: TToolButton
            Left = 256
            Top = 0
            Action = actLayers
            Grouped = True
          end
          object tbtSavePicture: TToolButton
            Left = 280
            Top = 0
            Action = actSaveBmp
          end
          object tbtSep8: TToolButton
            Left = 304
            Top = 0
            Width = 8
            Caption = 'tbtSep4'
            ImageIndex = 7
            Style = tbsSeparator
          end
          object tbtERP: TToolButton
            Left = 312
            Top = 0
            Action = actERP
          end
          object tbtOffset: TToolButton
            Left = 336
            Top = 0
            Action = actOffset
          end
          object tbtnDegradationSectorSelection: TToolButton
            Left = 360
            Top = 0
            Action = actDegradationSectorSelection
          end
          object tbtSep3: TToolButton
            Left = 384
            Top = 0
            Width = 8
            Caption = 'tbtSep3'
            ImageIndex = 6
            Style = tbsSeparator
          end
          object tbtSetTP: TToolButton
            Left = 392
            Top = 0
            Action = actSetTP
          end
          object tbtnCoordinationPointsShow: TToolButton
            Left = 416
            Top = 0
            Hint = #1042#1110#1076#1086#1073#1088#1072#1079#1080#1090#1080' '#1082#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080' '#1085#1072' '#1084#1077#1078#1110
            Action = actCoordinationPointsShow
            ImageIndex = 23
            Style = tbsCheck
          end
          object chbSaveZone: TCheckBox
            Left = 440
            Top = 0
            Width = 107
            Height = 22
            Caption = #1057#1086#1093#1088#1072#1085#1080#1090#1100' '#1079#1086#1085#1091
            TabOrder = 0
            Visible = False
          end
          object pnProjection: TPanel
            Left = 547
            Top = 0
            Width = 94
            Height = 22
            BevelOuter = bvNone
            TabOrder = 1
            object btProjection: TButton
              Left = 4
              Top = 1
              Width = 85
              Height = 20
              Caption = 'Map projection'
              TabOrder = 0
              OnClick = btProjectionClick
            end
          end
        end
        inline cmf: TCustomMapFrame
          Left = 168
          Top = 224
          Width = 384
          Height = 256
          ParentShowHint = False
          ShowHint = True
          TabOrder = 1
          inherited sb: TStatusBar
            Top = 237
            Width = 384
            Panels = <
              item
                Alignment = taCenter
                Text = #1050#1086#1086#1088#1076#1080#1085#1072#1090#1099
                Width = 150
              end
              item
                Alignment = taRightJustify
                Text = #1052#1072#1089#1096#1090#1072#1073
                Width = 50
              end
              item
                Alignment = taRightJustify
                Text = #1042#1099#1089#1086#1090#1072
                Width = 50
              end
              item
                Alignment = taCenter
                Text = #1055#1086#1082#1088#1099#1090#1080#1077
                Width = 90
              end
              item
                Alignment = taCenter
                Text = #1057#1080#1075#1085#1072#1083
                Width = 90
              end
              item
                Alignment = taCenter
                Text = #1055#1086#1084#1077#1093#1072
                Width = 100
              end
              item
                Alignment = taCenter
                Text = #1044#1080#1089#1090#1072#1085#1094#1080#1103
                Width = 100
              end
              item
                Alignment = taCenter
                Text = #1059#1075#1083#1099
                Width = 100
              end
              item
                Width = 50
              end>
          end
          inherited bmf: TBaseMapFrame
            Width = 384
            Height = 208
          end
          inherited tb: TToolBar
            Width = 384
            Images = cmf.bmf.iml
            Visible = False
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
  end
  object StBr: TStatusBar
    Left = 0
    Top = 602
    Width = 910
    Height = 19
    Panels = <
      item
        Width = 80
      end
      item
        Width = 80
      end
      item
        Width = 50
      end
      item
        Width = 50
      end>
    SimplePanel = False
  end
  object pnTime: TPanel
    Left = 288
    Top = 0
    Width = 622
    Height = 20
    Anchors = [akTop, akRight]
    BevelOuter = bvNone
    TabOrder = 2
    object lbUniTime: TLabel
      Left = 0
      Top = 3
      Width = 72
      Height = 13
      Caption = #1045#1076#1080#1085#1086#1077' '#1074#1088#1077#1084#1103
    end
    object dpTime: TDateTimePicker
      Left = 80
      Top = 0
      Width = 137
      Height = 21
      CalAlignment = dtaLeft
      Date = 39659.482925706
      Time = 39659.482925706
      DateFormat = dfLong
      DateMode = dmComboBox
      Kind = dtkDate
      ParseInput = False
      TabOrder = 0
      OnChange = dpTimeChange
    end
    object tpTime: TDateTimePicker
      Left = 224
      Top = 0
      Width = 73
      Height = 21
      CalAlignment = dtaLeft
      Date = 39659.484154919
      Time = 39659.484154919
      DateFormat = dfShort
      DateMode = dmComboBox
      Kind = dtkTime
      ParseInput = False
      TabOrder = 1
      OnChange = tpTimeChange
    end
    object btNow: TButton
      Left = 303
      Top = 1
      Width = 67
      Height = 19
      Hint = #1059#1089#1090#1072#1085#1086#1074#1080#1090#1100' '#1090#1077#1082#1091#1097#1077#1077' '#1074#1088#1077#1084#1103
      Caption = #1057#1077#1081#1095#1072#1089
      TabOrder = 2
      OnClick = btNowClick
    end
    object btNoon: TButton
      Left = 375
      Top = 1
      Width = 67
      Height = 19
      Hint = #1042#1082#1083#1102#1095#1080#1090#1100' '#1057#1086#1083#1085#1094#1077
      Caption = #1055#1086#1083#1076#1077#1085#1100
      TabOrder = 3
      OnClick = btNoonClick
    end
    object btMidnight: TButton
      Left = 447
      Top = 1
      Width = 67
      Height = 19
      Hint = #1042#1099#1082#1083#1102#1095#1080#1090#1100' '#1057#1086#1083#1085#1094#1077
      Caption = #1055#1086#1083#1085#1086#1095#1100
      TabOrder = 4
      OnClick = btMidnightClick
    end
    object btOpMode: TButton
      Left = 519
      Top = 1
      Width = 67
      Height = 19
      Hint = #1055#1077#1088#1077#1082#1083#1102#1095#1080#1090#1100' '#1087#1077#1088#1077#1076#1072#1090#1095#1080#1082#1080' '#1074' '#1089#1086#1086#1090#1074#1077#1090#1089#1090#1074#1091#1102#1097#1080#1081' '#1088#1077#1078#1080#1084' ('#1080#1083#1080' '#1074#1099#1082#1083#1102#1095#1080#1090#1100')'
      Caption = #1056#1077#1078#1080#1084#1099
      TabOrder = 5
      OnClick = btOpModeClick
    end
  end
  object sqlRefresh: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      
        'select TRANSMITTERS_ID, USED_IN_CALC, DISTANCE, SORTINDEX, RESUL' +
        'T '
      'from SELECTEDTRANSMITTERS'
      'where SELECTIONS_ID = :ID and TRANSMITTERS_ID <> :ID'
      'order by SORTINDEX')
    Transaction = dmMain.trMain
    Left = 32
    Top = 104
  end
  object sqlAddTx: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'insert into SELECTEDTRANSMITTERS'
      '(SELECTIONS_ID, TRANSMITTERS_ID, USED_IN_CALC)'
      'values'
      '(:SELECTIONS_ID, :TRANSMITTERS_ID, :USED_IN_CALC)')
    Transaction = dmMain.trMain
    Left = 32
    Top = 168
  end
  object sqlDeleteTx: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'delete from SELECTEDTRANSMITTERS'
      'where SELECTIONS_ID = :SEL_ID'
      'and TRANSMITTERS_ID = :TX_ID')
    Transaction = dmMain.trMain
    Left = 32
    Top = 200
  end
  object alSelection: TActionList
    Images = imlSelection
    Left = 32
    Top = 232
    object actAddTx: TAction
      Caption = #1044#1086#1076#1072#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095
      Hint = #1044#1086#1076#1072#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1091' '#1074#1080#1073#1110#1088#1082#1091
      ImageIndex = 0
      ShortCut = 16449
      OnExecute = actAddTxExecute
    end
    object actRemoveTx: TAction
      Caption = #1042#1080#1076#1072#1083#1080#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095
      Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1079' '#1074#1080#1073#1110#1088#1082#1080
      ImageIndex = 1
      ShortCut = 16452
      OnExecute = actRemoveTxExecute
    end
    object actRemoveAllUnused: TAction
      Caption = #1042#1080#1076#1072#1083#1080#1090#1080' '#1085#1077#1074#1080#1082#1086#1088#1080#1089#1090#1091#1108#1084#1110
      Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1074#1089#1110' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110', '#1097#1086' '#1085#1077' '#1074#1080#1082#1086#1088#1080#1089#1090#1086#1074#1091#1102#1090#1089#1103' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1072#1093
      ShortCut = 49220
      OnExecute = actRemoveAllUnusedExecute
    end
    object actRefresh: TAction
      Caption = #1055#1077#1088#1077#1095#1080#1090#1072#1090#1080
      Hint = #1055#1077#1088#1077#1095#1080#1090#1072#1090#1080' '#1076#1072#1085#1110' '#1079' '#1073#1072#1079#1080
      ShortCut = 116
      OnExecute = actRefreshExecute
    end
    object actUsedInCalc: TAction
      Caption = #1042#1080#1082#1086#1088#1080#1089#1090#1086#1074#1091#1108#1090#1100#1089#1103' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091
      OnExecute = actUsedInCalcExecute
      OnUpdate = actUsedInCalcUpdate
    end
    object actSortFromUs: TAction
      Caption = #1057#1086#1088#1090#1091#1074#1072#1090#1080' '#1087#1086' '#1079#1072#1074#1072#1076#1110' '#1074#1110#1076' '#1085#1072#1089
      Hint = #1057#1086#1088#1090#1091#1074#1072#1090#1080' '#1087#1086' '#1079#1072#1074#1072#1076#1110' '#1074#1110#1076' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      ImageIndex = 3
      ShortCut = 16469
      OnExecute = actSortFromUsExecute
    end
    object actSortToUs: TAction
      Caption = #1057#1086#1088#1090#1091#1074#1072#1090#1080' '#1087#1086' '#1079#1072#1074#1072#1076#1110' '#1085#1072#1084
      Hint = #1057#1086#1088#1090#1091#1074#1072#1090#1080' '#1087#1086' '#1079#1072#1074#1072#1076#1110' '#1073#1072#1078#1072#1085#1086#1084#1091' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1091
      ImageIndex = 4
      ShortCut = 16457
      OnExecute = actSortToUsExecute
    end
    object actEdit: TAction
      Caption = #1050#1072#1088#1090#1086#1095#1082#1072' '#1086#1073#1098#1077#1082#1090#1072
      ShortCut = 16496
      OnExecute = actEditExecute
    end
    object actCalcDuel: TAction
      Caption = #1044#1091#1077#1083#1100
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1076#1091#1077#1083#1100
      ImageIndex = 7
      OnExecute = actCalcDuelExecute
    end
    object actSaveList: TAction
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080' '#1089#1087#1080#1089#1086#1082' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074
      Hint = #1047#1073#1077#1088#1110#1075#1090#1080' '#1089#1087#1080#1089#1086#1082' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074' '#1091' '#1092#1072#1081#1083
      OnExecute = actSaveListExecute
    end
    object actSaveObject: TAction
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080' '#1086#1073#39#1108#1082#1090' '#1072#1085#1072#1083#1110#1079#1091
      Hint = #1047#1073#1077#1088#1110#1075#1090#1080' '#1086#1073#39#1108#1082#1090' '#1072#1085#1072#1083#1110#1079#1091' '#1091' '#1092#1072#1081#1083
      OnExecute = actSaveTxExecute
    end
    object actSaveTx: TAction
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '
      Hint = #1047#1073#1077#1088#1110#1075#1090#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095' '#1091' '#1092#1072#1081#1083
      OnExecute = actSaveTxExecute
    end
    object actSaveRes: TAction
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080' '#1088#1077#1079#1091#1083#1100#1090#1072#1090#1080' '
      Hint = #1047#1073#1077#1088#1110#1075#1090#1080' '#1088#1077#1079#1091#1083#1100#1090#1072#1090#1080' '#1091' '#1092#1072#1081#1083
      OnExecute = actSaveResExecute
    end
    object actDuelInterfere: TAction
      Caption = #1044#1091#1077#1083#1100#1085#1110' '#1079#1072#1074#1072#1076#1080
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1076#1091#1077#1083#1100#1085#1110' '#1079#1072#1074#1072#1076#1080
      ImageIndex = 2
      ShortCut = 16473
      OnExecute = actDuelInterfereExecute
    end
    object actSelectAll: TAction
      Caption = #1042#1080#1073#1088#1072#1090#1080' '#1074#1089#1110
      Hint = #1042#1080#1073#1088#1072#1090#1080' '#1074#1089#1110' '#1079#1072#1074#1072#1076#1080' '#1076#1083#1103' '#1091#1095#1072#1089#1090#1110' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091
      ImageIndex = 15
      OnExecute = actSelectAllExecute
    end
    object actDeselectAll: TAction
      Caption = #1047#1085#1103#1090#1080' '#1074#1080#1073#1110#1088
      Hint = #1047#1085#1103#1090#1080' '#1074#1080#1073#1110#1088' '#1076#1083#1103' '#1091#1095#1072#1089#1090#1110' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091' '#1079#1110' '#1074#1089#1110#1093' '#1079#1072#1074#1072#1076' '
      ImageIndex = 16
      OnExecute = actDeselectAllExecute
    end
    object actRevertAll: TAction
      Caption = #1056#1077#1074#1077#1088#1090#1091#1074#1072#1090#1080' '#1074#1080#1073#1110#1088
      Hint = #1056#1077#1074#1077#1088#1090#1091#1074#1072#1090#1080' '#1074#1080#1073#1110#1088' '#1079#1072#1074#1072#1076' '#1076#1083#1103' '#1091#1095#1072#1089#1090#1110' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091
      ImageIndex = 17
      OnExecute = actRevertAllExecute
    end
    object actSelect20: TAction
      Caption = #1042#1080#1073#1088#1072#1090#1080' '#1087#1077#1088#1096#1110' 20'
      Hint = #1042#1080#1073#1088#1072#1090#1080' 20 '#1087#1077#1088#1096#1080#1093' '#1079#1072#1074#1072#1076' '#1076#1083#1103' '#1091#1095#1072#1089#1090#1110' '#1074' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091
      ImageIndex = 14
      OnExecute = actSelect20Execute
    end
    object actPureCoverage: TAction
      Caption = #1058#1077#1086#1088#1077#1090#1080#1095#1085#1110' '#1079#1086#1085#1080
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1110' '#1087#1086#1082#1072#1079#1072#1090#1080' '#1090#1077#1086#1088#1077#1090#1080#1095#1085#1110' '#1079#1086#1085#1080' '#1086#1073#1089#1083#1091#1075#1086#1074#1091#1074#1072#1085#1085#1103
      ImageIndex = 18
      ShortCut = 16468
      OnExecute = actPureCoverageExecute
    end
    object actGetTxZones: TAction
      Caption = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1079#1086#1085#1080' (F2)'
      Hint = #1056#1086#1079#1088#1072#1093#1091#1085#1086#1082' '#1079#1086#1085' '#1087#1086#1082#1088#1080#1090#1090#1103' "'#1073#1077#1079' '#1085#1072#1089'" '#1090#1072' "'#1079' '#1085#1072#1084#1080'"'
      ShortCut = 113
      OnExecute = actGetTxZonesExecute
    end
    object actRemoveLessThanZero: TAction
      Caption = 'actRemoveLessThanZero'
      OnExecute = actRemoveLessThanZeroExecute
    end
    object actAnalyze: TAction
      Caption = #1045#1082#1089#1087#1077#1088#1090#1080#1079#1072
      Hint = #1047#1072#1085#1077#1089#1090#1080' '#1074' '#1086#1073#39#1108#1082#1090#1080' '#1072#1085#1072#1083#1110#1079#1091' '#1110' '#1079#1088#1086#1073#1080#1090#1080' '#1074#1080#1073#1110#1088#1082#1091
      OnExecute = actAnalyzeExecute
    end
    object actShowTestPoints: TAction
      Caption = #1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110' '#1082#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      OnExecute = actShowTestPointsExecute
    end
    object actExport: TAction
      Caption = #1045#1082#1089#1087#1086#1088#1090
      Hint = #1045#1082#1089#1087#1086#1088#1090' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1110#1074' '#1074' '#1092#1072#1081#1083#1080' '#1092#1086#1088#1084#1072#1090#1110#1074' TVA '#1110' TVD'
      ImageIndex = 12
      OnExecute = actExportExecute
    end
    object actTruncateSelection: TAction
      Caption = #1054#1073#1088#1110#1079#1072#1090#1080' '#1074#1110#1073#1110#1088#1082#1091' '#1079#1072' '#1084#1110#1085#1110#1084#1072#1083#1100#1085#1086#1102' '#1079#1072#1074#1072#1076#1086#1102
      OnExecute = actTruncateSelectionExecute
    end
    object actTruncateSelectionFromCurrentTx: TAction
      Caption = #1054#1073#1088#1110#1079#1072#1090#1080' '#1074#1110#1073#1110#1088#1082#1091' '#1087#1086#1095#1080#1085#1072#1102#1095#1080' '#1079' '#1074#1080#1073#1088#1072#1085#1086#1075#1086
      OnExecute = actTruncateSelectionFromCurrentTxExecute
    end
    object actAllotInterfZone: TAction
      Caption = #1047#1086#1085#1072' '#1087#1086#1084#1077#1093' '#1076#1083#1103' '#1074#1099#1076#1077#1083#1077#1085#1080#1103
    end
    object actSetAsObject: TAction
      Caption = #1042#1080#1073#1088#1072#1090#1080' '#1103#1082' '#1086#1073#39#1108#1082#1090' '#1072#1085#1072#1083'i'#1079#1091
      OnExecute = actSetAsObjectExecute
    end
    object actSetAsInterfere: TAction
      Caption = #1071#1082' '#1079#1072#1074#1072#1076#1091
      OnExecute = actSetAsInterfereExecute
    end
    object actDayNight: TAction
      Caption = #1044#1077#1085#1100
      GroupIndex = 10
      ShortCut = 115
      OnExecute = actDayNightExecute
      OnUpdate = actDayNightUpdate
    end
    object actDropEtalonZones: TAction
      Caption = #1055#1077#1088#1077#1088#1072#1093#1091#1074#1072#1090#1080' '#1101#1090#1072#1083#1086#1085#1085'i '#1079#1086#1085#1080
      OnExecute = actDropEtalonZonesExecute
    end
  end
  object pmnSelection: TPopupMenu
    Images = imlSelection
    Left = 104
    Top = 200
    object N1: TMenuItem
      Action = actAddTx
    end
    object N2: TMenuItem
      Action = actRemoveTx
    end
    object N46: TMenuItem
      Action = actRemoveAllUnused
    end
    object N7: TMenuItem
      Caption = '-'
    end
    object N3: TMenuItem
      Action = actRefresh
    end
    object N8: TMenuItem
      Caption = '-'
    end
    object N32: TMenuItem
      Action = actEdit
    end
    object N4: TMenuItem
      Action = actUsedInCalc
    end
    object N41: TMenuItem
      Caption = '-'
    end
    object N40: TMenuItem
      Action = actDayNight
    end
    object i2: TMenuItem
      Action = actDropEtalonZones
    end
    object N38: TMenuItem
      Caption = '-'
    end
    object N26: TMenuItem
      Action = actDuelInterfere
    end
    object N5: TMenuItem
      Action = actSortFromUs
    end
    object N6: TMenuItem
      Action = actSortToUs
    end
    object N52: TMenuItem
      Action = actPureCoverage
    end
    object N9: TMenuItem
      Caption = '-'
    end
    object N35: TMenuItem
      Caption = #1053#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1074' '#1090#1086#1095#1094#1110
      object N36: TMenuItem
        Caption = #1041#1072#1078#1072#1085#1080#1081' '#1087#1077#1088#1077#1076#1072#1074#1072#1095
        Hint = #1053#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      end
      object N37: TMenuItem
        Caption = #1053#1077#1073#1072#1078#1072#1085#1080#1081' '#1087#1077#1088#1077#1076#1072#1074#1072#1095
        Hint = #1053#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103' '#1085#1077#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      end
      object N39: TMenuItem
        Caption = #1042#1110#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072
        Hint = #1042#1110#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1087#1086#1083#1103' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      end
      object N43: TMenuItem
        Caption = '-'
      end
      object N44: TMenuItem
        Caption = #1044#1091#1077#1083#1100#1085#1072' '#1073#1072#1078#1072#1085#1086#1075#1086
        Hint = #1044#1091#1077#1083#1100#1085#1072' '#1074#1080#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      end
      object N45: TMenuItem
        Caption = #1044#1091#1077#1083#1100#1085#1072' '#1085#1077#1073#1072#1078#1072#1085#1086#1075#1086
        Hint = #1044#1091#1077#1083#1100#1085#1072' '#1074#1080#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1085#1077#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072
      end
      object N50: TMenuItem
        Caption = #1044#1091#1077#1083#1100#1085#1072' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1110#1079' '#1079#1072#1074#1072#1076#1072#1084#1080
        Hint = 
          #1044#1091#1077#1083#1100#1085#1072' '#1074#1080#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072' '#1079' '#1091#1088#1072#1093#1091#1074#1072#1085 +
          #1085#1103#1084' '#1079#1072#1074#1072#1076' '#1074#1080#1073#1110#1088#1082#1080
      end
      object N51: TMenuItem
        Caption = #1044#1091#1077#1083#1100#1085#1072' '#1085#1077#1073#1072#1078#1072#1085#1086#1075#1086' '#1110#1079' '#1079#1072#1074#1072#1076#1072#1084#1080
        Hint = 
          #1044#1091#1077#1083#1100#1085#1072' '#1074#1080#1082#1086#1088#1080#1089#1090#1091#1108#1084#1072' '#1085#1072#1087#1088#1091#1078#1077#1085#1110#1089#1090#1100' '#1085#1077#1073#1072#1078#1072#1085#1086#1075#1086' '#1087#1077#1088#1077#1076#1072#1074#1072#1095#1072' '#1079' '#1091#1088#1072#1093#1091#1074 +
          #1072#1085#1085#1103#1084' '#1079#1072#1074#1072#1076' '#1074#1080#1073#1110#1088#1082#1080
      end
    end
    object N10: TMenuItem
      Caption = #1047#1086#1085#1072' '#1087#1086#1082#1088#1080#1090#1090#1103
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1079#1086#1085#1091' '#1087#1086#1082#1088#1080#1090#1090#1103
      ImageIndex = 5
    end
    object N11: TMenuItem
      Caption = #1047#1086#1085#1072' '#1086#1073#1089#1083#1091#1075#1086#1074#1091#1074#1072#1085#1085#1103
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1079#1086#1085#1091' '#1086#1073#1089#1083#1091#1075#1086#1074#1091#1074#1072#1085#1085#1103
      ImageIndex = 6
    end
    object N12: TMenuItem
      Action = actCalcDuel
    end
    object N49: TMenuItem
      Caption = #1044#1091#1077#1083#1100' '#1074' '#1074#1080#1073#1110#1088#1094#1110
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1076#1091#1077#1083#1100' '#1074' '#1091#1084#1086#1074#1072#1093' '#1079#1072#1074#1072#1076' '#1074#1080#1073#1110#1088#1094#1110
      ImageIndex = 19
    end
    object N14: TMenuItem
      Action = actSetTP
    end
    object N13: TMenuItem
      Action = actCalcCoverSector
    end
    object N25: TMenuItem
      Caption = '-'
    end
    object N15: TMenuItem
      Caption = #1047#1073#1077#1088#1110#1075#1090#1080
      object N29: TMenuItem
        Action = actSaveObject
      end
      object N17: TMenuItem
        Action = actSaveTx
      end
      object N16: TMenuItem
        Action = actSaveList
      end
      object N30: TMenuItem
        Caption = '-'
      end
      object N18: TMenuItem
        Action = actSaveRes
      end
    end
    object N24: TMenuItem
      Caption = #1055#1086#1082#1072#1079#1072#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110' '#1074#1089#1110
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110' '#1074#1089#1110
    end
    object N20: TMenuItem
      Caption = '-'
    end
    object N27: TMenuItem
      Action = actTruncateSelection
    end
    object N28: TMenuItem
      Action = actTruncateSelectionFromCurrentTx
    end
    object N33: TMenuItem
      Caption = '-'
    end
    object N34: TMenuItem
      Caption = #1047#1088#1086#1073#1080#1090#1080' '#1082#1086#1087#1110#1102' '#1091' '#1055#1077#1088#1077#1076#1041#1072#1079#1110
      OnClick = N34Click
    end
  end
  object sqlUpdateUsed: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'update SELECTEDTRANSMITTERS'
      'set USED_IN_CALC = :USED_IN_CALC'
      'where SELECTIONS_ID = :SELECTIONS_ID'
      'and TRANSMITTERS_ID = :TRANSMITTERS_ID')
    Transaction = dmMain.trMain
    Left = 32
    Top = 136
  end
  object SaveDialog1: TSaveDialog
    Options = [ofOverwritePrompt, ofHideReadOnly, ofEnableSizing]
    Left = 72
    Top = 200
  end
  object sqlTxId: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'select TRANSMITTERS_ID  from SELECTIONS'
      'where ID = :ID')
    Transaction = dmMain.trMain
    Left = 112
    Top = 104
  end
  object imlSelection: TImageList
    Left = 208
    Top = 136
    Bitmap = {
      494C010116001800040010001000FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000400000006000000001002000000000000060
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000084000084848400848484000084000000840000848484008484
      8400008400000084000084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484008484840084848400848484008484840084848400848484008484
      840084848400000000000000000000000000AD7B7B00BD848400BD848400BD84
      8400BD8484000084000084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484008484840084848400000000000000000000000000000000000000
      000000000000000000000000000000000000AD847B00FFEFCE00FFE7C600F7DE
      B500F7DEAD008484840000840000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400848484000000
      000000000000000000000000000000000000AD847B00FFEFCE00F7E7CE00F7DE
      BD00F7DEB5008484840000840000FFFFFF000084000000840000FFFFFF000084
      000000840000FFFFFF00FFFFFF00008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000C6C6C600C6C6C6008484
      840000000000000000000000000000000000AD847B00FFEFD600F7E7CE00F7DE
      C600F7DEBD000084000084848400FFFFFF00FFFFFF0000840000008400000084
      000000840000FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF008484
      840084848400FFFFFF0000000000C6C6C600C6C6C6000000000000000000C6C6
      C60084848400000000000000000000000000AD847B00FFF7E700FFEFDE00F7E7
      CE00F7E7CE000084000084848400FFFFFF00FFFFFF0000840000008400000084
      0000FFFFFF00FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000848484008484
      840084848400848484008484840000000000C6C6C600C6C6C600848484000000
      0000C6C6C600000000000000000000000000B5848400FFF7EF00FFEFDE00F7E7
      D600F7E7CE008484840000840000FFFFFF00FFFFFF0000840000008400000084
      000000840000FFFFFF00FFFFFF00008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF008484840084848400848484008484840000000000C6C6C600C6C6C6008484
      840000000000000000000000000000000000BD8C8400FFFFF700FFF7EF00FFEF
      DE00FFEFDE0084848400008400000084000000840000FFFFFF00FFFFFF00FFFF
      FF000084000000840000FFFFFF00008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484008484840000000000C6C6C6008484
      840000000000000000000000000000000000BD948400FFFFFF00FFF7F700FFF7
      E700FFEFDE000084000084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000084848400848484008484840084848400848484000000000000000000C6C6
      C60000000000000000000000000000000000C69C8C00FFFFFF00FFFFFF00FFF7
      F700FFF7EF000084000084848400848484000084000000840000848484008484
      8400008400000084000084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008484840084848400848484000000000084848400848484000000
      000084848400000000000000000000000000CE9C8C00FFFFFF00FFFFFF00FFFF
      F700FFF7F7008484840000840000008400008484840084848400008400000084
      0000848484008484840000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000848484008484840084848400FFFF
      FF0084848400000000000000000000000000D6A58C00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFF7F700FFF7EF00FFEFDE00FFF7E700DED6C600C6BDAD009C8C
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484008484840084848400848484008484
      840000000000000000000000000000000000D6AD8C00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFF700FFF7EF00FFF7E700DEBDAD00AD8C8400AD848400AD84
      7B00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000008484840084848400848484008484
      8400FFFFFF00000000000000000000000000DEAD9400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00F7EFEF00C69C9400F7C67B00FFBD4A000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000084848400FFFFFF008484
      840084848400000000000000000000000000DEAD9400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00F7F7EF00C69C9400FFCE7300D69C73000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000FF000000
      00000000FF000000000000000000000000000000000000000000000000008484
      840084848400000000000000000000000000DEA58400DEA58400DEA58400DEA5
      8400DEA58400DEA58400DEA58400DEAD8400C6947B0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000042000000420000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000042
      0000004200000000000000000000004200000042000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000084000000000000000000000000000000008400000000
      0000000000000084000000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084848400000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000008484840000000000000000000000000000000000004200000000
      0000000000000000000000000000000000000000000000420000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000840000008400000000000000008400000000
      0000000000000084000000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084848400848484000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000008484840084848400000000000000000000000000004200000000
      0000000000000000000000000000000000000000000000420000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000084000000000000000084
      0000008400000000000000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000084848400000000000000000000420000000000000000
      0000000000000000000000000000000000000000000000000000004200000000
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000840000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484000000
      0000000000000000000084848400000000008484840000000000848484000000
      0000000000000000000084848400000000000000000000420000000000000000
      0000000000000000000000000000000000000000000000000000004200000000
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000008400840000000000
      0000000000000084000000000000008400000000000000000000848484000000
      0000848484008484840084848400848484008484840084848400848484008484
      8400848484008484840000000000848484000000000000000000848484000000
      0000000000008484840000000000000000000000000084848400848484000000
      0000000000000000000084848400000000000000000000000000004200000000
      0000000000000000000000000000000000000000000000420000000000000000
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000084000000840000000000840000000000
      8400000084000000000000840000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008484840000000000848484000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000084848400000000000000000000000000004200000000
      0000000000000000000000000000000000000000000000420000000000000042
      0000004200000042000000000000000000000000000000000000840000000000
      0000000000000000000000008400000000000000000084000000000000000000
      0000000000000000840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484000000
      0000000000008484840000000000000000000000000084848400000000000000
      0000000000000000000084848400000000000000000000000000000000000042
      0000004200000000000000000000004200000042000000000000004200000000
      0000000000000000000000420000000000000000000000000000840000000000
      0000000000000000000000008400000000000000000084000000000000000000
      0000000000000000840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484000000
      0000000000000000000084848400000000008484840000000000848484000000
      0000000000000000000084848400000000000000000000000000000000000000
      0000000000000042000000420000000000000000000000420000000000000000
      0000000000000000000000000000004200000000000000000000000000008400
      0000840000000000840000000000840000008400000000000000000000000000
      0000000000000000000000008400000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000084848400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000420000000000000000
      0000000000000000000000000000004200000000000000000000000000000000
      0000000000000000840084000000000000000000000000000000000000000000
      0000000000000000000000008400000000000000000000000000848484008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000848484008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084848400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000420000000000000000
      0000000000000000000000000000004200000000000000000000000000000000
      0000000000000000000000008400000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084848400000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000008484840000000000000000000000000000000000000000000000
      0000000000000000000000420000004200000000000000000000004200000000
      0000000000000000000000420000000000000000000000000000000000000084
      0000008400000000000000008400000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000042000000000000000000000042000000000000000000000042
      0000004200000042000000000000000000000000000000000000008400000000
      0000000000000084000000000000000084000000840000000000000000000000
      8400000084000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000042000000000000000000000042000000000000000000000000
      0000000000000000000000000000000000000000000000000000008400000000
      0000000000000084000000000000000000000000000000008400000084000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000420000004200000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      00007B7B7B007B7B7B00000000000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000BDBDBD000000
      0000BDBDBD00BDBDBD00000000000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000000000000000000000000000000000
      0000000000008484840000000000000000008484840000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000BDBDBD00BDBD
      BD00BDBDBD00BDBDBD0000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000008484840000000000000000000000
      0000848484000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000000000008484840084848400000000000000
      0000000000000000000084848400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000000000000000000000848484000000
      0000848484000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000084848400000000000000
      0000000000000000000084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      FF00000000000000000000000000000000000000000000000000FF0000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000008484840000000000000000000000
      0000848484000000000000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000008484840000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      00000000FF00000000000000FF000000FF0000000000FF00000000000000FF00
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008484840000000000000000008484840000000000000000008484
      8400000000000000000000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000008484840000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000007B7B7B007B7B7B00000000007B7B
      7B000000FF007B7B7B000000FF007B7B7B000000FF007B7B7B007B7B7B00FF00
      00007B7B7B007B7B7B007B7B7B007B7B7B000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000848484008484
      8400848484000000000000000000000000000000000000000000848484000000
      0000848484008484840084848400000000000000000084848400848484008484
      84008484840084848400000000008484840000000000FFFFFF00000000000000
      0000FFFFFF000000000000000000BDBDBD0000000000FF000000FF000000FF00
      00000000FF00FF000000FF000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000FF0000000000000000000000
      0000FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484008484840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000084848400000000008484840000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      FF000000FF000000FF000000000000000000000000007B7B7B0000000000FF00
      0000000000000000000000000000FF000000000000000000FF00000000000000
      00000000FF000000000000000000000000000000000000000000000000000000
      0000848484000000000000000000000000000000000084848400000000000000
      0000000000008484840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000008484840000000000FFFFFF00000000000000
      00000000000000000000FFFFFF0000000000FFFFFF00000000000000FF000000
      FF000000FF000000FF000000FF00000000000000000000000000000000000000
      0000FF00000000000000FF00000000000000000000000000FF00000000000000
      FF00FF0000000000FF0000000000000000000000000000000000000000000000
      0000000000008484840000000000000000000000000084848400000000000000
      0000000000008484840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000008484840000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000FF000000FF000000
      FF000000FF000000FF000000FF000000FF007B7B7B007B7B7B00000000007B7B
      7B00FF0000007B7B7B00FF0000007B7B7B007B7B7B007B7B7B000000FF007B7B
      7B007B7B7B00FF0000000000FF007B7B7B000000000000000000000000000000
      0000000000000000000084848400000000000000000084848400000000000000
      0000000000008484840000000000000000000000000000000000848484000000
      0000000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000008484840000000000FFFFFF00000000000000
      0000FFFFFF000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      000000000000FF00000000000000000000000000000000000000000000000000
      000000000000FF00000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000084848400000000000000
      0000000000008484840000000000000000000000000000000000848484008484
      8400000000000000000000000000000000000000000084848400000000000000
      00000000000000000000000000008484840000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00FFFFFF000000000000000000000000000000
      FF000000FF000000FF000000000000000000000000007B7B7B00000000000000
      000000000000FF00000000000000000000000000000000000000000000000000
      00000000000000000000FF000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000084848400000000000000
      0000000000008484840000000000000000000000000000000000000000008484
      8400000000000000000000000000000000000000000000000000000000000000
      00000000000000000000848484000000000000000000FFFFFF0000000000BDBD
      BD00FFFFFF0000000000FFFFFF000000000000000000000000007B7B7B000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840000000000848484000000000084848400848484008484
      8400000000008484840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF000000000000000000000000000000FF000000FF000000FF000000
      FF000000FF000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B000000000000000000000000000000
      0000000000000000000084848400000000000000000000000000000000000000
      0000848484000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000000000000000000000000000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      000084000000000000000000FF000000FF000000FF0000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      FF000000FF0000000000000000000000000000000000FF000000000000000000
      000000000000000000000000FF000000FF00000000007B7B7B00000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000FF000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B00FF000000FF000000FF0000007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B00000000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF00000084000000FF00000084
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000000000000000FF000000000000000000
      00000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF00FF000000000000000000000000000000FF00
      00000000FF0000000000000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000FF0000000000FF000000000000000000
      FF0000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000FF0000000000FF000000000000000000000000000000
      FF00FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF000000FF000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FF000000000000000000FF000000FF000000FF000000
      0000FF0000000000000000000000000000008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B00FF0000007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B00FF0000007B7B7B007B7B7B007B7B7B008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000000000000000000000000000FF00
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      000000000000000000000000000000000000FF000000FF000000FF0000000000
      0000000000000000000000000000000000000000000084000000000000008484
      84000000FF008484840000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B000000000000000000000000008484
      84000000FF008484840000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000084000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000840000008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000000000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000008400
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF00840000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000FF0000000000000000000000
      00000000FF008400000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000840000000000
      000000000000000000000000FF000000FF000000000000000000000000000000
      00000000FF008400000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000008400840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000084000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      00000000FF000000000084000000000000000000000084000000000000000000
      0000000000000000000000000000000084000000840000000000840000000000
      8400000084000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000084000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF0084000000000000000000000000000000840000000000
      0000000000000000000000008400000000000000000084000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000084000000000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      0000000000000000FF0084000000000000000000000000000000840000000000
      0000000000000000000000008400000000000000000084000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000084000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000084000000000000000000000000000000000000008400
      0000840000000000840000000000840000008400000000000000000000000000
      0000000000000000000000008400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084848400848484000000
      8400848484008484840000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000840000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      00000000FF008400000000000000000000000000000000000000000000000000
      0000000000000000840084000000000000000000000000000000000000000000
      0000000000000000000000008400000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000008400000084000000
      8400000084000000840000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000840000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      00000000FF008400000000000000000000000000000000000000000000000000
      0000000000000000000000008400000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000084000000
      8400000084000000000000000000000000000000000000000000000000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000008400
      00000000FF000000FF000000FF0000000000000000000000FF000000FF000000
      FF00840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000008400000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000084000000
      8400000084000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000084000000000000000000FF000000FF0000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000084000000840000000000000000000000
      8400000084000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000008400000084000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000000000FF
      000000FF00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000000000000000000000000000000000000000000000000000000FF
      000000FF00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400840000008484840000000000000000000000000000FF000000FF000000FF
      000000FF000000FF000000FF0000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000008400000000000000000000000000000000FF000000FF000000FF
      000000FF000000FF000000FF0000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000084000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000848484008400
      00008400000084000000848484000000000000000000000000000000000000FF
      000000FF00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000008400000084000000
      8400000084000000840000008400000084000000840000008400000084000000
      8400000084000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      00008400000084000000840000000000000000000000000000000000000000FF
      000000FF00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      8400000084000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000840000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF00
      0000FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000000000000000000000000000000000000FF000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF0000000000
      000000FF000000FF0000000000000000000000000000FF000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF0000000000
      00000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000000000000000000000000000000000000FF000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF0000000000
      000000FF000000FF0000000000000000000000000000FF000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF0000000000
      00000000FF000000FF0000000000000000000000000000000000840000008400
      0000840000008400000084000000840000008400000084000000840000008400
      0000840000008400000084000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF00
      0000FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000084000000000000000000000000000000424D3E000000000000003E000000
      2800000040000000600000000100010000000000000300000000000000000000
      000000000000000000000000FFFFFF00FFFFF80000000000E003000000000000
      F007000000000000FF1F000000000000E08F000000000000C007000000000000
      C003000000000000E007000000000000E007000000000000F007000000000000
      F807000000000000FC03000F00000000F003000F00000000F303001F00000000
      EF83001F00000000D7E7007F00000000FFFFFFFFF9FFFFE7FFFFFFFFE67FF9DB
      EFFDEFFBDFBFE65BCFF8CFF1DFBFDFA79FFC9E79BFDFDFBD9FFC9819BFDFBF9A
      90009819DFBFBE4580009009DFA3DDBB80049839E65DDDBB9FFC9819F9BEE27D
      9FFC9E79FFBEF9FD8FFC8FF9FFBEFDFBCFF9CFF3FCDDE5FBFFFFFFFFFB63DA67
      FFFFFFFFFB7FDB9FFFFFFFFFFCFFE7FFFF00DDDDFFFFFFFFFF00D555F24FFFFF
      FF000000E667EF3DFF00DFFFE007CE3800008FDFE6679E3C0000D4AFF24F9E3C
      00000000E0C790000000DB77C183800000238EB7E73380040001D5A3F3339E3C
      00000000F9339E3C0023DBFBFC338E3C00639BFDEC33CE7900C3DFFFC003FFFF
      01070000E187FFFF03FFFFFFFFFFFFFFFFBFDDDDDDDDFC7FFE1FD555D555F83F
      F81F00000000E011F01FDFBFDFFFD820F01F87BC87FC8C60F80FDBBBDBFB07E0
      F80F0000000007F1F80FDDB7DCE707FDF80F9EAF9CE78FFDF80FDF1FDD177FF1
      F01F000000007FE0F01FDFBFDEEFA3E0F81F9FBF9F1F81E0FE1FDFBFDFFFC1F1
      FFBF00000000C1CFFFFFFFFFFFFFE23FFFFFFFFFFFFFFFFFE3EFFC3FFC3FF9FF
      FDEFF3CFF3CFE67FF1EFEFF7EF87DFBFEDEFDFFBDF73DFBFEDEFDFFBDCF3BF9F
      EDEFBFFDBBF5BE47F3EFBFFDBBF9DDBBFFEFBFFDA7F9DDBBF3EFBFFDAFF5E27D
      ED83DFFBCFF3F9FDED83DFFBCFF3FDFBEDC7EFF7E187FDFBEDC7F3CFF24FFE67
      EDEFFC3FFC3FFF9FF3EFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE7F8FFF8FFFFF1F7
      E7F8FFF8FFFFFEE381FF81FFFBFFF8E381FC81FCE3FFF6C1E7FCFFFC8003F6C1
      E7FFFFFFE3FFF6F7FFFCFFFCFBFFF9F7FEFCF7FCFFFFFFF7FE7FE7FFFFDFF9F7
      80138013FFC7F6F780138013C001F6F7FE7FE7FFFFC7F6F7FEF8F7F8FFDFF6F7
      FFF8FFF8FFFFF6F7FFFFFFFFFFFFF9F700000000000000000000000000000000
      000000000000}
  end
  object pmnSave: TPopupMenu
    Left = 136
    Top = 200
    object N19: TMenuItem
      Action = actSaveTx
    end
    object N22: TMenuItem
      Action = actSaveList
    end
  end
  object dstResults: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    InsertSQL.Strings = (
      '')
    RefreshSQL.Strings = (
      'Select '
      '  ID,'
      '  TRANSMITTERS_ID,'
      '  NAMEQUERIES,'
      '  RESULT,'
      '  CREATEDATE,'
      '  CHANGEDATE,'
      '  NAMECALCMODEL,'
      '  USERID,'
      '  TYPERESULT,'
      '  RX_ANT_HEIGHT,'
      '  FREQUENCY'
      'from SELECTIONS '
      'where'
      '  ID = :ID')
    SelectSQL.Strings = (
      'select ID, RESULT from SELECTIONS'
      'where ID = :ID')
    ModifySQL.Strings = (
      'update SELECTIONS'
      'set'
      '  ID = :ID,'
      '  RESULT = :RESULT'
      'where'
      '  ID = :OLD_ID')
    Left = 32
    Top = 304
    object dstResultsID: TIntegerField
      FieldName = 'ID'
      Origin = 'SELECTIONS.ID'
      Required = True
    end
    object dstResultsRESULT: TBlobField
      FieldName = 'RESULT'
      Origin = 'SELECTIONS.RESULT'
      Size = 8
    end
  end
  object pmnResults: TPopupMenu
    Left = 168
    Top = 200
    object N21: TMenuItem
      Caption = #1042#1080#1076#1072#1083#1080#1090#1080' '#1089#1090#1086#1088#1110#1085#1082#1091
      Hint = #1042#1080#1076#1072#1083#1080#1090#1080' '#1089#1090#1086#1088#1110#1085#1082#1091' '#1079' '#1088#1077#1079#1091#1083#1100#1090#1072#1090#1072#1084#1080
      ShortCut = 16452
    end
    object N23: TMenuItem
      Caption = #1055#1086#1082#1072#1079#1072#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110
    end
    object N31: TMenuItem
      Action = actSaveRes
    end
  end
  object sqlChannelList: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'SELECT CH.NAMECHANNEL, CH.FREQCARRIERVISION, count(*)'
      'FROM SELECTEDTRANSMITTERS ST '
      
        '   LEFT OUTER JOIN TRANSMITTERS TX ON (ST.TRANSMITTERS_ID = TX.I' +
        'D)'
      '   LEFT OUTER JOIN SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)'
      '   LEFT OUTER JOIN CHANNELS CH ON  (TX.CHANNEL_ID = CH.ID)'
      'WHERE ST.SELECTIONS_ID = :SEL_ID'
      'and  SC.ENUMVAL = :SCAST_ID'
      'group by CH.NAMECHANNEL, CH.FREQCARRIERVISION')
    Transaction = dmMain.trMain
    Left = 152
    Top = 104
  end
  object sqlTxNameList: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    Transaction = dmMain.trMain
    Left = 32
    Top = 264
  end
  object sqlUpdateSort: TIBSQL
    Database = dmMain.dbMain
    ParamCheck = True
    SQL.Strings = (
      'update SELECTEDTRANSMITTERS'
      'set SORTINDEX = :SORTINDEX'
      'where SELECTIONS_ID = :SELECTIONS_ID'
      'and TRANSMITTERS_ID = :TRANSMITTERS_ID')
    Transaction = dmMain.trMain
    Left = 72
    Top = 136
  end
  object ibdsRefresh: TIBDataSet
    Database = dmMain.dbMain
    Transaction = dmMain.trMain
    BufferChunks = 1000
    CachedUpdates = False
    SelectSQL.Strings = (
      
        'select  s.AZIMUTH, s.DISTANCE, s.TRANSMITTERS_ID, s.USED_IN_CALC' +
        ', s.DISTANCE, s.SORTINDEX, s.RESULT, s.E_UNWANT, s.E_WANT, s.ZON' +
        'E_OVERLAPPING, c.ENUMVAL'
      'from SELECTEDTRANSMITTERS s'
      'left outer join Transmitters t on (s.TRANSMITTERS_ID = t.ID)'
      'left outer join Systemcast c on (t.SYSTEMCAST_ID = c.ID)'
      'where s.SELECTIONS_ID = :ID and s.TRANSMITTERS_ID <> :ID'
      'order by SORTINDEX')
    Left = 256
    Top = 104
    object ibdsRefreshTRANSMITTERS_ID: TIntegerField
      FieldName = 'TRANSMITTERS_ID'
      Origin = 'SELECTEDTRANSMITTERS.TRANSMITTERS_ID'
      Required = True
    end
    object ibdsRefreshUSED_IN_CALC: TSmallintField
      FieldName = 'USED_IN_CALC'
      Origin = 'SELECTEDTRANSMITTERS.USED_IN_CALC'
    end
    object ibdsRefreshDISTANCE: TFloatField
      FieldName = 'DISTANCE'
      Origin = 'SELECTEDTRANSMITTERS.DISTANCE'
    end
    object ibdsRefreshSORTINDEX: TIntegerField
      FieldName = 'SORTINDEX'
      Origin = 'SELECTEDTRANSMITTERS.SORTINDEX'
    end
    object ibdsRefreshRESULT: TBlobField
      FieldName = 'RESULT'
      Origin = 'SELECTEDTRANSMITTERS.RESULT'
      Size = 8
    end
    object ibdsRefreshE_UNWANT: TFloatField
      FieldName = 'E_UNWANT'
      Origin = 'SELECTEDTRANSMITTERS.E_UNWANT'
    end
    object ibdsRefreshE_WANT: TFloatField
      FieldName = 'E_WANT'
      Origin = 'SELECTEDTRANSMITTERS.E_WANT'
    end
    object ibdsRefreshZONE_OVERLAPPING: TSmallintField
      FieldName = 'ZONE_OVERLAPPING'
      Origin = 'SELECTEDTRANSMITTERS.ZONE_OVERLAPPING'
    end
    object ibdsRefreshAZIMUTH: TFloatField
      FieldName = 'AZIMUTH'
      Origin = 'SELECTEDTRANSMITTERS.AZIMUTH'
    end
    object ibdsRefreshENUMVAL: TIntegerField
      FieldName = 'ENUMVAL'
    end
  end
  object imlMap: TImageList
    Left = 544
    Top = 51
    Bitmap = {
      494C01011D002200040010001000FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000400000009000000001002000000000000090
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000008400000084000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000084000000000000000000FF000000000000000000000000000000
      0000840000008400000084000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000084000000840000000000FF000000FF000000FF00840000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000084000000840000000000FF000000FF0084000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF000000FF000000
      FF000000FF000000FF00840000008400000084000000840000000000FF000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000008400000084000000840000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00008400000084000000840000000000FF008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      00008400000084000000000000000000FF008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084000000840000008400
      0000000000000000000000000000000000000000000084000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008400000084000000840000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000084848400000000008484
      8400000000008484840000000000848484000000000084848400000000008484
      8400000000008484840000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000084000084848400848484000084000000840000848484008484
      8400008400000084000084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000AD7B7B00BD848400BD848400BD84
      8400BD8484000084000084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0084848400944A2100944A2100944A2100944A
      2100944A2100944A2100944A2100944A2100944A2100944A2100944A2100944A
      2100944A2100944A2100944A2100944A21000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000AD847B00FFEFCE00FFE7C600F7DE
      B500F7DEAD008484840000840000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000084000084422100FFF7EF00FFF7E700FFF7
      E700FFEFDE00FFEFD600DEC6B500CEBDA500CEB59C00D6BDA500F7D6B500FFDE
      B500FFDEB500FFDEAD00FFD6AD008C4A21008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000AD847B00FFEFCE00F7E7CE00F7DE
      BD00F7DEB5008484840000840000FFFFFF000084000000840000FFFFFF000084
      000000840000FFFFFF00FFFFFF000084000084422100FFFFF700FFF7EF00FFF7
      E700D6C6C6008C6B63007B5A5200845A5200845A52008C6B630094846B00CEAD
      9400FFDEB500FFDEAD00FFDEAD008C4A21000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000AD847B00FFEFD600F7E7CE00F7DE
      C600F7DEBD000084000084848400FFFFFF00FFFFFF0000840000008400000084
      000000840000FFFFFF00FFFFFF008484840084422100FFFFF700FFF7EF00C6BD
      D6005A397B00C67B6B00CE737300C66B6B00B5636300AD5A5A00525A31006B7B
      4A00CEAD9400FFDEB500FFDEAD008C4A21008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      000000000000000000000000000000000000AD847B00FFF7E700FFEFDE00F7E7
      CE00F7E7CE000084000084848400FFFFFF00FFFFFF0000840000008400000084
      0000FFFFFF00FFFFFF00FFFFFF008484840084422100FFFFF700FFFFF700424A
      B5007B7BCE00FFA5AD00FF949400F7848C00E7737300CE736B0042A54200087B
      100084846300FFDEB500FFDEB5008C4A21000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      000000000000000000000000000000000000B5848400FFF7EF00FFEFDE00F7E7
      D600F7E7CE008484840000840000FFFFFF00FFFFFF0000840000008400000084
      000000840000FFFFFF00FFFFFF000084000084422100FFFFFF00FFFFF7008484
      D600B5CEFF008C9CEF00CE94B500FF9C9400F7948C007BBD730029C6520018AD
      2900397B3100FFDEBD00FFDEB5008C4A210084848400000000000000FF000000
      00000000000000000000FF000000FF0000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      000000000000000000000000000000000000BD8C8400FFFFF700FFF7EF00FFEF
      DE00FFEFDE0084848400008400000084000000840000FFFFFF00FFFFFF00FFFF
      FF000084000000840000FFFFFF000084000084422100FFFFFF00FFFFF7007B7B
      CE00D6E7FF0094BDFF00638CF700AD84C6008CBDAD0039E7940031CE730021BD
      4A00399C3900FFDEBD00FFDEB5008C4A21000000000000000000000000000000
      FF00000000000000FF0000000000000000000000FF0000000000000000000000
      0000000000000000000000000000848484008484840084848400C6C6C600FFFF
      FF000000FF000000FF000000FF00FFFFFF00C6C6C60084848400848484000000
      000000000000000000000000000000000000BD948400FFFFFF00FFF7F700FFF7
      E700FFEFDE000084000084848400FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF008484840084422100FFFFFF00FFFFFF00C6BD
      DE00737BCE00BDD6FF0084ADFF004A7BF7002973F7001894F70008BDF70000D6
      EF0042B5B500FFE7BD00FFDEBD008C4A21008484840000000000000000000000
      FF00000000000000FF000000000000000000000000000000FF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      FF00FFFFFF00C6C6C600FFFFFF000000FF00FFFFFF0084848400000000000000
      000000000000000000000000000000000000C69C8C00FFFFFF00FFFFFF00FFF7
      F700FFF7EF000084000084848400848484000084000000840000848484008484
      84000084000000840000848484008484840084422100FFFFFF00FFFFFF00FFFF
      FF00DEDEE7007373C600738CDE006384EF00425AE7003952DE003152CE003173
      C600D6DECE00FFE7C600FFDEBD008C4A21000000000000000000000000000000
      00000000FF0000000000000000000000000000000000000000000000FF000000
      0000000000000000FF0000000000848484000000000084848400C6C6C6000000
      FF00C6C6C600FFFFFF00C6C6C6000000FF00C6C6C60084848400000000000000
      000000000000000000000000000000000000CE9C8C00FFFFFF00FFFFFF00FFFF
      F700FFF7F7008484840000840000008400008484840084848400008400000084
      00008484840084848400008400000084000084422100FFFFFF00FFFFFF00FFFF
      FF00FFFFF700E7DEE7009494CE00636BCE004A5ACE004252C6006B63BD00C6B5
      C600FFE7CE00FFE7C600FFE7BD008C4A21008484840000000000000000000000
      00000000000000000000000000000000000000000000000000000000FF000000
      00000000FF000000000000000000000000000000000084848400FFFFFF000000
      FF00FFFFFF00C6C6C600FFFFFF000000FF00FFFFFF0084848400000000000000
      000000000000000000000000000000000000D6A58C00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFF7F700FFF7EF00FFEFDE00FFF7E700DED6C600C6BDAD009C8C
      840000000000000000000000000000000000B56B3900D6A55A00D6A55A00D6A5
      5A00D6A55A00D6A55A00D6A55A00D6A55A00D6A55A00D6A55A00DEAD6B00D6A5
      5A00DEAD6B00D6A55A00CEAD7300B56B39000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000848484008484840084848400C6C6C600FFFF
      FF000000FF000000FF000000FF00FFFFFF00C6C6C60084848400848484000000
      000000000000000000000000000000000000D6AD8C00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFF700FFF7EF00FFF7E700DEBDAD00AD8C8400AD848400AD84
      7B0000000000000000000000000000000000AD5A2100DE7B2100DE7B2100DE7B
      2100DE7B2100DE7B2100DE7B2100DE7B2100DE7B2100DE7B2100F7BD8400DE7B
      2100F7BD8400B57B52004A6BD600BD5A18008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      000000000000000000000000000000000000DEAD9400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00F7EFEF00C69C9400F7C67B00FFBD4A000000
      00000000000000000000000000000000000000000000AD5A2900AD5A2900AD5A
      2900AD5A2900AD5A2900AD5A2900AD5A2900AD5A2900AD5A2900AD5A2900AD5A
      2900AD5A2900AD5A2900AD5A2900000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000848484000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      000000000000000000000000000000000000DEAD9400FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00F7F7EF00C69C9400FFCE7300D69C73000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      000000000000000000000000000000000000DEA58400DEA58400DEA58400DEA5
      8400DEA58400DEA58400DEA58400DEAD8400C6947B0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B000000000000000000000000007B7B7B000000
      000000000000000000007B7B7B00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      00000000000000000000000000000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B000000000000000000000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000007B7B7B00000000007B7B7B000000
      00007B7B7B00000000007B7B7B00000000000000000000000000000000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      000084000000000000000000FF000000FF000000FF0000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000000000
      00000000000000000000848484000000FF008484840000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      FF000000FF0000000000000000000000000000000000FF000000000000000000
      000000000000000000000000FF000000FF00000000007B7B7B00000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000FF000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000000
      0000000000000000FF00000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B000000FF007B7B7B00FF000000FF000000FF0000007B7B
      7B000000FF007B7B7B007B7B7B007B7B7B00000000000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF00000084000000FF00000084
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF000000000000000000FF000000000000000000
      00000000FF000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF00FF000000000000000000000000000000FF00
      00000000FF0000000000000000000000000000000000848484000000FF008484
      8400000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000084
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000FF0000000000FF000000000000000000
      FF0000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000FF0000000000FF000000000000000000000000000000
      FF00FF0000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000084000000000000000000000000000000000000000000
      0000000000000084000000FF00000084000000FF000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF000000FF000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FF000000000000000000FF000000FF000000FF000000
      0000FF0000000000000000000000000000008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000084000000FF00000084000000840000008400000084000000FF00000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00FF0000007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B00FF0000007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B00FF0000007B7B7B007B7B7B007B7B7B008400000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000FF00000084000000FF00000084000000FF000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000000000000000000000000000FF00
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000FF000000FF00000000000000000000000000000000000000
      00000000000000FF000000840000008400000084000000840000008400000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      000000000000000000000000000000000000000000007B7B7B00000000000000
      000000000000000000000000000000000000FF000000FF000000FF0000000000
      0000000000000000000000000000000000000000000084000000000000008484
      84000000FF008484840000000000000000000000000000000000000000000000
      0000848484000000FF0084848400000000000000000000000000000000000000
      000000000000000000000000000000FF00000084000000FF0000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000FF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      0000000000000000000000000000000000007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B00000000007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B007B7B7B000000000000000000000000008484
      84000000FF008484840000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000840000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000840000008400000084000000840000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000007B7B
      7B007B7B7B0000000000000000000000000000FF000000000000000000000000
      0000840000008400000000000000000000000000000000FF0000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF000000000000000000
      00000000000000000000000000000000000000000000000000007B7B7B007B7B
      7B000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000000000000000
      0000000000000000000000000000000000000000FF0000000000000000000000
      00000000FF00000000000000000000000000000000000000FF00000000000000
      000000000000000000007B7B7B000000000000000000000000009C6300009C63
      0000CE9C000094949400949494009494940094949400949494009C6331009C63
      31009C6331009C6331000000000000000000FF00000000000000FF000000FF00
      000000000000000000000000000000000000000000007B7B7B007B7B7B000000
      00000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000000000000000
      000000000000000000000000000000000000000000000000FF00000000000000
      00000000FF0000000000000000000000000000000000000000000000FF000000
      0000000000007B7B7B000000FF00000000000000000000000000FFCE6300CE9C
      0000CEFFFF00CEFFFF00CEFFFF009CCECE009CCECE0094ADAD0094ADAD009C63
      3100FFCE63009C6331000000000000000000FF00000000000000000000000000
      0000FF0000000000000000000000000000007B7B7B007B7B7B00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000FF00008400000000FF00008400
      000084000000840000008400000000FF00008400000000FF0000840000008400
      000084000000840000000000000000000000000000007B7B7B000000FF007B7B
      7B000000FF0000000000000000000000000000000000000000007B7B7B000000
      FF007B7B7B000000FF0000000000000000000000000000000000CE9C0000CEFF
      FF00CEFFFF0000000000000000000000000000000000000000009CCECE0094AD
      AD009C6331009C6331000000000000000000FF000000FF000000000000000000
      0000000000000000000000000000FFFFFF007B7B7B007B7B7B007B7B7B007B7B
      7B007B7B7B007B7B7B007B7B7B000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00008400000000FF0000840000008400
      00008400000084000000000000000000000000000000000000000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000CEFFFF00CEFF
      FF00CEFFFF0094949400FFFFFF00FFFFFF00CEFFFF00000000009CCECE0094AD
      AD009CCECE0094949400000000000000000000000000FF000000FF000000FF00
      0000FF000000FF00000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000FF00000000000000FF00000000
      000084000000840000000000000000FF00000000000000FF0000840000008400
      00000000000000000000000000000000000000000000000000000000FF000000
      FF00000000000000FF000000000000000000000000000000FF00000000000000
      FF000000FF000000000000000000000000000000000000000000CEFFFF00CEFF
      FF00CEFFFF0094949400FFFFFF00FFFFFF00CEFFFF00000000009CCECE009CCE
      CE0094ADAD009494940000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF000000000000FF000000000000000000000000
      0000840000008400000000FF0000000000000000000000FF0000840000008400
      000000000000000000000000000000FF0000000000000000FF000000FF000000
      FF000000FF0000000000000000000000000000000000000000000000FF000000
      FF000000FF000000FF000000FF00000000000000000000000000FFFFFF00CEFF
      FF00CEFFFF009494940000000000FFFFFF00CEFFFF0000000000CEFFFF0094AD
      AD009CCECE00949494000000000000000000000000000000000000000000FF00
      000000000000FF000000000000000000000000000000FFFFFF00000000000000
      0000FFFFFF0000000000FFFFFF00000000000000000000000000000000000000
      0000840000008400000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000FF00000000000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00CEFFFF0094949400639C9C0000639C0000000000000000009CCECE00CEFF
      FF0094ADAD009494940000000000000000000000000000000000FF0000000000
      000000000000FF000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      00000000FF000000FF00000000000000000000000000000000009C6300009CCE
      CE00FFFFFF009494940094949400009CCE009CFFFF0000639C0094ADAD00CEFF
      FF009C6331009C63000000000000000000000000000000000000FF0000000000
      000000000000FF000000000000000000000000000000FFFFFF0000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00008400000000FF000084000000840000008400
      00008400000000FF00008400000000FF00000000000000000000000000000000
      FF000000FF000000000000000000000000000000000000000000000000000000
      00000000FF000000FF0000000000000000000000000000000000CE9C00009C63
      00009CCECE00FFFFFF00CEFFFF00CEFFFF00009CCE0000CEFF00000000009C63
      3100FFCE63009C63000000000000000000000000000000000000FF0000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFCE6300CE9C
      00009C6300009CCECE00FFFFFF00CEFFFF00FFFFFF00009CCE0000639C000000
      0000CE9C00009C6300000000000000000000000000000000000000000000FF00
      0000FF000000FF000000000000000000000000000000FFFFFF0000000000FFFF
      FF0000000000FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000FF00000000000000FF000000000000840000008400
      00000000000000FF00000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000CE9C0000CE9C
      0000CE9C00009C63000094ADAD0094ADAD0094ADAD0094ADAD0000CEFF000063
      9C000000000094ADAD0000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF0000000000000000000000000000000000000000000000
      0000000000000000000000FF0000000000000000000000000000840000008400
      000000000000000000000000000000FF00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000005252
      FF000031CE000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000840000008400
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000C6C6C6000000
      0000C6C6C60000000000C6C6C60000000000C6C6C60000000000C6C6C6000000
      0000C6C6C60000000000C6C6C600000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000FF000000FF000000FF000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000C6C6C6000000
      000000000000000000000000000000000000C6C6C60000000000000000000000
      00000000000000000000C6C6C600000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000C6C6C6000000
      000000000000000000000000000000000000C6C6C60000000000000000000000
      00000000000000000000C6C6C600000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000C6C6C60000000000C6C6C600000000000000
      000000000000C6C6C60000000000C6C6C6000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000C6C6C6000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000C6C6C60000000000C6C6
      C60000000000000000000000000000000000C6C6C60000000000000000000000
      00000000000000000000C6C6C600000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000C6C6C6000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000C6C6C600000000000000
      000000000000000000000000000000000000C6C6C60000000000000000000000
      000000000000C6C6C60000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF000000FF000000FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF00000000000000000000000000000000000000000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF00000000000000000000000000000000000000000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000084000000840000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000FF0000000000000000000000000000000000000000000084
      0000008400000084000000840000008400000084000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000008400000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000008400000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000008400000084
      0000000000000000000000000000000000000000000000000000840000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000FF000000000000000000000000000000000000000000008400000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000000000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000084
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000084000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000840000000000000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000008400000084000000840000008400000084000000840000008400000084
      0000008400000084000000840000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000008400000000000000000000000000000000000000000000000000
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000084000000840000008400000084000000840000008400000084
      0000008400000084000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000084000000840000000000
      0000000000000000FF000000FF00000000000000000000000000000000000000
      00000000000000000000840000000000000000000000000000000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000840000008400000084000000840000008400000084
      0000008400000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008400
      0000840000000000000000000000000000000000000084000000840000000000
      0000000000000000FF000000FF00000000000000000000000000000000000000
      00000000000000000000000000008400000000000000000000000000FF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000008400000084000000840000008400000084
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000840000000000FF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000084000000840000008400000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000840000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF00FF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000BDBDBD000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000BDBDBD000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF000000
      00000000000000FFFF0000000000000000000000000000FFFF00000000000000
      000000FFFF000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF000000000000FFFF000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000000000FFFFFF000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000FFFF00848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000FFFF0000000000FFFFFF007B7B7B00FFFFFF000000000000FFFF0000FF
      FF0000FFFF000000000000000000000000000000000000000000000000008484
      840084848400848484008484840084848400000000000000000000FFFF000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF007B7B7B00FFFFFF0000000000000000000000
      000000000000000000000000000000000000000000000000000084848400FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF008484840000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000000000FFFFFF0000FFFF007B7B7B0000FFFF00FFFFFF000000000000FF
      FF0000FFFF00000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF007B7B7B00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000008484840084848400FFFFFF00C6C6
      C600FFFFFF000000FF00FFFFFF00C6C6C600FFFFFF0084848400848484000000
      00000000000000000000000000000000000000FFFF0000FFFF0000FFFF0000FF
      FF000000000000FFFF00FFFFFF007B7B7B00FFFFFF0000FFFF000000000000FF
      FF0000FFFF0000FFFF0000FFFF00000000008484840084848400C6C6C600FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF007B7B7B00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000084848400C6C6C600FFFF
      FF00C6C6C6000000FF00C6C6C600FFFFFF00C6C6C60084848400000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000000000FFFFFF0000FFFF00FFFFFF0000FFFF00FFFFFF000000000000FF
      FF0000FFFF000000000000000000000000000000000084848400FFFFFF00C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000084848400FFFFFF000000
      FF000000FF000000FF000000FF000000FF00FFFFFF0084848400000000000000
      000000000000000000000000000000000000000000000000000000FFFF0000FF
      FF0000FFFF0000000000FFFFFF0000FFFF00FFFFFF000000000000FFFF0000FF
      FF0000FFFF000000000000000000000000000000000084848400C6C6C6000000
      FF000000FF000000FF000000FF000000FF00C6C6C60084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      0000000000000000000000000000000000000000000084848400C6C6C600FFFF
      FF00C6C6C6000000FF00C6C6C600FFFFFF00C6C6C60084848400000000000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000000000000000000000000000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000000000000084848400FFFFFF00C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF0084848400000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008484840084848400FFFFFF00C6C6
      C600FFFFFF000000FF00FFFFFF00C6C6C600FFFFFF0084848400848484000000
      00000000000000000000000000000000000000000000000000000000000000FF
      FF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FFFF0000FF
      FF00000000000000000000000000000000008484840084848400C6C6C600FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6C60084848400848484000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000084848400FFFF
      FF00C6C6C600FFFFFF00C6C6C600FFFFFF008484840000000000000000000000
      000000000000000000000000000000000000000000000000000000FFFF000000
      00000000000000FFFF0000FFFF0000FFFF0000FFFF0000FFFF00000000000000
      000000FFFF00000000000000000000000000000000000000000084848400C6C6
      C600FFFFFF00C6C6C600FFFFFF00C6C6C6008484840000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400848484008484840084848400848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000FFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000008484
      8400000000000000000000000000848484000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000C6C6C600FFFFFF00C6C6C600C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000FFFFFF00C6C6C600FFFFFF00C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000008400000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF00000000000000FF000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6C600C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FF000000FF00000000000000FFFFFF0000000000FF0000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFF
      FF00C6C6C6000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FF0000000000000000000000FFFFFF0000000000FFFFFF0000000000FF00
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00C6C6C600FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFFFF00C6C6
      C600C6C6C6000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF00
      00000000000000000000FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF000000
      0000FF0000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6C600FFFF
      FF00C6C6C600C6C6C60000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      00000000000000000000000000000000000000000000FF000000FF0000000000
      0000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFF
      FF0000000000FF00000000000000000000000000000000000000000000000000
      000000000000FFFFFF000000000000000000FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000000000000C6C6C600FFFF
      FF00C6C6C600FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFFFF00C6C6
      C600FFFFFF00C6C6C60000000000000000000000000000000000000000008400
      0000840000008400000084000000FF000000FF00000084000000840000008400
      000084000000840000008400000000000000FF0000000000000000000000FFFF
      FF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000
      0000FFFFFF0000000000FF000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      00000000000000000000000000000000000000000000C6C6C600FFFFFF00C6C6
      C60000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6C600FFFF
      FF00C6C6C600C6C6C60000000000000000000000000000000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF00
      0000FF000000FF00000084000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF0000000000FFFFFF0000000000FF0000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      00000000000000000000000000000000000000000000FFFFFF00FFFFFF000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00C6C6
      C600C6C6C600FFFFFF00C6C6C600000000000000000000000000FF000000FF00
      0000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF00
      0000FF000000FF000000000000000000000000000000FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF0000000000FFFFFF00000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000C6C6C600C6C6C600FFFFFF00C6C6C600FFFFFF00FFFFFF00C6C6
      C60000000000C6C6C600C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000FFFFFF00FFFF
      FF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFFFF00FFFF
      FF00FFFFFF00FFFFFF0000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C60000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C60000000000FFFFFF00C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      000000000000000000000000000000000000000000000000000000000000FFFF
      FF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000FFFFFF00FFFFFF00FFFF
      FF00000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF00000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00C6C6C60000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C6000000000000000000C6C6C600000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FFFFFF00FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF00FFFFFF0000000000000000000000
      000000000000000000000000000000000000000000000000000000000000C6C6
      C600FFFFFF000000000000000000FFFFFF00C6C6C60000000000FFFFFF00C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000084000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00FFFFFF000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000C6C6
      C600FFFFFF000000000000000000C6C6C600C6C6C60000000000C6C6C600C6C6
      C600000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FF000000FF00000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF00FFFFFF00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000FFFFFF00C6C6C60000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FFFFFF0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000424D3E000000000000003E000000
      2800000040000000900000000100010000000000800400000000000000000000
      000000000000000000000000FFFFFF0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000FFFF000000000000FFFF000000000000
      9FFF0000000000008FFF000000000000E4F9000000000000F031000000000000
      F807000000000000000300000000000000000000000000000003000000000000
      F00F000000000000E03F0000000000008C9F0000000000001FCF000000000000
      3FE7000000000000FFE7000000000000AAAAFFFDF800FFFF0001FFF800000000
      BFFCFFF1000000001FF9FFE300000000AFF4FFC70000000033CDE08F00000000
      BC3CC01F000000001CFD803F00000000AB7C001F000000002BBD001F00000000
      B7D8001F000000003FD5001F000F0000BFEC001F000F00003FFD803F001F8001
      BFFCC07F001FFFFFFFFFE0FF007FFFFFFFBFDDDDDDDDFC7FFE1FD555D555F83F
      F81F00000000E011F01FDFBFDFFFD820F01F87BC87FC8C60F80FDBBBDBFB07E0
      F80F0000000007F1F80FDDB7DCE707FDF80F9EAF9CE78FFDF80FDF1FDD177FF1
      F01F000000007FE0F01FDFBFDEEFA3E0F81F9FBF9F1F81E0FE1FDFBFDFFFC1F1
      FFBF00000000C1CFFFFFFFFFFFFFE23FFFFFC0FF8000FFFFFFE7F3FF5555FFFF
      FFC773BF0000C0038F8F52BF76BCC003070052BFB6D8C0033200000386C2C003
      00005203CEE6C0038000528FCAA6C003F900718E86C0C003E100F14A4EF6C003
      C900FD4AE6F2C003C900FC00E6F2C003C300FD4AFEFEC003E300FD4A0000C003
      FF01FDCE5555FFE7FF03FFCF0000FFFFFFFFFFFFFFFFFFFFFFFFFC1FFFFFFFFF
      F80FFBEFFFFFD555F80FF7F7FFFF8000F80FEFFBFFFF9659F8FFEFFBFFFF9E79
      F8FFDFFDFFFFBEFBF8FFDFFDFE7FFEBAF8FFDFFDFE7F9C71F8FFEFFBFFFF0E79
      F8FFEFFBFFFF4EFCF8FFF7F7FFFF1C71F8FFFBEFFFFFBEFBF8FFFC1FFFFFFFFF
      F8FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFCFFFFFFF8001
      CF9FF0FFFFFFBFFDCF9FFF0FFFFFBFFDFFFFFFF3F03FBFFDF3FFFFFBE01FBFFD
      F3CFBFF7C00FBFFDFFCFDFF7C007BFFDFFFFEFEFE003BFFDFCFFF7EFF001BFFD
      FCFFFBEFF803BFFDE799FDDFFC07BFFDE799FEDFFE0FBFFDFFFFFF3FFF1FBFFD
      FFFFFFBFFFBF8001FFFFFFFFFFFFFFFFFFFCFEFFFFFDFEFFFFF8FC7FFFF8FC7F
      FFF1FC7FFFF1FC7FFFE3D837FFE3FC7FFFC7E00FFFC7FC7FE08FE00FE08FFC7F
      C01FC007C01FF83F803FC007803FF01F001F0001001FF01F001FC007001FF01F
      001FC007001FF83F001FE00F001FFC7F001FE00F001FFFFF803FD837803FFFFF
      C07FFEFFC07FFFFFE0FFFEFFE0FFFFFFF807FFFFFFFFFFCFF807FF3FFE3FFF87
      F007FE3FF81FFF87E003FE3FF40FF70FC003FE3FE007F30FC001FE3F8003F01F
      8001E0014001F01F0001C0010000F0030000C0030000F0079000FE3F8001F00F
      E000FE3FC003F01FE000FE3FE00FF03FC005FE3FF07FF07FC007FE7FF8FFF0FF
      E40FFFFFFFFFF1FFFE7FFFFFFFFFF3FF00000000000000000000000000000000
      000000000000}
  end
  object alMap: TActionList
    Images = imlMap
    Left = 580
    Top = 52
    object actClear: TAction
      Caption = #1054#1095#1080#1089#1090#1080#1090#1080
      Hint = #1059#1073#1088#1072#1090#1080' '#1074#1089#1110' '#1086#1073#39#1108#1082#1090#1080' '#1079' '#1082#1072#1088#1090#1110
      ImageIndex = 19
      OnExecute = actClearExecute
    end
    object actPan: TAction
      Caption = #1055#1077#1088#1077#1084#1110#1097#1077#1085#1085#1103
      Hint = #1058#1072#1089#1082#1072#1090#1080' '#1082#1072#1088#1090#1091' '#1082#1088#1080#1089#1086#1102
      ImageIndex = 0
      OnExecute = actPanExecute
    end
    object actLayers: TAction
      Caption = #1064#1072#1088#1080
      Hint = #1044'i'#1072#1083#1086#1075' '#1085#1072#1089#1090#1088#1086#1081#1082#1080' '#1096#1072#1088'i'#1074' '#1082#1072#1088#1090#1080
      ImageIndex = 2
      OnExecute = actLayersExecute
    end
    object actZoomIn: TAction
      Caption = #1047#1073#1110#1083#1100#1096#1080#1090#1080
      Hint = #1047#1073#1110#1083#1100#1096#1080#1090#1080' '#1084#1072#1089#1096#1090#1072#1073
      ImageIndex = 4
      OnExecute = actZoomInExecute
    end
    object actZoomOut: TAction
      Caption = #1047#1084#1077#1085#1096#1080#1090#1080
      Hint = #1047#1084#1077#1085#1096#1080#1090#1080' '#1084#1072#1089#1096#1090#1072#1073
      ImageIndex = 6
      OnExecute = actZoomOutExecute
    end
    object actDistance: TAction
      Caption = #1044#1080#1089#1090#1072#1085#1094#1110#1103
      Hint = #1052'i'#1088#1103#1090#1080' '#1076#1080#1089#1090#1072#1085#1094#1110#1102
      ImageIndex = 15
      OnExecute = actDistanceExecute
    end
    object actNone: TAction
      Caption = 'actNone'
      Hint = #1057#1090#1088'i'#1083#1082#1072
      ImageIndex = 3
      OnExecute = actNoneExecute
    end
    object actSaveBmp: TAction
      Caption = 'actSaveBmp'
      Hint = #1047#1072#1082#1072#1090#1072#1090#1080' '#1079#1086#1073#1088#1072#1078#1077#1085#1085#1103' '#1082#1072#1088#1090#1099' '#1074' '#1075#1088#1072#1092'i'#1095#1085#1080#1081' '#1092#1072#1081#1083
      ImageIndex = 18
      OnExecute = actSaveBmpExecute
    end
    object actCalcCoverSector: TAction
      Caption = #1056#1086#1079#1087#1086#1076#1110#1083' '#1085#1072#1087#1088#1091#1078#1077#1085#1086#1089#1090#1110' '#1087#1086#1083#1103
      Hint = #1056#1086#1079#1088#1072#1093#1091#1074#1072#1090#1080' '#1088#1086#1079#1087#1086#1076#1080#1083' '#1085#1072#1087#1088#1091#1078#1077#1085#1086#1089#1090#1110' '#1087#1086#1083#1103' '#1074' '#1089#1077#1082#1090#1086#1088#1110
      ImageIndex = 20
      OnExecute = actCalcCoverSectorExecute
    end
    object actSetTP: TAction
      Caption = #1050#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080
      Hint = 
        #1042#1089#1090#1072#1085#1086#1074#1080#1090#1080' '#1082#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1110' '#1079#1075#1110#1076#1085#1086' '#1088#1086#1079#1088#1072#1093#1091#1085#1082#1091' '#1079#1086#1085#1080' '#1086#1073#1089#1083#1091#1075#1086#1074#1091#1074#1072#1085#1085 +
        #1103
      ImageIndex = 23
      OnExecute = actSetTPExecute
    end
    object actOffset: TAction
      Caption = #1055#1110#1076#1073#1110#1088' '#1047#1053#1063
      Hint = #1055#1110#1076#1073#1110#1088' '#1047#1053#1063
      ImageIndex = 21
      OnExecute = actOffsetExecute
    end
    object actERP: TAction
      Caption = #1055#1110#1076#1073#1110#1088' '#1045#1042#1055
      Hint = #1055#1110#1076#1073#1110#1088' '#1045#1042#1055
      ImageIndex = 22
      OnExecute = actERPExecute
    end
    object actGetRelief: TAction
      Caption = #1056#1077#1083#1100#1077#1092
      Hint = #1055#1088#1086#1092#1080#1083#1100' '#1088#1077#1083#1100#1077#1092#1072' '#1090#1088#1072#1089#1089#1099' '#1088#1072#1089#1087#1088#1086#1089#1090#1088#1072#1085#1077#1085#1080#1103' '#1088#1072#1076#1080#1086#1089#1080#1075#1085#1072#1083#1072
      ImageIndex = 24
      OnExecute = actGetReliefExecute
    end
    object actZoomFit: TAction
      Caption = #1042#1084#1110#1089#1090#1080#1090#1080' '#1074#1089#1077
      Hint = #1055#1086#1082#1072#1079#1072#1090#1080' '#1074#1089#1110' '#1086#1073#39#1108#1082#1090#1080' '#1085#1072' '#1082#1072#1088#1090#1110
      ImageIndex = 25
      OnExecute = actZoomFitExecute
    end
    object actShowTx: TAction
      Caption = #1050#1072#1088#1090#1082#1072' '#1086#1073#39#1108#1082#1090#1072' '#1077#1082#1089#1087#1077#1088#1090#1080#1079#1080
      Hint = #1050#1072#1088#1090#1082#1072' '#1086#1073'`'#1108#1082#1090#1072
      OnExecute = actShowTxExecute
    end
    object actExportSelectionToExcel: TAction
      Hint = #1045#1082#1089#1087#1086#1088#1090#1091#1074#1072#1090#1080' '#1088#1077#1079#1091#1083#1100#1090#1072#1090#1080' '#1074' Excel'
      ImageIndex = 26
      OnExecute = actExportSelectionToExcelExecute
    end
    object actDegradationSectorSelection: TAction
      Caption = 'actDegradationSectorSelection'
      Hint = #1055#1110#1076#1073#1110#1088' '#1089#1077#1082#1090#1086#1088#1072' '#1087#1086#1089#1083#1072#1073#1083#1077#1085#1085#1103
      ImageIndex = 27
      OnExecute = actDegradationSectorSelectionExecute
    end
    object actCoordinationPointsShow: TAction
      Caption = 'actCoordinationPointsShow'
      Hint = #1042#1099#1076#1086#1073#1088#1072#1079#1080#1090#1080' '#1082#1086#1085#1090#1088#1086#1083#1100#1085#1110' '#1090#1086#1095#1082#1080' '#1085#1072' '#1084#1077#1078#1110
      OnExecute = actCoordinationPointsShowExecute
    end
  end
  object tmrInvalidate: TTimer
    Interval = 5000
    OnTimer = tmrInvalidateTimer
    Left = 456
    Top = 96
  end
  object pmnTx: TPopupMenu
    Left = 488
    Top = 152
    object mniTxEdit: TMenuItem
      Action = actEdit
    end
    object mniAnalyze: TMenuItem
      Action = actAnalyze
    end
    object mniUseInCalc: TMenuItem
      Action = actUsedInCalc
    end
    object mniShowTestPoints: TMenuItem
      Action = actShowTestPoints
    end
    object miDay: TMenuItem
      Action = actDayNight
    end
    object miNight: TMenuItem
      Caption = #1053'i'#1095
      OnClick = actDayNightExecute
    end
  end
  object pmnAllotZones: TPopupMenu
    Left = 760
    Top = 352
  end
end
