Imports System
Imports System.Data
Imports System.Collections
Imports System.Web.UI
Imports Telerik.Web.UI

Public Class RecordTypeConfigDetails
    Inherits System.Web.UI.UserControl

    Private _dataItem As Object = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
    End Sub

#Region "Web Form Designer generated code"

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        InitializeComponent()
        MyBase.OnInit(e)
    End Sub

    Private Sub InitializeComponent()
        AddHandler DataBinding, AddressOf Me.RecordTypeConfigDetails_DataBinding
    End Sub

#End Region

    Public Property DataItem() As Object
        Get
            Return Me._dataItem
        End Get
        Set(ByVal value As Object)
            Me._dataItem = value
        End Set
    End Property


    Protected Sub RecordTypeConfigDetails_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim contValue As Object = DataBinder.Eval(DataItem, "ts_fg")
        Dim record_type_id As Object = DataBinder.Eval(DataItem, "record_type_id")

        If record_type_id.Equals(DBNull.Value) Then
            lblHeading.Text = "Add New Record-Type"
        Else
            lblHeading.Text = "Edit Record-Type"
        End If

        If contValue.ToString = "True" Then
            lblContorNoncont.Text = "Has been set to:"
            lblCONStatus.Text = "Time-series"
            lblCONStatus.Visible = True
            rblContorNoncont.Visible = False
            rfvContorNoncont.Visible = False
        ElseIf contValue.ToString = "False" Then
            lblContorNoncont.Text = "Has been set to:"
            lblCONStatus.Text = "Non-time-series"
            lblCONStatus.Visible = True
            rblContorNoncont.Visible = False
            rfvContorNoncont.Visible = False
        ElseIf contValue.Equals(DBNull.Value) Then
            lblContorNoncont.Text = "Choose one:"
            rblContorNoncont.Visible = True
            lblCONStatus.Visible = False
            rfvContorNoncont.Visible = True
        End If
    End Sub

End Class