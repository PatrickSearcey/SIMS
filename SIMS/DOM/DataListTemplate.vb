Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Net

Public Class DataListTemplate
    Implements ITemplate

    Dim _itemType As ListItemType
    Private _no As String
    Private _view As String
    Private _recordType As String
    Private _wy As String
    Private _count As Integer

    Public Property NoOfPeriods() As String
        Get
            Return _no
        End Get
        Set(ByVal Value As String)
            _no = Value
        End Set
    End Property

    Public Property WY() As String
        Get
            Return _wy
        End Get
        Set(ByVal value As String)
            _wy = value
        End Set
    End Property

    Public Property RecordType() As String
        Get
            Return _recordType
        End Get
        Set(ByVal Value As String)
            _recordType = Value
        End Set
    End Property

    Public Property Count() As Integer
        Get
            Return _count
        End Get
        Set(ByVal value As Integer)
            _count = value
        End Set
    End Property

    Public Property WhichView() As String
        Get
            Return _view
        End Get
        Set(ByVal value As String)
            _view = value
        End Set
    End Property

    Public Sub New(ByVal Type As ListItemType)
        _itemType = Type
    End Sub

    Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements ITemplate.InstantiateIn
        Dim lc As Literal = New Literal()

        Select Case _itemType
            Case ListItemType.Header

                lc.Text = "<fieldset style=""width:340px;""><legend>" & RecordType & "</legend><div style=""padding:5px;"">" & _
                    "<table cellpadding=""3"" style=""border: 1px solid #863d02;background-color:white;"">" & _
                    "  <tr>" & _
                    "    <td style=""background-color:#863d02;color:white;text-align:center;"">Period ID</td>" & _
                    "    <td style=""background-color:#863d02;color:white;text-align:center;"">Begin Date</td>" & _
                    "    <td style=""background-color:#863d02;color:white;text-align:center;"">End Date</td>" & _
                    "    <td style=""background-color:#863d02;color:white;text-align:center;"">Status</td>" & _
                    "  </tr>"

                Count = 1

                container.Controls.Add(lc)
            Case ListItemType.Item

                Count += 1

                AddHandler lc.DataBinding, AddressOf TemplateControl_DataBinding

                container.Controls.Add(lc)
            Case ListItemType.Footer

                If WhichView = "dates" Then
                    lc.Text = "</table>" & _
                        "<div style=""text-align:right;padding-top:10px;"">showing the most recent record period(s)</div></div></fieldset><br /><br />"
                Else
                    lc.Text = "</table>" & _
                        "<div style=""text-align:right;padding-top:10px;"">showing record period(s) for the " & WY & " WY</div></div></fieldset><br /><br />"
                End If

                container.Controls.Add(lc)
        End Select

    End Sub

    Private Sub TemplateControl_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim lc As Literal

        lc = CType(sender, Literal)

        Dim container As DataListItem = CType(lc.NamingContainer, DataListItem)

        Dim status_va As String = DataBinder.Eval(container.DataItem, "status_va")
        Dim period_id As Integer = DataBinder.Eval(container.DataItem, "period_id")
        Dim lock_img As String
        Dim locked As Boolean

        Dim p As New RecordPeriod(period_id)

        locked = p.PeriodIsLocked

        If locked Then
            lock_img = " &nbsp;<img src=""images/lock.png"" alt=""LOCKED"" />"
        Else
            lock_img = ""
        End If

        If WhichView = "dates" Then
            If Not locked Then
                If Count = CInt(NoOfPeriods) Then
                    lc.Text += "<tr><td>" & period_id & "</td><td><a href=""EditPeriodDates.aspx?period_id=" & _
                        period_id & "&dt=beg2"">" & DataBinder.Eval(container.DataItem, "period_beg_dt") & "</a>" & _
                        "</td><td><a href=""EditPeriodDates.aspx?period_id=" & period_id & "&dt=end2"">" & _
                        DataBinder.Eval(container.DataItem, "period_end_dt") & "</a></td><td>" & _
                        status_va & "</td></tr>"
                Else
                    lc.Text += "<tr><td>" & period_id & "</td><td>" & _
                        DataBinder.Eval(container.DataItem, "period_beg_dt") & _
                        "</td><td><a href=""EditPeriodDates.aspx?period_id=" & period_id & "&dt=end1"">" & _
                        DataBinder.Eval(container.DataItem, "period_end_dt") & "</a></td><td>" & _
                        status_va & "</td></tr>"
                End If
            Else
                lc.Text += "<tr><td style=""color:#CCCCCC;"">" & period_id & "</td><td style=""color:#CCCCCC;"">" & _
                    DataBinder.Eval(container.DataItem, "period_beg_dt") & "</td><td style=""color:#CCCCCC;"">" & DataBinder.Eval(container.DataItem, "period_end_dt") & _
                    "</td><td style=""color:#CCCCCC;"">" & status_va & lock_img & "</td></tr>"
            End If
        Else
            If Count = CInt(NoOfPeriods) Then
                If Not status_va = "Working" And Not status_va = "Checking" And Not status_va = "Reviewing" And Not status_va = "Rework" And Not status_va = "Reworking" And Not locked Then
                    lc.Text += "<tr><td>" & period_id & "</td><td>" & DataBinder.Eval(container.DataItem, "period_beg_dt") & _
                        "</td><td>" & DataBinder.Eval(container.DataItem, "period_end_dt") & "</td><td><a href=""EditPeriodStatus.aspx?period_id=" & DataBinder.Eval(container.DataItem, "period_id") & _
                        """>" & status_va & "</a></td></tr>"
                Else
                    lc.Text += "<tr><td style=""color:#CCCCCC;"">" & period_id & "</td><td style=""color:#CCCCCC;"">" & _
                        DataBinder.Eval(container.DataItem, "period_beg_dt") & "</td><td style=""color:#CCCCCC;"">" & DataBinder.Eval(container.DataItem, "period_end_dt") & _
                        "</td><td style=""color:#CCCCCC;"">" & status_va & lock_img & "</td></tr>"
                End If
            Else
                lc.Text += "<tr><td style=""color:#CCCCCC;"">" & period_id & "</td><td style=""color:#CCCCCC;"">" & _
                    DataBinder.Eval(container.DataItem, "period_beg_dt") & "</td><td style=""color:#CCCCCC;"">" & DataBinder.Eval(container.DataItem, "period_end_dt") & _
                    "</td><td style=""color:#CCCCCC;"">" & status_va & lock_img & "</td></tr>"
            End If
        End If




    End Sub

End Class
