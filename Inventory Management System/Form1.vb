
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

    Private Sub LoadItemName()
        Try
            Con.Open()
            Dim query As String = "SELECT ItemName FROM Stock_Item"
            Dim reader As MySqlDataReader
            Using command As New MySqlCommand(query, Con)
                reader = command.ExecuteReader
                While reader.Read
                    Dim itemName = reader.GetString("itemName")
                    Items.Items.Add(itemName)
                End While
                Con.Close()
            End Using

        Catch ex As Exception
            MsgBox("An error occurred: " & ex.Message)
        End Try

    End Sub




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
                        Dim roomNumber = reader.GetString("RoomNo")
                        roomNo.Items.Add(roomNumber)
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
        LoadItemName()
        FetchRoom()

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
                LoadItemName()
                populateStock()
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


    Private Sub stock()
        Try
            ' Replace with your actual product ID
            Con.Open()
            Dim quantityToDistribute As Integer = CInt(QuantityForDistribute.Text) ' Assuming txtQuantity is a TextBox for entering quantity

            ' Start a transaction
            Using transaction As MySqlTransaction = Con.BeginTransaction()
                Try
                    ' Check if there's enough stock before updating

                    Dim checkStockQuery As String = "SELECT Stocks FROM Stock_Item WHERE id = @id"
                    Using checkStockCommand As New MySqlCommand(checkStockQuery, Con)
                        ' Use ItemId2.Text instead of ItemId2
                        checkStockCommand.Parameters.AddWithValue("@id", ItemId2.Text)
                        Dim currentStock As Integer = CInt(checkStockCommand.ExecuteScalar())

                        If currentStock >= quantityToDistribute Then
                            ' Update the stock
                            Dim updateStockQuery As String = "UPDATE Stock_Item SET Stocks = Stocks - @Quantity WHERE id = @id"
                            Using updateStockCommand As New MySqlCommand(updateStockQuery, Con)
                                updateStockCommand.Parameters.AddWithValue("@Quantity", quantityToDistribute)
                                ' Use ItemId2.Text instead of ItemId2
                                updateStockCommand.Parameters.AddWithValue("@id", ItemId2.Text)

                                updateStockCommand.ExecuteNonQuery()

                                MessageBox.Show("Stock distributed successfully.")

                                ' Commit the transaction if everything is successful
                                transaction.Commit()

                                ' Open the connection just before executing the second query

                                Dim insertQuery As String = "INSERT INTO Distributed_Item (id, Item, RoomNo, Quantity) VALUES (@id, @itemName, @roomNo, @Quantity)"
                                Using cmd As New MySqlCommand(insertQuery, Con)
                                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString().Substring(0, 6))
                                    cmd.Parameters.AddWithValue("@itemName", Items.Text.ToUpper())
                                    cmd.Parameters.AddWithValue("@roomNo", roomNo.Text.ToUpper())
                                    cmd.Parameters.AddWithValue("@Quantity", Integer.Parse(QuantityForDistribute.Text))
                                    cmd.ExecuteNonQuery()

                                End Using
                            End Using
                        Else
                            MessageBox.Show("Insufficient stock.")
                        End If
                    End Using
                Catch ex As Exception
                    ' Handle exceptions here, log them, or display an error message.
                    MessageBox.Show("An error occurred: " & ex.Message)
                    ' Rollback the transaction in case of an error
                    transaction.Rollback()
                End Try
            End Using
        Catch ex As Exception
            ' Handle exceptions here, log them, or display an error message.
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally

        End Try
        Con.Close()
        clearBtn()
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
        'FetchItemId()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs)
        LoadItemName()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        LoadItemName()
    End Sub

    Private Sub roomNo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles roomNo.SelectedIndexChanged
        'FetchRoom()
    End Sub

    Private Sub Button9_Click_1(sender As Object, e As EventArgs) Handles Button9.Click
        Application.Exit()
    End Sub
End Class
