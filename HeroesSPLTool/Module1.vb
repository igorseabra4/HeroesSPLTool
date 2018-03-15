Imports System.IO

Module Module1

    Public Structure Vertex
        Dim X As Single
        Dim Y As Single
        Dim Z As Single
    End Structure

    Sub Main()
        If Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator <> "." Then
            Threading.Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo("en-US")
        End If

        Dim Arguments As String() = Environment.GetCommandLineArgs()

        Console.WriteLine("Heroes Spline Tool by igorseabra4")

        If Arguments.Count > 1 Then
            For i = 1 To Arguments.Count - 1
                If Arguments(i).Substring(Arguments(i).Length - 3, 3).ToLower = "obj".ToLower Then
                    ConvertOBJToSPL(Arguments(i))
                End If
                If Arguments(i).Substring(Arguments(i).Length - 3, 3).ToLower = "spl".ToLower Then
                    ConvertSPLToOBJ(Arguments(i))
                End If
            Next
        Else
            Dim FilesInFolder As String() = Directory.GetFiles(Directory.GetCurrentDirectory)

            For Each i As String In FilesInFolder
                If i.Substring(i.Length - 3, 3).ToLower = "obj".ToLower Then
                    ConvertOBJToSPL(i)
                End If
                If i.Substring(i.Length - 3, 3).ToLower = "spl".ToLower Then
                    ConvertSPLToOBJ(i)
                End If
            Next
        End If
        Console.ReadKey()
    End Sub

    Sub ConvertOBJToSPL(FileName As String)
        Console.WriteLine("Reading " + FileName)
        Dim StringStream As String() = File.ReadAllLines(FileName)

        Dim VertexStream As New List(Of Vertex)

        For Each k As String In StringStream
            If k.Length > 2 Then
                If k.Substring(0, 2) = "v " Then
                    Dim TempVertex As New Vertex
                    TempVertex.X = Convert.ToSingle(k.Split(" ")(k.Split.Count - 3))
                    TempVertex.Y = Convert.ToSingle(k.Split(" ")(k.Split.Count - 2))
                    TempVertex.Z = Convert.ToSingle(k.Split(" ")(k.Split.Count - 1))
                    VertexStream.Add(TempVertex)
                End If
            End If
        Next

        Console.WriteLine("Creating " + Path.ChangeExtension(FileName, "spl"))

        Dim SPLWriter As New BinaryWriter(New FileStream(Path.ChangeExtension(FileName, "spl"), FileMode.Create))

        SPLWriter.Write({&HC, 0, 0, 0, 0, 0, 0, 0, &HFF, &HFF, 0, &H14})
        SPLWriter.BaseStream.Position = &H2C
        SPLWriter.Write(VertexStream.Count)
        SPLWriter.Write({1, 0, 0, 0})

        For Each i As Vertex In VertexStream
            SPLWriter.Write(i.X)
            SPLWriter.Write(i.Y)
            SPLWriter.Write(i.Z)
        Next

        SPLWriter.BaseStream.Position = 4
        SPLWriter.Write(Convert.ToUInt32(SPLWriter.BaseStream.Length - &HC))

        Console.WriteLine("Success.")
    End Sub

    Sub ConvertSPLToOBJ(FileName As String)
        Console.WriteLine("Reading " + FileName)

        Dim VertexStream As New List(Of Vertex)

        Dim SPLReader As New BinaryReader(New FileStream(FileName, FileMode.Open))

        SPLReader.BaseStream.Position = &H2C
        Dim AmountOfVertices As Integer = SPLReader.ReadUInt32

        SPLReader.BaseStream.Position = &H34

        For i = 1 To AmountOfVertices
            Dim TempVertex As New Vertex
            TempVertex.X = SPLReader.ReadSingle
            TempVertex.Y = SPLReader.ReadSingle
            TempVertex.Z = SPLReader.ReadSingle
            VertexStream.Add(TempVertex)
        Next

        Console.WriteLine("Creating " + Path.ChangeExtension(FileName, "obj"))
        Dim SplineOBJWriter As New StreamWriter(New FileStream(Path.ChangeExtension(FileName, "obj"), FileMode.Create))

        SplineOBJWriter.WriteLine("#Spline exported by Heroes Spline Tool")
        SplineOBJWriter.WriteLine()

        For Each i As Vertex In VertexStream
            SplineOBJWriter.WriteLine("v " + i.X.ToString + " " + i.Y.ToString + " " + i.Z.ToString)
        Next

        SplineOBJWriter.WriteLine()

        Dim Everynumber As String = ""

        For i = 1 To VertexStream.Count
            Everynumber += " " + i.ToString
        Next

        SplineOBJWriter.WriteLine("g " + "spline_" + FileName.Split("\").Last.Substring(0, FileName.Split("\").Last.Count - 4))
        SplineOBJWriter.WriteLine("l" + Everynumber.ToArray)

        SplineOBJWriter.Close()

        Console.WriteLine("Success.")
    End Sub
End Module
