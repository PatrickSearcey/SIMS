Imports System.Configuration

''' <summary>
''' Provides basic configuration settings for the entire SIMS site
''' </summary>
''' <remarks>Use this class to setup global variables</remarks>
Public Class Config
    Public Shared ReadOnly Property ConnectionInfo() As String
        Get
            Return ConfigurationManager.AppSettings("DBConnString")
        End Get
    End Property

    Public Shared ReadOnly Property SitePath() As String
        Get
            Return ConfigurationManager.AppSettings("SitePath")
        End Get
    End Property

    Public Shared ReadOnly Property ServerURL() As String
        Get
            Return ConfigurationManager.AppSettings("ServerURL")
        End Get
    End Property

    Public Shared ReadOnly Property DBName() As String
        Get
            Return ConfigurationManager.AppSettings("DBName")
        End Get
    End Property

    Public Shared ReadOnly Property WY() As String
        Get
            Dim curr_date As Date = Now()
            Dim curr_yr As Integer = DatePart(DateInterval.Year, curr_date)
            Dim curr_mo As Integer = DatePart(DateInterval.Month, curr_date)

            If curr_mo > 9 Then
                curr_yr = curr_yr + 1
            End If

            Dim curr_wy As String = CStr(curr_yr)

            Return curr_wy
        End Get
    End Property

    Public Shared ReadOnly Property DischargeMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("DischargeMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property LakeMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("LakeMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property EcoMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("EcoMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property AtmMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("AtmMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property GWMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("GWMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property QWMeasElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("QWMeasElem")
        End Get
    End Property

    Public Shared ReadOnly Property SiteHazardElem() As Integer
        Get
            Return ConfigurationManager.AppSettings("SiteHazardElem")
        End Get
    End Property
End Class
