Imports System.Convert
Imports System.Security.Cryptography
Imports System.Text.Encoding

Module modMain
    'Les entêtes et les tailles des réseaux SFNNv4 et SFNNv5 sont identiques
    Sub Main()
        Dim fichierEXE As String, lectureEXE As IO.FileStream
        Dim pos As Long, tailleEXE As Long, tailleTampon As Integer
        Dim tabEXE() As Byte, tabTampon() As Byte
        Dim tabNNUE() As Byte, tailleNNUE As Long, offsetNNUE As Long
        Dim entete20M As String, offset20M As Long
        Dim entete30M As String, offset30M As Long
        Dim enteteSFNNv0 As String, offsetSFNNv0 As Long 'SFNNv0 SFdev  (18/05/2021) nn-8a08400ed089 (16/05/2021)
        Dim enteteSFNNv1 As String, offsetSFNNv1 As Long 'SFNNv1 SF14.0 (02/07/2021) nn-3475407dc199 (27/06/2021)
        Dim enteteSFNNv2 As String, offsetSFNNv2 As Long 'SFNNv2 SFdev  (15/08/2021) nn-e8321e467bf6 (09/08/2021)
        Dim enteteSFNNv3 As String, offsetSFNNv3 As Long 'SFNNv3 SF14.1 (28/10/2021) nn-13406b1dcbe0 (13/09/2021)
        Dim enteteSFNNv4 As String, offsetSFNNv4 As Long 'SFNNv4 SFdev  (10/02/2022) nn-6877cd24400e (09/02/2022)
        Dim enteteSFNNv5 As String, offsetSFNNv5 As Long 'SFNNv5 SFdev  (14/05/2022) nn-3c0aa92af1da (13/05/2022)
        Dim mySHA256 As SHA256, hash As String

        mySHA256 = SHA256Managed.Create()

        fichierEXE = Replace(Command(), """", "")

        tabTampon = {ToInt64("16", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("ee", 16), ToInt64("a6", 16), ToInt64("5a", 16), ToInt64("3e", 16), ToInt64("b1", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        entete20M = Unicode.GetString(tabTampon) ' & "Features=HalfKP(Friend)[41024->256x2]"

        tabTampon = {ToInt64("16", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("ce", 16), ToInt64("a7", 16), ToInt64("5a", 16), ToInt64("3e", 16), ToInt64("b1", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        entete30M = Unicode.GetString(tabTampon) ' & "Features=HalfKP(Friend)[41024->384x2]"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("72", 16), ToInt64("3e", 16), ToInt64("10", 16), ToInt64("3c", 16), ToInt64("b1", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv0 = Unicode.GetString(tabTampon) ' & "Features=HalfKA(Friend)[49216->256x2]"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("72", 16), ToInt64("3e", 16), ToInt64("10", 16), ToInt64("3c", 16), ToInt64("4b", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv1 = Unicode.GetString(tabTampon) ' & "Network trained with the"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("ec", 16), ToInt64("2e", 16), ToInt64("10", 16), ToInt64("1c", 16), ToInt64("b1", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv2 = Unicode.GetString(tabTampon) ' & "Network trained with the"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("ec", 16), ToInt64("2e", 16), ToInt64("10", 16), ToInt64("1c", 16), ToInt64("4b", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv3 = Unicode.GetString(tabTampon) ' & "Network trained with the"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("f2", 16), ToInt64("2e", 16), ToInt64("10", 16), ToInt64("1c", 16), ToInt64("4b", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv4 = Unicode.GetString(tabTampon) ' & "Network trained with the"

        tabTampon = {ToInt64("20", 16), ToInt64("2f", 16), ToInt64("f3", 16), ToInt64("7a", 16), ToInt64("f2", 16), ToInt64("2e", 16), ToInt64("10", 16), ToInt64("1c", 16), ToInt64("4b", 16), ToInt64("0", 16), ToInt64("0", 16), ToInt64("0", 16)}
        enteteSFNNv5 = Unicode.GetString(tabTampon) ' & "Network trained with the"

        tailleEXE = FileLen(fichierEXE)
        lectureEXE = New IO.FileStream(fichierEXE, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)

        ReDim tabEXE(tailleEXE - 1)
        lectureEXE.Read(tabEXE, 0, tabEXE.Length)
        lectureEXE.Close()

        tailleTampon = 1000
        ReDim tabTampon(tailleTampon - 1)
        ReDim tabNNUE(0)
        For pos = 0 To UBound(tabEXE) Step tailleTampon
            Array.Copy(tabEXE, pos, tabTampon, 0, tailleTampon)

            offset20M = InStr(Unicode.GetString(tabTampon), entete20M, CompareMethod.Text)
            offset30M = InStr(Unicode.GetString(tabTampon), entete30M, CompareMethod.Text)
            offsetSFNNv0 = InStr(Unicode.GetString(tabTampon), enteteSFNNv0, CompareMethod.Text)
            offsetSFNNv1 = InStr(Unicode.GetString(tabTampon), enteteSFNNv1, CompareMethod.Text)
            offsetSFNNv2 = InStr(Unicode.GetString(tabTampon), enteteSFNNv2, CompareMethod.Text)
            offsetSFNNv3 = InStr(Unicode.GetString(tabTampon), enteteSFNNv3, CompareMethod.Text)
            offsetSFNNv4 = InStr(Unicode.GetString(tabTampon), enteteSFNNv4, CompareMethod.Text)
            offsetSFNNv5 = InStr(Unicode.GetString(tabTampon), enteteSFNNv5, CompareMethod.Text)
            
            If offset20M > 0 Then

                offsetNNUE = pos + 2 * offset20M - 2
                tailleNNUE = 21022697
                Exit For

            ElseIf offset30M > 0 Then

                offsetNNUE = pos + 2 * offset30M - 2
                tailleNNUE = 31533289
                Exit For

            ElseIf offsetSFNNv0 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv0 - 2
                tailleNNUE = 47721473
                Exit For

            ElseIf offsetSFNNv1 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv1 - 2
                tailleNNUE = 47721371
                Exit For

            ElseIf offsetSFNNv2 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv2 - 2
                tailleNNUE = 47001345
                Exit For

            ElseIf offsetSFNNv3 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv3 - 2
                tailleNNUE = 47001243
                Exit For

            ElseIf offsetSFNNv4 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv4 - 2
                tailleNNUE = 47001499
                Exit For

            ElseIf offsetSFNNv5 > 0 Then

                offsetNNUE = pos + 2 * offsetSFNNv5 - 2
                tailleNNUE = 47001499
                Exit For

            End If

            Console.Title = Format(pos / UBound(tabEXE), "0.00%")
            Threading.Thread.Sleep(1)
        Next

        ReDim tabNNUE(tailleNNUE - 1)
        Array.Copy(tabEXE, offsetNNUE, tabNNUE, 0, tailleNNUE)

        tabTampon = mySHA256.ComputeHash(tabNNUE)
        hash = ""
        For i = 0 To 7
            hash = hash & hexa(tabTampon(i))
        Next

        My.Computer.FileSystem.WriteAllBytes("nn-" & LCase(Microsoft.VisualBasic.Left(hash, 12)) & " (embedded).nnue", tabNNUE, False)
    End Sub

    Public Function hexa(valeur As Integer) As String
        Dim chaine As String

        chaine = Hex(valeur)
        If Len(chaine) = 1 Then
            chaine = "0" & chaine
        End If
        Return chaine

    End Function


End Module
