
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports Org.BouncyCastle.Asn1.X509


Public Class Form1


    Dim connection As String = "server=127.0.0.1; user=root; database=inventory_stock_DB; password="
    Dim Con As New MySqlConnection(connection)


    Private Sub clearBtn()
        ItemName.Text = ""
        Quantity.Text = ""
        UnitPrice.Text = ""
        Items.Text = ""
        roomNo.Text = ""
        QuantityForDistribute.Text = ""
        ItemId2.Text = ""
        scheduleId.Text = ""
        roomNumber.Text = ""
        schedule.Text = ""
        prioroty.Text = ""
        status.Text = ""
    End Sub
    Private Sub populateStock()
        Con.Open()
        Dim sql As String = "SELECT id, ItemName, Stocks, TotalPrice FROM Stock_Item"
        Dim cmd As New MySqlCommand(sql, Con)
        Dim adapter As New MySqlDataAdapter(cmd)
        Dim builder As New MySqlCommandBuilder(adapter)
        Dim ds As New DataSet()
        adapter.Fill(ds)
        DVGstock.DataSource = ds.Tables(0)
        Con.Close()
    End Sub
    Private Sub populateDistributed()
        Con.Open()
        Dim sql As String = "SELECT id, Item, RoomNo, Quantity FROM Distributed_Item"
        Dim cmd As New MySqlCommand(sql, Con)
        Dim adapter As New MySqlDataAdapter(cmd)
        Dim builder As New MySqlCommandBuilder(adapter)
        Dim ds As New DataSet()
        adapter.Fill(ds)
        DVGdistributed.DataSource = ds.Tables(0)
        Con.Close()
    End Sub

    Private Sub populatSchedule()
        Con.Open()
        Dim sql As String = "SELECT * FROM scheduling"
        Dim cmd As New MySqlCommand(sql, Con)
        Dim adapter As New MySqlDataAdapter(cmd)
        Dim builder As New MySqlCommandBuilder(adapter)
        Dim ds As New DataSet()
        adapter.Fill(ds, "scheduling")
        schedDVG.DataSource = ds.Tables("scheduling")

        Con.Close()
    End Sub

    Private Sub loadItems()
        Try
            ' Assuming Con is a MySqlConnection object declared and initialized somewhere else in your code
            ' Also, make sure to replace "YourConnectionString" with your actual connection string

            Con.Open()

            Dim query As String = "SELECT ItemName FROM Stock_Item"
            Using command As New MySqlCommand(query, Con)
                Dim adapter As New MySqlDataAdapter(command)
                Dim ds As New DataSet()

                ' Fill the dataset with data from the database
                adapter.Fill(ds)

                ' Clear existing items in the ComboBox before adding new ones
                Items.Items.Clear()

                ' Check if there are any rows in the dataset
                If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                    ' Iterate through each row in the dataset and add the item names to the ComboBox
                    For Each row As DataRow In ds.Tables(0).Rows
                        ' Assuming "ItemName" is the column name in your database
                        Items.Items.Add(row("ItemName").ToString())
                    Next
                Else
                    MsgBox("No items found in the database.")
                End If
            End Using

        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        Finally
            ' Always close the connection, whether an exception occurred or not
            If Con.State = ConnectionState.Open Then
                Con.Close()
            End If
        End Try
    End Sub


    'Private Sub LoadItemName()

    'Try
    '    Con.Open()
    '    Dim query As String = "SELECT ItemName FROM Stock_Item WHERE id = @id"
    '    Dim reader As MySqlDataReader
    '    Using command As New MySqlCommand(query, Con)
    '        ' Assuming you have a parameter named @id in your query
    '        command.Parameters.AddWithValue("@id", ItemId.Text)

    '        reader = command.ExecuteReader()
    '        While reader.Read
    '            ' Corrected the case of "itemName" to "ItemName"
    '            Dim itemName As String = reader.GetString("ItemName")
    '            Items.Items.Add(itemName)
    '        End While
    '    End Using
    'Catch ex As Exception
    '    MsgBox("An error occurred: " & ex.Message)
    'Finally
    '    ' Ensure that the connection is closed, whether an exception occurred or not
    '    If Con.State = ConnectionState.Open Then
    '        Con.Close()
    '    End If
    'End Try

    'End Sub




    Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label7.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TabControl1.SelectedTab = TabPage1
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TabControl1.SelectedTab = TabPage2
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs)
    End Sub
    Private Sub FetchItemId()
        Try

            Con.Open()
            Dim query As String = "SELECT id FROM Stock_Item WHERE ItemName = '" & Items.Text & "'"
            Using cmd As New MySqlCommand(query, Con)
                ' Assuming Items.SelectedValue is a string, you may need to adjust the DbType accordingly.
                'cmd.Parameters.AddWithValue("@ItemName", Items.SelectedValue.ToString())
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        ItemId2.Text = reader("id").ToString()
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' Handle exceptions here, log them, or display an error message.
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
        Con.Close()




    End Sub
    Private Sub FetchRoom()
        Dim connections As String = "server=127.0.0.1; user=root; database=roomdb; password="

        Try
            Using Connn As New MySqlConnection(connections)
                Connn.Open()
                Dim query As String = "SELECT RoomNo FROM RoomAvailability WHERE Status = 'AVAILABLE'"
                Dim reader As MySqlDataReader
                Using command As New MySqlCommand(query, Connn)
                    reader = command.ExecuteReader
                    While reader.Read
                        Dim roomNumbers = reader.GetString("RoomNo")
                        roomNo.Items.Add(roomNumbers)
                        roomNumber.Items.Add(roomNumbers)
                    End While
                    Connn.Close()
                End Using
            End Using

        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        End Try
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populateStock()
        populateDistributed()
        loadItems()
        FetchRoom()
        populatSchedule()

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            If String.IsNullOrWhiteSpace(ItemName.Text) OrElse String.IsNullOrWhiteSpace(Quantity.Text) OrElse String.IsNullOrWhiteSpace(UnitPrice.Text) Then
                MsgBox("Please add an Item.")
            Else
                Con.Open()
                Dim query As String = "INSERT INTO Stock_Item (id, ItemName, Stocks, TotalPrice) VALUES (@id, @itemName, @Quantity, @totalPrice)"
                Using cmd As New MySqlCommand(query, Con)
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString().Substring(0, 6))
                    cmd.Parameters.AddWithValue("@itemName", ItemName.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@Quantity", Integer.Parse(Quantity.Text))
                    cmd.Parameters.AddWithValue("@totalPrice", Decimal.Parse(UnitPrice.Text))
                    cmd.ExecuteNonQuery()
                    Con.Close()
                End Using
                MsgBox("Item added successfully!")
                populateStock()
                'LoadItemName()
                loadItems()
                clearBtn()
            End If
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        End Try

    End Sub

    Private Sub DGVshuttle_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DVGstock.CellContentClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Me.DVGstock.Rows(e.RowIndex)

            ItemId.Text = row.Cells("id").Value.ToString()
            ItemName.Text = row.Cells("ItemName").Value.ToString()
            Quantity.Text = row.Cells("Stocks").Value.ToString()
            UnitPrice.Text = row.Cells("TotalPrice").Value.ToString()

        End If

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If ItemId.Text = "" Then
            MsgBox("Select item to be updated")
        Else
            Con.Open()
            Dim sql = "UPDATE Stock_Item SET ItemName=@itemName, Stocks=@Quantity, TotalPrice=@totalPrice WHERE id=@ItemId"
            Dim cmd As New MySqlCommand(sql, Con)
            cmd.Parameters.AddWithValue("@itemName", ItemName.Text)
            cmd.Parameters.AddWithValue("@Quantity", Quantity.Text)
            cmd.Parameters.AddWithValue("@totalPrice", UnitPrice.Text)
            cmd.Parameters.AddWithValue("@ItemId", ItemId.Text)
            cmd.ExecuteNonQuery()
            MsgBox("Item Updated!")
            Con.Close()
            populateStock()
            clearBtn()

        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If ItemId.Text = "" Then
            MsgBox("Select Item to be deleted!")
        Else
            Con.Open()

            Dim query As String = "DELETE FROM Stock_Item WHERE id= @id"
            Dim cmd As MySqlCommand
            cmd = New MySqlCommand(query, Con)
            cmd.Parameters.AddWithValue("@id", ItemId.Text)
            cmd.ExecuteNonQuery()
            MsgBox("Item deleted!")
            Con.Close()
            populateStock()
            clearBtn()

        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        clearBtn()
    End Sub

    Public Sub stock()
        Try
            ' Step 1: Connect to the database
            Con.Open()

            ' Step 2: Retrieve the current stock quantity
            Dim getCurrentStockQuery As String = "SELECT Stocks FROM Stock_Item WHERE id = @productId"
            Using cmdGetCurrentStock As New MySqlCommand(getCurrentStockQuery, Con)
                ' Use parameterized query to prevent SQL injection
                cmdGetCurrentStock.Parameters.AddWithValue("@productId", ItemId2.Text)

                ' Execute the command to get the current stock value
                Dim currentStock As Integer = Convert.ToInt32(cmdGetCurrentStock.ExecuteScalar())
                Dim quantityToDeduct As Integer = Convert.ToInt32(QuantityForDistribute.Text)
                If currentStock >= quantityToDeduct Then
                    Dim newStock As Integer = currentStock - quantityToDeduct

                    ' Step 3: Update the stock quantity in the database
                    Dim updateStockQuery As String = "UPDATE Stock_Item SET Stocks = @newStock WHERE id = @productId"
                    Using cmdUpdateStock As New MySqlCommand(updateStockQuery, Con)
                        ' Use parameterized query to prevent SQL injection
                        cmdUpdateStock.Parameters.AddWithValue("@newStock", newStock)
                        cmdUpdateStock.Parameters.AddWithValue("@productId", ItemId2.Text)
                        cmdUpdateStock.ExecuteNonQuery()

                        MsgBox("Item was distributed")
                        Dim query As String = "INSERT INTO Distributed_Item (id, Item, RoomNo, Quantity) VALUES (@id, @itemName, @roomNo, @quantity)"
                        Using cmd As New MySqlCommand(query, Con)
                            cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString().Substring(0, 6))
                            cmd.Parameters.AddWithValue("@itemName", Items.Text.ToUpper())
                            cmd.Parameters.AddWithValue("@roomNo", roomNo.Text)
                            cmd.Parameters.AddWithValue("@quantity", Integer.Parse(QuantityForDistribute.Text))
                            cmd.ExecuteNonQuery()
                            clearBtn()
                        End Using
                    End Using
                Else
                    MsgBox("Insufficient stocks")
                End If
            End Using
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        Finally
            ' Ensure that the connection is closed, whether an exception occurred or not
            If Con.State = ConnectionState.Open Then
                Con.Close()
            End If
        End Try
    End Sub


    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Try
            If Items.SelectedItem Is Nothing OrElse String.IsNullOrWhiteSpace(roomNo.Text.ToString()) OrElse String.IsNullOrWhiteSpace(QuantityForDistribute.Text) Then
                MsgBox("Add item to be distributed")
            Else
                stock()
                populateDistributed()
                populateStock()


            End If
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        End Try


    End Sub




    Private Sub DVGdistributed_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DVGdistributed.CellContentClick

    End Sub

    Private Sub ItemName_TextChanged(sender As Object, e As EventArgs) Handles ItemName.TextChanged

    End Sub

    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click

    End Sub

    Private Sub Items_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Items.SelectedIndexChanged
        FetchItemId()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs)
        'LoadItemName()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs)
        'LoadItemName()
    End Sub

    Private Sub roomNo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles roomNo.SelectedIndexChanged
        'FetchRoom()
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        Application.Exit()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        clearBtn()
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        TabControl1.SelectedTab = TabPage3
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Try
            If String.IsNullOrWhiteSpace(roomNumber.Text) OrElse String.IsNullOrWhiteSpace(schedule.Text) OrElse String.IsNullOrWhiteSpace(prioroty.Text) Then
                MsgBox("Please comp.lete details.")
            Else

                Con.Open()
                Dim query As String = "INSERT INTO Scheduling (id, RoomNo, Schedule, Priority, Status) VALUES (@id, @roomNo, NOW(), @priority, @status)"
                Dim sched As DateTime = schedule.Value
                Using cmd As New MySqlCommand(query, Con)
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString().Substring(0, 6))
                    cmd.Parameters.AddWithValue("@roomNo", roomNumber.Text.ToUpper())
                    cmd.Parameters.AddWithValue("@sched", sched)
                    cmd.Parameters.AddWithValue("@priority", prioroty.Text)
                    cmd.Parameters.AddWithValue("@status", status.Text)
                    cmd.ExecuteNonQuery()
                    Con.Close()
                End Using
                MsgBox("adding schedule successfully!")
                populatSchedule()
                clearBtn()
            End If
        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        End Try
    End Sub

    Private Sub roomNumber_SelectedIndexChanged(sender As Object, e As EventArgs) Handles roomNumber.SelectedIndexChanged

    End Sub

    Private Sub schedDVG_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles schedDVG.CellContentClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = Me.schedDVG.Rows(e.RowIndex)
            scheduleId.Text = row.Cells("id").Value.ToString()
            roomNumber.Text = row.Cells("RoomNo").Value.ToString()
            status.Text = row.Cells("Status").Value.ToString()
            prioroty.Text = row.Cells("Priority").Value.ToString()
            schedule.Text = row.Cells("Schedule").Value.ToString()

        End If
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If scheduleId.Text = "" Then
            MsgBox("Please select you want to be cleaning")
        Else
            status.Text = "Under Cleaning"
            Con.Open()
            Dim updateSched As String = "UPDATE scheduling SET Status = @status WHERE id = @schedId"
            Using updateStatus As New MySqlCommand(updateSched, Con)
                ' Use parameterized query to prevent SQL injection
                updateStatus.Parameters.AddWithValue("@schedId", scheduleId.Text)
                updateStatus.Parameters.AddWithValue("@status", status.Text.ToUpper)
                updateStatus.ExecuteNonQuery()
                Con.Close()
                populatSchedule()
                clearBtn()
            End Using
        End If

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If scheduleId.Text = "" Then
            MsgBox("Please select you want to be cleaned")
        Else
            status.Text = "Cleaned"
            Con.Open()
            Dim updateSched As String = "UPDATE scheduling SET Status = @status WHERE id = @schedId"
            Using updateStatus As New MySqlCommand(updateSched, Con)
                ' Use parameterized query to prevent SQL injection
                updateStatus.Parameters.AddWithValue("@schedId", scheduleId.Text)
                updateStatus.Parameters.AddWithValue("@status", status.Text)
                updateStatus.ExecuteNonQuery()
                Con.Close()
                populatSchedule()
                clearBtn()
            End Using
        End If

    End Sub
End Class
