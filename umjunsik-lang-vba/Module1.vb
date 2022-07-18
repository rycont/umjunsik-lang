Public initial_sound_is_allowed As Boolean, is_running As Boolean, is_debugging As Boolean, is_on_console As Boolean, blocking As Boolean, visual As Boolean
Public current_command_row As Long, current_console_row As Long, last_command_row As Long
Public prev_cell_row As Long, prev_cell_col As Long
Public updated_max_var As Long, updated_min_var As Long, return_num As Long
'사전에 입력한 입력값에서 아직 사용하지 않은 입력값의 위치
Public has_input_list As Boolean
Public input_to_use As Long

Sub ClearAll()
    Worksheets("실행기").Range("E105:K1048576").ClearContents
    Worksheets("변수").Range("C5:C1048576").ClearContents
    Worksheets("변수").Range("G5:G1048576").ClearContents
    
    Worksheets("변수").Range("J5:J1048576").ClearContents
    
    With Range("K5:K1048576") '변수(양수)
        .Value = 0: .Font.ColorIndex = 1
    End With
    With Range("H16:H1048576") '변수(음수)
        .Value = 0: .Font.ColorIndex = 1
    End With
    Worksheets("변수").Range("C5:C1048576").Value = 0 '변경한 변수(양수)
    Worksheets("변수").Range("G5:G1048576").Value = 0 '변경한 변수(음수)
End Sub

Sub Clear()
    Worksheets("실행기").Range("E105:K1000").ClearContents
    Worksheets("실행기").Range("E5:F105").ClearContents
    Worksheets("변수").Range("C5:C1000").ClearContents
    Worksheets("변수").Range("G5:G1000").ClearContents
End Sub

Sub TransformInputs(ByRef error_code As Integer)
    Worksheets("변수").Range("J5:J1000").ClearContents
    
    Dim Str As String
    Dim prev_pos As Long, line As Long
    Str = vbNullString
    prev_pos = 1
    line = 1
    
    For i = 0 To 9
        current_command_row = i + 1
        Str = Cells(i + 5, 8).Value
        
        If (IsEmpty(Replace(Cells(i + 5, 8), " ", ""))) Then
            GoTo Trans_Continue1
        End If
        
        For j = 1 To Len(Str)
            Worksheets("변수").Cells(3, 10).Value = Mid(Str, j, 1)
            If (WorksheetFunction.IsNumber(Worksheets("변수").Cells(3, 10))) Then '0~9의 숫자
                If (Int(Mid(Str, j, 1)) - Mid(Str, j, 1) = 0) Then
                    Worksheets("변수").Cells(line + 4, 10).Value = Worksheets("변수").Cells(line + 4, 10).Value & Mid(Str, j, 1)
                End If
            ElseIf (Mid(Str, j, 1) = "-") Then '-부호
             Worksheets("변수").Cells(line + 4, 10).Value = Worksheets("변수").Cells(line + 4, 10).Value & Mid(Str, j, 1)
            ElseIf (Mid(Str, j, 1) = " ") Then '공백 처리
                If (Not IsEmpty(Worksheets("변수").Cells(line + 4, 10))) Then '다음 저장 공간으로 이동
                    line = line + 1
                Else '무시
                End If
            Else
                Call ErrorCode(-108)
                error_code = -108
                Exit Sub
            End If
            
            Worksheets("변수").Cells(3, 10).ClearContents
        Next
        
        If (Not IsEmpty(Worksheets("변수").Cells(line + 4, 10))) Then '다음 저장 공간으로 이동
            line = line + 1
        Else '무시
        End If
        
Trans_Continue1:
    Next

    input_to_use = 1
    If (IsEmpty(Worksheets("변수").Cells(5, 10))) Then
        has_input_list = False
    Else
        has_input_list = True
    End If
End Sub

Sub OneLineToSeveralLine()
    Dim code, current_line
    code = Cells(1 + 4, 4).Value
    Cells(1 + 4, 4).ClearContents
    current_line = 1
    
    For i = 1 To Len(code)
        character = Mid(code, i, 1)
        If (character = "~") Then '줄바꿈
            current_line = current_line + 1
        Else
            Cells(current_line + 4, 4).Value = Cells(current_line + 4, 4).Value & character
        End If
    Next
End Sub

Sub Debug_code()
    If Not (is_running) Then '실행 중이 아니면 프로그램 실행
        is_debugging = True
        visual = False
        is_running = True
        is_on_console = False
        blocking = True
        has_input_list = False
        input_to_use = 0
        
        Call Clear
        
        Dim error_c As Integer
        error_c = 0
        
        Call TransformInputs(error_c)
        If (error_c <> 0) Then
            Call Stop_code
            Exit Sub
        Else
            Call start
        End If
        
        If (is_running) Then
            Call Stop_code
        Else '도중 종료가 일어났을 때
            If (return_num = 0) Then
                return_num = -1 '일반적인 오류 시 반환값
            End If
        End If
        
        Cells(current_console_row + 1 + 4, 6).Value = "Exited with code " & return_num
        
    ElseIf (is_debugging) Then '실행 중이면 중단된 프로그램을 계속 실행
        blocking = False
    End If
End Sub

Sub Init()
    If Not (is_running) Then '실행 중이 아니면 프로그램 실행
        is_debugging = False
        is_running = True
        is_on_console = False
        blocking = False
        has_input_list = False
        input_to_use = 0
        
        Call Clear
        
        Dim error_c As Integer
        error_c = 0
        
        Call TransformInputs(error_c)
        If (error_c <> 0) Then
            Call Stop_code
            Exit Sub
        Else
            Call start
        End If
        
        If (is_running) Then
            Call Stop_code
        Else '도중 종료가 일어났을 때
            If (return_num = 0) Then
                return_num = -1 '일반적인 오류 시 반환값
            End If
        End If
        
        Cells(current_console_row + 1 + 4, 6).Value = "Exited with code " & return_num
    ElseIf (blocking = True) Then '실행 중이면 중단된 프로그램을 계속 실행
        blocking = False
    End If
End Sub

Sub Stop_code()
    If Not (is_running) Then '이미 종료되었을 때
        Exit Sub
    End If
    
    is_running = False
    is_debugging = False
    blocking = False
    
    If (is_on_console) Then
        is_on_console = False
    End If
    
    Cells(current_console_row + 2 + 4, 5).Value = "$"
    Application.ScreenUpdating = True
    If (visual) Then
        Cells(current_console_row + 2 + 4, 6).Select
    End If
End Sub

Sub start()
    '변수 초기화
    With Range("K5:K1000") '변수(양수)
        .Value = 0: .Font.ColorIndex = 1
    End With
    With Range("H16:H1000") '변수(음수)
        .Value = 0: .Font.ColorIndex = 1
    End With
    Worksheets("변수").Range("C5:C1000").Value = 0 '변경한 변수(양수)
    Worksheets("변수").Range("G5:G1000").Value = 0 '변경한 변수(음수)
    Worksheets("실행기").Range("E5:F105").ClearContents
    Cells(5, 5).Value = "$"
    Cells(5, 6).Value = "umjunsik main.umm"
    
    current_command_row = 1
    current_console_row = 2
    
    prev_cell_row = current_command_row + 4
    prev_cell_col = 4
    
    Cells(current_command_row + 4, 4).Select
    '콘솔 입력 부분을 제외한 나머지 명령 처리는 Selection 대신 command 사용
    Dim command As Variant
    command = Selection.Value
    
    Dim error_code As Integer
    error_code = 0
    
    Dim count_for_visual As Integer
    count_for_visual = 0
    
    '반환값
    return_num = 0
    
    '초기화 완료된 화면을 업데이트 후 시각 효과 생략
    If Not (is_debugging) Then
        Application.Calculation = CalculationManual
        If (visual) Then
            Application.ScreenUpdating = False
            Application.EnableEvents = False
        End If
    End If
    
    '코드 첫 부분이 "어떻게"로 시작되는지 확인
    If Not (Cells(current_command_row + 4, 4).Value = "어떻게") Then
        ErrorCode (-101) 'Error 101
        Cells(current_command_row + 4, 4).Select
        return_num = -1
        Exit Sub
    End If
    

    '코드 마지막 부분에 "이 사람이름이냐ㅋㅋ"가 있는지 확인
    last_command_row = FindEndPoint
    If (last_command_row = -1) Then
        ErrorCode (-102) 'Error 102
        return_num = -1
        Exit Sub
    End If
    For Each cell1 In Range("B5:B1000") '중단점 외 모든 정보 삭제
        If (Not IsEmpty(cell1) And cell1.Value <> "?") Then
            cell1.ClearContents
        End If
    Next
    Cells(last_command_row + 4, 2).Value = "▲"

    
    '코드 실행
    Do
        current_command_row = current_command_row + 1
        
Continue2:
        prev_cell_row = current_command_row + 4
        prev_cell_col = 4 '콘솔 입력 외의 상황
        
        If Not (visual) Then
            Cells(current_command_row + 4, 4).Select
        End If
        command = Cells(current_command_row + 4, 4).Value
        
        error_code = 0
        count_for_visual = count_for_visual + 1
        
        '마지막 부분에 도달하면 코드 종료
        If (Cells(current_command_row + 4, 2).Value = "▲") Then
            Exit Do
        End If
        
        If (Cells(current_command_row + 4, 2).Value = "?" Or Cells(current_command_row + 4, 2).Value = "¶") Then
            Cells(current_command_row + 4, 2).Value = "¶"
            blocking = True
        Else
            If (Cells(current_command_row + 4, 2).Value <> "▲") Then
                Cells(current_command_row + 4, 2).Value = "→"
            End If
        End If
        
        '조건문 속 명령은 여기서부터 실행
IfCommand:
        
        '마지막 부분에 도달하면 코드 종료(조건문으로 도달 시)
        If (Cells(current_command_row + 4, 2).Value = "▲") Then
            Exit Do
        End If
		
		'앞뒤 공백 자르기
		command = Trim(command)
		
        '빈 줄은 무시
        If (Len(command) = 0) Then
            GoTo Continue1
        End If
        
        '변수 계산
        Dim variable_count As Long, number As Long, updated_var_row As Long, var_end_point As Long
        Dim is_called As Boolean
        is_called = False
        var_end_point = 1
        variable_count = 1
        number = 0
        updated_var_row = 0
        
        '콘솔 출력
        Dim output_char
        Dim is_numeral As Boolean
        
        Dim dongtan_end_point As Long
        dongtan_end_point = 0
        
        If (Cells(current_command_row + 4, 4).Value = "어떻게") Then '첫 번째 줄
            GoTo Continue1
        End If

        Select Case Mid(command, 1, 1)
            Case "엄", "어", "엌" '변수 대입 혹은 호출(출력)
                If (current_command_row = 1) Then '"어떻게"
                    GoTo Continue1
                End If
                
                '대입할 변수 탐색
                variable_count = ParseVariable(command, 1, Len(command), var_end_point, is_called, error_code)
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                ElseIf (is_called) Then
                    Call ErrorCode(251) 'Warn 251
                End If
                
                error_code = 0
                
                '(뒤쪽에 수식이 있다면) 변수에 대입할 숫자 계산
                number = CalculateNumber(command, var_end_point + 1, Len(command), error_code)

                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                '변수에 값 대입
                Call MapVar(variable_count, number, error_code)
                
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                '변경된 변수 표시
                If Not (visual) Then
                    If (variable_count > 0) Then
                        Cells(4 + variable_count, 11).Font.ColorIndex = 3
                        Worksheets("변수").Cells(4 + variable_count, 3).Value = 1
                        If (updated_max_var < variable_count) Then '최대 변수
                            updated_max_var = variable_count
                        End If
                    ElseIf (variable_count <= 0) Then
                        Cells(16 - variable_count, 8).Font.ColorIndex = 3
                        Worksheets("변수").Cells(5 - variable_count, 7).Value = 1
                        If (updated_min_var > variable_count) Then '최소 변수
                            updated_min_var = variable_count
                        End If
                    End If
                End If
            
            Case "식" '값 출력
                If (Mid(command, 2, 1) = "?") Then '대입 없이 입력만 받을 경우(출력과는 관계 X)
                    '숫자 계산
                    Call ErrorCode(252)
                    number = CalculateNumber(command, 1, Len(command), error_code)
                    
                    If (error_code <> 0) Then
                        Call ErrorCode(error_code)
                        If (error_code < 0) Then
                            return_num = -1
                            Exit Sub
                        End If
                        
                        error_code = 0
                    End If
                    
                    GoTo Continue1
                End If
                
                
                Call PrintToConsole(command, error_code)
                
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
            
            Case "동" '조건문
                If Not (Mid(command, 2, 1) = "탄") Then '"동탄"의 현태가 아님
                    Call ErrorCode(-106) 'Error 106
                    return_num = -1
                    Exit Sub
                End If
                
                For j = 3 To Len(command)
                    If (Mid(command, j, 1) = "?") Then '"?"를 찾음
                        dongtan_end_point = j
                        Exit For
                    ElseIf (j = Len(command)) Then '"?"를 찾지 못함
                        Call ErrorCode(-106) 'Error 106
                        return_num = -1
                        Exit Sub
                    End If
                Next
                
                number = CalculateNumber(command, 3, dongtan_end_point - 1, error_code)
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                If (number = 0) Then '조건문이 참이면
                    command = Mid(command, dongtan_end_point + 1)
                    GoTo IfCommand
                Else '조건문이 거짓이면
                    GoTo Continue1
                End If
                
            Case "준" 'Goto문
                number = CalculateNumber(command, 2, Len(command), error_code)
                
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                If (number <= 0) Then '0, 음수번째 줄
                    Call ErrorCode(-401)
                    return_num = -1
                    Exit Sub
                ElseIf (number > last_command_row) Then '종료 지점 이후로 이동 불가
                    Call ErrorCode(-402)
                    return_num = -1
                    Exit Sub
                ElseIf (number = current_command_row) Then '자기 자신으로 이동 불가
                    Call ErrorCode(-404)
                    return_num = -1
                    Exit Sub
                Else
                	'현재 실행 중인 줄에서 화살표 지우기
                    If (Cells(current_command_row + 4, 2).Value = "¶" Or Cells(current_command_row + 4, 2).Value = "?") Then
                        Cells(current_command_row + 4, 2).Value = "?"
                    Else
                        Cells(current_command_row + 4, 2).ClearContents
                    End If
                    
                    '이동하는 줄에 중단점이 있을 경우
                    If (Cells(number + 4, 2).Value = "?" Or Cells(number + 4, 2).Value = "¶") Then
                        Cells(number + 4, 2).Value = "¶"
                        blocking = True
                    End If
                    
                    If Not (visual) Then
                        Cells(number + 4, 4).Select
                    End If
                    current_command_row = number
                    GoTo Continue2
                End If
                
            Case "화" 'return문
                If Not (Mid(command, 2, 3) = "이팅!") Then '"화이팅!"의 형태가 아님
                    Call ErrorCode(-107) 'Error 107
                    return_num = -1
                    Exit Sub
                End If
                
                number = CalculateNumber(command, 5, Len(command), error_code)
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                '반환 후 프로그램 종료
                If Not (visual) Then
                    For j = 1 To updated_max_var '양수 변수 지우기
                        If (Worksheets("변수").Cells(4 + j, 3).Value <> 0) Then
                            Cells(4 + j, 11).Font.ColorIndex = 1
                            Worksheets("변수").Cells(4 + j, 3).Value = 0
                        End If
                    Next
                    
                    For j = 0 To updated_min_var Step -1 '음수 변수 지우기
                        If (Worksheets("변수").Cells(5 - j, 7).Value <> 0) Then
                            Cells(16 - j, 8).Font.ColorIndex = 1
                            Worksheets("변수").Cells(5 - j, 7).Value = 0
                        End If
                    Next
                End If
                    
                If (Cells(current_command_row + 4, 2).Value = "¶" Or Cells(current_command_row + 4, 2).Value = "?") Then
                    Cells(current_command_row + 4, 2).Value = "?"
                Else
                    Cells(current_command_row + 4, 2).Value = vbNullString
                End If
                
                return_num = number
                Exit Sub
                
            Case ".", "," '맨 앞에 바로 수식이 나오는 경우
                '숫자 계산
                Call ErrorCode(252)
                number = CalculateNumber(command, 1, Len(command), error_code)
                
                If (error_code <> 0) Then
                    Call ErrorCode(error_code)
                    If (error_code < 0) Then
                        return_num = -1
                        Exit Sub
                    End If
                    
                    error_code = 0
                End If
                
                GoTo Continue1
            Case "이" '이 사람이름이냐ㅋㅋ
                If (Cells(current_command_row + 4, 2).Value = "▲") Then '코드의 끝 부분
                    return_num = 0
                    Exit Sub
                Else
                    Call ErrorCode(-103) '잘못된 문자로 시작
                    return_num = -1
                    Exit Sub
                End If
                
            Case Else
                Call ErrorCode(-103) '잘못된 문자로 시작
                return_num = -1
                Exit Sub
        End Select
        
Continue1:
        
        If (visual And count_for_visual >= 100) Then '시각 모드 해제 시 100회마다 1번씩 화면 업데이트
            Application.ScreenUpdating = True
            Cells(current_console_row + 4, 6).Select
            Application.ScreenUpdating = False
            count_for_visual = 0
        End If
        
        If (blocking) Then '비주얼 생략 모드에서 중단점 적용 시 화면 업데이트
            Application.ScreenUpdating = True
            Cells(current_command_row + 4, 4).Select
        End If
        Do
            
            If Not (blocking) Then
                If (is_debugging) Then '디버깅 중일 때만 다음 코드 실행 후 중단 적용
                    blocking = True
                End If
                
                If (Cells(current_command_row + 4, 2).Value = "¶" Or Cells(current_command_row + 4, 2).Value = "?") Then
                    Cells(current_command_row + 4, 2).Value = "?"
                Else
                    Cells(current_command_row + 4, 2).Value = vbNullString
                End If
                
                Exit Do
            End If
            
            Delay (0.1)
        Loop
        
        If (visual) Then
            Application.ScreenUpdating = False
        End If
        
        If Not (visual) Then
            For j = 1 To updated_max_var '양수 변수 지우기
                If (Worksheets("변수").Cells(4 + j, 3).Value <> 0) Then
                    Cells(4 + j, 11).Font.ColorIndex = 1
                    Worksheets("변수").Cells(4 + j, 3).Value = 0
                End If
            Next
            
            For j = 0 To updated_min_var Step -1 '음수 변수 지우기
                If (Worksheets("변수").Cells(5 - j, 7).Value <> 0) Then
                    Cells(16 - j, 8).Font.ColorIndex = 1
                    Worksheets("변수").Cells(5 - j, 7).Value = 0
                End If
            Next
        End If
        
        
        If (Cells(current_command_row + 4, 2).Value = "¶" Or Cells(current_command_row + 4, 2).Value = "?") Then
            Cells(current_command_row + 4, 2).Value = "?"
        Else
            Cells(current_command_row + 4, 2).Value = vbNullString
        End If
        
        If Not (is_running) Then '처리 도중 종료 명령이 들어왔을 때
            return_num = -1
            Exit Sub
        End If
        
    Loop
End Sub
                    
Sub Delay(ByVal a As Double)
    Dim start
    start = Timer
    
    Do While Timer < start + a
        DoEvents
    Loop
End Sub

Sub ErrorCode(ByVal error_code As Integer)
    Dim contents
    contents = vbNullString
    
    If (error_code < 0) Then '오류
        contents = Application.VLookup(-error_code, Worksheets("오류 코드").Range("C2:D42"), 2, False)
        MsgBox current_command_row & "번째 줄에서 오류 발생(" & -error_code & "): " & contents, vbCritical, "오류"
    ElseIf (error_code > 0) Then '경고
        contents = Application.VLookup(error_code, Worksheets("오류 코드").Range("G2:H42"), 2, False)
        MsgBox current_command_row & "번째 줄에서 경고 발생(" & error_code & "): " & contents, vbExclamation, "경고"
    Else
        MsgBox "프로그래밍 오류(오류 코드 0)"
    End If
    
    Cells(current_command_row + 4, 4).Select
End Sub

Private Function FindEndPoint() As Long
    FindEndPoint = 1
    
    For i = 0 To 10000
        If (Cells(FindEndPoint + 4, 4).Value = "이 사람이름이냐ㅋㅋ" Or Cells(FindEndPoint + 4, 4).Value = "이 사람이름이냐크크") Then
            Exit Function
        ElseIf (Cells(FindEndPoint + 4, 4).Value = "이 사람이름이냐크ㅋ" Or Cells(FindEndPoint + 4, 4).Value = "이 사람이름이냐ㅋ크") Then
            Exit Function
        End If
        
        FindEndPoint = FindEndPoint + 1
    Next
    
    FindEndPoint = -1
End Function

Function ParseVariable(ByVal Target As Variant, ByVal start_point As Long, ByVal restriction_end_point As Long, ByRef end_point As Long, ByRef is_called As Boolean, ByRef error_code As Integer) As Long
'MsgBox "target:" & target & "start_point: " & start_point & "restriction_end_point: " & restriction_end_point
    Dim n_start_point As Long, tmp_end_point As Long, tmp_number As Long, orig_len_of_target As Long
    Dim target_is_modified As Boolean, is_called_for_v3 As Boolean
    Dim is_called_for_v3_last_k As Long '"어" 변수 호출 또는 "어...ㅋ"의 변수 호출인지 확인
    n_start_point = start_point
    tmp_end_point = start_point
    tmp_number = 0
    target_is_modified = False
    orig_len_of_target = restriction_end_point
    ParseVariable = 1
    is_called = True
    
    is_called_for_v3 = False
    is_called_for_v3_last_k = restriction_end_point
    
    If (Mid(Target, start_point, 1) = "어") Then '"어" 변수 호출 또는 "어...ㅋ"의 변수 호출인지 확인
        For i = restriction_end_point To start_point Step -1
            If (Mid(Target, i, 1) = "ㅋ" Or Mid(Target, i, 1) = "크") Then
                is_called_for_v3 = True
                is_called_for_v3_last_k = i
                Exit For
            End If
        Next
    End If
    
    If (start_point = 1) Then 'ㅋ가 들어 있지만 맨 처음 L-Value 부분인 경우
        is_called_for_v3 = False
        is_called_for_v3_last_k = restriction_end_point
    End If
    
    If (Not is_called_for_v3 And (Mid(Target, start_point, 1) = "엄" Or Mid(Target, start_point, 2) = "어어" Or Mid(Target, start_point, 2) = "어엄" Or Mid(Target, start_point, 1) = "어")) Then '연음으로 변수 탐색
        For i = start_point To restriction_end_point
            If (Mid(Target, i, 1) = "엄") Then
                is_called = False
                ParseVariable = i - start_point + 1
                end_point = start_point + ParseVariable - 1
                Exit Function
            ElseIf (Mid(Target, i, 1) = "어") Then
                If (i <> restriction_end_point) And (Mid(Target, i + 1, 1) = "어" Or Mid(Target, i + 1, 1) = "엄") Then '변수 표현이 끝나지 않음
                    ParseVariable = i - start_point + 1
                Else '어 뒤로 변수와 관련 없는 문자가 옴, 또는 탐색 범위가 끝남(=변수 끝 또는 오류)
                    ParseVariable = i - start_point + 1
                    end_point = start_point + ParseVariable - 1
                    Exit Function
                End If
            Else
                Exit For
            End If
        Next
    ElseIf (Mid(Target, start_point, 1) = "엌") Then '값으로 변수 탐색
        For i = start_point + 1 To restriction_end_point 'N 값의 시작 부분 직전 찾기
            If Not (Mid(Target, i, 1) = "ㅋ" Or Mid(Target, i, 1) = "크") Then
                n_start_point = i - 1
                Exit For
            End If
        Next
        
        For i = n_start_point + 1 To restriction_end_point 'N 값의 끝 부분 찾기
            If (Mid(Target, i, 1) = "ㅋ" Or Mid(Target, i, 1) = "크") Then
                tmp_end_point = i - 1
                Exit For
            End If
        Next
        
        'N 값 계산
        tmp_number = CalculateNumber(Target, n_start_point + 1, tmp_end_point, error_code)
        If (error_code < 0) Then
            ParseVariable = -1
            Exit Function
        End If
        
        ParseVariable = tmp_number
        is_called = False
        
        For i = tmp_end_point + 1 To restriction_end_point + 1 '변수 호출의 끝 부분 찾기
            If Not (Mid(Target, i, 1) = "ㅋ" Or Mid(Target, i, 1) = "크") Or (i = restriction_end_point + 1) Then
                tmp_end_point = i - 1
                Exit For
            End If
        Next
        
        end_point = tmp_end_point
        
        Exit Function
        
    ElseIf (Mid(Target, start_point, 1) = "어") Then '값으로 변수 호출(v3) 또는 대입 없는 변수 호출
        tmp_end_point = -1
        
        n_start_point = start_point + 1 'N 값의 시작 부분
        
        tmp_end_point = is_called_for_v3_last_k - 1 'N 값의 끝 부분
        
        'N 값 계산
        tmp_number = CalculateNumber(Target, n_start_point, tmp_end_point, error_code)
        If (error_code < 0) Then
            ParseVariable = -1
            Exit Function
        End If
        
        ParseVariable = tmp_number
        is_called = True
        
        For i = tmp_end_point + 1 To restriction_end_point + 1 '변수 호출의 끝 부분 찾기
            If (i = restriction_end_point + 1) Then
                tmp_end_point = i - 1
            ElseIf Not (Mid(Target, i, 1) = "ㅋ" Or Mid(Target, i, 1) = "크") Then
                tmp_end_point = i
                Exit For
            End If
        Next
        
        end_point = tmp_end_point
        Exit Function
        
        is_called = True
        ParseVariable = 1
        Exit Function
    End If
    
    error_code = -201
    Exit Function
End Function

Function CalculateNumber(ByVal Target As Variant, ByVal start_point As Long, ByVal end_point As Long, ByRef error_code As Integer) As Long
'MsgBox "target:" & target & "start_point: " & start_point & "end_point: " & end_point
    Dim list_infix(), list_postfix(), stack_translate(), stack_calc() As Variant
    Dim prev_char As String
    Dim count As Long
    
    Dim number_of_uk As Integer
    number_of_uk = 0
    
    '첫 글자가 아닌 "엌"을 "어ㅋ"로 변환
    For i = start_point To end_point
        If (Mid(Target, i, 1) = "엌" And i <> 1) Then
            number_of_uk = number_of_uk + 1
            Exit For
        End If
    Next
    
    If (number_of_uk <> 0) Then
        Target = Left(Target, 1) & Replace(Target, "엌", "어ㅋ", 2)
    End If
    
    end_point = end_point + number_of_uk
    
    If (start_point > 1) Then
        prev_char = Mid(Target, start_point - 1, 1)
    Else '명령어의 첫 번째 글자부터 파싱됨
        prev_char = vbNullString
    End If
    count = 0
    
    For i = start_point To end_point
        If (Mid(Target, i, 1) <> prev_char) Then
            'prev_char = Mid(target, i, 1)
            count = count + 1
        End If
    Next
    
    '연산자와 피연산자를 담을 중위 표기 배열 생성
    ReDim list_infix(2 * count + 10), list_postfix(2 * count + 10), stack_translate(2 * count + 10), stack_calc(2 * count + 10)
    Dim list_infix_pointer As Long, list_postfix_pointer As Long, stack_translate_pointer As Long, stack_calc_pointer As Long
    Dim count_for_num As Variant
    Dim successive_minus As Long, variable_count As Long
    Dim var_end_point As Long
    Dim has_v3_var_pos As Long
    Dim is_minus As Boolean, is_called As Boolean, has_v3_var As Boolean
    
    has_v3_var = False
    has_v3_var_pos = 0
    
    If (start_point > 1) Then
        prev_char = Mid(Target, start_point - 1, 1)
    Else '명령어의 첫 번째 글자부터 파싱됨
        prev_char = vbNullString
    End If
    
    list_infix_pointer = -1
    list_postfix_pointer = -1
    stack_translate_pointer = -1
    stack_calc_pointer = -1
    
    count_for_num = 0
    is_minus = False
    successive_minus = 1
    
    variable_count = 0
    is_called = True
    
    var_end_point = 0
    
    If (start_point > end_point) Then '입력 영역의 길이가 음수일 때
        CalculateNumber = 0
        Exit Function
    End If
    
    Dim iii
    iii = start_point - 1
    Do
        iii = iii + 1
        If (iii > end_point + 1) Then
            Exit Do
        End If
        
        If (iii - 1 > 0) Then
            prev_char = Mid(Target, iii - 1, 1)
        Else '명령어의 첫 번째 글자부터 파싱됨
            prev_char = vbNullString
        End If
        
        If (iii = end_point + 1) Then '배열 끝 부분
            If (prev_char = "." Or prev_char = ",") Then '앞쪽에 상수가 있는 경우
            '앞쪽 수를 배열에 삽입
            list_infix_pointer = list_infix_pointer + 1
            list_infix(list_infix_pointer) = count_for_num
            count_for_num = 0
        ElseIf (prev_char = "어" Or prev_char = "?" Or prev_char = "ㅋ" Or prev_char = "크") Then '앞쪽에 변수 또는 입력값이 있는 경우
            '앞쪽 수를 배열에 삽입
            list_infix_pointer = list_infix_pointer + 1
            list_infix(list_infix_pointer) = count_for_num
            count_for_num = 0
        Else '매열 끝이 숫자가 아님
            CalculateNumber = 0
            error_code = -303 'Error 303
            Exit Function
        End If
        
        Exit Do
    End If
    
    Select Case Mid(Target, iii, 1)
        Case "." '증가 기호
            If (prev_char = "." Or prev_char = ",") Then '연이은 증가, 감소 후 증가
                count_for_num = count_for_num + 1
            ElseIf (iii = start_point) Then '맨 앞 증가 기호(주의 : "어(값)ㅋ"을 포함)
                count_for_num = 1
            ElseIf (prev_char = "어" Or prev_char = "?" Or prev_char = "ㅋ" Or prev_char = "크") Then '변수 + 1(주의: "어(값)ㅋ"와 혼동하면 안됨), 입력값 + 1
                '앞쪽 변수와 더하기 기호를 배열에 삽입 후 1부터 시작
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = 1
            ElseIf (prev_char = " ") Then '앞쪽에 곱하기 기호가 있는 경우
                '곱하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "m"
                
                count_for_num = 1
            End If
            
        Case "," '감소 기호
            If (prev_char = "." Or prev_char = ",") Then '연이은 감소, 증가 후 감소
                count_for_num = count_for_num - 1
            ElseIf (iii = start_point) Then '맨 앞 감소 기호
                count_for_num = -1
            ElseIf (prev_char = "어" Or prev_char = "?" Or prev_char = "ㅋ" Or prev_char = "크") Then '변수 + (-1), 입력값 + (-1)
                '앞쪽 변수와 더하기 기호를 배열에 삽입 후 -1부터 시작
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = -1
            ElseIf (prev_char = " ") Then '앞쪽에 곺하기 기호가 있는 경우
                '곱하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "m"
                
                count_for_num = -1
            End If
        
        Case " " '곱하기 기호
            If (iii = end_point) Then '곱하기 기호 뒤에 아무것도 없는 경우(오류)
                CalculateNumber = 0
                error_code = -304 'Error 304
                Exit Function
            ElseIf (prev_char = " ") Then '연이은 곱하기 기호
                error_code = 351 'Warn 351
            ElseIf (prev_char = "." Or prev_char = ",") Then '상수 뒤 곱하기 기호
                '앞쪽 수를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                count_for_num = 0
            ElseIf (prev_char = "어" Or prev_char = "?" Or prev_char = "ㅋ" Or prev_char = "크") Then '변수 또는 입력값 뒤 곱하기 기호
                '앞쪽 변수를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                count_for_num = 0
            Else '앞쪽 피연산자 없는 곱하기 기호
                CalculateNumber = 0
                error_code = -302 'Error 302
                Exit Function
            End If
        
        Case "어" '변수 호출(R-Value)
            If (prev_char = "." Or prev_char = ",") Then '상수 뒤 변수
                '앞쪽 수와 더하기 기호를 배열에 삼입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = 0
            ElseIf (prev_char = " ") Then '곱하기 기호 뒤 변수
                '곱하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "m"
            ElseIf (prev_char = "어" Or prev_char = "?" Or prev_char = "ㅋ" Or prev_char = "크") Then '입력값 및 변수("어...ㅋ") 뒤 변수
                '입력값과 더하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = 0
            ElseIf (iii = start_point) Then '첫 부분
                count_for_num = 0
            Else
            End If
            
            has_v3_var = False
            has_v3_var_pos = 0
            
            'v3 형태의 변수가 맨 앞에 있는지 확인
            For k = iii + 1 To end_point
                If (Mid(Target, k, 1) = "어") Then
                    'v3 형태의 변수가 맨 앞에 있음(맨 뒤의 ㅋ을 탐색)
                    For k2 = end_point To k + 1 Step -1
                        If (Mid(Target, k2, 1) = "ㅋ" Or Mid(Target, k2, 1) = "크") Then
                            has_v3_var = True
                            has_v3_var_pos = k2
                            Exit For
                        End If
                    Next
                    
                    Exit For
                ElseIf (k = end_point) Then
                    'v3 형태의 변수가 맨 앞에 있음(맨 앞의 ㅋ을 탐색)
                    For k3 = iii + 1 To end_point
                        If (Mid(Target, k3, 1) = "ㅋ" Or Mid(Target, k3, 1) = "크") Then
                            has_v3_var = True
                            has_v3_var_pos = k3
                            Exit For
                        End If
                    Next
                    
                    Exit For
                End If
            Next
            
            Dim result As Long
            result = 0
            
            If (has_v3_var) Then 'v3 형태의 변수가 맨 앞에 있으면
                Call RunCalculateNumber(Target, iii + 1, has_v3_var_pos - 1, error_code, result) '어~(맨 뒤의 ㅋ, 단, 마지막으로 나타나는 "어"면 첫 번째의 ㅋ) 사이에 있는 값 계산
                variable_count = result
                var_end_point = has_v3_var_pos
            Else '연음 형태의 변수가 맨 앞에 있으면
                variable_count = ParseVariable(Target, iii, end_point, var_end_point, is_called, error_code)
            End If
            
            If Not (is_called) Then
                CalculateNumber = 0
                error_code = -202 'Error 202
                Exit Function
            End If
            
            If Not (visual) Then '호출한 변수를 표시
                If (variable_count > 0) Then
                    Cells(4 + variable_count, 11).Font.ColorIndex = 33
                    Worksheets("변수").Cells(4 + variable_count, 3).Value = 1
                    If (updated_max_var < variable_count) Then '최대 변수
                        updated_max_var = variable_count
                    End If
                ElseIf (variable_count <= 0) Then
                    Cells(16 - variable_count, 8).Font.ColorIndex = 33
                    Worksheets("변수").Cells(5 - variable_count, 7).Value = 1
                    If (updated_min_var > variable_count) Then '최소 변수
                        updated_min_var = variable_count
                    End If
                End If
            End If
            
            count_for_num = count_for_num + FindVarPos(variable_count, error_code)
            iii = var_end_point '변수 부분 이후로 점프
            
        Case "식" '입력값
            If (prev_char = "." Or prev_char = ",") Then '상수 뒤 입력값
                '앞쪽 수와 더하기 기호를 배열에 삼입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = 0
            ElseIf (prev_char = " ") Then '곱하기 기호 뒤 입력값
                '곱하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "m"
            ElseIf (prev_char = "어" Or prev_char = "?") Then '변수 뒤 입력값, 입력값 뒤 입력값
                '앞 값과 더하기 기호를 배열에 삽입
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = count_for_num
                
                list_infix_pointer = list_infix_pointer + 1
                list_infix(list_infix_pointer) = "p"
                
                count_for_num = 0
            ElseIf (iii = start_point) Then '첫 부분
                count_for_num = 0
            Else
            End If
               
               
            '입력을 받는 형태인지 확인 후 맞으면 입력을 받음
            If Not (Mid(Target, iii + 1, 1) = "?") Then '식?의 형태가 아닐 경우
                CalculateNumber = 0
                error_code = -104 'Error 104
                Exit Function
            End If
            
            '입력받은 값 가져오기
            count_for_num = InputNum()
            iii = iii + 1
            
            If Not (is_running) Then '입력 도중 종료 명령이 들어왔을 때
                CalculateNumber = 0
                Exit Function
            End If
        
        Case "ㅋ", "크" '잉여 ㅋ
        
        
        Case Else '허용되지 않은 문자
            CalculateNumber = 0
            error_code = -305 'Error 305
            Exit Function
        End Select
    Loop
    
    Dim item As Variant
    
    '중위 표기를 후위 표기로 변경
    For i = 0 To list_infix_pointer
        item = list_infix(i)
        
        If (item = "p" Or item = "s") Then '더하기 또는 빼기
            If (stack_translate_pointer = -1) Then '스택이 비었을 때
                stack_translate_pointer = stack_translate_pointer + 1
                stack_translate(stack_translate_pointer) = item
            Else '곱하기 기호가 들어 있을 때
                list_postfix_pointer = list_postfix_pointer + 1
                list_postfix(list_postfix_pointer) = stack_translate(stack_translate_pointer)
                stack_translate(stack_translate_pointer) = item
            End If
        ElseIf (item = "m") Then '곱하기
            '스택이 비었을 때 and (더하기 or 빼기 기호가 들어 있을 때)
            stack_translate_pointer = stack_translate_pointer + 1
            stack_translate(stack_translate_pointer) = item
        Else '숫자
            list_postfix_pointer = list_postfix_pointer + 1
            list_postfix(list_postfix_pointer) = item
        End If
    Next
    
    '스택 비우기
    For i = stack_translate_pointer To 0 Step -1
        list_postfix_pointer = list_postfix_pointer + 1
        list_postfix(list_postfix_pointer) = stack_translate(i)
    Next
    
    '연산
    For i = 0 To list_postfix_pointer
        Dim temp_calc As Variant
        temp_calc = 0
        item = list_postfix(i)
        
        If (item = "p") Then '더하기 연산
            temp_calc = stack_calc(stack_calc_pointer - 1) + stack_calc(stack_calc_pointer)
            stack_calc(stack_calc_pointer - 1) = temp_calc
            stack_calc_pointer = stack_calc_pointer - 1
        ElseIf (item = "s") Then '빼기 연산
            temp_calc = stack_calc(stack_calc_pointer - 1) - stack_calc(stack_calc_pointer)
            stack_calc(stack_calc_pointer - 1) = temp_calc
            stack_calc_pointer = stack_calc_pointer - 1
        ElseIf (item = "m") Then '곱하기 연산
            temp_calc = stack_calc(stack_calc_pointer - 1) * stack_calc(stack_calc_pointer)
            stack_calc(stack_calc_pointer - 1) = temp_calc
            stack_calc_pointer = stack_calc_pointer - 1
        Else '숫자
            item = OverFlow(item) '오버플로 처리
            
            stack_calc_pointer = stack_calc_pointer + 1
            stack_calc(stack_calc_pointer) = item
        End If
    Next
    
    CalculateNumber = OverFlow(stack_calc(0)) '오버플로 처리
End Function

Function OverFlow(ByVal number As Variant) As Long '32비트 정수형 기준으로 처리
    Dim power31 As Variant
    power31 = 2 ^ 31
    
    Do
        For i = 62 To 32 Step -1
            If (number > (2 ^ i)) Then '양수 오버플로
                number = number - (2 ^ i)
            ElseIf (number < -(2 ^ i)) Then '음수 오버플로
                number = number + (2 ^ i)
            End If
        Next
        
        If (number > power31 - 1) Then '양수 오버플로
            number = number - power31 * 2 '2^32
        ElseIf (number < -1 * power31) Then '음수 오버플로
            number = number + power31 * 2 '2^32
        Else
            Exit Do
        End If
    Loop
    
    OverFlow = number
End Function

Function InputNum() As Long
    If (has_input_list) Then
        If Not IsEmpty(Worksheets("변수").Cells(4 + input_to_use, 10)) Then
            InputNum = Worksheets("변수").Cells(4 + input_to_use, 10).Value
            input_to_use = input_to_use + 1
            Exit Function
        Else '입력값이 다 떨어졌으면 새로 받기
        End If
    End If
    
    Application.ScreenUpdating = True
    Application.EnableEvents = True
    
    '입력 받기
    Dim input_num
    input_num = 0
    
    If Not (IsEmpty(Cells(current_console_row + 4, 6))) Then '현재 칸에 이미 출력이 있을 때
        current_console_row = current_console_row + 1
    End If
    Cells(current_console_row + 4, 6).NumberFormatLocal = "0" '셀 서식 변경
    
    is_on_console = True
    Cells(current_console_row + 4, 5).Value = ">"
    Cells(current_console_row + 4, 6).Select
    prev_cell_row = current_console_row + 4
    prev_cell_col = 6
    
    Do
        If Not (is_running) Then '입력 도중 종료 명령이 들어왔을 때
            InputNum = 0
            Exit Function
        End If
        
        If Not (Selection.Row = current_console_row + 4 And Selection.Column = 6) Then '셀 변경
            If Not (is_running) Then '입력 도중 종료 명령이 들어왔을 때
                InputNum = 0
                Exit Function
            End If
            
            If Not (IsEmpty(Cells(current_console_row + 4, 6))) Then '입력 O
                If Not (WorksheetFunction.IsNumber(Cells(current_console_row + 4, 6))) Then '입력받은 값이 숫자가 아님
                    Cells(current_console_row + 4, 6).Value = "(숫자가 아닌 값)"
                    is_on_console = True
                ElseIf (Int(Cells(current_console_row + 4, 6)) - Cells(current_console_row + 4, 6).Value <> 0) Then '입력받은 값이 정수가 아님
                    Cells(current_console_row + 4, 6).Value = "(정수가 아닌 값)"
                    is_on_console = True
                Else
                    current_console_row = current_console_row + 1
                    is_on_console = False
                    Exit Do
                End If
                
                current_console_row = current_console_row + 1
                Cells(current_console_row + 4, 5).Value = ">"
                Cells(current_console_row + 4, 6).NumberFormatLocal = "0" '셀 서식 변경
                Cells(current_console_row + 4, 6).Select
            Else '입력 X
                If Not (is_running) Then '입력 도중 종료 명령이 들어왔을 때
                    InputNum = 0
                    Exit Function
                End If
                
                Cells(current_console_row + 4, 6).Select
            End If
        End If
            
        Call Delay(0.01)
    Loop
    
    InputNum = Cells(current_console_row - 1 + 4, 6).Value
    
    If (visual) Then
        Application.ScreenUpdating = False
        Application.EnableEvents = False
    End If
    
End Function

Sub PrintToConsole(ByVal Target As Variant, ByRef error_code As Integer)
    Dim is_numeral As Boolean
    Dim character As String
    Dim output_num As Long, max_code As Long
    is_numeral = True
    character = vbNullString
    output_num = 0
    max_code = Worksheets("유니코드").Cells(1, 1).Value 'DB의 끝 부분
    
    output_num = ParseOutput(Target, is_numeral, error_code)
    
    If (error_code <> 0 Or Not is_running) Then '입력 중 중단 명령이 들어왔을 때
        Exit Sub
    End If
    
    If (current_console_row = 1) Then '첫 출력
        current_console_row = current_console_row + 1
        Cells(current_console_row + 4, 6).NumberFormatLocal = "0" '셀 서식 변경
    End If
    
    If (is_numeral) Then '정수
    	If (IsEmpty(Cells(current_console_row + 4, 6))) Then '빈 줄에 숫자 입력
    		Cells(current_console_row + 4, 6).NumberFormatLocal = "0" '셀 서식 변경
    	End If
        
        Cells(current_console_row + 4, 6).Value = Cells(current_console_row + 4, 6).Value & output_num
    Else '문자일 경우
        If (output_num < 0) Then '음수 코드
            error_code = -403
            Exit Sub
        End If
        
        If (Len(Target) = 2) Then '"식ㅋ" = 개행
            current_console_row = current_console_row + 1
        ElseIf (output_num = 0) Then '코드 0
        	current_console_row = current_console_row + 1
        	
            error_code = 151 'Warn 151
            Exit Sub
        Else
            If (output_num <= max_code) Then '범위 이하의 경우
                character = Worksheets("유니코드").Cells(output_num Mod 1000, (output_num \ 1000) * 3 + 3).Value '한셀 호환
            Else
            	character = WorksheetFunction.Unichar(output_num) 'MS Office Excel, LibreOffice Calc 전용
            End If
            
            Cells(current_console_row + 4, 6).NumberFormatLocal = "@" '셀 서식 변경
            
            Cells(current_console_row + 4, 6).Value = Cells(current_console_row + 4, 6).Value & character
        End If
    End If
    
    If (visual) Then
        Cells(current_console_row + 4, 6).Select
    End If
End Sub

Function ParseOutput(ByVal Target As Variant, ByRef is_numeral As Boolean, ByRef error_code As Integer) As Long
    Dim print_end_point As Long
    is_numeral = True '정수 혹은 문자
    print_end_point = 1
    
    If (Right(Target, 1) = "!") Then '정수
        print_end_point = i
    ElseIf (Right(Target, 1) = "ㅋ" Or Right(Target, 1) = "크") Then '문자
        is_numeral = False
        print_end_point = i
    Else '명령어 끝 부분에 "!", "ㅋ", "크"가 없음
        ParseOutput = -1
        error_code = -105 'Error 105
        Exit Function
    End If
    
    Dim output_num As Long
    output_num = 0
    output_num = CalculateNumber(Target, 2, Len(Target) - 1, error_code)
    
    ParseOutput = output_num
End Function

Sub RunCalculateNumber(ByVal Target As Variant, ByVal start_point As Long, ByVal end_point As Long, ByRef error_code As Integer, ByRef result As Long)
    result = CalculateNumber(Target, start_point, end_point, error_code)
End Sub

Function FindVarPos(ByVal index As Long, ByRef error_code As Integer) As Variant
    If (index > 0) Then
        FindVarPos = Cells(4 + index, 11).Value
    ElseIf (index <= 0) Then
        FindVarPos = Cells(16 - index, 8).Value
    End If
End Function

Sub MapVar(ByVal index As Long, ByVal num As Long, ByRef error_code As Integer)
    If (index > 0) Then
        If (index > 1048572) Then
            error_code = -204
            Exit Sub
        End If
        
        Cells(4 + index, 11).Value = num
    ElseIf (index <= 0) Then
        If (index < -1048560) Then
            error_code = -204
            Exit Sub
        End If
        
        Cells(16 - index, 8).Value = num
    End If
End Sub

Sub Enable_visual()
	visual = False
End Sub

Sub Disable_visual()
	visual = True
End Sub
