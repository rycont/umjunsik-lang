Private Sub Worksheet_Change(ByVal Target As Range) '콘솔에 값을 입력하고 선택 셀을 바꿨올 때
    If (is_running) Then
        If (is_on_console) Then
            If (Target = Cells(current_console_row + 4, 6)) Then
                prev_cell_row = prev_cell_row + 1
                Cells(prev_cell_row, prev_cell_col).Select
                is_on_console = False '입력 모드에서 빠져나음
            End If
        Else
        End If
    End If
End Sub

Private Sub Worksheet_SelectionChange(ByVal Target As Range)
    If (is_running) Then
        Cells(prev_cell_row, prev_cell_col).Select
    End If
End Sub

